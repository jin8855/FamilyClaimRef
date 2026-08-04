using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Threading;

[assembly: InternalsVisibleTo("FamilyClaimRef.App.Tests")]

namespace FamilyClaimRef.App.Startup;

internal sealed class StartupDiagnosticSession : IDisposable
{
    internal const string EnableEnvironmentVariable =
        "FAMILYCLAIMREF_ENABLE_STARTUP_DIAGNOSTICS";
    internal const string RootEnvironmentVariable =
        "FAMILYCLAIMREF_STARTUP_DIAGNOSTIC_ROOT";
    internal const string LogFileName = "startup.ndjson";
    internal const long MaximumLogFileBytes = 128 * 1024;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly object sync = new();
    private readonly FileStream? logStream;
    private readonly DirectoryHandleLease? directoryLease;
    private readonly Stopwatch? stopwatch;
    private IStartupDiagnosticEventRegistrar? eventRegistrar;
    private long sequence;
    private bool handlersRegistered;
    private bool writeStopped;
    private bool disposed;

    private StartupDiagnosticSession()
    {
    }

    private StartupDiagnosticSession(
        string diagnosticRootPath,
        FileStream logStream,
        DirectoryHandleLease directoryLease)
    {
        DiagnosticRootPath = diagnosticRootPath;
        LogFilePath = Path.Combine(diagnosticRootPath, LogFileName);
        this.logStream = logStream;
        this.directoryLease = directoryLease;
        stopwatch = Stopwatch.StartNew();
        IsEnabled = true;
    }

    internal bool IsEnabled { get; }

    internal string? DiagnosticRootPath { get; }

    internal string? LogFilePath { get; }

    internal int HandlerRegistrationCount { get; private set; }

    internal static StartupDiagnosticSession CreateFromEnvironment()
    {
        return CreateForConfiguration(
            Environment.GetEnvironmentVariable(EnableEnvironmentVariable),
            Environment.GetEnvironmentVariable(RootEnvironmentVariable));
    }

    internal static StartupDiagnosticSession CreateForConfiguration(
        string? enableValue,
        string? diagnosticRootPath)
    {
        return CreateForConfiguration(
            enableValue,
            diagnosticRootPath,
            TryValidateLogFileHandle);
    }

    internal static StartupDiagnosticSession CreateForConfiguration(
        string? enableValue,
        string? diagnosticRootPath,
        StartupDiagnosticLogFileHandleValidator logFileHandleValidator)
    {
        if (!string.Equals(enableValue, "1", StringComparison.Ordinal))
        {
            return new StartupDiagnosticSession();
        }

        if (!TryNormalizeDiagnosticRoot(
                diagnosticRootPath,
                out var normalizedRoot))
        {
            return new StartupDiagnosticSession();
        }

        if (!OperatingSystem.IsWindows() ||
            !Directory.Exists(normalizedRoot) ||
            logFileHandleValidator is null)
        {
            return new StartupDiagnosticSession();
        }

        DirectoryHandleLease? lease = null;
        FileStream? stream = null;
        try
        {
            if (!DirectoryHandleLease.TryAcquire(normalizedRoot, out lease))
            {
                return new StartupDiagnosticSession();
            }

            var logFilePath = Path.Combine(normalizedRoot, LogFileName);
            stream = new FileStream(
                logFilePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.Read);

            if (!logFileHandleValidator(stream.SafeFileHandle, logFilePath))
            {
                TryDispose(stream);
                stream = null;
                TryDispose(lease);
                lease = null;
                return new StartupDiagnosticSession();
            }

            return new StartupDiagnosticSession(normalizedRoot, stream, lease!);
        }
        catch (Exception)
        {
            TryDispose(stream);
            TryDispose(lease);
            return new StartupDiagnosticSession();
        }
    }

    internal void RegisterHandlers(Application application)
    {
        if (!IsEnabled || application is null)
        {
            return;
        }

        RegisterHandlers(new RuntimeStartupDiagnosticEventRegistrar(application));
    }

    internal void RegisterHandlers(
        IStartupDiagnosticEventRegistrar registrar)
    {
        if (!IsEnabled || registrar is null)
        {
            return;
        }

        lock (sync)
        {
            if (disposed || handlersRegistered)
            {
                return;
            }

            try
            {
                registrar.Attach(
                    OnAppDomainUnhandledException,
                    OnDispatcherUnhandledException,
                    OnTaskSchedulerUnobservedException);
                eventRegistrar = registrar;
                handlersRegistered = true;
                HandlerRegistrationCount = registrar.HandlerCount;
            }
            catch (Exception exception)
            {
                TryDetach(registrar);
                RecordCore(
                    "StartupDiagnosticSession",
                    "startup_diagnostics.handler_registration",
                    "event",
                    "failed",
                    exception,
                    "FamilyClaimRef.App.Startup.StartupDiagnosticSession.RegisterHandlers");
            }
        }
    }

    internal void Record(
        string owner,
        string milestone,
        string phase,
        string result,
        string? methodIdentifier = null)
    {
        RecordCore(
            owner,
            milestone,
            phase,
            result,
            null,
            methodIdentifier);
    }

    internal void RecordException(
        string owner,
        string milestone,
        string phase,
        string result,
        Exception exception,
        string? methodIdentifier = null)
    {
        RecordCore(
            owner,
            milestone,
            phase,
            result,
            exception,
            methodIdentifier);
    }

    public void Dispose()
    {
        lock (sync)
        {
            if (disposed)
            {
                return;
            }

            if (handlersRegistered && eventRegistrar is not null)
            {
                TryDetach(eventRegistrar);
            }

            handlersRegistered = false;
            HandlerRegistrationCount = 0;
            eventRegistrar = null;
            disposed = true;

            try
            {
                logStream?.Dispose();
            }
            catch (Exception)
            {
                // Diagnostics disposal is intentionally no-throw.
            }

            TryDispose(directoryLease);
        }
    }

    private void OnAppDomainUnhandledException(Exception? exception)
    {
        RecordCore(
            "StartupDiagnosticSession",
            "app_domain.unhandled_exception",
            "event",
            "observed",
            exception,
            "FamilyClaimRef.App.Startup.StartupDiagnosticSession.OnAppDomainUnhandledException");
    }

    private void OnDispatcherUnhandledException(Exception? exception)
    {
        RecordCore(
            "StartupDiagnosticSession",
            "dispatcher.unhandled_exception",
            "event",
            "observed",
            exception,
            "FamilyClaimRef.App.Startup.StartupDiagnosticSession.OnDispatcherUnhandledException");
    }

    private void OnTaskSchedulerUnobservedException(Exception? exception)
    {
        RecordCore(
            "StartupDiagnosticSession",
            "task_scheduler.unobserved_task_exception",
            "event",
            "observed",
            exception,
            "FamilyClaimRef.App.Startup.StartupDiagnosticSession.OnTaskSchedulerUnobservedException");
    }

    private void RecordCore(
        string owner,
        string milestone,
        string phase,
        string result,
        Exception? exception,
        string? methodIdentifier)
    {
        if (!IsEnabled)
        {
            return;
        }

        lock (sync)
        {
            if (disposed || writeStopped || logStream is null || stopwatch is null)
            {
                return;
            }

            try
            {
                var nextSequence = sequence + 1;
                var record = new StartupDiagnosticRecord(
                    nextSequence,
                    DateTimeOffset.UtcNow,
                    stopwatch.ElapsedTicks,
                    Environment.ProcessId,
                    Environment.CurrentManagedThreadId,
                    NormalizeOwner(owner),
                    NormalizeMilestone(milestone),
                    NormalizePhase(phase),
                    NormalizeResult(result),
                    NormalizeExceptionType(exception),
                    exception?.HResult,
                    NormalizeMethodIdentifier(methodIdentifier));

                var payload = JsonSerializer.SerializeToUtf8Bytes(
                    record,
                    JsonOptions);
                var requiredBytes = payload.Length + 1L;
                if (logStream.Position + requiredBytes > MaximumLogFileBytes)
                {
                    writeStopped = true;
                    return;
                }

                logStream.Write(payload);
                logStream.WriteByte((byte)'\n');
                logStream.Flush(flushToDisk: true);
                sequence = nextSequence;
            }
            catch (Exception)
            {
                writeStopped = true;
            }
        }
    }

    private static bool TryNormalizeDiagnosticRoot(
        string? configuredPath,
        out string normalizedPath)
    {
        normalizedPath = string.Empty;
        if (string.IsNullOrWhiteSpace(configuredPath) ||
            !Path.IsPathFullyQualified(configuredPath))
        {
            return false;
        }

        try
        {
            var tempRoot = TrimEndingSeparator(
                Path.GetFullPath(Path.GetTempPath()));
            var allowedRoot = TrimEndingSeparator(
                Path.GetFullPath(Path.Combine(
                    tempRoot,
                    "FamilyClaimRef",
                    "StartupDiagnostics")));
            var candidate = TrimEndingSeparator(
                Path.GetFullPath(configuredPath));

            if (!IsStrictChildPath(candidate, allowedRoot))
            {
                return false;
            }

            normalizedPath = candidate;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool TryValidateLogFileHandle(
        SafeFileHandle handle,
        string expectedPath)
    {
        try
        {
            if (!TryGetFileAttributeTagInfo(handle, out var attributeInfo) ||
                (attributeInfo.FileAttributes & FileAttributes.Directory) != 0 ||
                (attributeInfo.FileAttributes & FileAttributes.ReparsePoint) != 0 ||
                !TryGetFinalDosPath(handle, out var finalPath))
            {
                return false;
            }

            return string.Equals(
                Path.GetFullPath(expectedPath),
                finalPath,
                StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool IsStrictChildPath(
        string candidate,
        string parent)
    {
        return !string.Equals(
                   candidate,
                   parent,
                   StringComparison.OrdinalIgnoreCase) &&
               IsSameOrChildPath(candidate, parent);
    }

    private static bool IsSameOrChildPath(
        string candidate,
        string parent)
    {
        if (string.Equals(
                candidate,
                parent,
                StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var parentWithSeparator = parent + Path.DirectorySeparatorChar;
        return candidate.StartsWith(
            parentWithSeparator,
            StringComparison.OrdinalIgnoreCase);
    }

    private static string TrimEndingSeparator(string path)
    {
        return Path.TrimEndingDirectorySeparator(path);
    }

    private static bool TryGetFileAttributeTagInfo(
        SafeFileHandle handle,
        out FileAttributeTagInfo attributeInfo)
    {
        attributeInfo = default;
        if (handle.IsInvalid || handle.IsClosed)
        {
            return false;
        }

        return NativeMethods.GetFileInformationByHandleEx(
            handle,
            NativeMethods.FileAttributeTagInfoClass,
            out attributeInfo,
            (uint)Marshal.SizeOf<FileAttributeTagInfo>());
    }

    private static bool TryGetFinalDosPath(
        SafeFileHandle handle,
        out string finalPath)
    {
        finalPath = string.Empty;
        if (handle.IsInvalid || handle.IsClosed)
        {
            return false;
        }

        var capacity = 512;
        while (capacity <= 32768)
        {
            var buffer = new StringBuilder(capacity);
            var length = NativeMethods.GetFinalPathNameByHandleW(
                handle,
                buffer,
                (uint)buffer.Capacity,
                0);
            if (length == 0)
            {
                return false;
            }

            if (length < buffer.Capacity)
            {
                return TryNormalizeNativeFinalPath(
                    buffer.ToString(),
                    out finalPath);
            }

            capacity = checked((int)length + 1);
        }

        return false;
    }

    private static bool TryNormalizeNativeFinalPath(
        string nativePath,
        out string normalizedPath)
    {
        normalizedPath = string.Empty;
        const string DosPrefix = @"\\?\";
        const string UncPrefix = @"\\?\UNC\";
        if (!nativePath.StartsWith(
                DosPrefix,
                StringComparison.OrdinalIgnoreCase) ||
            nativePath.StartsWith(
                UncPrefix,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        try
        {
            var dosPath = nativePath[DosPrefix.Length..];
            if (!Path.IsPathFullyQualified(dosPath))
            {
                return false;
            }

            normalizedPath = Path.GetFullPath(dosPath);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static void TryDispose(IDisposable? disposable)
    {
        try
        {
            disposable?.Dispose();
        }
        catch (Exception)
        {
            // Diagnostics cleanup is intentionally no-throw.
        }
    }

    private static void TryDetach(
        IStartupDiagnosticEventRegistrar registrar)
    {
        try
        {
            registrar.Detach();
        }
        catch (Exception)
        {
            // Handler detach is intentionally no-throw.
        }
    }

    private static string NormalizeOwner(string owner)
    {
        return owner switch
        {
            "App" => owner,
            "ProductShellWindow" => owner,
            "StartupDiagnosticSession" => owner,
            _ => "invalid"
        };
    }

    private static string NormalizeMilestone(string milestone)
    {
        return milestone switch
        {
            "app_constructor.body_enter" => milestone,
            "startup_diagnostics.handler_registration" => milestone,
            "app_constructor.body_ready" => milestone,
            "app_on_startup.enter" => milestone,
            "base_on_startup" => milestone,
            "startup_mode.selection" => milestone,
            "app_services_create_default" => milestone,
            "product_shell_window.construction" => milestone,
            "application.main_window_assignment" => milestone,
            "product_shell_window.show" => milestone,
            "app_on_startup.exception" => milestone,
            "app_on_exit" => milestone,
            "product_shell_window.constructor" => milestone,
            "product_shell_window.initialize_component" => milestone,
            "product_shell_window.data_context_assignment" => milestone,
            "product_shell_window.loaded" => milestone,
            "product_shell_window.content_rendered" => milestone,
            "product_shell_window.dispatcher_callback" => milestone,
            "product_shell_window.closed" => milestone,
            "app_domain.unhandled_exception" => milestone,
            "dispatcher.unhandled_exception" => milestone,
            "task_scheduler.unobserved_task_exception" => milestone,
            _ => "invalid"
        };
    }

    private static string NormalizePhase(string phase)
    {
        return phase switch
        {
            "begin" => phase,
            "end" => phase,
            "enter" => phase,
            "return" => phase,
            "event" => phase,
            "decision" => phase,
            "callback" => phase,
            _ => "invalid"
        };
    }

    private static string NormalizeResult(string result)
    {
        return result switch
        {
            "started" => result,
            "completed" => result,
            "enabled" => result,
            "disabled" => result,
            "default" => result,
            "product_shell_preview" => result,
            "observed" => result,
            "scheduled" => result,
            "executed" => result,
            "failed" => result,
            _ => "invalid"
        };
    }

    private static string? NormalizeExceptionType(Exception? exception)
    {
        if (exception is null)
        {
            return null;
        }

        var typeName = exception.GetType().FullName;
        if (string.IsNullOrWhiteSpace(typeName) ||
            typeName.Length > 160 ||
            typeName.Any(character =>
                !(char.IsLetterOrDigit(character) ||
                  character is '.' or '+' or '`' or '[' or ']' or ',')))
        {
            return "Exception";
        }

        return typeName;
    }

    private static string? NormalizeMethodIdentifier(
        string? methodIdentifier)
    {
        return methodIdentifier switch
        {
            "FamilyClaimRef.App.App..ctor" => methodIdentifier,
            "FamilyClaimRef.App.App.OnStartup" => methodIdentifier,
            "FamilyClaimRef.App.App.OnExit" => methodIdentifier,
            "FamilyClaimRef.App.ProductShell.ProductShellWindow..ctor" =>
                methodIdentifier,
            "FamilyClaimRef.App.ProductShell.ProductShellWindow.ScheduleStartupDispatcherObservation" =>
                methodIdentifier,
            "FamilyClaimRef.App.Startup.StartupDiagnosticSession.RegisterHandlers" =>
                methodIdentifier,
            "FamilyClaimRef.App.Startup.StartupDiagnosticSession.OnAppDomainUnhandledException" =>
                methodIdentifier,
            "FamilyClaimRef.App.Startup.StartupDiagnosticSession.OnDispatcherUnhandledException" =>
                methodIdentifier,
            "FamilyClaimRef.App.Startup.StartupDiagnosticSession.OnTaskSchedulerUnobservedException" =>
                methodIdentifier,
            _ => null
        };
    }

    private sealed record StartupDiagnosticRecord(
        long Sequence,
        DateTimeOffset TimestampUtc,
        long ElapsedTicks,
        int ProcessId,
        int ManagedThreadId,
        string Owner,
        string Milestone,
        string Phase,
        string Result,
        string? ExceptionType,
        int? HResult,
        string? MethodIdentifier);

    private sealed class DirectoryHandleLease : IDisposable
    {
        private readonly List<SafeFileHandle> handles;
        private bool disposed;

        private DirectoryHandleLease(List<SafeFileHandle> handles)
        {
            this.handles = handles;
        }

        internal static bool TryAcquire(
            string requestedRoot,
            out DirectoryHandleLease? lease)
        {
            lease = null;
            List<SafeFileHandle>? acquiredHandles = [];
            try
            {
                if (!TryBuildComponentPaths(
                        requestedRoot,
                        out var componentPaths))
                {
                    return false;
                }

                foreach (var componentPath in componentPaths)
                {
                    var handle = NativeMethods.CreateFileW(
                        componentPath,
                        NativeMethods.FileReadAttributes,
                        FileShare.Read | FileShare.Write,
                        IntPtr.Zero,
                        NativeMethods.OpenExisting,
                        NativeMethods.FileFlagBackupSemantics |
                        NativeMethods.FileFlagOpenReparsePoint,
                        IntPtr.Zero);
                    if (handle.IsInvalid)
                    {
                        handle.Dispose();
                        return false;
                    }

                    if (!TryValidateDirectoryHandle(
                            handle,
                            componentPath))
                    {
                        handle.Dispose();
                        return false;
                    }

                    acquiredHandles.Add(handle);
                }

                lease = new DirectoryHandleLease(acquiredHandles);
                acquiredHandles = null;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                DisposeHandles(acquiredHandles);
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            DisposeHandles(handles);
        }

        private static bool TryBuildComponentPaths(
            string requestedRoot,
            out string[] componentPaths)
        {
            componentPaths = [];
            try
            {
                var tempRoot = TrimEndingSeparator(
                    Path.GetFullPath(Path.GetTempPath()));
                var normalizedRoot = TrimEndingSeparator(
                    Path.GetFullPath(requestedRoot));
                if (!IsStrictChildPath(normalizedRoot, tempRoot))
                {
                    return false;
                }

                var relativePath = Path.GetRelativePath(
                    tempRoot,
                    normalizedRoot);
                if (Path.IsPathFullyQualified(relativePath) ||
                    relativePath.Equals("..", StringComparison.Ordinal) ||
                    relativePath.StartsWith(
                        string.Concat("..", Path.DirectorySeparatorChar),
                        StringComparison.Ordinal))
                {
                    return false;
                }

                var paths = new List<string> { tempRoot };
                var currentPath = tempRoot;
                foreach (var segment in relativePath.Split(
                             [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
                             StringSplitOptions.RemoveEmptyEntries))
                {
                    currentPath = Path.GetFullPath(
                        Path.Combine(currentPath, segment));
                    paths.Add(currentPath);
                }

                componentPaths = [.. paths];
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool TryValidateDirectoryHandle(
            SafeFileHandle handle,
            string expectedPath)
        {
            if (!TryGetFileAttributeTagInfo(handle, out var attributeInfo) ||
                (attributeInfo.FileAttributes & FileAttributes.Directory) == 0 ||
                (attributeInfo.FileAttributes & FileAttributes.ReparsePoint) != 0 ||
                !TryGetFinalDosPath(handle, out var finalPath))
            {
                return false;
            }

            return string.Equals(
                TrimEndingSeparator(Path.GetFullPath(expectedPath)),
                TrimEndingSeparator(finalPath),
                StringComparison.OrdinalIgnoreCase);
        }

        private static void DisposeHandles(
            List<SafeFileHandle>? handles)
        {
            if (handles is null)
            {
                return;
            }

            for (var index = handles.Count - 1; index >= 0; index--)
            {
                TryDispose(handles[index]);
            }

            handles.Clear();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct FileAttributeTagInfo
    {
        internal FileAttributes FileAttributes;
        internal uint ReparseTag;
    }

    private static class NativeMethods
    {
        internal const uint FileReadAttributes = 0x00000080;
        internal const uint OpenExisting = 3;
        internal const uint FileFlagBackupSemantics = 0x02000000;
        internal const uint FileFlagOpenReparsePoint = 0x00200000;
        internal const int FileAttributeTagInfoClass = 9;

        [DllImport(
            "kernel32.dll",
            CharSet = CharSet.Unicode,
            ExactSpelling = true,
            SetLastError = true)]
        internal static extern SafeFileHandle CreateFileW(
            string fileName,
            uint desiredAccess,
            FileShare shareMode,
            IntPtr securityAttributes,
            uint creationDisposition,
            uint flagsAndAttributes,
            IntPtr templateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetFileInformationByHandleEx(
            SafeFileHandle fileHandle,
            int fileInformationClass,
            out FileAttributeTagInfo fileInformation,
            uint bufferSize);

        [DllImport(
            "kernel32.dll",
            CharSet = CharSet.Unicode,
            ExactSpelling = true,
            SetLastError = true)]
        internal static extern uint GetFinalPathNameByHandleW(
            SafeFileHandle fileHandle,
            StringBuilder filePath,
            uint filePathLength,
            uint flags);
    }
}

internal delegate bool StartupDiagnosticLogFileHandleValidator(
    SafeFileHandle handle,
    string expectedPath);

internal interface IStartupDiagnosticEventRegistrar
{
    int HandlerCount { get; }

    void Attach(
        Action<Exception?> appDomainUnhandled,
        Action<Exception?> dispatcherUnhandled,
        Action<Exception?> taskSchedulerUnobserved);

    void Detach();
}

internal sealed class RuntimeStartupDiagnosticEventRegistrar(
    Application application) : IStartupDiagnosticEventRegistrar
{
    private UnhandledExceptionEventHandler? appDomainHandler;
    private DispatcherUnhandledExceptionEventHandler? dispatcherHandler;
    private EventHandler<UnobservedTaskExceptionEventArgs>? taskSchedulerHandler;
    private bool appDomainAttached;
    private bool dispatcherAttached;
    private bool taskSchedulerAttached;

    public int HandlerCount => 3;

    public void Attach(
        Action<Exception?> appDomainUnhandled,
        Action<Exception?> dispatcherUnhandled,
        Action<Exception?> taskSchedulerUnobserved)
    {
        appDomainHandler = (_, args) =>
            appDomainUnhandled(args.ExceptionObject as Exception);
        dispatcherHandler = (_, args) =>
            dispatcherUnhandled(args.Exception);
        taskSchedulerHandler = (_, args) =>
            taskSchedulerUnobserved(args.Exception);

        try
        {
            AppDomain.CurrentDomain.UnhandledException += appDomainHandler;
            appDomainAttached = true;
            application.DispatcherUnhandledException += dispatcherHandler;
            dispatcherAttached = true;
            TaskScheduler.UnobservedTaskException += taskSchedulerHandler;
            taskSchedulerAttached = true;
        }
        catch (Exception)
        {
            Detach();
            throw;
        }
    }

    public void Detach()
    {
        TryDetachTaskScheduler();
        TryDetachDispatcher();
        TryDetachAppDomain();
    }

    private void TryDetachTaskScheduler()
    {
        try
        {
            if (taskSchedulerAttached && taskSchedulerHandler is not null)
            {
                TaskScheduler.UnobservedTaskException -= taskSchedulerHandler;
            }
        }
        catch (Exception)
        {
            // Each handler detach is independently no-throw.
        }
        finally
        {
            taskSchedulerAttached = false;
            taskSchedulerHandler = null;
        }
    }

    private void TryDetachDispatcher()
    {
        try
        {
            if (dispatcherAttached && dispatcherHandler is not null)
            {
                application.DispatcherUnhandledException -= dispatcherHandler;
            }
        }
        catch (Exception)
        {
            // Each handler detach is independently no-throw.
        }
        finally
        {
            dispatcherAttached = false;
            dispatcherHandler = null;
        }
    }

    private void TryDetachAppDomain()
    {
        try
        {
            if (appDomainAttached && appDomainHandler is not null)
            {
                AppDomain.CurrentDomain.UnhandledException -= appDomainHandler;
            }
        }
        catch (Exception)
        {
            // Each handler detach is independently no-throw.
        }
        finally
        {
            appDomainAttached = false;
            appDomainHandler = null;
        }
    }
}

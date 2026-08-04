using System.Text.Json;
using FamilyClaimRef.App.Startup;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class StartupDiagnosticSessionTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("0")]
    [InlineData("true")]
    [InlineData("01")]
    public void Disabled_enable_values_create_no_directory_file_or_handlers(
        string? enableValue)
    {
        var root = CreateUniqueDiagnosticRoot();

        using var session = StartupDiagnosticSession.CreateForConfiguration(
            enableValue,
            root);
        var exception = Record.Exception(() =>
            session.Record(
                "App",
                "app_constructor.body_enter",
                "enter",
                "started"));

        Assert.False(session.IsEnabled);
        Assert.Equal(0, session.HandlerRegistrationCount);
        Assert.Null(exception);
        Assert.False(Directory.Exists(root));
        Assert.False(File.Exists(Path.Combine(
            root,
            StartupDiagnosticSession.LogFileName)));
    }

    [Fact]
    public void Missing_root_disables_diagnostics_without_throwing()
    {
        var exception = Record.Exception(() =>
        {
            using var session =
                StartupDiagnosticSession.CreateForConfiguration("1", null);
            Assert.False(session.IsEnabled);
        });

        Assert.Null(exception);
    }

    [Fact]
    public void Valid_missing_root_disables_without_creating_directory_or_file()
    {
        var root = CreateUniqueDiagnosticRoot();

        using var session =
            StartupDiagnosticSession.CreateForConfiguration("1", root);

        Assert.False(session.IsEnabled);
        Assert.False(Directory.Exists(root));
        Assert.False(File.Exists(Path.Combine(
            root,
            StartupDiagnosticSession.LogFileName)));
    }

    [Theory]
    [InlineData("relative")]
    [InlineData("")]
    public void Relative_or_empty_root_disables_diagnostics_without_artifacts(
        string root)
    {
        var exception = Record.Exception(() =>
        {
            using var session =
                StartupDiagnosticSession.CreateForConfiguration("1", root);
            Assert.False(session.IsEnabled);
            Assert.Equal(0, session.HandlerRegistrationCount);
        });

        Assert.Null(exception);
    }

    [Fact]
    public void Non_temp_root_disables_diagnostics_without_creating_it()
    {
        var driveRoot = Path.GetPathRoot(Path.GetTempPath())
            ?? throw new InvalidOperationException("Drive root was not found.");
        var root = Path.Combine(
            driveRoot,
            "FamilyClaimRef-StartupDiagnostics-NotTemp",
            Guid.NewGuid().ToString("N"));

        using var session =
            StartupDiagnosticSession.CreateForConfiguration("1", root);

        Assert.False(session.IsEnabled);
        Assert.False(Directory.Exists(root));
    }

    [Fact]
    public void Allowed_area_itself_is_not_an_isolated_run_root()
    {
        var allowedArea = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef",
            "StartupDiagnostics");

        using var session =
            StartupDiagnosticSession.CreateForConfiguration("1", allowedArea);

        Assert.False(session.IsEnabled);
    }

    [Fact]
    public void Existing_file_root_disables_diagnostics_and_preserves_file()
    {
        var parent = CreateUniqueDiagnosticRoot();
        Directory.CreateDirectory(parent);
        var fileRoot = Path.Combine(parent, "not-a-directory");
        const string ExistingContent = "existing-test-content";
        File.WriteAllText(fileRoot, ExistingContent);

        try
        {
            using var session =
                StartupDiagnosticSession.CreateForConfiguration("1", fileRoot);

            Assert.False(session.IsEnabled);
            Assert.Equal(ExistingContent, File.ReadAllText(fileRoot));
            Assert.Single(Directory.GetFiles(parent));
        }
        finally
        {
            Directory.Delete(parent, recursive: true);
        }
    }

    [Fact]
    public void Existing_log_file_disables_diagnostics_without_overwrite()
    {
        var root = CreateUniqueDiagnosticRoot();
        Directory.CreateDirectory(root);
        var logFilePath = Path.Combine(
            root,
            StartupDiagnosticSession.LogFileName);
        const string ExistingContent = "do-not-overwrite";
        File.WriteAllText(logFilePath, ExistingContent);

        try
        {
            using var session =
                StartupDiagnosticSession.CreateForConfiguration("1", root);

            Assert.False(session.IsEnabled);
            Assert.Equal(ExistingContent, File.ReadAllText(logFilePath));
            Assert.Single(Directory.GetFiles(root));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Normalized_parent_segment_inside_allowed_area_is_accepted()
    {
        var root = CreateUniqueDiagnosticRoot();
        Directory.CreateDirectory(root);
        var parent = Path.GetDirectoryName(root)
            ?? throw new InvalidOperationException("Test parent was not found.");
        var configuredRoot = Path.Combine(
            parent,
            "normalization-segment",
            "..",
            Path.GetFileName(root));

        try
        {
            using var session =
                StartupDiagnosticSession.CreateForConfiguration(
                    "1",
                    configuredRoot);

            Assert.True(session.IsEnabled);
            Assert.Equal(Path.GetFullPath(root), session.DiagnosticRootPath);
        }
        finally
        {
            DeleteExactTestRoot(root);
        }
    }

    [Fact]
    public void Reparse_point_root_disables_diagnostics()
    {
        var linkRoot = CreateUniqueDiagnosticRoot();
        var targetRoot = string.Concat(linkRoot, "-target");
        Directory.CreateDirectory(targetRoot);
        Directory.CreateDirectory(
            Path.GetDirectoryName(linkRoot)
            ?? throw new InvalidOperationException("Test parent was not found."));
        Directory.CreateSymbolicLink(linkRoot, targetRoot);

        try
        {
            using var session =
                StartupDiagnosticSession.CreateForConfiguration("1", linkRoot);

            Assert.False(session.IsEnabled);
            Assert.Empty(Directory.GetFileSystemEntries(targetRoot));
        }
        finally
        {
            if (Directory.Exists(linkRoot))
            {
                Directory.Delete(linkRoot, recursive: false);
            }

            if (Directory.Exists(targetRoot))
            {
                Directory.Delete(targetRoot, recursive: true);
            }
        }
    }

    [Fact]
    public void Ancestor_reparse_point_disables_diagnostics()
    {
        var linkParent = CreateUniqueDiagnosticRoot();
        var targetParent = string.Concat(linkParent, "-target");
        var targetRoot = Path.Combine(targetParent, "run");
        var configuredRoot = Path.Combine(linkParent, "run");
        Directory.CreateDirectory(targetRoot);
        Directory.CreateSymbolicLink(linkParent, targetParent);

        try
        {
            using var session =
                StartupDiagnosticSession.CreateForConfiguration(
                    "1",
                    configuredRoot);

            Assert.False(session.IsEnabled);
            Assert.Empty(Directory.GetFileSystemEntries(targetRoot));
        }
        finally
        {
            if (Directory.Exists(linkParent))
            {
                Directory.Delete(linkParent, recursive: false);
            }

            if (Directory.Exists(targetParent))
            {
                Directory.Delete(targetParent, recursive: true);
            }
        }
    }

    [Fact]
    public void Directory_lease_blocks_leaf_rename_and_replacement_until_dispose()
    {
        var root = CreateUniqueDiagnosticRoot();
        var displacedRoot = string.Concat(root, "-displaced");
        var competitorRoot = string.Concat(root, "-competitor");
        Directory.CreateDirectory(root);
        Directory.CreateDirectory(competitorRoot);
        File.WriteAllText(
            Path.Combine(competitorRoot, "sentinel.txt"),
            "competitor-owned");
        var session =
            StartupDiagnosticSession.CreateForConfiguration("1", root);

        try
        {
            Assert.True(session.IsEnabled);
            Assert.NotNull(Record.Exception(() =>
                Directory.Move(root, displacedRoot)));
            Assert.NotNull(Record.Exception(() =>
                Directory.Move(competitorRoot, root)));
            Assert.True(Directory.Exists(root));
            Assert.True(Directory.Exists(competitorRoot));

            session.Dispose();

            Directory.Move(root, displacedRoot);
            Directory.Move(competitorRoot, root);
            Assert.True(Directory.Exists(displacedRoot));
            Assert.Equal(
                "competitor-owned",
                File.ReadAllText(Path.Combine(root, "sentinel.txt")));
        }
        finally
        {
            session.Dispose();
            DeleteExactTestRoot(root);
            DeleteExactTestRoot(displacedRoot);
            DeleteExactTestRoot(competitorRoot);
        }
    }

    [Fact]
    public void Directory_lease_blocks_ancestor_rename_until_dispose()
    {
        var parent = CreateUniqueDiagnosticRoot();
        var renamedParent = string.Concat(parent, "-renamed");
        var root = Path.Combine(parent, "run");
        Directory.CreateDirectory(root);
        var session =
            StartupDiagnosticSession.CreateForConfiguration("1", root);

        try
        {
            Assert.True(session.IsEnabled);
            Assert.NotNull(Record.Exception(() =>
                Directory.Move(parent, renamedParent)));
            Assert.True(Directory.Exists(parent));

            session.Dispose();

            Directory.Move(parent, renamedParent);
            Assert.True(Directory.Exists(
                Path.Combine(renamedParent, "run")));
        }
        finally
        {
            session.Dispose();
            DeleteExactTestRoot(parent);
            DeleteExactTestRoot(renamedParent);
        }
    }

    [Fact]
    public void Setup_failure_preserves_competitor_sentinel_and_owned_log_residue()
    {
        var root = CreateUniqueDiagnosticRoot();
        var sentinelDirectory = Path.Combine(root, "competitor");
        var sentinelFile = Path.Combine(
            sentinelDirectory,
            "sentinel.bin");
        var sentinelBytes = new byte[] { 0x11, 0x22, 0x33, 0x44 };
        Directory.CreateDirectory(sentinelDirectory);
        File.WriteAllBytes(sentinelFile, sentinelBytes);

        try
        {
            using var session =
                StartupDiagnosticSession.CreateForConfiguration(
                    "1",
                    root,
                    (_, _) => false);

            Assert.False(session.IsEnabled);
            Assert.True(Directory.Exists(root));
            Assert.True(Directory.Exists(sentinelDirectory));
            Assert.Equal(sentinelBytes, File.ReadAllBytes(sentinelFile));
            var residue = Path.Combine(
                root,
                StartupDiagnosticSession.LogFileName);
            Assert.True(File.Exists(residue));
            Assert.Equal(0, new FileInfo(residue).Length);
        }
        finally
        {
            DeleteExactTestRoot(root);
        }
    }

    [Fact]
    public void Enabled_session_creates_one_parseable_immediately_flushed_log()
    {
        var root = CreateUniqueDiagnosticRoot();
        Directory.CreateDirectory(root);
        try
        {
            using var session =
                StartupDiagnosticSession.CreateForConfiguration("1", root);

            Assert.True(session.IsEnabled);
            Assert.Equal(Path.GetFullPath(root), session.DiagnosticRootPath);
            Assert.Equal(
                Path.Combine(root, StartupDiagnosticSession.LogFileName),
                session.LogFilePath);

            session.Record(
                "App",
                "app_constructor.body_enter",
                "enter",
                "started",
                "FamilyClaimRef.App.App..ctor");
            session.Record(
                "App",
                "app_constructor.body_ready",
                "return",
                "completed",
                "FamilyClaimRef.App.App..ctor");

            var logFiles = Directory.GetFiles(root);
            var logFile = Assert.Single(logFiles);
            Assert.Equal(StartupDiagnosticSession.LogFileName, Path.GetFileName(logFile));

            var lines = ReadAllLinesShared(logFile);
            Assert.Equal(2, lines.Length);
            var first = ParseRecord(lines[0]);
            var second = ParseRecord(lines[1]);
            Assert.Equal(1, first.Sequence);
            Assert.Equal(2, second.Sequence);
            Assert.True(second.ElapsedTicks >= first.ElapsedTicks);
            Assert.Equal(Environment.ProcessId, first.ProcessId);
            Assert.True(first.ManagedThreadId > 0);
            Assert.Equal("App", first.Owner);
            Assert.Equal("app_constructor.body_enter", first.Milestone);
            Assert.Equal("enter", first.Phase);
            Assert.Equal("started", first.Result);
            Assert.Equal("FamilyClaimRef.App.App..ctor", first.MethodIdentifier);
            Assert.NotEqual(default, first.TimestampUtc);
        }
        finally
        {
            DeleteExactTestRoot(root);
        }
    }

    [Fact]
    public void Privacy_sensitive_inputs_and_exception_message_are_not_logged()
    {
        var root = CreateUniqueDiagnosticRoot();
        Directory.CreateDirectory(root);
        var windowsPrivatePath = string.Concat(
            "C:",
            "\\",
            "Users",
            "\\",
            "ExampleUser",
            "\\",
            "secret.txt");
        var unixPrivatePath = string.Concat(
            "/home/",
            "example/",
            "private.txt");
        var arbitraryDocumentName = string.Concat(
            "original-",
            "insurance-claim-document.pdf");
        var claimLikeValue = string.Concat(
            "claim_",
            "private_001");
        var exceptionMessage = string.Join(
            "\n",
            windowsPrivatePath,
            unixPrivatePath,
            arbitraryDocumentName,
            claimLikeValue,
            "\uBBFC\uAC10\uD55C \uBB38\uC790\uC5F4");

        try
        {
            using var session =
                StartupDiagnosticSession.CreateForConfiguration("1", root);
            session.RecordException(
                windowsPrivatePath,
                arbitraryDocumentName,
                unixPrivatePath,
                claimLikeValue,
                new InvalidOperationException(exceptionMessage),
                windowsPrivatePath);

            var log = ReadAllTextShared(
                Assert.Single(Directory.GetFiles(root)));

            Assert.DoesNotContain(windowsPrivatePath, log, StringComparison.Ordinal);
            Assert.DoesNotContain(unixPrivatePath, log, StringComparison.Ordinal);
            Assert.DoesNotContain(arbitraryDocumentName, log, StringComparison.Ordinal);
            Assert.DoesNotContain(claimLikeValue, log, StringComparison.Ordinal);
            Assert.DoesNotContain(exceptionMessage, log, StringComparison.Ordinal);
            Assert.DoesNotContain("\uBBFC\uAC10\uD55C \uBB38\uC790\uC5F4", log, StringComparison.Ordinal);
            Assert.Contains(
                "System.InvalidOperationException",
                log,
                StringComparison.Ordinal);

            var record = ParseRecord(Assert.Single(
                log.Split('\n', StringSplitOptions.RemoveEmptyEntries)));
            Assert.Equal("invalid", record.Owner);
            Assert.Equal("invalid", record.Milestone);
            Assert.Equal("invalid", record.Phase);
            Assert.Equal("invalid", record.Result);
            Assert.Null(record.MethodIdentifier);
            Assert.Equal(typeof(InvalidOperationException).FullName, record.ExceptionType);
        }
        finally
        {
            DeleteExactTestRoot(root);
        }
    }

    [Fact]
    public void Log_is_bounded_and_all_written_records_remain_parseable()
    {
        var root = CreateUniqueDiagnosticRoot();
        Directory.CreateDirectory(root);
        try
        {
            using var session =
                StartupDiagnosticSession.CreateForConfiguration("1", root);

            var exception = Record.Exception(() =>
            {
                for (var index = 0; index < 1000; index++)
                {
                    session.Record(
                        "ProductShellWindow",
                        "product_shell_window.dispatcher_callback",
                        "callback",
                        "executed",
                        "FamilyClaimRef.App.ProductShell.ProductShellWindow.ScheduleStartupDispatcherObservation");
                }
            });

            Assert.Null(exception);
            var logFile = Assert.Single(Directory.GetFiles(root));
            Assert.InRange(
                new FileInfo(logFile).Length,
                1,
                StartupDiagnosticSession.MaximumLogFileBytes);

            var records = ReadAllLinesShared(logFile)
                .Select(ParseRecord)
                .ToArray();
            Assert.NotEmpty(records);
            Assert.True(records.Length < 1000);
            Assert.Equal(
                Enumerable.Range(1, records.Length).Select(value => (long)value),
                records.Select(record => record.Sequence));
            Assert.True(records
                .Zip(records.Skip(1))
                .All(pair => pair.Second.ElapsedTicks >= pair.First.ElapsedTicks));
        }
        finally
        {
            DeleteExactTestRoot(root);
        }
    }

    [Fact]
    public async Task Concurrent_record_calls_produce_parseable_monotonic_bounded_ndjson()
    {
        var root = CreateUniqueDiagnosticRoot();
        Directory.CreateDirectory(root);
        try
        {
            using var session =
                StartupDiagnosticSession.CreateForConfiguration("1", root);
            using var start = new ManualResetEventSlim(false);
            var exceptions = new List<Exception>();
            var exceptionSync = new object();
            const int WorkerCount = 8;
            const int RecordsPerWorker = 20;
            var tasks = Enumerable.Range(0, WorkerCount)
                .Select(_ => Task.Run(() =>
                {
                    start.Wait();
                    try
                    {
                        for (var index = 0; index < RecordsPerWorker; index++)
                        {
                            session.Record(
                                "ProductShellWindow",
                                "product_shell_window.dispatcher_callback",
                                "callback",
                                "executed");
                        }
                    }
                    catch (Exception exception)
                    {
                        lock (exceptionSync)
                        {
                            exceptions.Add(exception);
                        }
                    }
                }))
                .ToArray();

            start.Set();
            await Task.WhenAll(tasks);

            Assert.Empty(exceptions);
            var logFile = Assert.Single(Directory.GetFiles(root));
            Assert.InRange(
                new FileInfo(logFile).Length,
                1,
                StartupDiagnosticSession.MaximumLogFileBytes);
            var records = ReadAllLinesShared(logFile)
                .Select(ParseRecord)
                .ToArray();
            Assert.Equal(WorkerCount * RecordsPerWorker, records.Length);
            Assert.Equal(
                Enumerable.Range(1, records.Length).Select(value => (long)value),
                records.Select(record => record.Sequence));
            Assert.Equal(
                records.Length,
                records.Select(record => record.Sequence).Distinct().Count());
        }
        finally
        {
            DeleteExactTestRoot(root);
        }
    }

    [Fact]
    public async Task Record_and_dispose_race_is_no_throw_parseable_and_length_stable()
    {
        var root = CreateUniqueDiagnosticRoot();
        Directory.CreateDirectory(root);
        var session =
            StartupDiagnosticSession.CreateForConfiguration("1", root);
        try
        {
            session.Record(
                "App",
                "app_constructor.body_enter",
                "enter",
                "started");
            var logFile = Assert.Single(Directory.GetFiles(root));
            using var start = new ManualResetEventSlim(false);
            var exceptions = new List<Exception>();
            var exceptionSync = new object();
            var writers = Enumerable.Range(0, 8)
                .Select(_ => Task.Run(() =>
                {
                    start.Wait();
                    try
                    {
                        for (var index = 0; index < 40; index++)
                        {
                            session.Record(
                                "App",
                                "app_constructor.body_ready",
                                "return",
                                "completed");
                        }
                    }
                    catch (Exception exception)
                    {
                        lock (exceptionSync)
                        {
                            exceptions.Add(exception);
                        }
                    }
                }))
                .ToArray();
            var disposer = Task.Run(() =>
            {
                start.Wait();
                try
                {
                    session.Dispose();
                }
                catch (Exception exception)
                {
                    lock (exceptionSync)
                    {
                        exceptions.Add(exception);
                    }
                }
            });

            start.Set();
            await Task.WhenAll(writers.Append(disposer));

            Assert.Empty(exceptions);
            var records = File.ReadAllLines(logFile)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(ParseRecord)
                .ToArray();
            Assert.NotEmpty(records);
            Assert.Equal(
                Enumerable.Range(1, records.Length).Select(value => (long)value),
                records.Select(record => record.Sequence));
            Assert.Equal(
                records.Length,
                records.Select(record => record.Sequence).Distinct().Count());
            var lengthAfterDispose = new FileInfo(logFile).Length;
            Assert.InRange(
                lengthAfterDispose,
                1,
                StartupDiagnosticSession.MaximumLogFileBytes);

            session.Record(
                "App",
                "app_constructor.body_ready",
                "return",
                "completed");

            Assert.Equal(
                lengthAfterDispose,
                new FileInfo(logFile).Length);
        }
        finally
        {
            session.Dispose();
            DeleteExactTestRoot(root);
        }
    }

    [Fact]
    public void Handler_registration_is_idempotent_and_dispose_detaches()
    {
        var root = CreateUniqueDiagnosticRoot();
        Directory.CreateDirectory(root);
        try
        {
            var registrar = new FakeEventRegistrar();
            var session =
                StartupDiagnosticSession.CreateForConfiguration("1", root);

            session.RegisterHandlers(registrar);
            session.RegisterHandlers(registrar);

            Assert.Equal(1, registrar.AttachCount);
            Assert.Equal(3, session.HandlerRegistrationCount);

            registrar.RaiseDispatcher(
                new InvalidOperationException("message-must-not-be-logged"));
            var lineCountBeforeDispose = ReadAllLinesShared(
                Assert.Single(Directory.GetFiles(root))).Length;
            Assert.Equal(1, lineCountBeforeDispose);

            session.Dispose();

            Assert.Equal(1, registrar.DetachCount);
            Assert.Equal(0, session.HandlerRegistrationCount);
            registrar.RaiseDispatcher(
                new InvalidOperationException("not-observed-after-detach"));
            var lineCountAfterDispose = ReadAllLinesShared(
                Assert.Single(Directory.GetFiles(root))).Length;
            Assert.Equal(lineCountBeforeDispose, lineCountAfterDispose);
        }
        finally
        {
            DeleteExactTestRoot(root);
        }
    }

    [Fact]
    public void Record_after_dispose_is_no_throw_and_creates_no_new_record()
    {
        var root = CreateUniqueDiagnosticRoot();
        Directory.CreateDirectory(root);
        try
        {
            var session =
                StartupDiagnosticSession.CreateForConfiguration("1", root);
            session.Record(
                "App",
                "app_constructor.body_enter",
                "enter",
                "started");
            var logFile = Assert.Single(Directory.GetFiles(root));
            var before = ReadAllTextShared(logFile);
            session.Dispose();

            var exception = Record.Exception(() =>
                session.Record(
                    "App",
                    "app_constructor.body_ready",
                    "return",
                    "completed"));

            Assert.Null(exception);
            Assert.Equal(before, ReadAllTextShared(logFile));
        }
        finally
        {
            DeleteExactTestRoot(root);
        }
    }

    private static DiagnosticRecord ParseRecord(string json)
    {
        return JsonSerializer.Deserialize<DiagnosticRecord>(
                   json,
                   new JsonSerializerOptions
                   {
                       PropertyNameCaseInsensitive = true
                   })
               ?? throw new InvalidOperationException(
                   "Diagnostic record was null.");
    }

    private static string[] ReadAllLinesShared(string path)
    {
        return ReadAllTextShared(path)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);
    }

    private static string ReadAllTextShared(string path)
    {
        using var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite | FileShare.Delete);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static string CreateUniqueDiagnosticRoot()
    {
        return Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef",
            "StartupDiagnostics",
            "tests",
            Guid.NewGuid().ToString("N"));
    }

    private static void DeleteExactTestRoot(string root)
    {
        if (Directory.Exists(root))
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private sealed record DiagnosticRecord(
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

    private sealed class FakeEventRegistrar : IStartupDiagnosticEventRegistrar
    {
        private Action<Exception?>? appDomainUnhandled;
        private Action<Exception?>? dispatcherUnhandled;
        private Action<Exception?>? taskSchedulerUnobserved;

        public int HandlerCount => 3;

        public int AttachCount { get; private set; }

        public int DetachCount { get; private set; }

        public void Attach(
            Action<Exception?> appDomainHandler,
            Action<Exception?> dispatcherHandler,
            Action<Exception?> taskSchedulerHandler)
        {
            AttachCount++;
            appDomainUnhandled = appDomainHandler;
            dispatcherUnhandled = dispatcherHandler;
            taskSchedulerUnobserved = taskSchedulerHandler;
        }

        public void Detach()
        {
            DetachCount++;
            appDomainUnhandled = null;
            dispatcherUnhandled = null;
            taskSchedulerUnobserved = null;
        }

        public void RaiseDispatcher(Exception exception)
        {
            dispatcherUnhandled?.Invoke(exception);
        }
    }
}

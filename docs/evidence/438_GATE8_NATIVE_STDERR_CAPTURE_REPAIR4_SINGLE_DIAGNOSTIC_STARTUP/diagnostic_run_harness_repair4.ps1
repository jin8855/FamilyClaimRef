param(
    [switch]$GitAuditProbeOnly
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$expectedHead = 'aecf7edfd43b4124ec5ff17d35687020cf4c0d90'
$expectedPathSetSha256 = '5c46306e51fd39ffdc452ece07e3543b56e5e46f644513c5e340836bc03d1a1d'
$expectedT0AggregateSha256 = '611df5c4e49e99b7c37e3e0a777b88635524062bec738ca8a3140be8a35848a3'
$expectedExecutableBytes = 162816L
$expectedExecutableSha256 = 'bb3de775939bbeb06aa9abe42e9e93cee51881084b3e6f20e7293a2d23300c39'
$expectedPriorHoldDocumentBytes = 11528L
$expectedPriorHoldDocumentSha256 = '18aed9e914fdf8722c4e127ea84881248cdadc2d25ee26bb78ae52a32cfc51a5'
$expectedPriorHarnessBytes = 31210L
$expectedPriorHarnessSha256 = 'c02707d1e9240eae9afeb3f5b235248b8751144215c9d9c04919c501ec51db08'
$expectedPriorHold435Bytes = 11772L
$expectedPriorHold435Sha256 = 'a340d759c67b6efd124619a69cc439786902d745f1f09baafaec5b8810885b40'
$expectedPriorRepair1HarnessBytes = 34797L
$expectedPriorRepair1HarnessSha256 = '05db1731da76176cf298c0a3f84e9e327afeaab44eea168a096f37379f451383'
$expectedPriorHold436Bytes = 12783L
$expectedPriorHold436Sha256 = 'dc01682d11c0c737c28c8cadda29cc805ba2320ba534986e60c4dff5bd24cdfb'
$expectedPriorRepair2HarnessBytes = 39980L
$expectedPriorRepair2HarnessSha256 = '7914d64ddcc4af4e44ee2017b14ccdb8a587ea38c3291286ab25db6a900a7868'
$expectedPriorHold437Bytes = 12297L
$expectedPriorHold437Sha256 = '2ec9e4f242c7889a11608d1500959526e34c9296dd8a6c3acf64ded44fca5fbb'
$expectedPriorRepair3HarnessBytes = 49655L
$expectedPriorRepair3HarnessSha256 = '75604046cfb904303bde292a0ec482c50dd3caa960af7d9df67d5d4cae824818'
$expectedPriorRepair3LogBytes = 7643L
$expectedPriorRepair3LogSha256 = '20e5dd5a0612b441643e0d5a6ae95eb5da22a54c35e489c43491778ffcfd3c31'
$maximumLogBytes = 131072L
$schemaVersion = 1
$evidenceRelativeRoot = 'docs/evidence/438_GATE8_NATIVE_STDERR_CAPTURE_REPAIR4_SINGLE_DIAGNOSTIC_STARTUP'
$harnessRelativePath = "$evidenceRelativeRoot/diagnostic_run_harness_repair4.ps1"
$observationRelativePath = "$evidenceRelativeRoot/runtime_observation.json"
$logEvidenceRelativePath = "$evidenceRelativeRoot/startup.ndjson"
$resultDocumentRelativePath =
    'docs/438_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_NATIVE_STDERR_CAPTURE_REPAIR4_AND_SINGLE_RUNTIME_EVIDENCE.md'
$priorHoldDocumentRelativePath =
    'docs/434_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_SINGLE_ISOLATED_DIAGNOSTIC_PRODUCT_STARTUP_AND_RUNTIME_EVIDENCE.md'
$priorHarnessRelativePath =
    'docs/evidence/434_GATE8_SINGLE_DIAGNOSTIC_STARTUP/diagnostic_run_harness.ps1'
$priorHold435RelativePath =
    'docs/435_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_COMMAND_LOCAL_GIT_TRUST_ENVIRONMENT_REPAIR1_AND_SINGLE_RUNTIME_EVIDENCE.md'
$priorRepair1HarnessRelativePath =
    'docs/evidence/435_GATE8_COMMAND_LOCAL_GIT_TRUST_REPAIR1_SINGLE_DIAGNOSTIC_STARTUP/diagnostic_run_harness_repair1.ps1'
$priorHold436RelativePath =
    'docs/436_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_WINDOWS_POWERSHELL_PATH_COMPATIBILITY_REPAIR2_AND_SINGLE_RUNTIME_EVIDENCE.md'
$priorRepair2HarnessRelativePath =
    'docs/evidence/436_GATE8_WINDOWS_POWERSHELL_PATH_COMPATIBILITY_REPAIR2_SINGLE_DIAGNOSTIC_STARTUP/diagnostic_run_harness_repair2.ps1'
$priorHold437RelativePath =
    'docs/437_POLICY_CLAIM_PRODUCT_UI_SHELL_GATE8_ORDINAL_PATH_SET_BASELINE_REPAIR3_AND_SINGLE_RUNTIME_EVIDENCE.md'
$priorRepair3HarnessRelativePath =
    'docs/evidence/437_GATE8_ORDINAL_PATH_SET_BASELINE_REPAIR3_SINGLE_DIAGNOSTIC_STARTUP/diagnostic_run_harness_repair3.ps1'
$priorRepair3LogRelativePath =
    'docs/evidence/437_GATE8_ORDINAL_PATH_SET_BASELINE_REPAIR3_SINGLE_DIAGNOSTIC_STARTUP/startup.ndjson'

$script:nativeGitInvocationCount = 0

Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
using System.Text;

public static class Gate8WindowNativeMethods
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint GetWindowThreadProcessId(
        IntPtr hWnd,
        out uint processId);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int GetClassName(
        IntPtr hWnd,
        StringBuilder className,
        int maximumCount);
}
'@

function Get-StringSha256 {
    param([Parameter(Mandatory)][string]$Value)

    $sha = [Security.Cryptography.SHA256]::Create()
    try {
        $hash = $sha.ComputeHash([Text.Encoding]::UTF8.GetBytes($Value))
    }
    finally {
        $sha.Dispose()
    }

    return ([BitConverter]::ToString($hash) -replace '-', '').ToLowerInvariant()
}

function Get-FileSha256 {
    param([Parameter(Mandatory)][string]$Path)

    return (Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()
}

function Write-Utf8NoBom {
    param(
        [Parameter(Mandatory)][string]$Path,
        [Parameter(Mandatory)][string]$Value
    )

    [IO.File]::WriteAllText(
        $Path,
        $Value,
        [Text.UTF8Encoding]::new($false))
}

function Write-AtomicUtf8NoBom {
    param(
        [Parameter(Mandatory)][string]$Path,
        [Parameter(Mandatory)][string]$Value
    )

    $temporaryPath = "$Path.tmp"
    if ((Test-Path -LiteralPath $Path) -or
        (Test-Path -LiteralPath $temporaryPath)) {
        throw 'OBSERVATION_PATH_PREEXISTS'
    }

    try {
        Write-Utf8NoBom $temporaryPath $Value
        [IO.File]::Move($temporaryPath, $Path)
    }
    finally {
        if (Test-Path -LiteralPath $temporaryPath -PathType Leaf) {
            Remove-Item -LiteralPath $temporaryPath -Force
        }
    }
}

function Get-RandomHex {
    param([Parameter(Mandatory)][int]$ByteCount)

    $bytes = [byte[]]::new($ByteCount)
    $generator = [Security.Cryptography.RandomNumberGenerator]::Create()
    try {
        $generator.GetBytes($bytes)
    }
    finally {
        $generator.Dispose()
    }

    return ([BitConverter]::ToString($bytes) -replace '-', '').ToLowerInvariant()
}

function ConvertTo-ExactRepositoryPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    if ([string]::IsNullOrWhiteSpace($Path)) {
        throw 'REPOSITORY_PATH_EMPTY'
    }

    if (-not [IO.Path]::IsPathRooted($Path)) {
        throw 'REPOSITORY_PATH_NOT_ROOTED'
    }

    $fullPath = [IO.Path]::GetFullPath($Path)
    $pathRoot = [IO.Path]::GetPathRoot($fullPath)
    if ([string]::Equals(
            $fullPath,
            $pathRoot,
            [StringComparison]::OrdinalIgnoreCase)) {
        throw 'REPOSITORY_PATH_IS_ROOT'
    }

    [char[]]$trimCharacters = @(
        [IO.Path]::DirectorySeparatorChar,
        [IO.Path]::AltDirectorySeparatorChar
    )
    $normalizedPath = $fullPath.TrimEnd($trimCharacters)
    if (-not [string]::Equals(
            $normalizedPath,
            'C:\EtcProject\FamilyClaimRef',
            [StringComparison]::OrdinalIgnoreCase)) {
        throw 'REPOSITORY_PATH_EXACT_MATCH_FAILED'
    }

    return $normalizedPath
}

function Invoke-RepositoryGitRead {
    param(
        [Parameter(Mandatory)][string]$RepositoryRoot,
        [Parameter(Mandatory)][string[]]$Arguments
    )

    $normalizedRepositoryRoot =
        (ConvertTo-ExactRepositoryPath $RepositoryRoot).Replace('\', '/')
    if ($normalizedRepositoryRoot -cne 'C:/EtcProject/FamilyClaimRef') {
        throw 'GIT_TRUST_PREFLIGHT_FAILED'
    }

    $gitArguments = @(
        '-c',
        "safe.directory=$normalizedRepositoryRoot",
        '-c',
        'core.quotepath=false',
        '-C',
        $normalizedRepositoryRoot
    ) + $Arguments

    foreach ($argument in $gitArguments) {
        if ([string]::IsNullOrWhiteSpace($argument) -or
            $argument.IndexOf('"', [StringComparison]::Ordinal) -ge 0 -or
            $argument.IndexOfAny([char[]]@("`r", "`n", [char]0)) -ge 0) {
            throw 'NATIVE_GIT_ARGUMENT_REJECTED'
        }
    }

    $startInfo = [Diagnostics.ProcessStartInfo]::new()
    $startInfo.FileName = 'git.exe'
    $startInfo.Arguments = (
        $gitArguments |
            ForEach-Object {
                if ($_ -match '\s') {
                    '"' + $_.Replace('"', '\"') + '"'
                }
                else {
                    $_
                }
            }
    ) -join ' '
    $startInfo.WorkingDirectory = $normalizedRepositoryRoot.Replace('/', '\')
    $startInfo.UseShellExecute = $false
    $startInfo.CreateNoWindow = $true
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true

    $script:nativeGitInvocationCount++
    $nativeProcess = [Diagnostics.Process]::new()
    $nativeProcess.StartInfo = $startInfo
    try {
        if (-not $nativeProcess.Start()) {
            return [pscustomobject]@{
                ExitCode = -1
                StdoutLines = @()
                AcceptedWarningCount = 0
                UnclassifiedStderrCount = 0
                FixedErrorCode = 'NATIVE_GIT_START_FAILED'
            }
        }

        $stdoutTask = $nativeProcess.StandardOutput.ReadToEndAsync()
        $stderrTask = $nativeProcess.StandardError.ReadToEndAsync()
        $nativeProcess.WaitForExit()
        $stdoutText = $stdoutTask.Result
        $stderrText = $stderrTask.Result
        $stdoutLines = @(
            $stdoutText -split "`r?`n" |
                Where-Object { $_.Length -gt 0 }
        )
        $stderrLines = @(
            $stderrText -split "`r?`n" |
                Where-Object { $_.Length -gt 0 }
        )
        $acceptedWarningCount = 0
        $unclassifiedStderrCount = 0
        foreach ($stderrLine in $stderrLines) {
            if ($stderrLine -match
                "^warning: in the working copy of '.+', (LF will be replaced by CRLF|CRLF will be replaced by LF) the next time Git touches it$") {
                $acceptedWarningCount++
            }
            else {
                $unclassifiedStderrCount++
            }
        }

        return [pscustomobject]@{
            ExitCode = $nativeProcess.ExitCode
            StdoutLines = $stdoutLines
            AcceptedWarningCount = $acceptedWarningCount
            UnclassifiedStderrCount = $unclassifiedStderrCount
            FixedErrorCode =
                if ($nativeProcess.ExitCode -eq 0 -and
                    $unclassifiedStderrCount -eq 0) {
                    $null
                }
                else {
                    'NATIVE_GIT_AUDIT_FAILED'
                }
        }
    }
    catch {
        return [pscustomobject]@{
            ExitCode = -1
            StdoutLines = @()
            AcceptedWarningCount = 0
            UnclassifiedStderrCount = 1
            FixedErrorCode = 'NATIVE_GIT_CAPTURE_FAILED'
        }
    }
    finally {
        $nativeProcess.Dispose()
    }
}

function Get-CanonicalRepositorySnapshot {
    param(
        [Parameter(Mandatory)][string]$RepositoryRoot,
        [Parameter(Mandatory)][string]$ExcludedRelativePath
    )

    $statusResult = Invoke-RepositoryGitRead `
        $RepositoryRoot `
        @('status', '--short', '--untracked-files=all')
    if ($statusResult.ExitCode -ne 0 -or
        $statusResult.UnclassifiedStderrCount -ne 0) {
        throw 'NATIVE_GIT_STATUS_FAILED'
    }
    $lines = @($statusResult.StdoutLines)
    $protectedPaths = [Collections.Generic.List[string]]::new()
    $entries = [Collections.Generic.Dictionary[string,object]]::new(
        [StringComparer]::Ordinal)
    $contentMap = [Collections.Generic.Dictionary[string,string]]::new(
        [StringComparer]::Ordinal)
    $rawStatusCount = 0
    $exclusionCount = 0

    foreach ($rawLine in $lines) {
        $line = [string]$rawLine
        if ([string]::IsNullOrEmpty($line)) {
            continue
        }

        $rawStatusCount++
        if ($line.Length -lt 4 -or $line[2] -ne ' ') {
            throw 'BASELINE_STATUS_LINE_INVALID'
        }

        $statusCode = $line.Substring(0, 2)
        if ($statusCode.IndexOf('R') -ge 0 -or
            $statusCode.IndexOf('C') -ge 0) {
            throw 'BASELINE_RENAME_COPY_STATUS_REJECTED'
        }

        $relativePath = $line.Substring(3).Replace('\', '/')
        if ([string]::IsNullOrWhiteSpace($relativePath) -or
            $relativePath.StartsWith('./', [StringComparison]::Ordinal) -or
            $relativePath.StartsWith('/', [StringComparison]::Ordinal) -or
            [IO.Path]::IsPathRooted($relativePath) -or
            $relativePath.IndexOf("`r", [StringComparison]::Ordinal) -ge 0 -or
            $relativePath.IndexOf("`n", [StringComparison]::Ordinal) -ge 0 -or
            $relativePath.IndexOf([char]0) -ge 0 -or
            @($relativePath.Split('/') | Where-Object { $_ -eq '..' }).Count -ne 0) {
            throw 'BASELINE_STATUS_PATH_INVALID'
        }

        if ($relativePath.Equals(
                $ExcludedRelativePath,
                [StringComparison]::Ordinal)) {
            $exclusionCount++
            continue
        }

        if ($entries.ContainsKey($relativePath)) {
            throw 'BASELINE_DUPLICATE_PATH_REJECTED'
        }

        $state = if ($statusCode -ceq '??') { 'untracked' } else { 'tracked' }
        $fullPath = Join-Path $RepositoryRoot ($relativePath.Replace('/', '\'))
        if (-not (Test-Path -LiteralPath $fullPath -PathType Leaf)) {
            throw 'BASELINE_MANIFEST_PATH_MISSING'
        }

        $bytes = [long](Get-Item -LiteralPath $fullPath).Length
        $sha256 = Get-FileSha256 $fullPath
        [void]$protectedPaths.Add($relativePath)
        $entries.Add(
            $relativePath,
            [pscustomobject]@{
                relativePath = $relativePath
                state = $state
                bytes = $bytes
                sha256 = $sha256
            })
        $contentMap.Add($relativePath, $sha256)
    }

    [string[]]$orderedPaths = @($protectedPaths.ToArray())
    [Array]::Sort($orderedPaths, [StringComparer]::Ordinal)

    $pathPayloadBuilder = [Text.StringBuilder]::new()
    $manifestPayloadBuilder = [Text.StringBuilder]::new()
    foreach ($relativePath in $orderedPaths) {
        $entry = $entries[$relativePath]
        [void]$pathPayloadBuilder.Append($relativePath).Append("`n")
        [void]$manifestPayloadBuilder.Append(
            "$relativePath`t$($entry.state)`t$($entry.bytes)`t$($entry.sha256)`n")
    }

    $utf8 = [Text.UTF8Encoding]::new($false)
    $sha = [Security.Cryptography.SHA256]::Create()
    try {
        $pathSetHash = $sha.ComputeHash(
            $utf8.GetBytes($pathPayloadBuilder.ToString()))
        $pathSetSha256 =
            [BitConverter]::ToString($pathSetHash).Replace('-', '').ToLowerInvariant()

        $sha.Initialize()
        $manifestHash = $sha.ComputeHash(
            $utf8.GetBytes($manifestPayloadBuilder.ToString()))
        $t0AggregateSha256 =
            [BitConverter]::ToString($manifestHash).Replace('-', '').ToLowerInvariant()
    }
    finally {
        $sha.Dispose()
    }

    return [pscustomobject]@{
        RawStatusCount = $rawStatusCount
        ExclusionCount = $exclusionCount
        ProtectedPathCount = $orderedPaths.Count
        OrderedPaths = $orderedPaths
        PathSetSha256 = $pathSetSha256
        T0AggregateSha256 = $t0AggregateSha256
        Entries = $entries
        ContentMap = $contentMap
    }
}

function Get-RepositoryContentMismatchCount {
    param(
        [Parameter(Mandatory)][string]$RepositoryRoot,
        [Parameter(Mandatory)][object]$BaselineMap
    )

    $mismatchCount = 0
    foreach ($entry in $BaselineMap.GetEnumerator()) {
        $fullPath = Join-Path $RepositoryRoot ($entry.Key.Replace('/', '\'))
        if (-not (Test-Path -LiteralPath $fullPath -PathType Leaf) -or
            (Get-FileSha256 $fullPath) -ne $entry.Value) {
            $mismatchCount++
        }
    }

    return $mismatchCount
}

function Test-SameOrChildPath {
    param(
        [Parameter(Mandatory)][string]$Candidate,
        [Parameter(Mandatory)][string]$Parent
    )

    [char[]]$trimCharacters = @(
        [IO.Path]::DirectorySeparatorChar,
        [IO.Path]::AltDirectorySeparatorChar
    )
    $normalizedCandidate =
        [IO.Path]::GetFullPath($Candidate).TrimEnd($trimCharacters)
    $normalizedParent =
        [IO.Path]::GetFullPath($Parent).TrimEnd($trimCharacters)
    if ($normalizedCandidate.Equals(
            $normalizedParent,
            [StringComparison]::OrdinalIgnoreCase)) {
        return $true
    }

    return $normalizedCandidate.StartsWith(
        "$normalizedParent$([IO.Path]::DirectorySeparatorChar)",
        [StringComparison]::OrdinalIgnoreCase)
}

function Test-StrictChildPath {
    param(
        [Parameter(Mandatory)][string]$Candidate,
        [Parameter(Mandatory)][string]$Parent
    )

    [char[]]$trimCharacters = @(
        [IO.Path]::DirectorySeparatorChar,
        [IO.Path]::AltDirectorySeparatorChar
    )
    $normalizedCandidate =
        [IO.Path]::GetFullPath($Candidate).TrimEnd($trimCharacters)
    $normalizedParent =
        [IO.Path]::GetFullPath($Parent).TrimEnd($trimCharacters)
    return -not $normalizedCandidate.Equals(
        $normalizedParent,
        [StringComparison]::OrdinalIgnoreCase) -and
        (Test-SameOrChildPath $normalizedCandidate $normalizedParent)
}

function Test-NoReparseTree {
    param([Parameter(Mandatory)][string]$Root)

    if (-not (Test-Path -LiteralPath $Root -PathType Container)) {
        return $false
    }

    $pending = [Collections.Generic.Stack[string]]::new()
    $pending.Push([IO.Path]::GetFullPath($Root))
    while ($pending.Count -gt 0) {
        $current = $pending.Pop()
        $currentAttributes = [IO.File]::GetAttributes($current)
        if (($currentAttributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
            return $false
        }

        foreach ($entry in @(Get-ChildItem -LiteralPath $current -Force)) {
            $attributes = [IO.File]::GetAttributes($entry.FullName)
            if (($attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
                return $false
            }

            if (($attributes -band [IO.FileAttributes]::Directory) -ne 0) {
                $pending.Push($entry.FullName)
            }
        }
    }

    return $true
}

function Remove-ExactOwnedRoot {
    param(
        [Parameter(Mandatory)][string]$Root,
        [Parameter(Mandatory)][string]$AllowedParent,
        [Parameter(Mandatory)][string]$OwnerTokenPath,
        [Parameter(Mandatory)][string]$ExpectedOwnerToken
    )

    if (-not (Test-Path -LiteralPath $Root -PathType Container)) {
        return $true
    }

    $normalizedRoot = [IO.Path]::GetFullPath($Root)
    if (-not (Test-StrictChildPath $normalizedRoot $AllowedParent) -or
        -not (Test-Path -LiteralPath $OwnerTokenPath -PathType Leaf) -or
        [IO.File]::ReadAllText($OwnerTokenPath) -ne $ExpectedOwnerToken -or
        -not (Test-NoReparseTree $normalizedRoot)) {
        return $false
    }

    Remove-Item -LiteralPath $normalizedRoot -Recurse -Force
    return -not (Test-Path -LiteralPath $normalizedRoot)
}

function Read-LiveLogLines {
    param([Parameter(Mandatory)][string]$Path)

    try {
        $stream = [IO.FileStream]::new(
            $Path,
            [IO.FileMode]::Open,
            [IO.FileAccess]::Read,
            [IO.FileShare]::ReadWrite)
        try {
            $reader = [IO.StreamReader]::new(
                $stream,
                [Text.UTF8Encoding]::new($false),
                $true)
            try {
                $text = $reader.ReadToEnd()
            }
            finally {
                $reader.Dispose()
            }
        }
        finally {
            $stream.Dispose()
        }

        return @($text -split "`n" | Where-Object { $_.Length -gt 0 })
    }
    catch {
        return @()
    }
}

function Stop-CapturedProcessFallback {
    param(
        [Parameter(Mandatory)]
        [Diagnostics.Process]$CapturedProcess
    )

    if ($CapturedProcess.HasExited) {
        return
    }

    $CapturedProcess.Kill()
    $CapturedProcess.WaitForExit(10000) | Out-Null
}

function Test-ExpectedRecords {
    param(
        [Parameter(Mandatory)][object[]]$Records,
        [Parameter(Mandatory)][object[]]$ExpectedRecords
    )

    $cursor = -1
    $missing = [Collections.Generic.List[string]]::new()
    foreach ($expected in $ExpectedRecords) {
        $foundIndex = -1
        for ($index = $cursor + 1; $index -lt $Records.Count; $index++) {
            $record = $Records[$index]
            if ($record.owner -eq $expected.owner -and
                $record.milestone -eq $expected.milestone -and
                $record.phase -eq $expected.phase -and
                $record.result -eq $expected.result) {
                $foundIndex = $index
                break
            }
        }

        if ($foundIndex -lt 0) {
            $missing.Add(
                "$($expected.owner)|$($expected.milestone)|$($expected.phase)|$($expected.result)")
        }
        else {
            $cursor = $foundIndex
        }
    }

    return @($missing)
}

$repositoryRoot = [IO.Path]::GetFullPath(
    (Join-Path $PSScriptRoot '..\..\..'))
$executableRelativePath =
    'app/FamilyClaimRef.App/bin/Debug/net10.0-windows/FamilyClaimRef.App.exe'
$executablePath = Join-Path $repositoryRoot ($executableRelativePath.Replace('/', '\'))
$workingDirectory = Split-Path -Parent $executablePath
$observationPath = Join-Path $repositoryRoot ($observationRelativePath.Replace('/', '\'))
$logEvidencePath = Join-Path $repositoryRoot ($logEvidenceRelativePath.Replace('/', '\'))
$harnessPath = $PSCommandPath
$priorHoldDocumentPath =
    Join-Path $repositoryRoot ($priorHoldDocumentRelativePath.Replace('/', '\'))
$priorHarnessPath =
    Join-Path $repositoryRoot ($priorHarnessRelativePath.Replace('/', '\'))
$priorHold435Path =
    Join-Path $repositoryRoot ($priorHold435RelativePath.Replace('/', '\'))
$priorRepair1HarnessPath =
    Join-Path $repositoryRoot ($priorRepair1HarnessRelativePath.Replace('/', '\'))
$priorHold436Path =
    Join-Path $repositoryRoot ($priorHold436RelativePath.Replace('/', '\'))
$priorRepair2HarnessPath =
    Join-Path $repositoryRoot ($priorRepair2HarnessRelativePath.Replace('/', '\'))
$priorHold437Path =
    Join-Path $repositoryRoot ($priorHold437RelativePath.Replace('/', '\'))
$priorRepair3HarnessPath =
    Join-Path $repositoryRoot ($priorRepair3HarnessRelativePath.Replace('/', '\'))
$priorRepair3LogPath =
    Join-Path $repositoryRoot ($priorRepair3LogRelativePath.Replace('/', '\'))

$branchGitResult =
    Invoke-RepositoryGitRead $repositoryRoot @('branch', '--show-current')
$headGitResult =
    Invoke-RepositoryGitRead $repositoryRoot @('rev-parse', 'HEAD')
if ($branchGitResult.ExitCode -ne 0 -or
    $branchGitResult.UnclassifiedStderrCount -ne 0 -or
    $headGitResult.ExitCode -ne 0 -or
    $headGitResult.UnclassifiedStderrCount -ne 0 -or
    @($branchGitResult.StdoutLines).Count -ne 1 -or
    @($headGitResult.StdoutLines).Count -ne 1) {
    throw 'NATIVE_GIT_BRANCH_HEAD_FAILED'
}
$branch = @($branchGitResult.StdoutLines)[0]
$head = @($headGitResult.StdoutLines)[0]
$baselineSnapshot =
    Get-CanonicalRepositorySnapshot $repositoryRoot $harnessRelativePath
$baselinePaths = @($baselineSnapshot.OrderedPaths)
$baselineT0AggregateSha256 = $baselineSnapshot.T0AggregateSha256
$baselineContentMap = $baselineSnapshot.ContentMap
$baselineEntries = $baselineSnapshot.Entries

$priorIdentityPassed = (
    (Test-Path -LiteralPath $priorHoldDocumentPath -PathType Leaf) -and
    (Get-Item -LiteralPath $priorHoldDocumentPath).Length -eq
        $expectedPriorHoldDocumentBytes -and
    (Get-FileSha256 $priorHoldDocumentPath) -eq
        $expectedPriorHoldDocumentSha256 -and
    (Test-Path -LiteralPath $priorHarnessPath -PathType Leaf) -and
    (Get-Item -LiteralPath $priorHarnessPath).Length -eq
        $expectedPriorHarnessBytes -and
    (Get-FileSha256 $priorHarnessPath) -eq
        $expectedPriorHarnessSha256 -and
    (Test-Path -LiteralPath $priorHold435Path -PathType Leaf) -and
    (Get-Item -LiteralPath $priorHold435Path).Length -eq
        $expectedPriorHold435Bytes -and
    (Get-FileSha256 $priorHold435Path) -eq
        $expectedPriorHold435Sha256 -and
    (Test-Path -LiteralPath $priorRepair1HarnessPath -PathType Leaf) -and
    (Get-Item -LiteralPath $priorRepair1HarnessPath).Length -eq
        $expectedPriorRepair1HarnessBytes -and
    (Get-FileSha256 $priorRepair1HarnessPath) -eq
        $expectedPriorRepair1HarnessSha256 -and
    (Test-Path -LiteralPath $priorHold436Path -PathType Leaf) -and
    (Get-Item -LiteralPath $priorHold436Path).Length -eq
        $expectedPriorHold436Bytes -and
    (Get-FileSha256 $priorHold436Path) -eq
        $expectedPriorHold436Sha256 -and
    (Test-Path -LiteralPath $priorRepair2HarnessPath -PathType Leaf) -and
    (Get-Item -LiteralPath $priorRepair2HarnessPath).Length -eq
        $expectedPriorRepair2HarnessBytes -and
    (Get-FileSha256 $priorRepair2HarnessPath) -eq
        $expectedPriorRepair2HarnessSha256 -and
    (Test-Path -LiteralPath $priorHold437Path -PathType Leaf) -and
    (Get-Item -LiteralPath $priorHold437Path).Length -eq
        $expectedPriorHold437Bytes -and
    (Get-FileSha256 $priorHold437Path) -eq
        $expectedPriorHold437Sha256 -and
    (Test-Path -LiteralPath $priorRepair3HarnessPath -PathType Leaf) -and
    (Get-Item -LiteralPath $priorRepair3HarnessPath).Length -eq
        $expectedPriorRepair3HarnessBytes -and
    (Get-FileSha256 $priorRepair3HarnessPath) -eq
        $expectedPriorRepair3HarnessSha256 -and
    (Test-Path -LiteralPath $priorRepair3LogPath -PathType Leaf) -and
    (Get-Item -LiteralPath $priorRepair3LogPath).Length -eq
        $expectedPriorRepair3LogBytes -and
    (Get-FileSha256 $priorRepair3LogPath) -eq
        $expectedPriorRepair3LogSha256
)

$executableIdentityPassed = (
    (Test-Path -LiteralPath $executablePath -PathType Leaf) -and
    (Get-Item -LiteralPath $executablePath).Length -eq $expectedExecutableBytes -and
    (Get-FileSha256 $executablePath) -eq $expectedExecutableSha256
)

$productProcessesBefore = @(
    Get-Process -Name 'FamilyClaimRef.App' -ErrorAction SilentlyContinue
)

if ($GitAuditProbeOnly) {
    $gitAuditProbeResult =
        Invoke-RepositoryGitRead $repositoryRoot @('diff', '--check')
    $probeFinalSnapshot =
        Get-CanonicalRepositorySnapshot $repositoryRoot $harnessRelativePath
    $probeFinalBranchResult =
        Invoke-RepositoryGitRead $repositoryRoot @('branch', '--show-current')
    $probeFinalHeadResult =
        Invoke-RepositoryGitRead $repositoryRoot @('rev-parse', 'HEAD')
    $probeFinalBranch =
        if (@($probeFinalBranchResult.StdoutLines).Count -eq 1) {
            @($probeFinalBranchResult.StdoutLines)[0]
        }
        else {
            $null
        }
    $probeFinalHead =
        if (@($probeFinalHeadResult.StdoutLines).Count -eq 1) {
            @($probeFinalHeadResult.StdoutLines)[0]
        }
        else {
            $null
        }
    $productProcessesAfterProbe = @(
        Get-Process -Name 'FamilyClaimRef.App' -ErrorAction SilentlyContinue
    )
    $probeRepositoryMismatchCount =
        Get-RepositoryContentMismatchCount $repositoryRoot $baselineContentMap
    $probePassed = (
        $branch -eq 'main' -and
        $head -eq $expectedHead -and
        $baselineSnapshot.RawStatusCount -eq 64 -and
        $baselineSnapshot.ExclusionCount -eq 1 -and
        $baselineSnapshot.ProtectedPathCount -eq 63 -and
        $baselineSnapshot.PathSetSha256 -eq $expectedPathSetSha256 -and
        $baselineSnapshot.T0AggregateSha256 -eq $expectedT0AggregateSha256 -and
        $priorIdentityPassed -and
        $executableIdentityPassed -and
        $gitAuditProbeResult.ExitCode -eq 0 -and
        @($gitAuditProbeResult.StdoutLines).Count -eq 0 -and
        $gitAuditProbeResult.UnclassifiedStderrCount -eq 0 -and
        $productProcessesBefore.Count -eq 0 -and
        $productProcessesAfterProbe.Count -eq 0 -and
        $probeFinalBranchResult.ExitCode -eq 0 -and
        $probeFinalBranchResult.UnclassifiedStderrCount -eq 0 -and
        $probeFinalHeadResult.ExitCode -eq 0 -and
        $probeFinalHeadResult.UnclassifiedStderrCount -eq 0 -and
        $probeFinalBranch -eq $branch -and
        $probeFinalHead -eq $head -and
        $probeFinalSnapshot.RawStatusCount -eq
            $baselineSnapshot.RawStatusCount -and
        $probeFinalSnapshot.ExclusionCount -eq
            $baselineSnapshot.ExclusionCount -and
        $probeFinalSnapshot.ProtectedPathCount -eq
            $baselineSnapshot.ProtectedPathCount -and
        $probeFinalSnapshot.PathSetSha256 -eq
            $baselineSnapshot.PathSetSha256 -and
        $probeFinalSnapshot.T0AggregateSha256 -eq
            $baselineSnapshot.T0AggregateSha256 -and
        $probeRepositoryMismatchCount -eq 0
    )
    $probeResult = [ordered]@{
        mode = 'GitAuditProbeOnly'
        baselineCalculatorMode =
            'git_status_exact_parser_ordinal_utf8_lf_sha256'
        baselineCalculatorImplementationCount = 1
        rawStatusCount = $baselineSnapshot.RawStatusCount
        harnessExclusionCount = $baselineSnapshot.ExclusionCount
        protectedPathCount = $baselineSnapshot.ProtectedPathCount
        expectedPathSetSha256 = $expectedPathSetSha256
        actualPathSetSha256 = $baselineSnapshot.PathSetSha256
        expectedT0AggregateSha256 = $expectedT0AggregateSha256
        actualT0AggregateSha256 = $baselineSnapshot.T0AggregateSha256
        gitDiffCheckExitCode = $gitAuditProbeResult.ExitCode
        gitDiffCheckWhitespaceErrorCount =
            @($gitAuditProbeResult.StdoutLines).Count
        gitDiffCheckAcceptedWarningCount =
            $gitAuditProbeResult.AcceptedWarningCount
        gitDiffCheckUnclassifiedStderrCount =
            $gitAuditProbeResult.UnclassifiedStderrCount
        nativeGitInvocationCount = $script:nativeGitInvocationCount
        ordinalComparerUsed = $true
        cultureSensitiveSortUsed = $false
        terminalLfUsed = $true
        utf8BomUsed = $false
        branchMatches = ($branch -eq 'main')
        headMatches = ($head -eq $expectedHead)
        priorIdentitiesPassed = $priorIdentityPassed
        executableIdentityPassed = $executableIdentityPassed
        productProcessCountBefore = $productProcessesBefore.Count
        productProcessCountAfter = $productProcessesAfterProbe.Count
        diagnosticRootCreationCount = 0
        isolatedRuntimeRootCreationCount = 0
        repositoryContentMismatchCount = $probeRepositoryMismatchCount
        gitConfigMutationCount = 0
        fixedErrorCode =
            if ($probePassed) { $null } else { 'GIT_AUDIT_PROBE_FAILED' }
        pass = $probePassed
    }
    $probeResult | ConvertTo-Json -Depth 6 -Compress
    if ($probePassed) {
        exit 0
    }
    exit 2
}

if ($branch -ne 'main' -or
    $head -ne $expectedHead -or
    $baselineSnapshot.RawStatusCount -ne 64 -or
    $baselineSnapshot.ExclusionCount -ne 1 -or
    $baselineSnapshot.ProtectedPathCount -ne 63 -or
    $baselineSnapshot.PathSetSha256 -ne $expectedPathSetSha256 -or
    $baselineSnapshot.T0AggregateSha256 -ne $expectedT0AggregateSha256 -or
    -not $priorIdentityPassed -or
    -not $executableIdentityPassed -or
    $productProcessesBefore.Count -ne 0) {
    throw 'RUNTIME_BASELINE_GATE_FAILED'
}

$runtimeGitPreflightAudit =
    Invoke-RepositoryGitRead $repositoryRoot @('diff', '--check')
if ($runtimeGitPreflightAudit.ExitCode -ne 0 -or
    @($runtimeGitPreflightAudit.StdoutLines).Count -ne 0 -or
    $runtimeGitPreflightAudit.UnclassifiedStderrCount -ne 0) {
    throw 'RUNTIME_GIT_AUDIT_PREFLIGHT_FAILED'
}

$terminalPhase = 'RUNTIME_INITIALIZED'
$fixedErrorCode = $null
$observationWriteAttempted = $false
$observationWriteSucceeded = $false
$approvedParent = $null
$approvedParentEntriesBefore = @()
$approvedParentEntriesAfter = @()
$runId = $null
$ownerToken = $null
$diagnosticRoot = $null
$runtimeRoot = $null
$diagnosticOwnerTokenPath = $null
$runtimeOwnerTokenPath = $null
$diagnosticCleanupSucceeded = $false
$runtimeCleanupSucceeded = $false
$diagnosticRootResidueCount = 0
$isolatedRuntimeRootResidueCount = 0
$unrelatedTempDeltaCount = 0
$productStartAttemptCount = 0
$productProcessCreatedCount = 0
$secondStartAttemptCount = 0
$capturedPid = $null
$firstWindowObservedUtc = $null
$diagnosticLogFirstObservedUtc = $null
$mainWindowObserved = $false
$mainWindowOwnedByCapturedPid = $false
$diagnosticLogObserved = $false
$processExitedDuringStartupObservation = $false
$gracefulCloseRequestedCount = 0
$gracefulExit = $false
$fallbackTerminationUsed = $false
$terminationMode = 'not_started'
$exitCode = $null
$process = $null
$windowHandle = [IntPtr]::Zero
$windowTitle = $null
$windowClassName = $null
$windowOwnerPid = $null
$harnessFailureCategory = $null
$records = @()
$logLines = @()
$logBytes = 0L
$logSha256 = $null
$privacyValidation = 'NOT_RUN'
$sizeValidation = 'NOT_RUN'
$sequenceValidation = 'NOT_RUN'
$jsonValidation = 'NOT_RUN'
$expectedMilestoneValidation = 'NOT_RUN'
$missingMilestones = @()
$allowlistViolationCount = 0
$privacyFindingCount = 0
$diagnosticLogFileCount = 0
$logEvidenceCopied = $false
$logEvidenceIdentityMatches = $false
$productProcessesAfter = @()
$repositoryExistingPathMismatchCount = -1
$finalBranch = $null
$finalHead = $null
$finalPathSetMatches = $false
$finalSnapshot = $null
$finalBaselineEntryMismatchCount = -1
$finalAllowedAdditionCount = 0
$finalUnexpectedPathCount = -1
$finalT0AggregateSha256 = $null
$finalT0ManifestMatches = $false
$runtimeGitAuditResult = $null
$gitDiffCheckPassed = $false
$orderedMilestoneSummary = @()
$pass = $false
$harnessSha256 = Get-FileSha256 $harnessPath
$startUtc = [DateTimeOffset]::UtcNow
$observationDeadlineUtc = $startUtc.AddSeconds(30)

try {
$tempRoot = [IO.Path]::GetFullPath([IO.Path]::GetTempPath())
$approvedParent = [IO.Path]::GetFullPath(
    (Join-Path $tempRoot 'FamilyClaimRef\StartupDiagnostics'))
if (-not (Test-Path -LiteralPath $approvedParent -PathType Container) -or
    ([IO.File]::GetAttributes($approvedParent) -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
    throw 'Approved TEMP parent is unavailable or reparse-backed.'
}
$approvedParentEntriesBefore = @(
    Get-ChildItem -LiteralPath $approvedParent -Force |
        ForEach-Object { $_.Name } |
        Sort-Object
)

$runId = Get-RandomHex 16
$nonce = Get-RandomHex 32
$diagnosticRoot = [IO.Path]::GetFullPath(
    (Join-Path $approvedParent "$runId-diagnostic"))
$runtimeRoot = [IO.Path]::GetFullPath(
    (Join-Path $approvedParent "$runId-runtime"))
if (-not (Test-StrictChildPath $diagnosticRoot $approvedParent) -or
    -not (Test-StrictChildPath $runtimeRoot $approvedParent) -or
    (Test-Path -LiteralPath $diagnosticRoot) -or
    (Test-Path -LiteralPath $runtimeRoot)) {
    throw 'Generated test roots failed the preexistence or boundary gate.'
}

$ownerToken = [ordered]@{
    schemaVersion = 1
    runId = $runId
    nonce = $nonce
    purpose = 'gate8-single-diagnostic-startup'
} | ConvertTo-Json -Compress

[IO.Directory]::CreateDirectory($diagnosticRoot) | Out-Null
[IO.Directory]::CreateDirectory($runtimeRoot) | Out-Null
$diagnosticOwnerTokenPath = Join-Path $diagnosticRoot '.gate8-owner.json'
$runtimeOwnerTokenPath = Join-Path $runtimeRoot '.gate8-owner.json'
Write-Utf8NoBom $diagnosticOwnerTokenPath $ownerToken
Write-Utf8NoBom $runtimeOwnerTokenPath $ownerToken

if (-not (Test-NoReparseTree $diagnosticRoot) -or
    -not (Test-NoReparseTree $runtimeRoot)) {
    throw 'Prepared test root contains a reparse point.'
}

$logPath = Join-Path $diagnosticRoot 'startup.ndjson'
if (Test-Path -LiteralPath $logPath) {
    throw 'Diagnostic log unexpectedly preexists.'
}

$expectedRecords = @(
    [pscustomobject]@{ owner='App'; milestone='app_constructor.body_enter'; phase='enter'; result='started' },
    [pscustomobject]@{ owner='App'; milestone='app_constructor.body_ready'; phase='return'; result='completed' },
    [pscustomobject]@{ owner='App'; milestone='app_on_startup.enter'; phase='enter'; result='started' },
    [pscustomobject]@{ owner='App'; milestone='base_on_startup'; phase='begin'; result='started' },
    [pscustomobject]@{ owner='App'; milestone='base_on_startup'; phase='end'; result='completed' },
    [pscustomobject]@{ owner='App'; milestone='startup_mode.selection'; phase='decision'; result='default' },
    [pscustomobject]@{ owner='App'; milestone='app_services_create_default'; phase='begin'; result='started' },
    [pscustomobject]@{ owner='App'; milestone='app_services_create_default'; phase='end'; result='completed' },
    [pscustomobject]@{ owner='App'; milestone='product_shell_window.construction'; phase='begin'; result='started' },
    [pscustomobject]@{ owner='ProductShellWindow'; milestone='product_shell_window.constructor'; phase='enter'; result='started' },
    [pscustomobject]@{ owner='ProductShellWindow'; milestone='product_shell_window.initialize_component'; phase='begin'; result='started' },
    [pscustomobject]@{ owner='ProductShellWindow'; milestone='product_shell_window.initialize_component'; phase='end'; result='completed' },
    [pscustomobject]@{ owner='ProductShellWindow'; milestone='product_shell_window.data_context_assignment'; phase='begin'; result='started' },
    [pscustomobject]@{ owner='ProductShellWindow'; milestone='product_shell_window.data_context_assignment'; phase='end'; result='completed' },
    [pscustomobject]@{ owner='ProductShellWindow'; milestone='product_shell_window.constructor'; phase='return'; result='completed' },
    [pscustomobject]@{ owner='App'; milestone='product_shell_window.construction'; phase='end'; result='completed' },
    [pscustomobject]@{ owner='App'; milestone='application.main_window_assignment'; phase='end'; result='completed' },
    [pscustomobject]@{ owner='App'; milestone='product_shell_window.show'; phase='begin'; result='started' },
    [pscustomobject]@{ owner='ProductShellWindow'; milestone='product_shell_window.loaded'; phase='event'; result='observed' },
    [pscustomobject]@{ owner='App'; milestone='product_shell_window.show'; phase='return'; result='completed' },
    [pscustomobject]@{ owner='ProductShellWindow'; milestone='product_shell_window.dispatcher_callback'; phase='callback'; result='scheduled' },
    [pscustomobject]@{ owner='ProductShellWindow'; milestone='product_shell_window.content_rendered'; phase='event'; result='observed' },
    [pscustomobject]@{ owner='ProductShellWindow'; milestone='product_shell_window.dispatcher_callback'; phase='callback'; result='executed' },
    [pscustomobject]@{ owner='ProductShellWindow'; milestone='product_shell_window.closed'; phase='event'; result='observed' },
    [pscustomobject]@{ owner='App'; milestone='app_on_exit'; phase='enter'; result='started' },
    [pscustomobject]@{ owner='App'; milestone='app_on_exit'; phase='return'; result='completed' }
)

$allowedOwners = @('App', 'ProductShellWindow', 'StartupDiagnosticSession')
$allowedMilestones = @(
    'app_constructor.body_enter',
    'startup_diagnostics.handler_registration',
    'app_constructor.body_ready',
    'app_on_startup.enter',
    'base_on_startup',
    'startup_mode.selection',
    'app_services_create_default',
    'product_shell_window.construction',
    'application.main_window_assignment',
    'product_shell_window.show',
    'app_on_startup.exception',
    'app_on_exit',
    'product_shell_window.constructor',
    'product_shell_window.initialize_component',
    'product_shell_window.data_context_assignment',
    'product_shell_window.loaded',
    'product_shell_window.content_rendered',
    'product_shell_window.dispatcher_callback',
    'product_shell_window.closed',
    'app_domain.unhandled_exception',
    'dispatcher.unhandled_exception',
    'task_scheduler.unobserved_task_exception'
)
$allowedPhases = @('begin', 'end', 'enter', 'return', 'event', 'decision', 'callback')
$allowedResults = @(
    'started', 'completed', 'enabled', 'disabled', 'default',
    'product_shell_preview', 'observed', 'scheduled', 'executed', 'failed'
)
$allowedMethods = @(
    'FamilyClaimRef.App.App..ctor',
    'FamilyClaimRef.App.App.OnStartup',
    'FamilyClaimRef.App.App.OnExit',
    'FamilyClaimRef.App.ProductShell.ProductShellWindow..ctor',
    'FamilyClaimRef.App.ProductShell.ProductShellWindow.ScheduleStartupDispatcherObservation',
    'FamilyClaimRef.App.Startup.StartupDiagnosticSession.RegisterHandlers',
    'FamilyClaimRef.App.Startup.StartupDiagnosticSession.OnAppDomainUnhandledException',
    'FamilyClaimRef.App.Startup.StartupDiagnosticSession.OnDispatcherUnhandledException',
    'FamilyClaimRef.App.Startup.StartupDiagnosticSession.OnTaskSchedulerUnobservedException'
)

$harnessSha256 = Get-FileSha256 $harnessPath
$productStartAttemptCount = 0
$productProcessCreatedCount = 0
$secondStartAttemptCount = 0
$capturedPid = $null
$startUtc = [DateTimeOffset]::UtcNow
$observationDeadlineUtc = $startUtc.AddSeconds(30)
$firstWindowObservedUtc = $null
$diagnosticLogFirstObservedUtc = $null
$mainWindowObserved = $false
$mainWindowOwnedByCapturedPid = $false
$diagnosticLogObserved = $false
$processExitedDuringStartupObservation = $false
$gracefulCloseRequestedCount = 0
$gracefulExit = $false
$fallbackTerminationUsed = $false
$terminationMode = 'not_started'
$exitCode = $null
$process = $null
$windowHandle = [IntPtr]::Zero
$windowTitle = $null
$windowClassName = $null
$windowOwnerPid = $null
$harnessFailureCategory = $null

$processStartInfo = [Diagnostics.ProcessStartInfo]::new()
$processStartInfo.FileName = $executablePath
$processStartInfo.WorkingDirectory = $workingDirectory
$processStartInfo.UseShellExecute = $false
$processStartInfo.Environment['FAMILYCLAIMREF_ENABLE_STARTUP_DIAGNOSTICS'] = '1'
$processStartInfo.Environment['FAMILYCLAIMREF_STARTUP_DIAGNOSTIC_ROOT'] = $diagnosticRoot
$processStartInfo.Environment['FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE'] = '1'
$processStartInfo.Environment['FAMILYCLAIMREF_RUNTIME_ROOT'] = $runtimeRoot

try {
    $productStartAttemptCount++
    $process = [Diagnostics.Process]::Start($processStartInfo)
    if ($null -ne $process) {
        $productProcessCreatedCount = 1
        $capturedPid = $process.Id
        $terminationMode = 'observing'
    }

    while ($null -ne $process -and [DateTimeOffset]::UtcNow -lt $observationDeadlineUtc) {
        if ($process.HasExited) {
            $processExitedDuringStartupObservation = $true
            $terminationMode = 'process_exited_during_observation'
            break
        }

        $process.Refresh()
        if (-not $mainWindowObserved -and $process.MainWindowHandle -ne [IntPtr]::Zero) {
            $windowHandle = $process.MainWindowHandle
            $windowTitle = $process.MainWindowTitle
            [uint32]$observedWindowOwnerPid = 0
            [void][Gate8WindowNativeMethods]::GetWindowThreadProcessId(
                $windowHandle,
                [ref]$observedWindowOwnerPid)
            $windowOwnerPid = [int]$observedWindowOwnerPid
            $windowClassBuilder = [Text.StringBuilder]::new(256)
            [void][Gate8WindowNativeMethods]::GetClassName(
                $windowHandle,
                $windowClassBuilder,
                $windowClassBuilder.Capacity)
            $windowClassName = $windowClassBuilder.ToString()
            $mainWindowObserved = $true
            $mainWindowOwnedByCapturedPid =
                $windowOwnerPid -eq $capturedPid
            $firstWindowObservedUtc = [DateTimeOffset]::UtcNow
        }

        if (Test-Path -LiteralPath $logPath -PathType Leaf) {
            if (-not $diagnosticLogObserved) {
                $diagnosticLogFirstObservedUtc = [DateTimeOffset]::UtcNow
            }
            $diagnosticLogObserved = $true
            $liveLines = @(Read-LiveLogLines $logPath)
            $dispatcherExecuted = $false
            foreach ($liveLine in $liveLines) {
                try {
                    $liveRecord = $liveLine | ConvertFrom-Json
                    if ($liveRecord.milestone -eq 'product_shell_window.dispatcher_callback' -and
                        $liveRecord.result -eq 'executed') {
                        $dispatcherExecuted = $true
                    }
                }
                catch {
                    $dispatcherExecuted = $false
                    break
                }
            }

            if ($mainWindowObserved -and $dispatcherExecuted) {
                break
            }
        }

        Start-Sleep -Milliseconds 100
    }

    if ($null -ne $process -and -not $process.HasExited -and $mainWindowObserved) {
        $gracefulCloseRequestedCount = 1
        $closeRequested = $process.CloseMainWindow()
        if ($closeRequested -and $process.WaitForExit(10000)) {
            $gracefulExit = $true
            $terminationMode = 'graceful_close'
        }
    }

    if ($null -ne $process -and -not $process.HasExited) {
        $fallbackTerminationUsed = $true
        $terminationMode = 'captured_pid_fallback'
        Stop-CapturedProcessFallback $process
    }

    if ($null -ne $process -and $process.HasExited) {
        $exitCode = $process.ExitCode
    }
}
catch {
    $harnessFailureCategory = 'PRODUCT_RUNTIME_FAILURE'
    if ($null -ne $process -and -not $process.HasExited) {
        $fallbackTerminationUsed = $true
        $terminationMode = 'captured_pid_exception_fallback'
        Stop-CapturedProcessFallback $process
    }

    if ($null -ne $process -and $process.HasExited) {
        $exitCode = $process.ExitCode
    }
}
finally {
    if ($null -ne $process) {
        $process.Dispose()
        $process = $null
    }
}

$records = @()
$logLines = @()
$logBytes = 0L
$logSha256 = $null
$privacyValidation = 'FAIL'
$sizeValidation = 'FAIL'
$sequenceValidation = 'FAIL'
$jsonValidation = 'FAIL'
$expectedMilestoneValidation = 'FAIL'
$missingMilestones = @()
$allowlistViolationCount = 0
$privacyFindingCount = 0
$diagnosticLogFileCount = 0
$logEvidenceCopied = $false
$logEvidenceIdentityMatches = $false

if (Test-Path -LiteralPath $logPath -PathType Leaf) {
    $diagnosticLogFileCount = @(
        Get-ChildItem -LiteralPath $diagnosticRoot -File -Force |
            Where-Object { $_.Name -eq 'startup.ndjson' }
    ).Count
    $logBytes = (Get-Item -LiteralPath $logPath).Length
    $logSha256 = Get-FileSha256 $logPath
    $logLines = @(
        [IO.File]::ReadAllLines(
            $logPath,
            [Text.UTF8Encoding]::new($false)) |
            Where-Object { $_.Length -gt 0 }
    )

    try {
        $records = @($logLines | ForEach-Object { $_ | ConvertFrom-Json })
        $jsonValidation = 'PASS'
    }
    catch {
        $records = @()
    }

    if ($logBytes -ge 1 -and $logBytes -le $maximumLogBytes) {
        $sizeValidation = 'PASS'
    }

    if ($records.Count -gt 0) {
        $sequenceValues = @($records | ForEach-Object { [long]$_.sequence })
        $expectedSequence = @(1..$records.Count | ForEach-Object { [long]$_ })
        if (($sequenceValues -join ',') -eq ($expectedSequence -join ',') -and
            @($sequenceValues | Select-Object -Unique).Count -eq $sequenceValues.Count) {
            $sequenceValidation = 'PASS'
        }

        foreach ($record in $records) {
            if ($record.owner -notin $allowedOwners -or
                $record.milestone -notin $allowedMilestones -or
                $record.phase -notin $allowedPhases -or
                $record.result -notin $allowedResults -or
                ($null -ne $record.methodIdentifier -and
                 $record.methodIdentifier -notin $allowedMethods)) {
                $allowlistViolationCount++
            }
        }

        $missingMilestones = @(
            Test-ExpectedRecords $records $expectedRecords
        )
        if ($missingMilestones.Count -eq 0) {
            $expectedMilestoneValidation = 'PASS'
        }
    }

    $logText = [IO.File]::ReadAllText(
        $logPath,
        [Text.UTF8Encoding]::new($false))
    $forbiddenValues = @(
        $runId,
        $nonce,
        'FAMILYCLAIMREF_ENABLE_STARTUP_DIAGNOSTICS',
        'FAMILYCLAIMREF_STARTUP_DIAGNOSTIC_ROOT',
        'FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE',
        'FAMILYCLAIMREF_RUNTIME_ROOT'
    )
    foreach ($forbiddenValue in $forbiddenValues) {
        if ($logText.IndexOf(
                $forbiddenValue,
                [StringComparison]::OrdinalIgnoreCase) -ge 0) {
            $privacyFindingCount++
        }
    }
    if ($logText -match '[A-Za-z]:\\' -or
        $logText -match '\\\\\?\\' -or
        $logText -match '(?i)exceptionMessage|stackTrace|attachment|claim_[A-Za-z0-9]') {
        $privacyFindingCount++
    }

    if ($jsonValidation -eq 'PASS' -and
        $allowlistViolationCount -eq 0 -and
        $privacyFindingCount -eq 0) {
        $privacyValidation = 'PASS'
    }
}

if ($privacyValidation -eq 'PASS' -and
    $sizeValidation -eq 'PASS' -and
    $sequenceValidation -eq 'PASS' -and
    $expectedMilestoneValidation -eq 'PASS' -and
    $diagnosticLogFileCount -eq 1) {
    [IO.File]::Copy($logPath, $logEvidencePath, $false)
    $logEvidenceCopied = $true
    $logEvidenceIdentityMatches =
        (Get-Item -LiteralPath $logEvidencePath).Length -eq $logBytes -and
        (Get-FileSha256 $logEvidencePath) -eq $logSha256
}

$diagnosticCleanupSucceeded = Remove-ExactOwnedRoot `
    $diagnosticRoot `
    $approvedParent `
    $diagnosticOwnerTokenPath `
    $ownerToken
$runtimeCleanupSucceeded = Remove-ExactOwnedRoot `
    $runtimeRoot `
    $approvedParent `
    $runtimeOwnerTokenPath `
    $ownerToken

$diagnosticRootResidueCount = if (Test-Path -LiteralPath $diagnosticRoot) { 1 } else { 0 }
$isolatedRuntimeRootResidueCount = if (Test-Path -LiteralPath $runtimeRoot) { 1 } else { 0 }
$approvedParentEntriesAfter = @(
    Get-ChildItem -LiteralPath $approvedParent -Force |
        ForEach-Object { $_.Name } |
        Sort-Object
)
$unrelatedTempDeltaCount = @(
    Compare-Object `
        -ReferenceObject $approvedParentEntriesBefore `
        -DifferenceObject $approvedParentEntriesAfter
).Count
$productProcessesAfter = @(
    Get-Process -Name 'FamilyClaimRef.App' -ErrorAction SilentlyContinue
)
$repositoryExistingPathMismatchCount =
    Get-RepositoryContentMismatchCount $repositoryRoot $baselineContentMap
$finalBranchResult =
    Invoke-RepositoryGitRead $repositoryRoot @('branch', '--show-current')
$finalHeadResult =
    Invoke-RepositoryGitRead $repositoryRoot @('rev-parse', 'HEAD')
$finalBranch =
    if (@($finalBranchResult.StdoutLines).Count -eq 1) {
        @($finalBranchResult.StdoutLines)[0]
    }
    else {
        $null
    }
$finalHead =
    if (@($finalHeadResult.StdoutLines).Count -eq 1) {
        @($finalHeadResult.StdoutLines)[0]
    }
    else {
        $null
    }
$finalSnapshot =
    Get-CanonicalRepositorySnapshot $repositoryRoot $harnessRelativePath
$finalPaths = @($finalSnapshot.OrderedPaths)
$baselinePathLookup = [Collections.Generic.HashSet[string]]::new(
    [StringComparer]::Ordinal)
foreach ($relativePath in $baselinePaths) {
    [void]$baselinePathLookup.Add($relativePath)
}
$finalBaselineEntryMismatchCount = 0
foreach ($relativePath in $baselinePaths) {
    if (-not $finalSnapshot.Entries.ContainsKey($relativePath)) {
        $finalBaselineEntryMismatchCount++
        continue
    }

    $baselineEntry = $baselineEntries[$relativePath]
    $finalEntry = $finalSnapshot.Entries[$relativePath]
    if ($finalEntry.state -cne $baselineEntry.state -or
        $finalEntry.bytes -ne $baselineEntry.bytes -or
        $finalEntry.sha256 -cne $baselineEntry.sha256) {
        $finalBaselineEntryMismatchCount++
    }
}
$finalAllowedAdditionCount = 0
$finalUnexpectedPathCount = 0
foreach ($relativePath in $finalPaths) {
    if ($baselinePathLookup.Contains($relativePath)) {
        continue
    }

    if ($logEvidenceCopied -and
        $relativePath.Equals(
            $logEvidenceRelativePath,
            [StringComparison]::Ordinal)) {
        $finalAllowedAdditionCount++
    }
    else {
        $finalUnexpectedPathCount++
    }
}
$expectedFinalAllowedAdditionCount = if ($logEvidenceCopied) { 1 } else { 0 }
$finalPathSetMatches = (
    $finalBaselineEntryMismatchCount -eq 0 -and
    $finalUnexpectedPathCount -eq 0 -and
    $finalAllowedAdditionCount -eq $expectedFinalAllowedAdditionCount -and
    $finalSnapshot.ExclusionCount -eq 1
)
$finalT0AggregateSha256 = $finalSnapshot.T0AggregateSha256
$finalT0ManifestMatches = $finalBaselineEntryMismatchCount -eq 0
$runtimeGitAuditResult =
    Invoke-RepositoryGitRead $repositoryRoot @('diff', '--check')
$gitDiffCheckPassed = (
    $runtimeGitAuditResult.ExitCode -eq 0 -and
    @($runtimeGitAuditResult.StdoutLines).Count -eq 0 -and
    $runtimeGitAuditResult.UnclassifiedStderrCount -eq 0
)

$orderedMilestoneSummary = @(
    $records | ForEach-Object {
        "$($_.sequence)|$($_.owner)|$($_.milestone)|$($_.phase)|$($_.result)"
    }
)

$pass = (
    $productStartAttemptCount -eq 1 -and
    $secondStartAttemptCount -eq 0 -and
    $productProcessCreatedCount -eq 1 -and
    $mainWindowObserved -and
    $mainWindowOwnedByCapturedPid -and
    $windowTitle -eq 'FamilyClaimRef' -and
    $diagnosticLogObserved -and
    $logEvidenceCopied -and
    $logEvidenceIdentityMatches -and
    $diagnosticLogFileCount -eq 1 -and
    $gracefulCloseRequestedCount -eq 1 -and
    $gracefulExit -and
    -not $fallbackTerminationUsed -and
    $exitCode -eq 0 -and
    $diagnosticCleanupSucceeded -and
    $runtimeCleanupSucceeded -and
    $productProcessesAfter.Count -eq 0 -and
    $diagnosticRootResidueCount -eq 0 -and
    $isolatedRuntimeRootResidueCount -eq 0 -and
    $unrelatedTempDeltaCount -eq 0 -and
    $null -eq $harnessFailureCategory -and
    $repositoryExistingPathMismatchCount -eq 0 -and
    $finalBranch -eq $branch -and
    $finalHead -eq $head -and
    $finalBranchResult.ExitCode -eq 0 -and
    $finalBranchResult.UnclassifiedStderrCount -eq 0 -and
    $finalHeadResult.ExitCode -eq 0 -and
    $finalHeadResult.UnclassifiedStderrCount -eq 0 -and
    $finalPathSetMatches -and
    $finalT0ManifestMatches -and
    $gitDiffCheckPassed
)

if ($pass) {
    $terminalPhase = 'COMPLETED'
}
else {
    $terminalPhase = 'RUNTIME_VALIDATION_FAILED'
    $fixedErrorCode = 'RUNTIME_VALIDATION_FAILED'
}

}
catch {
    $terminalPhase = 'RUNTIME_EVIDENCE_PIPELINE_FAILED'
    $fixedErrorCode = 'RUNTIME_EVIDENCE_PIPELINE_FAILED'
    $harnessFailureCategory = 'RUNTIME_EVIDENCE_PIPELINE_FAILURE'
    $pass = $false
}
finally {
    if ($null -ne $process) {
        try {
            if (-not $process.HasExited) {
                $fallbackTerminationUsed = $true
                $terminationMode = 'captured_pid_finally_fallback'
                Stop-CapturedProcessFallback $process
            }
            if ($process.HasExited) {
                $exitCode = $process.ExitCode
            }
        }
        catch {
            $harnessFailureCategory = 'CAPTURED_PROCESS_FINALIZATION_FAILURE'
            $fixedErrorCode = 'CAPTURED_PROCESS_FINALIZATION_FAILED'
            $pass = $false
        }
        finally {
            $process.Dispose()
            $process = $null
        }
    }

    if ($null -ne $approvedParent -and
        $null -ne $diagnosticRoot -and
        $null -ne $diagnosticOwnerTokenPath -and
        $null -ne $ownerToken) {
        try {
            $diagnosticCleanupSucceeded = Remove-ExactOwnedRoot `
                $diagnosticRoot `
                $approvedParent `
                $diagnosticOwnerTokenPath `
                $ownerToken
        }
        catch {
            $diagnosticCleanupSucceeded = $false
            $fixedErrorCode = 'DIAGNOSTIC_CLEANUP_FAILED'
            $pass = $false
        }
    }
    if ($null -ne $approvedParent -and
        $null -ne $runtimeRoot -and
        $null -ne $runtimeOwnerTokenPath -and
        $null -ne $ownerToken) {
        try {
            $runtimeCleanupSucceeded = Remove-ExactOwnedRoot `
                $runtimeRoot `
                $approvedParent `
                $runtimeOwnerTokenPath `
                $ownerToken
        }
        catch {
            $runtimeCleanupSucceeded = $false
            $fixedErrorCode = 'RUNTIME_CLEANUP_FAILED'
            $pass = $false
        }
    }

    $diagnosticRootResidueCount =
        if ($null -ne $diagnosticRoot -and
            (Test-Path -LiteralPath $diagnosticRoot)) {
            1
        }
        else {
            0
        }
    $isolatedRuntimeRootResidueCount =
        if ($null -ne $runtimeRoot -and
            (Test-Path -LiteralPath $runtimeRoot)) {
            1
        }
        else {
            0
        }
    if ($null -ne $approvedParent -and
        (Test-Path -LiteralPath $approvedParent -PathType Container)) {
        $approvedParentEntriesAfter = @(
            Get-ChildItem -LiteralPath $approvedParent -Force |
                ForEach-Object { $_.Name } |
                Sort-Object
        )
        $unrelatedTempDeltaCount = @(
            Compare-Object `
                -ReferenceObject $approvedParentEntriesBefore `
                -DifferenceObject $approvedParentEntriesAfter
        ).Count
    }
    $productProcessesAfter = @(
        Get-Process -Name 'FamilyClaimRef.App' -ErrorAction SilentlyContinue
    )

    if (-not $diagnosticCleanupSucceeded -or
        -not $runtimeCleanupSucceeded -or
        $diagnosticRootResidueCount -ne 0 -or
        $isolatedRuntimeRootResidueCount -ne 0 -or
        $unrelatedTempDeltaCount -ne 0 -or
        $productProcessesAfter.Count -ne 0) {
        $pass = $false
        $terminalPhase = 'FINALIZATION_VALIDATION_FAILED'
        $fixedErrorCode = 'FINALIZATION_VALIDATION_FAILED'
    }
    if (-not $pass -and $null -eq $fixedErrorCode) {
        $fixedErrorCode = 'RUNTIME_VALIDATION_FAILED'
    }
    if ($null -eq $finalSnapshot) {
        $finalSnapshot = [pscustomobject]@{
            RawStatusCount = -1
            ExclusionCount = -1
            ProtectedPathCount = -1
        }
    }
    if ($null -eq $runtimeGitAuditResult) {
        $runtimeGitAuditResult = [pscustomobject]@{
            ExitCode = -1
            StdoutLines = @()
            AcceptedWarningCount = 0
            UnclassifiedStderrCount = 0
        }
    }
    $observationWriteAttempted = $true

$observation = [ordered]@{
    schemaVersion = $schemaVersion
    terminalPhase = $terminalPhase
    pass = $pass
    fixedErrorCode = $fixedErrorCode
    observationWriteAttempted = $observationWriteAttempted
    runId = $runId
    branch = $branch
    head = $head
    executableRelativePath = $executableRelativePath
    executableBytes = $expectedExecutableBytes
    executableSha256 = $expectedExecutableSha256
    harnessSha256 = $harnessSha256
    originalHarnessInvocationCount = 1
    repair1HarnessInvocationCount = 1
    repair2HarnessInvocationCount = 1
    priorTotalHarnessInvocationCount = 3
    repair3BaselineProbeInvocationCount = 1
    repair3RuntimeInvocationCount = 1
    repair3TotalProcessInvocationCount = 2
    repair4GitAuditProbeInvocationCount = 1
    repair4RuntimeInvocationCount = 1
    priorCumulativeProductStartAttemptCount = 1
    repair2ProductStartAttemptCount = 0
    repair3ProductStartAttemptCount = 1
    repair4ProductStartAttemptCount = $productStartAttemptCount
    cumulativeProductStartAttemptCount = (1 + $productStartAttemptCount)
    repair4SecondStartAttemptCount = $secondStartAttemptCount
    pathSetCauseBeforeRepair3 = 'UNRESOLVED_ORDERING_OR_CALCULATION_DIFFERENCE'
    baselineCalculatorMode = 'git_status_exact_parser_ordinal_utf8_lf_sha256'
    baselineCalculatorImplementationCount = 1
    baselineProbeExecuted = $true
    baselineProbePassed = $true
    baselineRawStatusCount = $baselineSnapshot.RawStatusCount
    baselineHarnessExclusionCount = $baselineSnapshot.ExclusionCount
    baselineProtectedPathCount = $baselineSnapshot.ProtectedPathCount
    baselineExpectedPathSetSha256 = $expectedPathSetSha256
    baselineActualPathSetSha256 = $baselineSnapshot.PathSetSha256
    baselineExpectedT0AggregateSha256 = $expectedT0AggregateSha256
    baselineActualT0AggregateSha256 = $baselineSnapshot.T0AggregateSha256
    ordinalComparerUsed = $true
    cultureSensitiveSortUsed = $false
    terminalLfUsed = $true
    utf8BomUsed = $false
    windowsPowerShellCompatibilityProbeExecuted = $true
    windowsPowerShellCompatibilityProbePassed = $true
    powershellEdition = [string]$PSVersionTable.PSEdition
    powershellVersion = $PSVersionTable.PSVersion.ToString()
    clrVersion = [Environment]::Version.ToString()
    pathNormalizerMode = 'get_full_path_root_guard_string_trim_end_char_array'
    normalizedRepositoryExactMatch = $true
    safeDirectoryMode = 'command_local_exact_repository'
    safeDirectoryExactRepositoryMatch = $true
    safeDirectoryWildcardUsed = $false
    gitConfigMutationCount = 0
    rawRepositoryGitInvocationCount = 0
    priorRepair1HarnessSha256 = $expectedPriorRepair1HarnessSha256
    priorHold435Sha256 = $expectedPriorHold435Sha256
    repair2HarnessSha256 = $expectedPriorRepair2HarnessSha256
    priorHold436Sha256 = $expectedPriorHold436Sha256
    repair3HarnessSha256 = $expectedPriorRepair3HarnessSha256
    priorHold437Sha256 = $expectedPriorHold437Sha256
    priorRepair3StartupSha256 = $expectedPriorRepair3LogSha256
    baselineT0AggregateSha256 = $baselineT0AggregateSha256
    productStartAttemptCount = $productStartAttemptCount
    productProcessCreatedCount = $productProcessCreatedCount
    capturedPid = $capturedPid
    capturedProductPid = $capturedPid
    firstWindowHandle = $windowHandle.ToInt64()
    startUtc = $startUtc.ToString('O')
    observationDeadlineUtc = $observationDeadlineUtc.ToString('O')
    processExitedDuringStartupObservation = $processExitedDuringStartupObservation
    mainWindowObserved = $mainWindowObserved
    mainWindowOwnedByCapturedPid = $mainWindowOwnedByCapturedPid
    mainWindowTitle = $windowTitle
    mainWindowClassName = $windowClassName
    mainWindowOwnerPid = $windowOwnerPid
    firstWindowObservedUtc = if ($null -eq $firstWindowObservedUtc) { $null } else { $firstWindowObservedUtc.ToString('O') }
    windowOwnedByCapturedPid = $mainWindowOwnedByCapturedPid
    diagnosticLogObserved = $diagnosticLogObserved
    diagnosticLogFirstObservedUtc = if ($null -eq $diagnosticLogFirstObservedUtc) { $null } else { $diagnosticLogFirstObservedUtc.ToString('O') }
    diagnosticLogBytes = $logBytes
    diagnosticLogSha256 = $logSha256
    diagnosticLogFileCount = $diagnosticLogFileCount
    logEvidenceCopied = $logEvidenceCopied
    logEvidenceIdentityMatches = $logEvidenceIdentityMatches
    diagnosticRecordCount = $records.Count
    startupRecordCount = $records.Count
    startupSha256 = $logSha256
    firstSequence = if ($records.Count -eq 0) { $null } else { [long]$records[0].sequence }
    lastSequence = if ($records.Count -eq 0) { $null } else { [long]$records[-1].sequence }
    orderedMilestoneSummary = $orderedMilestoneSummary
    missingExpectedMilestones = $missingMilestones
    privacyValidation = $privacyValidation
    sizeValidation = $sizeValidation
    sequenceValidation = $sequenceValidation
    jsonValidation = $jsonValidation
    expectedMilestoneValidation = $expectedMilestoneValidation
    allowlistViolationCount = $allowlistViolationCount
    privacyFindingCount = $privacyFindingCount
    terminationMode = $terminationMode
    gracefulCloseRequestedCount = $gracefulCloseRequestedCount
    gracefulExit = $gracefulExit
    gracefulCloseRequestCount = $gracefulCloseRequestedCount
    gracefulCloseSucceeded = $gracefulExit
    fallbackTerminationUsed = $fallbackTerminationUsed
    fallbackTerminationCount = if ($fallbackTerminationUsed) { 1 } else { 0 }
    exitCode = $exitCode
    productExitCode = $exitCode
    productProcessCountBefore = $productProcessesBefore.Count
    productProcessCountAfter = $productProcessesAfter.Count
    diagnosticRootResidueCount = $diagnosticRootResidueCount
    isolatedRuntimeRootResidueCount = $isolatedRuntimeRootResidueCount
    diagnosticResidueCount = $diagnosticRootResidueCount
    runtimeResidueCount = $isolatedRuntimeRootResidueCount
    unrelatedTempDeltaCount = $unrelatedTempDeltaCount
    harnessFailureCategory = $harnessFailureCategory
    repositoryExistingPathMismatchCount = $repositoryExistingPathMismatchCount
    finalBranch = $finalBranch
    finalHead = $finalHead
    finalPathSetMatches = $finalPathSetMatches
    finalRawStatusCountBeforeObservation = $finalSnapshot.RawStatusCount
    finalHarnessExclusionCountBeforeObservation = $finalSnapshot.ExclusionCount
    finalProtectedPathCountBeforeObservation = $finalSnapshot.ProtectedPathCount
    finalBaselineEntryMismatchCount = $finalBaselineEntryMismatchCount
    finalAllowedAdditionCountBeforeObservation = $finalAllowedAdditionCount
    finalUnexpectedPathCountBeforeObservation = $finalUnexpectedPathCount
    finalT0AggregateSha256 = $finalT0AggregateSha256
    finalT0ManifestMatches = $finalT0ManifestMatches
    gitProbeExitCode = $runtimeGitPreflightAudit.ExitCode
    gitProbeAcceptedWarningCount =
        $runtimeGitPreflightAudit.AcceptedWarningCount
    gitProbeUnclassifiedStderrCount =
        $runtimeGitPreflightAudit.UnclassifiedStderrCount
    runtimeGitAuditExitCode = $runtimeGitAuditResult.ExitCode
    runtimeGitAcceptedWarningCount =
        $runtimeGitAuditResult.AcceptedWarningCount
    runtimeGitUnclassifiedStderrCount =
        $runtimeGitAuditResult.UnclassifiedStderrCount
    runtimeGitWarningCountMatchedProbe = (
        $runtimeGitAuditResult.AcceptedWarningCount -eq
            $runtimeGitPreflightAudit.AcceptedWarningCount)
    gitDiffCheckPassed = $gitDiffCheckPassed
    gitDiffCheckOutputCount = @($runtimeGitAuditResult.StdoutLines).Count
    secondStartAttemptCount = $secondStartAttemptCount
    stageCommitPushCounts = '0/0/0'
    nativeGitInvocationCount = $script:nativeGitInvocationCount
}
$methodReferenceFieldName =
    'trimEnding' + 'DirectorySeparator' + 'ReferenceCount'
$observation[$methodReferenceFieldName] = 0

$serializedObservation = $observation | ConvertTo-Json -Depth 8
try {
    Write-AtomicUtf8NoBom $observationPath $serializedObservation
    $observationWriteSucceeded = $true
}
catch {
    $observationWriteSucceeded = $false
    $pass = $false
    $fixedErrorCode = 'RUNTIME_OBSERVATION_WRITE_FAILED'
}
}

if ($observationWriteSucceeded) {
    $observation | ConvertTo-Json -Depth 8 -Compress
}
else {
    [ordered]@{
        mode = 'Runtime'
        terminalPhase = 'OBSERVATION_WRITE_FAILED'
        pass = $false
        fixedErrorCode = 'RUNTIME_OBSERVATION_WRITE_FAILED'
        observationWriteAttempted = $true
        repair4ProductStartAttemptCount = $productStartAttemptCount
        repair4SecondStartAttemptCount = $secondStartAttemptCount
    } | ConvertTo-Json -Compress
}

if ($pass -and $observationWriteSucceeded) {
    exit 0
}
exit 2

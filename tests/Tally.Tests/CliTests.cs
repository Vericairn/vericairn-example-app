// SPDX-License-Identifier: Apache-2.0
using System.Diagnostics;
using System.Text;

namespace Tally.Tests;

public sealed class CliTests : IDisposable
{
    private readonly string directory = Path.Combine(AppContext.BaseDirectory, $"tally-tests-{Guid.NewGuid():N}");

    public CliTests() => Directory.CreateDirectory(directory);

    public static TheoryData<string, string> Inputs => new()
    {
        { "", "0 0 0" },
        { "one two", "1 2 7" },
        { "one\ntwo\n", "2 2 8" },
        { "one\r\ntwo", "2 2 8" },
        { "\n\n", "2 0 2" },
        { " \t\r\n", "1 0 4" },
        { "café\u00a0世界 \U0001F600\n", "1 3 10" },
        { "café\u00a0世界 \U0001F600", "1 3 9" },
        { "e\u0301", "1 1 2" }
    };

    [Theory]
    [MemberData(nameof(Inputs))]
    public async Task StandardInputPrintsLinesWordsAndCharacters(string input, string counts)
    {
        var result = await RunAsync(input);

        Assert.Equal(0, result.ExitCode);
        Assert.Equal($"{counts}{Environment.NewLine}", result.Output);
        Assert.Empty(result.Error);
    }

    [Fact]
    public async Task StandardInputRecognizesUtf8ByteOrderMark()
    {
        var result = await RunAsync("\ufeffcafé\u00a0世界 \U0001F600");

        Assert.Equal(0, result.ExitCode);
        Assert.Equal($"1 3 9{Environment.NewLine}", result.Output);
        Assert.Empty(result.Error);
    }

    [Theory]
    [MemberData(nameof(Inputs))]
    public async Task FilePrintsLinesWordsCharactersAndPath(string input, string counts)
    {
        const string path = "notes with spaces.txt";
        await File.WriteAllTextAsync(Path.Combine(directory, path), input);

        var result = await RunAsync("", path);

        Assert.Equal(0, result.ExitCode);
        Assert.Equal($"{counts} {path}{Environment.NewLine}", result.Output);
        Assert.Empty(result.Error);
    }

    [Fact]
    public async Task FilesPrintInArgumentOrder()
    {
        await File.WriteAllTextAsync(Path.Combine(directory, "first.txt"), "one\n");
        await File.WriteAllTextAsync(Path.Combine(directory, "second.txt"), "two three");

        var result = await RunAsync("", "second.txt", "first.txt");

        Assert.Equal(0, result.ExitCode);
        Assert.Equal($"1 2 9 second.txt{Environment.NewLine}1 1 4 first.txt{Environment.NewLine}", result.Output);
        Assert.Empty(result.Error);
    }

    [Fact]
    public async Task UnreadableFileReportsErrorAndContinuesWithRemainingFiles()
    {
        await File.WriteAllTextAsync(Path.Combine(directory, "first.txt"), "one\n");
        await File.WriteAllTextAsync(Path.Combine(directory, "last.txt"), "two three");

        var result = await RunAsync("", "first.txt", "missing.txt", "last.txt");

        Assert.Equal(1, result.ExitCode);
        Assert.Equal($"1 1 4 first.txt{Environment.NewLine}1 2 9 last.txt{Environment.NewLine}", result.Output);
        Assert.StartsWith("tally: missing.txt: ", result.Error);
    }

    private async Task<(int ExitCode, string Output, string Error)> RunAsync(string input, params string[] paths)
    {
        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = directory,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardInputEncoding = new UTF8Encoding(false),
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };
        startInfo.ArgumentList.Add("exec");
        startInfo.ArgumentList.Add("--runtimeconfig");
        startInfo.ArgumentList.Add(Path.ChangeExtension(typeof(CliTests).Assembly.Location, ".runtimeconfig.json"));
        startInfo.ArgumentList.Add(typeof(Counter).Assembly.Location);
        foreach (string path in paths)
        {
            startInfo.ArgumentList.Add(path);
        }

        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Could not start tally.");
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        Task<string> output = process.StandardOutput.ReadToEndAsync(timeout.Token);
        Task<string> error = process.StandardError.ReadToEndAsync(timeout.Token);
        try
        {
            await process.StandardInput.WriteAsync(input.AsMemory(), timeout.Token);
            process.StandardInput.Close();
            await process.WaitForExitAsync(timeout.Token);
            return (process.ExitCode, await output, await error);
        }
        finally
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                await process.WaitForExitAsync();
            }
        }
    }

    public void Dispose() => Directory.Delete(directory, recursive: true);
}

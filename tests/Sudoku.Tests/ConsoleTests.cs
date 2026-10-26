using System.Diagnostics;
using sudoku.Entities;

namespace Sudoku.Tests;

public class ConsoleTests
{
    [Fact]
    public async Task RedirectedConsoleCanCompleteRestartAndQuit()
    {
        var start = new ProcessStartInfo("dotnet")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        start.ArgumentList.Add("exec");
        start.ArgumentList.Add("--runtimeconfig");
        start.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "Sudoku.Tests.runtimeconfig.json"));
        start.ArgumentList.Add("--depsfile");
        start.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "Sudoku.Tests.deps.json"));
        start.ArgumentList.Add(typeof(SudokuGame).Assembly.Location);

        using var process = Process.Start(start)!;
        Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> errorTask = process.StandardError.ReadToEndAsync();
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        try
        {
            await process.StandardInput.WriteLineAsync("help");
            await process.StandardInput.WriteLineAsync("Z1 5");
            await process.StandardInput.WriteLineAsync("check");
            for (int i = 0; i < 51; i++)
                await process.StandardInput.WriteLineAsync("hint");
            await process.StandardInput.WriteLineAsync("y");
            await process.StandardInput.WriteLineAsync("quit");
            process.StandardInput.Close();
            await process.WaitForExitAsync(timeout.Token);
            string output = await outputTask;
            string error = await errorTask;
            Assert.True(process.ExitCode == 0, error);
            Assert.Contains("Commands:", output);
            Assert.Contains("Invalid command", output);
            Assert.Contains("No rule violations found.", output);
            Assert.Contains("Hint: Cell", output);
            Assert.Contains("Congratulations!", output);
            Assert.Contains("Start a new game?", output);
            Assert.Equal(2, output.Split("Sudoku command-line game").Length - 1);
        }
        finally
        {
            if (!process.HasExited) process.Kill(entireProcessTree: true);
        }
    }
}

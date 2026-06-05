using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls.Primitives;
using Avalonia.Utilities;

namespace Chess.Services;

public class StockfishService : IDisposable
{
    private Process? _process;
    private StreamWriter? _input;
    private StreamReader? _output;

    public void Start()
    {
        var enginePath = Path.Combine(
            Environment.CurrentDirectory,
            "Engines", "stockfish", "stockfish"
        );

        if(OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
        {
            Process.Start("chmod", $"+x \"{enginePath}\"")?.WaitForExit();
        }

        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = enginePath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        _process.Start();
        _input  = _process.StandardInput;
        _output = _process.StandardOutput;

        SendCommand("uci");
        WaitForAsync("uciok");

        SendCommand("isready");
        WaitForAsync("readyok");

    }

    public void SendCommand(string command)
    {
        _input?.WriteLine(command);
        _input?.Flush();
    }

    public string? ReadLine() => _output?.ReadLine();

    public void WaitForAsync(string response)
    {
        while(true)
        {
            var line = ReadLine();
            if (line != null && line.Contains(response)) break;
        }
    }

    public void Dispose()
    {
        SendCommand("quit");
        _process?.WaitForExit();
        _process?.Dispose();
    }

    public string GetBestMove(string fen, int movetime)
    {
        SendCommand($"position fen {fen}");
        SendCommand($"go movetime {movetime}");

        while(true)
        {
            var line = ReadLine();
            if (line != null && line.StartsWith("bestmove"))
                return line.Split(' ')[1];
        }
    }

    public async Task<string> GetBestMoveAsync(string fen, int movetime)
    {
        return await Task.Run(() => GetBestMove(fen, movetime));
    }

}
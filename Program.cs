using System;
using System.Threading.Tasks;
using EWIM.Classes;
using EWIM.Utilities;
using EWIM.System;
using System.IO;

namespace EWIM {
  internal static class Program {
    private static readonly Indicators indicators = new Indicators();

    static async Task Main() {
      CleanupDSApiLogs();

      DSAPI Simulation = new DSAPI(indicators);

      Simulation.Simulate();

      while (Simulation.IsRunning) {
        Logger.Log(indicators);
        await Task.Delay(2000);
      }

      Simulation.Exit();
    }

    private static void CleanupDSApiLogs() {
      try {
        // Get current directory
        var currentDir = Directory.GetCurrentDirectory();

        // Find and delete all DS API log files in current directory
        var logFiles = Directory.GetFiles(currentDir, "DSApi-*.log");
        foreach (var logFile in logFiles) {
          try {
            File.Delete(logFile);
            Console.WriteLine($"Deleted DS API log file: {Path.GetFileName(logFile)}");
          } catch {
            // Ignore if file is in use or can't be deleted
          }
        }

        // Also clean up log files in bin directories
        var binPath = Path.Combine(currentDir, "bin");
        if (Directory.Exists(binPath)) {
          var binLogFiles = Directory.GetFiles(binPath, "DSApi-*.log", SearchOption.AllDirectories);
          foreach (var logFile in binLogFiles) {
            try {
              File.Delete(logFile);
              Console.WriteLine($"Deleted DS API log file: {logFile}");
            } catch {
              // Ignore if file is in use or can't be deleted
            }
          }
        }

        if (logFiles.Length > 0 || (Directory.Exists(binPath) && Directory.GetFiles(binPath, "DSApi-*.log", SearchOption.AllDirectories).Length > 0)) {
          Console.WriteLine("DS API log cleanup completed.");
        }
      } catch (Exception ex) {
        Console.WriteLine($"Error during DS API log cleanup: {ex.Message}");
      }
    }

  }
}

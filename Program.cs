using System;
using System.Threading.Tasks;
using EWIM.Classes;
using EWIM.Utilities;
using EWIM.System;
using EWIM.Services;

namespace EWIM {
  internal static class Program {
    private static readonly Indicators indicators = new Indicators();
    private static readonly DynamicThresholdOrchestrator thresholdOrchestrator = new DynamicThresholdOrchestrator();

    static async Task Main() {
      try {
        Console.WriteLine("Starting EWIM - Early Warning Indicator Monitoring");
        Console.WriteLine("===================================================");

        CleanupService.CleanupDSApiLogs();

        DSAPI Simulation = new DSAPI(indicators);
        Simulation.Simulate();

        // Initialize the dynamic threshold system with DSAPI reference
        thresholdOrchestrator.Initialize(Simulation);

        Console.WriteLine("Drilling simulator connected and running...");
        Console.WriteLine("Package reading is ENABLED - EWIM has control");
        Console.WriteLine("Press 'P' to toggle package reading, or '?' for help\n");

        while (Simulation.IsRunning) {
          try {
            // Process indicators for threshold monitoring
            thresholdOrchestrator.ProcessIndicators(indicators);

            // Handle user input for calibration
            if (!thresholdOrchestrator.HandleUserInput(indicators)) {
              Console.WriteLine("User requested shutdown...");
              break;
            }

            // Log indicators (will only update display if no input is pending)
            Logger.Log(indicators, Simulation);

            // Shorter delay to make system more responsive
            await Task.Delay(500);
          } catch (Exception ex) {
            // Log the error but continue running - don't exit on temporary issues
            Console.WriteLine($"Temporary error in main loop: {ex.Message}");
            Console.WriteLine("System will continue running...");
            await Task.Delay(1000); // Wait a bit longer after errors
          }
        }

        // If we get here, check why the simulation stopped
        if (!Simulation.IsRunning) {
          Console.WriteLine("Simulation stopped running - this may be due to DrillSIM connection issues.");
        }

        Simulation.Exit();
      } catch (Exception ex) {
        Console.WriteLine($"Fatal error: {ex.Message}");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
      }
    }
  }
}

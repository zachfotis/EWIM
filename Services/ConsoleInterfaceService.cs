using System;
using System.Linq;
using System.Threading;
using EWIM.Classes;
using EWIM.Models;

namespace EWIM.Services {
  public class ConsoleInterfaceService {
    private readonly BaselineCaptureService _baselineCapture;
    private readonly ThresholdCalibrationService _calibration;
    private readonly ThresholdPersistenceService _persistence;

    public ConsoleInterfaceService(
        BaselineCaptureService baselineCapture,
        ThresholdCalibrationService calibration,
        ThresholdPersistenceService persistence) {
      _baselineCapture = baselineCapture;
      _calibration = calibration;
      _persistence = persistence;

      // Subscribe to auto-apply when capture completes
      _baselineCapture.CaptureCompleted += OnCaptureCompleted;
    }

    public void ShowHelp() {
      Console.Clear();
      Console.WriteLine("=== EWIM Streamlined Commands ===");
      Console.WriteLine("All commands execute immediately - no confirmations");
      Console.WriteLine();
      Console.WriteLine("  C - Capture baseline (auto-applies when complete)");
      Console.WriteLine("  A - Manual apply (if needed)");
      Console.WriteLine("  V - View current thresholds");
      Console.WriteLine("  T - Show system status");
      Console.WriteLine("  P - Toggle package reading");
      Console.WriteLine("  E - Enable package reading");
      Console.WriteLine("  D - Disable package reading");
      Console.WriteLine("  ? - Show this help");
      Console.WriteLine("  Q - Quit application");
      Console.WriteLine();
      Console.WriteLine("Additional Commands:");
      Console.WriteLine("  S - Stop current baseline capture");
      Console.WriteLine("  H - Show baseline history");
      Console.WriteLine("  R - Reset to default thresholds");
      Console.WriteLine("  N - Reset orange sequence numbering");
      Console.WriteLine("  O - Show orange sequence summary");
      Console.WriteLine("======================================");
      PauseForUserInput();
    }
    public bool HandleInput(ConsoleKeyInfo keyInfo, Indicators indicators, EWIM.System.DSAPI dsapi = null) {
      switch (keyInfo.Key) {
        case ConsoleKey.C:
          StartBaselineCapture();
          break;

        case ConsoleKey.S:
          StopBaselineCapture();
          break;

        case ConsoleKey.A:
          ApplyBaselineToThresholds();
          break;

        case ConsoleKey.V:
          ViewCurrentThresholds();
          break;

        case ConsoleKey.H:
          ShowBaselineHistory();
          break;

        case ConsoleKey.R:
          ResetToDefaults();
          break;

        case ConsoleKey.N:
          ResetOrangeSequence();
          break;

        case ConsoleKey.O:
          ShowOrangeSequenceSummary();
          break;

        case ConsoleKey.T:
          ShowCurrentStatus(indicators, dsapi);
          break;

        case ConsoleKey.P:
          TogglePackageReading(dsapi);
          break;

        case ConsoleKey.E:
          EnablePackageReading(dsapi);
          break;

        case ConsoleKey.D:
          DisablePackageReading(dsapi);
          break;

        case ConsoleKey.Oem2: // ? key
          if (keyInfo.KeyChar == '?') {
            ShowHelp();
          }
          break;

        case ConsoleKey.Q:
          return false; // Signal to quit

        case ConsoleKey.Enter:
          // Ignore Enter key presses to prevent accidental exits
          break;

        default:
          if (keyInfo.KeyChar != '\0') {
            Console.WriteLine($"Unknown command: {keyInfo.KeyChar}. Press '?' for help.");
            PauseForUserInput();
          }
          break;
      }

      return true; // Continue running
    }

    private void PauseForUserInput() {
      Console.WriteLine("Press any key to continue...");
      Console.ReadKey(true);
      ClearInputBuffer(); // Clear any additional input
    }

    private void ClearInputBuffer() {
      // Clear any remaining keys in the input buffer
      while (Console.KeyAvailable) {
        Console.ReadKey(true);
      }
    }

    private void StartBaselineCapture() {
      try {
        if (_baselineCapture.IsCapturing) {
          Console.Clear();
          Console.WriteLine("Baseline capture is already in progress.");
          Console.WriteLine($"Current progress: {_baselineCapture.SampleCount} samples, {_baselineCapture.CaptureProgress.TotalSeconds:F0}s elapsed");
          PauseForUserInput();
          return;
        }

        // Start capture immediately
        _baselineCapture.StartCapture();
        Console.Clear();
        Console.WriteLine("=== Baseline Capture Started ===");
        Console.WriteLine("Capturing 60 seconds of indicator data for threshold calculation.");
        Console.WriteLine("Thresholds will be applied automatically when capture completes.");
        Console.WriteLine("Press 'S' to stop early, or wait for automatic completion.");
        PauseForUserInput();
      } catch (Exception ex) {
        Console.WriteLine($"Error starting baseline capture: {ex.Message}");
        PauseForUserInput();
      }
    }

    private void StopBaselineCapture() {
      if (!_baselineCapture.IsCapturing) {
        Console.WriteLine("No baseline capture in progress.");
        return;
      }

      _baselineCapture.StopCapture();
    }

    private void ApplyBaselineToThresholds() {
      try {
        if (_baselineCapture.SampleCount == 0) {
          Console.Clear();
          Console.WriteLine("=== Manual Apply ===");
          Console.WriteLine("No baseline data available.");
          Console.WriteLine("Note: Baseline capture (C) now auto-applies thresholds.");
          Console.WriteLine("Manual apply is only needed for special cases.");
          PauseForUserInput();
          return;
        }

        var baselineData = _baselineCapture.GetBaselineData();
        var currentThresholds = _persistence.LoadThresholds();
        var newThresholds = _calibration.CalculateThresholds(baselineData);

        // Generate calibration report
        var report = _calibration.GenerateCalibrationReport(baselineData, newThresholds, currentThresholds);
        DisplayCalibrationReport(report);

        // Apply thresholds immediately without confirmation
        _persistence.SaveThresholds(newThresholds, baselineData);
        _persistence.SaveBaselineHistory(baselineData);

        // Update the threshold engine with new thresholds
        EWIM.Engine.ThresholdEngine.UpdateThresholds(newThresholds);

        Console.WriteLine("New thresholds applied successfully!");
        Console.WriteLine("Thresholds updated in monitoring system!");

        // Clear captured samples after successful application
        _baselineCapture.ClearSamples();
      } catch (Exception ex) {
        Console.WriteLine($"Error applying baseline: {ex.Message}");
      }
    }

    private void ViewCurrentThresholds() {
      try {
        var thresholds = _persistence.LoadThresholds();

        Console.Clear();
        Console.WriteLine("=== Current Thresholds ===");
        Console.WriteLine($"{"Indicator",-20} {"Green Max",-12} {"Yellow Max",-12}");
        Console.WriteLine(new string('-', 50));

        foreach (var kvp in thresholds) {
          Console.WriteLine($"{kvp.Key,-20} {kvp.Value.GreenMax,-12:F2} {kvp.Value.YellowMax,-12:F2}");
        }
        Console.WriteLine();
        PauseForUserInput();
      } catch (Exception ex) {
        Console.WriteLine($"Error viewing thresholds: {ex.Message}");
        PauseForUserInput();
      }
    }

    private void ShowBaselineHistory() {
      try {
        var history = _persistence.LoadBaselineHistory();

        Console.Clear();
        if (history.Count == 0) {
          Console.WriteLine("No baseline history available.");
          PauseForUserInput();
          return;
        }

        Console.WriteLine("=== Baseline History ===");
        for (int i = 0; i < history.Count; i++) {
          var baseline = history[i];
          Console.WriteLine($"{i + 1}. {baseline.CaptureDate:yyyy-MM-dd HH:mm:ss} - {baseline.SampleCount} samples ({baseline.CaptureDurationSeconds}s)");
        }
        Console.WriteLine();
        PauseForUserInput();
      } catch (Exception ex) {
        Console.WriteLine($"Error showing baseline history: {ex.Message}");
        PauseForUserInput();
      }
    }

    private void ResetToDefaults() {
      try {
        // Reset to defaults immediately
        _persistence.ResetToDefaults();

        // Reload the default thresholds in the engine
        EWIM.Engine.ThresholdEngine.LoadThresholds();

        Console.Clear();
        Console.WriteLine("=== Reset Complete ===");
        Console.WriteLine("All thresholds have been reset to default values.");
        Console.WriteLine("Thresholds updated in monitoring system!");
        PauseForUserInput();
      } catch (Exception ex) {
        Console.WriteLine($"Error resetting thresholds: {ex.Message}");
        PauseForUserInput();
      }
    }

    private void ShowCurrentStatus(Indicators indicators, EWIM.System.DSAPI dsapi = null) {
      Console.Clear();
      Console.WriteLine("=== Current System Status ===");
      Console.WriteLine($"Overall Risk: {indicators.OverallRisk}");

      // Show package reading status and connection health
      if (dsapi != null) {
        var packageStatus = dsapi.IsPackageEnabled ? "ENABLED" : "DISABLED";
        var controlStatus = dsapi.IsPackageEnabled ? "EWIM" : "Simulator";
        var connectionStatus = dsapi.IsConnectionValid() ? "CONNECTED" : "DISCONNECTED";

        Console.WriteLine($"DrillSIM Connection: {connectionStatus}");
        Console.WriteLine($"Package Reading: {packageStatus} (Control: {controlStatus})");

        if (!dsapi.IsConnectionValid()) {
          Console.WriteLine("WARNING: Connection to DrillSIM is not stable!");
          Console.WriteLine("Consider restarting EWIM if issues persist.");
        }
      }

      Console.WriteLine();
      Console.WriteLine($"{"Indicator",-20} {"Value",-12} {"Risk Level",-10}");
      Console.WriteLine(new string('-', 50));

      foreach (var indicator in indicators.IndicatorsList) {
        Console.WriteLine($"{indicator.Name,-20} {indicator.Value,-12:F2} {indicator.RiskLevel,-10}");
      }

      if (_baselineCapture.IsCapturing) {
        Console.WriteLine($"\nBaseline Capture: In Progress ({_baselineCapture.SampleCount} samples, {_baselineCapture.CaptureProgress.TotalSeconds:F0}s elapsed)");
      } else if (_baselineCapture.SampleCount > 0) {
        Console.WriteLine($"\nBaseline Data: {_baselineCapture.SampleCount} samples ready for analysis");
      }

      Console.WriteLine();
      PauseForUserInput();
    }

    private void DisplayCalibrationReport(ThresholdCalibrationReport report) {
      Console.WriteLine("\n=== Calibration Report ===");
      Console.WriteLine($"Baseline Date: {report.BaselineData.CaptureDate:yyyy-MM-dd HH:mm:ss}");
      Console.WriteLine($"Samples: {report.BaselineData.SampleCount} over {report.BaselineData.CaptureDurationSeconds} seconds");
      Console.WriteLine();

      Console.WriteLine($"{"Indicator",-20} {"Old Green",-10} {"New Green",-10} {"Old Yellow",-10} {"New Yellow",-10} {"Change %",-10}");
      Console.WriteLine(new string('-', 80));

      foreach (var kvp in report.IndicatorAnalysis) {
        var analysis = kvp.Value;
        var greenChange = analysis.GreenThresholdChange?.ToString("F1") ?? "N/A";

        Console.WriteLine($"{kvp.Key,-20} {analysis.OldGreenMax?.ToString("F2") ?? "N/A",-10} " +
                        $"{analysis.NewGreenMax,-10:F2} {analysis.OldYellowMax?.ToString("F2") ?? "N/A",-10} " +
                        $"{analysis.NewYellowMax,-10:F2} {greenChange,-10}");
      }
      Console.WriteLine();
    }

    private void TogglePackageReading(EWIM.System.DSAPI dsapi) {
      if (dsapi == null) {
        Console.Clear();
        Console.WriteLine("Error: DrillSIM API not available.");
        PauseForUserInput();
        return;
      }

      Console.Clear();
      var oldState = dsapi.IsPackageEnabled ? "ENABLED" : "DISABLED";

      if (dsapi.TogglePackageReading()) {
        var newState = dsapi.IsPackageEnabled ? "ENABLED" : "DISABLED";
        Console.WriteLine("=== Package Reading Toggled ===");
        Console.WriteLine($"Was: {oldState} → Now: {newState}");
        Console.WriteLine($"Control: {(dsapi.IsPackageEnabled ? "EWIM" : "Simulator")}");
      } else {
        Console.WriteLine("=== Toggle Failed ===");
        Console.WriteLine("Could not toggle package reading.");
      }
      PauseForUserInput();
    }

    private void EnablePackageReading(EWIM.System.DSAPI dsapi) {
      if (dsapi == null) {
        Console.Clear();
        Console.WriteLine("Error: DrillSIM API not available.");
        PauseForUserInput();
        return;
      }

      Console.Clear();
      if (dsapi.IsPackageEnabled) {
        Console.WriteLine("=== Already Enabled ===");
        Console.WriteLine("Package reading is already enabled.");
        Console.WriteLine("EWIM has control.");
      } else if (dsapi.EnablePackageReading()) {
        Console.WriteLine("=== Package Reading Enabled ===");
        Console.WriteLine("Package reading ENABLED. EWIM now has control.");
      } else {
        Console.WriteLine("=== Enable Failed ===");
        Console.WriteLine("Could not enable package reading.");
      }
      PauseForUserInput();
    }

    private void DisablePackageReading(EWIM.System.DSAPI dsapi) {
      if (dsapi == null) {
        Console.Clear();
        Console.WriteLine("Error: DrillSIM API not available.");
        PauseForUserInput();
        return;
      }

      Console.Clear();
      try {
        if (!dsapi.IsPackageEnabled) {
          Console.WriteLine("=== Already Disabled ===");
          Console.WriteLine("Package reading is already disabled.");
          Console.WriteLine("Simulator has control.");
        } else {
          dsapi.DisablePackageReading(); // Always succeeds now
          Console.WriteLine("=== Package Reading Disabled ===");
          Console.WriteLine("Package reading DISABLED. Simulator has control.");
        }
      } catch (Exception ex) {
        Console.WriteLine("=== Disable Completed with Warning ===");
        Console.WriteLine($"Warning: {ex.Message}");
        Console.WriteLine("Package reading has been disabled.");
      }
      PauseForUserInput();
    }

    private void ResetOrangeSequence() {
      try {
        IndicatorSequenceTracker.Instance.ResetSequence();
        Console.Clear();
        Console.WriteLine("=== Orange Sequence Reset ===");
        Console.WriteLine("Orange indicator sequence numbering has been reset.");
        Console.WriteLine("Next indicator to turn orange will be numbered #1.");
        PauseForUserInput();
      } catch (Exception ex) {
        Console.WriteLine($"Error resetting orange sequence: {ex.Message}");
        PauseForUserInput();
      }
    }

    private void ShowOrangeSequenceSummary() {
      try {
        var orangeSequence = IndicatorSequenceTracker.Instance.GetCurrentOrangeSequence();
        var totalOrange = IndicatorSequenceTracker.Instance.GetTotalOrangeIndicators();

        Console.Clear();
        Console.WriteLine("=== Orange Sequence Summary ===");

        if (totalOrange == 0) {
          Console.WriteLine("No indicators are currently orange.");
        } else {
          Console.WriteLine($"Total Orange Indicators: {totalOrange}");
          Console.WriteLine();
          Console.WriteLine($"{"Sequence #",-12} {"Indicator",-20}");
          Console.WriteLine(new string('-', 35));

          // Sort by sequence number for display
          var sortedSequence = orangeSequence.OrderBy(kvp => kvp.Value);
          foreach (var kvp in sortedSequence) {
            Console.WriteLine($"{kvp.Value,-12} {kvp.Key,-20}");
          }
        }

        Console.WriteLine();
        PauseForUserInput();
      } catch (Exception ex) {
        Console.WriteLine($"Error showing orange sequence summary: {ex.Message}");
        PauseForUserInput();
      }
    }

    private void OnCaptureCompleted() {
      try {
        // Automatically apply captured baseline to thresholds
        if (_baselineCapture.SampleCount == 0) {
          Console.WriteLine("Warning: No baseline samples to apply.");
          return;
        }

        Console.WriteLine("Auto-applying captured baseline to thresholds...");

        var baselineData = _baselineCapture.GetBaselineData();
        var currentThresholds = _persistence.LoadThresholds();
        var newThresholds = _calibration.CalculateThresholds(baselineData);

        // Generate and display calibration report
        var report = _calibration.GenerateCalibrationReport(baselineData, newThresholds, currentThresholds);
        DisplayCalibrationReport(report);

        // Apply thresholds automatically
        _persistence.SaveThresholds(newThresholds, baselineData);
        _persistence.SaveBaselineHistory(baselineData);

        // Update the threshold engine with new thresholds
        EWIM.Engine.ThresholdEngine.UpdateThresholds(newThresholds);

        Console.WriteLine("=== Auto-Application Complete ===");
        Console.WriteLine("New thresholds applied successfully!");
        Console.WriteLine("Thresholds updated in monitoring system!");

        // Clear captured samples after successful application
        _baselineCapture.ClearSamples();

        // Give user time to see the results
        Thread.Sleep(3000);

      } catch (Exception ex) {
        Console.WriteLine($"Error during auto-application: {ex.Message}");
      }
    }
  }
}

using System;
using System.Collections.Generic;
using EWIM.Classes;
using EWIM.Models;
using EWIM.Engine;
using EWIM.Services;

namespace EWIM.Utilities {
  public static class Logger {
    public static void Log(Indicators indicators, EWIM.System.DSAPI dsapi = null) {
      // Only update display if no keyboard input is waiting
      // This prevents clearing the screen when user is trying to type
      if (!Console.KeyAvailable) {
        PrintHeader(dsapi);

        var currentThresholds = ThresholdEngine.GetCurrentThresholds();

        foreach (var indicator in indicators.IndicatorsList) {
          PrintIndicatorDetails(indicator, currentThresholds);
        }

        PrintOverallRisk(indicators.OverallRisk);
        PrintInputPrompt();
      }
    }

    private static void PrintHeader(EWIM.System.DSAPI dsapi = null) {
      Console.Clear();
      Console.WriteLine($"Timestamp: {DateTime.Now:HH:mm:ss}");

      // Show package status
      if (dsapi != null) {
        var status = dsapi.IsPackageEnabled ? "ENABLED" : "DISABLED";
        var control = dsapi.IsPackageEnabled ? "EWIM" : "Simulator";
        Console.WriteLine($"Package Reading: {status} | Control: {control}");
      }

      Console.WriteLine($"{"Indicator",-21}{"Value",-9} {"Risk",-8} {"Seq#",-6} {"Green ≤",-9} {"Yellow ≤",-9}");
      Console.WriteLine("------------------------------------------------------------------------");
    }

    private static void PrintInputPrompt() {
      Console.WriteLine("\nAvailable Commands: [C]apture | [V]iew | [T]status | [P]toggle | [E]enable | [D]disable | [N]reset seq | [O]range summary | [?]help | [Q]uit");
      Console.Write("Command: ");
    }

    private static void PrintIndicatorDetails(Indicator indicator, Dictionary<IndicatorName, Threshold> thresholds) {
      var screenName = indicator.Name.GetScreenName();
      var value = indicator.Value;
      var riskLevel = indicator.RiskLevel;

      // Get thresholds for this indicator
      var threshold = thresholds.ContainsKey(indicator.Name) ? thresholds[indicator.Name] : null;
      var greenMax = threshold?.GreenMax.ToString("F2") ?? "N/A";
      var yellowMax = threshold?.YellowMax.ToString("F2") ?? "N/A";

      // Get sequence number for orange indicators
      var sequenceNumber = IndicatorSequenceTracker.Instance.GetOrangeSequenceNumber(indicator.Name);
      var sequenceDisplay = sequenceNumber?.ToString() ?? "-";

      Console.Write($"{screenName,-21}{value,-9:F2}");

      // Print risk level with color
      switch (riskLevel) {
        case RiskLevel.Green:
          Console.BackgroundColor = ConsoleColor.Green;
          break;
        case RiskLevel.Yellow:
          Console.BackgroundColor = ConsoleColor.Yellow;
          break;
        default:
          Console.BackgroundColor = ConsoleColor.Red;
          break;
      }
      Console.ForegroundColor = ConsoleColor.Black;
      Console.Write($" {riskLevel,-6} ");
      Console.ResetColor();

      // Print sequence number
      Console.Write($" {sequenceDisplay,-4} ");

      // Print thresholds
      Console.WriteLine($" {greenMax,-9} {yellowMax,-9}");
    }

    private static void PrintOverallRisk(RiskLevel overall) {
      Console.Write("\nOverall Risk: ");
      switch (overall) {
        case RiskLevel.Green:
          Console.BackgroundColor = ConsoleColor.Green;
          break;
        case RiskLevel.Yellow:
          Console.BackgroundColor = ConsoleColor.Yellow;
          break;
        default:
          Console.BackgroundColor = ConsoleColor.Red;
          break;
      }
      Console.ForegroundColor = ConsoleColor.Black;
      Console.Write($" {overall} ");
      Console.ResetColor();
      Console.WriteLine("");
    }
  }
}


using EWIM.Classes;
using EWIM.Models;

namespace EWIM.Utilities;

public static class Logger {
  public static void Log(Indicators indicators) {
    PrintHeader();

    foreach (var indicator in indicators.IndicatorsList) {
      PrintIndicatorDetails(indicator);
    }

    PrintOverallRisk(indicators.OverallRisk);
  }

  private static void PrintHeader() {
    Console.Clear();
    Console.WriteLine($"Timestamp: {DateTime.Now:HH:mm:ss}");
    Console.WriteLine($"{"Indicator",-21}{"Value",-9} Risk");
    Console.WriteLine("------------------------------------");
  }

  private static void PrintIndicatorDetails(Indicator indicator) {
    var screenName = indicator.Name.GetScreenName();
    var value = indicator.Value;
    var riskLevel = indicator.RiskLevel;

    Console.Write($"{screenName,-21}{value,-9:F2}");

    Console.BackgroundColor = riskLevel switch {
      RiskLevel.Green => ConsoleColor.Green,
      RiskLevel.Yellow => ConsoleColor.Yellow,
      _ => ConsoleColor.Red
    };
    Console.ForegroundColor = ConsoleColor.Black;
    Console.Write($" {riskLevel} ");
    Console.ResetColor();
    Console.WriteLine("");
  }

  private static void PrintOverallRisk(RiskLevel overall) {
    Console.Write("\nOverall Risk: ");
    Console.BackgroundColor = overall switch {
      RiskLevel.Green => ConsoleColor.Green,
      RiskLevel.Yellow => ConsoleColor.Yellow,
      _ => ConsoleColor.Red
    };
    Console.ForegroundColor = ConsoleColor.Black;
    Console.Write($" {overall} ");
    Console.ResetColor();
    Console.WriteLine("");
  }
}


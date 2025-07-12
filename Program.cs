namespace EWIM;

internal abstract class Program {
  private static ThresholdEngine? _engine;

  private static async Task Main() {
    _engine = new ThresholdEngine();

    await GenerateDummyDataLoop();
  }

  // Generates dummy data each second and processes it
  private static async Task GenerateDummyDataLoop() {
    var rnd = new Random();
    var indicatorData = new IndicatorData();

    while (true) {
      indicatorData.UpdateValue(IndicatorName.ReturnFlowPercent, rnd.NextDouble() * 10);
      indicatorData.UpdateValue(IndicatorName.PitGainBbl, rnd.NextDouble() * 3);
      indicatorData.UpdateValue(IndicatorName.StandpipePressure, rnd.Next(300, 1200));
      indicatorData.UpdateValue(IndicatorName.Rop, rnd.Next(50, 300));
      indicatorData.UpdateValue(IndicatorName.HookLoad, rnd.Next(-1500, 0));
      indicatorData.UpdateValue(IndicatorName.MudWeight, rnd.NextDouble() * 1.0);

      // Process and display
      ProcessIndicatorData(indicatorData);

      await Task.Delay(2000);
    }
  }

  // Evaluates data and prints matrix row to console
  private static void ProcessIndicatorData(IndicatorData data) {
    if (_engine == null) return;

    _engine.Evaluate(data);
    var overall = MatrixCalculator.ComputeOverallRisk(data);

    Console.Clear();
    Console.WriteLine($"Timestamp: {DateTime.Now:HH:mm:ss}");
    Console.WriteLine($"{"Indicator",-21}{"Value",-9} Risk");
    Console.WriteLine("------------------------------------");
    foreach (var kv in data.Fields) {
      var screenName = kv.Name.GetScreenName();
      var value = kv.Value;
      var riskLevel = kv.RiskLevel;

      Console.Write($"{screenName,-20} ");
      Console.Write($"{value,-10:0.00}");

      Console.BackgroundColor = riskLevel switch {
        RiskLevel.Green => ConsoleColor.Green,
        RiskLevel.Yellow => ConsoleColor.Yellow,
        _ => ConsoleColor.Red
      };

      Console.ForegroundColor = ConsoleColor.Black;
      Console.Write($" {riskLevel, -6} ");
      Console.ResetColor();
      Console.WriteLine("");
    }

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

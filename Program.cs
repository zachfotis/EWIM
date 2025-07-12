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
    while (true) {
      // Create dummy indicator data
      var data = new IndicatorData {
        ReturnFlowPercent = rnd.NextDouble() * 10,
        PitGainBbl = rnd.NextDouble() * 3,
        StandpipePressure = rnd.Next(300, 1200),
        Rop = rnd.Next(50, 300),
        HookLoad = rnd.Next(-1500, 0),
        MudWeight = rnd.NextDouble() * 1.0
      };

      // Process and display
      ProcessIndicatorData(data);

      await Task.Delay(1000);
    }
  }

  // Evaluates data and prints matrix row to console
  private static void ProcessIndicatorData(IndicatorData data) {
    var risks = _engine.Evaluate(data);
    var overall = MatrixCalculator.ComputeOverallRisk(risks);

    Console.Clear();
    Console.WriteLine($"Timestamp: {DateTime.Now:HH:mm:ss}");
    Console.WriteLine($"{"Indicator",-21}{"Value",-9} Risk");
    Console.WriteLine("------------------------------------");
    foreach (var kv in risks) {
      var val = kv.Key switch {
        "ReturnFlowPercent" => data.ReturnFlowPercent,
        "PitGainBbl" => data.PitGainBbl,
        "StandpipePressure" => data.StandpipePressure,
        "Rop" => data.Rop,
        "HookLoad" => data.HookLoad,
        "MudWeight" => data.MudWeight,
        _ => 0.0
      };

      Console.Write($"{kv.Key,-20} ");
      Console.Write($"{val,-10:0.00}");

      Console.BackgroundColor = kv.Value switch {
        RiskLevel.Green => ConsoleColor.Green,
        RiskLevel.Yellow => ConsoleColor.Yellow,
        _ => ConsoleColor.Red
      };

      Console.ForegroundColor = ConsoleColor.Black;
      Console.Write($"{kv.Value}");
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
    Console.Write(overall);
    Console.ResetColor();
    Console.WriteLine("");
  }
}

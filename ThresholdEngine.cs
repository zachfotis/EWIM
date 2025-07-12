// Engine to evaluate indicator risk levels

namespace EWIM;

public class ThresholdEngine {
  private readonly Dictionary<string, Threshold> _thresholds = new() {
    { "ReturnFlowPercent", new Threshold { Name = "ReturnFlowPercent", YellowMin = 2, YellowMax = 5, RedMin = 5 } },
    { "PitGainBbl", new Threshold { Name = "PitGainBbl", YellowMin = 0.5, YellowMax = 1.5, RedMin = 1.5 } },
    { "StandpipePressure", new Threshold { Name = "StandpipePressure", YellowMin = 500, YellowMax = 1000, RedMin = 1000 } },
    { "Rop", new Threshold { Name = "Rop", YellowMin = 100, YellowMax = 200, RedMin = 200 } },
    { "HookLoad", new Threshold { Name = "HookLoad", YellowMin = -1000, YellowMax = -500, RedMin = -500 } },
    { "MudWeight", new Threshold { Name = "MudWeight", YellowMin = 0.2, YellowMax = 0.5, RedMin = 0.5 } }
  };

  public Dictionary<string, RiskLevel> Evaluate(IndicatorData data) {
    var results = new Dictionary<string, RiskLevel>();
    foreach (var (key, t) in _thresholds) {
      var value = key switch {
        "ReturnFlowPercent" => data.ReturnFlowPercent,
        "PitGainBbl" => data.PitGainBbl,
        "StandpipePressure" => data.StandpipePressure,
        "Rop" => data.Rop,
        "HookLoad" => data.HookLoad,
        "MudWeight" => data.MudWeight,
        _ => 0.0
      };

      var level = RiskLevel.Green;

      if (value >= t.RedMin) {
        level = RiskLevel.Red;
      } else if (value >= t.YellowMin && value <= t.YellowMax) {
        level = RiskLevel.Yellow;
      }

      results[key] = level;
    }

    return results;
  }
}

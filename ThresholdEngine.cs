// Engine to evaluate indicator risk levels

namespace EWIM;

public class ThresholdEngine {
  private readonly Dictionary<IndicatorName, Threshold> _thresholds = new() {
    { IndicatorName.ReturnFlowPercent, new Threshold { YellowMin = 2, YellowMax = 5, RedMin = 5 } },
    { IndicatorName.PitGainBbl, new Threshold { YellowMin = 0.5, YellowMax = 1.5, RedMin = 1.5 } },
    { IndicatorName.StandpipePressure, new Threshold { YellowMin = 500, YellowMax = 1000, RedMin = 1000 } },
    { IndicatorName.Rop, new Threshold { YellowMin = 100, YellowMax = 200, RedMin = 200 } },
    { IndicatorName.HookLoad, new Threshold { YellowMin = -1000, YellowMax = -500, RedMin = -500 } },
    { IndicatorName.MudWeight, new Threshold { YellowMin = 0.2, YellowMax = 0.5, RedMin = 0.5 } }
  };

  private RiskLevel CheckRiskLevel(IndicatorField data) {
    var currentThreshold = _thresholds[data.Name];

    if (data.Value >= currentThreshold.RedMin) {
      return RiskLevel.Red;
    } else if (data.Value >= currentThreshold.YellowMin && data.Value <= currentThreshold.YellowMax) {
      return RiskLevel.Yellow;
    } else {
      return RiskLevel.Green;
    }
  }

  public void Evaluate(IndicatorData data) {
    foreach (var field in data.Fields) {
      var level = CheckRiskLevel(field);
      data.UpdateRiskLevel(field.Name, level);
    }
  }
}

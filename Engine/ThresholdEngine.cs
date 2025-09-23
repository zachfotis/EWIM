using EWIM.Models;

namespace EWIM.Engine;

public static class ThresholdEngine {
  public static RiskLevel ComputeRiskLevel(Indicator data) {
    var currentThreshold = ThresholdLimits.Thresholds[data.Name];

    if (data.Value >= currentThreshold.YellowMax) {
      return RiskLevel.Red;
    }

    if (data.Value >= currentThreshold.GreenMax && data.Value <= currentThreshold.YellowMax) {
      return RiskLevel.Yellow;
    }

    return RiskLevel.Green;
  }

  public static RiskLevel ComputeOverallRisk(Indicators indicatorData) {
    var riskLevels = indicatorData.IndicatorsList.Select(f => f.RiskLevel).ToArray();

    if (riskLevels.Any(r => r == RiskLevel.Red)) return RiskLevel.Red;

    return riskLevels.Any(r => r == RiskLevel.Yellow) ? RiskLevel.Yellow : RiskLevel.Green;
  }
}

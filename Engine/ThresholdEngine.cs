using System.Collections.Generic;
using System.Linq;
using EWIM.Classes;
using EWIM.Models;
using EWIM.Services;

namespace EWIM.Engine {
  public static class ThresholdEngine {
    private static Dictionary<IndicatorName, Threshold> _thresholds;
    private static ThresholdPersistenceService _persistenceService;

    static ThresholdEngine() {
      _persistenceService = new ThresholdPersistenceService();
      LoadThresholds();
    }

    public static void LoadThresholds() {
      _thresholds = _persistenceService.LoadThresholds();
    }

    public static Dictionary<IndicatorName, Threshold> GetCurrentThresholds() {
      return new Dictionary<IndicatorName, Threshold>(_thresholds);
    }

    public static void UpdateThresholds(Dictionary<IndicatorName, Threshold> newThresholds) {
      _thresholds = new Dictionary<IndicatorName, Threshold>(newThresholds);
    }

    public static RiskLevel ComputeRiskLevel(Indicator data) {
      if (!_thresholds.ContainsKey(data.Name)) {
        // Fallback to default if threshold not found
        return RiskLevel.Green;
      }

      var currentThreshold = _thresholds[data.Name];

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
}

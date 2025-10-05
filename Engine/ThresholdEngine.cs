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
        var defaultRisk = RiskLevel.Green;
        IndicatorSequenceTracker.Instance.UpdateIndicatorStatus(data.Name, defaultRisk);
        return defaultRisk;
      }

      // Do not set red for indicators with value 0
      if (data.Value == 0) {
        var greenRisk = RiskLevel.Green;
        IndicatorSequenceTracker.Instance.UpdateIndicatorStatus(data.Name, greenRisk);
        return greenRisk;
      }

      var currentThreshold = _thresholds[data.Name];
      RiskLevel riskLevel;

      if (data.Value >= currentThreshold.YellowMax) {
        riskLevel = RiskLevel.Red;
      } else if (data.Value >= currentThreshold.GreenMax && data.Value <= currentThreshold.YellowMax) {
        riskLevel = RiskLevel.Yellow;
      } else {
        riskLevel = RiskLevel.Green;
      }

      // Update sequence tracker
      IndicatorSequenceTracker.Instance.UpdateIndicatorStatus(data.Name, riskLevel);

      return riskLevel;
    }

    public static RiskLevel ComputeOverallRisk(Indicators indicatorData) {
      var riskLevels = indicatorData.IndicatorsList.Select(f => f.RiskLevel).ToArray();

      if (riskLevels.Any(r => r == RiskLevel.Red)) return RiskLevel.Red;

      return riskLevels.Any(r => r == RiskLevel.Yellow) ? RiskLevel.Yellow : RiskLevel.Green;
    }
  }
}

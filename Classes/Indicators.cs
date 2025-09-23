using EWIM.Engine;
using EWIM.Models;

namespace EWIM;

public class Indicators {
  public List<Indicator> IndicatorsList { get; init; } = [];
  public RiskLevel OverallRisk { get; set; } = RiskLevel.Green;

  public Indicators() {
    foreach (IndicatorName name in Enum.GetValues(typeof(IndicatorName))) {
      Indicator indicator = new Indicator { Name = name, Value = 0, RiskLevel = RiskLevel.Green };
      IndicatorsList.Add(indicator);
    }
  }

  public void UpdateIndicatorValue(RawIndicator rawIndicator) {
    var indicator = IndicatorsList.FirstOrDefault(i => i.Name == rawIndicator.Name);
    if (indicator != null) {
      indicator.Value = rawIndicator.Value;
      indicator.RiskLevel = ThresholdEngine.ComputeRiskLevel(indicator);
    }

    OverallRisk = ThresholdEngine.ComputeOverallRisk(this);
  }
}

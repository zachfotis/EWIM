using System;
using System.Collections.Generic;
using System.Linq;
using EWIM.Engine;
using EWIM.Models;

namespace EWIM.Classes
{
  public class Indicators
  {
    public List<Indicator> IndicatorsList { get; set; } = new List<Indicator>();
    public RiskLevel OverallRisk { get; set; } = RiskLevel.Green;

    public Indicators()
    {
      foreach (IndicatorName name in Enum.GetValues(typeof(IndicatorName)))
      {
        Indicator indicator = new Indicator { Name = name, Value = 0, RiskLevel = RiskLevel.Green };
        IndicatorsList.Add(indicator);
      }
    }

    public void UpdateIndicatorValue(RawIndicator rawIndicator)
    {
      var indicator = IndicatorsList.FirstOrDefault(i => i.Name == rawIndicator.Name);
      if (indicator != null)
      {
        indicator.Value = rawIndicator.Value;
        indicator.RiskLevel = ThresholdEngine.ComputeRiskLevel(indicator);
      }

      OverallRisk = ThresholdEngine.ComputeOverallRisk(this);
    }
  }
}

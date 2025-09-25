using System.Collections.Generic;
using System.Linq;
using EWIM.Classes;
using EWIM.Models;

namespace EWIM.Engine
{
  public static class ThresholdEngine
  {
    private static readonly Dictionary<IndicatorName, Threshold> Thresholds = new Dictionary<IndicatorName, Threshold>
    {
      { IndicatorName.ReturnFlowPercent, new Threshold { GreenMax = 2, YellowMax = 5 } },
      { IndicatorName.PitGainBbl, new Threshold { GreenMax = 0.5, YellowMax = 1.5 } },
      { IndicatorName.StandpipePressure, new Threshold { GreenMax = 500, YellowMax = 1000 } },
      { IndicatorName.Rop, new Threshold { GreenMax = 100, YellowMax = 200 } },
      { IndicatorName.Wob, new Threshold { GreenMax = 20000, YellowMax = 40000 } },
      { IndicatorName.HookLoad, new Threshold { GreenMax = -1000, YellowMax = -5000 } },
      { IndicatorName.MudWeight, new Threshold { GreenMax = 0.2, YellowMax = 0.5 } }
    };

    public static RiskLevel ComputeRiskLevel(Indicator data)
    {
      var currentThreshold = Thresholds[data.Name];

      if (data.Value >= currentThreshold.YellowMax)
      {
        return RiskLevel.Red;
      }

      if (data.Value >= currentThreshold.GreenMax && data.Value <= currentThreshold.YellowMax)
      {
        return RiskLevel.Yellow;
      }

      return RiskLevel.Green;
    }

    public static RiskLevel ComputeOverallRisk(Indicators indicatorData)
    {
      var riskLevels = indicatorData.IndicatorsList.Select(f => f.RiskLevel).ToArray();

      if (riskLevels.Any(r => r == RiskLevel.Red)) return RiskLevel.Red;

      return riskLevels.Any(r => r == RiskLevel.Yellow) ? RiskLevel.Yellow : RiskLevel.Green;
    }
  }
}

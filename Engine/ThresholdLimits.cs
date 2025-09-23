using EWIM.Models;

public static class ThresholdLimits {
  public static readonly Dictionary<IndicatorName, Threshold> Thresholds = new() {
    { IndicatorName.ReturnFlowPercent, new Threshold { GreenMax = 2, YellowMax = 5 } },
    { IndicatorName.PitGainBbl, new Threshold { GreenMax = 0.5, YellowMax = 1.5 } },
    { IndicatorName.StandpipePressure, new Threshold { GreenMax = 500, YellowMax = 1000 } },
    { IndicatorName.Rop, new Threshold { GreenMax = 100, YellowMax = 200 } },
    { IndicatorName.HookLoad, new Threshold { GreenMax = -1000, YellowMax = -5000 } },
    { IndicatorName.MudWeight, new Threshold { GreenMax = 0.2, YellowMax = 0.5 } }
  };
}

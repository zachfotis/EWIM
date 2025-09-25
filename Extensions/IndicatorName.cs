using System;
using EWIM.Models;

public static class IndicatorNameExtensions
{
  public static string GetScreenName(this IndicatorName name)
  {
    switch (name)
    {
      case IndicatorName.ReturnFlowPercent:
        return "Return Flow (%)";
      case IndicatorName.PitGainBbl:
        return "Pit Gain (bbl)";
      case IndicatorName.StandpipePressure:
        return "SPP (psi)";
      case IndicatorName.Rop:
        return "ROP (ft/hr)";
      case IndicatorName.HookLoad:
        return "Hook Load (lbs)";
      case IndicatorName.MudWeight:
        return "Mud Weight (ppg)";
      case IndicatorName.Wob:
        return "Weight on Bit (lbs)";
      default:
        throw new ArgumentOutOfRangeException(nameof(name), name, null);
    }
  }
}

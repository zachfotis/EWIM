using EWIM.Models;

public static class IndicatorNameExtensions {
  public static string GetScreenName(this IndicatorName name) {
    return name switch {
      IndicatorName.ReturnFlowPercent => "Return Flow (%)",
      IndicatorName.PitGainBbl => "Pit Gain (bbl)",
      IndicatorName.StandpipePressure => "SPP (psi)",
      IndicatorName.Rop => "ROP (ft/hr)",
      IndicatorName.HookLoad => "Hook Load (lbs)",
      IndicatorName.MudWeight => "Mud Weight (ppg)",
      IndicatorName.Wob => "Weight on Bit (lbs)",
      _ => throw new ArgumentOutOfRangeException(nameof(name), name, null)
    };
  }
}

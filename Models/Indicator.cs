namespace EWIM.Models;

public class Indicator {
  public IndicatorName Name { get; init; }
  public double Value { get; set; }

  public RiskLevel RiskLevel { get; set; }
}


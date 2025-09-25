namespace EWIM.Models
{
  public class Indicator
  {
    public IndicatorName Name { get; set; }
    public double Value { get; set; }

    public RiskLevel RiskLevel { get; set; }
  }
}


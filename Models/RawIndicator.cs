namespace EWIM.Models
{
  public class RawIndicator
  {
    public IndicatorName Name { get; set; }
    public double Value { get; private set; }

    public RawIndicator(IndicatorName name)
    {
      Name = name;
      Value = 0;
    }

    public void UpdateValue(double newValue)
    {
      Value = newValue;
    }
  }
}

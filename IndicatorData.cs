namespace EWIM;

// Data model for drilling indicators
public class IndicatorField {
  public IndicatorName Name { get; init; }
  public double Value { get; set; }

  public RiskLevel RiskLevel { get; set; }
}

public class IndicatorData {
  public List<IndicatorField> Fields { get; } = [];

  public RiskLevel[] Risks => Fields.Select(f => f.RiskLevel).ToArray();

  private void CreateField(IndicatorName name, double value) {
    Fields.Add(new IndicatorField { Name = name, Value = value, RiskLevel = RiskLevel.Green });
  }

  public void UpdateValue(IndicatorName name, double value) {
    var field = Fields.FirstOrDefault(f => f.Name == name);
    if (field != null) {
      field.Value = value;
    } else {
      CreateField(name, value);
    }
  }

  public void UpdateRiskLevel(IndicatorName name, RiskLevel riskLevel) {
    var field = Fields.FirstOrDefault(f => f.Name == name);
    if (field != null) {
      field.RiskLevel = riskLevel;
    }
  }
}

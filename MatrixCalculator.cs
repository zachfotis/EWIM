namespace EWIM;

public static class MatrixCalculator {
  public static RiskLevel ComputeOverallRisk(IndicatorData indicatorData) {
    var risks = indicatorData.Risks;

    if (risks.Any(r => r == RiskLevel.Red)) {
      return RiskLevel.Red;
    }

    return risks.Any(r => r == RiskLevel.Yellow) ? RiskLevel.Yellow : RiskLevel.Green;
  }
}

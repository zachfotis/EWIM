namespace EWIM;

  // Computes overall risk from individual indicator risks
public static class MatrixCalculator {
  public static RiskLevel ComputeOverallRisk(Dictionary<string, RiskLevel> indicatorRisks) {
    if (indicatorRisks.ContainsValue(RiskLevel.Red)) {
      return RiskLevel.Red;
    }

    return indicatorRisks.ContainsValue(RiskLevel.Yellow) ? RiskLevel.Yellow : RiskLevel.Green;
  }
}

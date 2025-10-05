using System;
using System.Collections.Generic;

namespace EWIM.Models {
  public class ThresholdCalibrationReport {
    public DateTime CalibrationDate { get; set; }
    public BaselineData BaselineData { get; set; }
    public Dictionary<IndicatorName, Threshold> NewThresholds { get; set; }
    public Dictionary<IndicatorName, Threshold> OldThresholds { get; set; }
    public Dictionary<IndicatorName, IndicatorAnalysis> IndicatorAnalysis { get; set; }

    public ThresholdCalibrationReport() {
      NewThresholds = new Dictionary<IndicatorName, Threshold>();
      OldThresholds = new Dictionary<IndicatorName, Threshold>();
      IndicatorAnalysis = new Dictionary<IndicatorName, IndicatorAnalysis>();
    }
  }

  public class IndicatorAnalysis {
    public double BaselineMean { get; set; }
    public double BaselineStdDev { get; set; }
    public double BaselineRange { get; set; }
    public double NewGreenMax { get; set; }
    public double NewYellowMax { get; set; }
    public double? OldGreenMax { get; set; }
    public double? OldYellowMax { get; set; }
    public double? GreenThresholdChange { get; set; }
    public double? YellowThresholdChange { get; set; }
  }
}

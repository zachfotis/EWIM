using System;
using System.Collections.Generic;

namespace EWIM.Models {
  public class BaselineData {
    public DateTime CaptureDate { get; set; }
    public int SampleCount { get; set; }
    public int CaptureDurationSeconds { get; set; }
    public Dictionary<IndicatorName, IndicatorBaseline> IndicatorBaselines { get; set; }

    public BaselineData() {
      IndicatorBaselines = new Dictionary<IndicatorName, IndicatorBaseline>();
    }
  }

  public class IndicatorBaseline {
    public double Mean { get; set; }
    public double StandardDeviation { get; set; }
    public double Minimum { get; set; }
    public double Maximum { get; set; }
    public double Median { get; set; }
    public double Percentile95 { get; set; }
    public double Percentile99 { get; set; }
  }
}

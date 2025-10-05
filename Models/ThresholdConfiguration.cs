using System;
using System.Collections.Generic;

namespace EWIM.Models {
  public class ThresholdConfiguration {
    public DateTime LastUpdated { get; set; }
    public string Version { get; set; }
    public Dictionary<IndicatorName, Threshold> Thresholds { get; set; }
    public BaselineData BaselineData { get; set; }

    public ThresholdConfiguration() {
      Thresholds = new Dictionary<IndicatorName, Threshold>();
      Version = "1.0";
    }
  }
}

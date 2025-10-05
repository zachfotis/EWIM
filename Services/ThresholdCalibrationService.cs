using System;
using System.Collections.Generic;
using EWIM.Models;

namespace EWIM.Services {
  public class ThresholdCalibrationService {
    public enum CalibrationMethod {
      StandardDeviation,
      Percentile,
      StatisticalControl
    }

    public Dictionary<IndicatorName, Threshold> CalculateThresholds(
        BaselineData baselineData,
        CalibrationMethod method = CalibrationMethod.StandardDeviation) {
      var thresholds = new Dictionary<IndicatorName, Threshold>();

      foreach (var kvp in baselineData.IndicatorBaselines) {
        var indicatorName = kvp.Key;
        var baseline = kvp.Value;

        Threshold threshold;
        switch (method) {
          case CalibrationMethod.StandardDeviation:
            threshold = CalculateStandardDeviationThreshold(baseline);
            break;
          case CalibrationMethod.Percentile:
            threshold = CalculatePercentileThreshold(baseline);
            break;
          case CalibrationMethod.StatisticalControl:
            threshold = CalculateStatisticalControlThreshold(baseline);
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof(method), method, null);
        }

        thresholds[indicatorName] = threshold;
      }

      return thresholds;
    }

    private Threshold CalculateStandardDeviationThreshold(IndicatorBaseline baseline) {
      // Green: Mean ± 1 standard deviation
      // Yellow: Mean ± 2 standard deviations  
      // Red: Beyond Mean ± 2 standard deviations

      var greenMax = baseline.Mean + baseline.StandardDeviation;
      var yellowMax = baseline.Mean + (2 * baseline.StandardDeviation);

      return new Threshold {
        GreenMax = Math.Max(greenMax, baseline.Mean * 1.1), // At least 10% above mean
        YellowMax = Math.Max(yellowMax, baseline.Mean * 1.2) // At least 20% above mean
      };
    }

    private Threshold CalculatePercentileThreshold(IndicatorBaseline baseline) {
      // Green: Up to 95th percentile
      // Yellow: Up to 99th percentile
      // Red: Beyond 99th percentile

      return new Threshold {
        GreenMax = baseline.Percentile95,
        YellowMax = baseline.Percentile99
      };
    }

    private Threshold CalculateStatisticalControlThreshold(IndicatorBaseline baseline) {
      // Statistical Process Control approach
      // Green: Mean + 2σ (95% confidence)
      // Yellow: Mean + 3σ (99.7% confidence)
      // Red: Beyond Mean + 3σ

      var twoSigma = baseline.Mean + (2 * baseline.StandardDeviation);
      var threeSigma = baseline.Mean + (3 * baseline.StandardDeviation);

      return new Threshold {
        GreenMax = twoSigma,
        YellowMax = threeSigma
      };
    }

    public ThresholdCalibrationReport GenerateCalibrationReport(
        BaselineData baselineData,
        Dictionary<IndicatorName, Threshold> newThresholds,
        Dictionary<IndicatorName, Threshold> oldThresholds = null) {
      var report = new ThresholdCalibrationReport {
        CalibrationDate = DateTime.Now,
        BaselineData = baselineData,
        NewThresholds = newThresholds,
        OldThresholds = oldThresholds ?? new Dictionary<IndicatorName, Threshold>(),
        IndicatorAnalysis = new Dictionary<IndicatorName, IndicatorAnalysis>()
      };

      foreach (var kvp in newThresholds) {
        var indicatorName = kvp.Key;
        var newThreshold = kvp.Value;
        var baseline = baselineData.IndicatorBaselines[indicatorName];

        var hasOldThreshold = oldThresholds?.ContainsKey(indicatorName) == true;
        var oldThreshold = hasOldThreshold ? oldThresholds[indicatorName] : null;

        var analysis = new IndicatorAnalysis {
          BaselineMean = baseline.Mean,
          BaselineStdDev = baseline.StandardDeviation,
          BaselineRange = baseline.Maximum - baseline.Minimum,
          NewGreenMax = newThreshold.GreenMax,
          NewYellowMax = newThreshold.YellowMax,
          OldGreenMax = oldThreshold?.GreenMax,
          OldYellowMax = oldThreshold?.YellowMax
        };

        if (hasOldThreshold) {
          analysis.GreenThresholdChange = ((newThreshold.GreenMax - oldThreshold.GreenMax) / oldThreshold.GreenMax) * 100;
          analysis.YellowThresholdChange = ((newThreshold.YellowMax - oldThreshold.YellowMax) / oldThreshold.YellowMax) * 100;
        }

        report.IndicatorAnalysis[indicatorName] = analysis;
      }

      return report;
    }
  }
}

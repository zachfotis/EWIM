using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWIM.Classes;
using EWIM.Models;

namespace EWIM.Services {
  public class BaselineCaptureService {
    private readonly List<Dictionary<IndicatorName, double>> _capturedSamples;
    private bool _isCapturing;
    private DateTime _captureStartTime;
    private readonly int _captureDurationSeconds;
    private readonly int _sampleIntervalMs;

    // Event that fires when capture is completed
    public event Action CaptureCompleted;

    public bool IsCapturing => _isCapturing;
    public int SampleCount => _capturedSamples.Count;
    public TimeSpan CaptureProgress => _isCapturing ? DateTime.Now - _captureStartTime : TimeSpan.Zero;

    public BaselineCaptureService(int captureDurationSeconds = 60, int sampleIntervalMs = 1000) {
      _capturedSamples = new List<Dictionary<IndicatorName, double>>();
      _captureDurationSeconds = captureDurationSeconds;
      _sampleIntervalMs = sampleIntervalMs;
    }

    public void StartCapture() {
      if (_isCapturing) {
        throw new InvalidOperationException("Baseline capture is already in progress");
      }

      _capturedSamples.Clear();
      _isCapturing = true;
      _captureStartTime = DateTime.Now;

      Console.WriteLine($"Starting baseline capture for {_captureDurationSeconds} seconds...");
    }

    public void StopCapture() {
      if (!_isCapturing) {
        return;
      }

      _isCapturing = false;
      Console.WriteLine($"Baseline capture completed. Collected {_capturedSamples.Count} samples.");

      // Fire the completed event for auto-application
      CaptureCompleted?.Invoke();
    }

    public void CaptureSample(Indicators indicators) {
      if (!_isCapturing) {
        return;
      }

      // Check if capture duration has elapsed
      if (DateTime.Now - _captureStartTime >= TimeSpan.FromSeconds(_captureDurationSeconds)) {
        StopCapture();
        return;
      }

      var sample = new Dictionary<IndicatorName, double>();
      foreach (var indicator in indicators.IndicatorsList) {
        sample[indicator.Name] = indicator.Value;
      }

      _capturedSamples.Add(sample);
    }

    public BaselineData GetBaselineData() {
      if (_capturedSamples.Count == 0) {
        throw new InvalidOperationException("No baseline samples captured");
      }

      var baselineData = new BaselineData {
        CaptureDate = DateTime.Now,
        SampleCount = _capturedSamples.Count,
        CaptureDurationSeconds = _captureDurationSeconds,
        IndicatorBaselines = new Dictionary<IndicatorName, IndicatorBaseline>()
      };

      foreach (IndicatorName indicatorName in Enum.GetValues(typeof(IndicatorName))) {
        var values = _capturedSamples.Select(s => s[indicatorName]).ToList();

        baselineData.IndicatorBaselines[indicatorName] = new IndicatorBaseline {
          Mean = values.Average(),
          StandardDeviation = CalculateStandardDeviation(values),
          Minimum = values.Min(),
          Maximum = values.Max(),
          Median = CalculateMedian(values),
          Percentile95 = CalculatePercentile(values, 0.95),
          Percentile99 = CalculatePercentile(values, 0.99)
        };
      }

      return baselineData;
    }

    public void ClearSamples() {
      _capturedSamples.Clear();
      _isCapturing = false;
    }

    private double CalculateStandardDeviation(List<double> values) {
      var mean = values.Average();
      var sumOfSquaredDifferences = values.Sum(v => Math.Pow(v - mean, 2));
      return Math.Sqrt(sumOfSquaredDifferences / values.Count);
    }

    private double CalculateMedian(List<double> values) {
      var sorted = values.OrderBy(v => v).ToList();
      var count = sorted.Count;

      if (count % 2 == 0) {
        return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
      } else {
        return sorted[count / 2];
      }
    }

    private double CalculatePercentile(List<double> values, double percentile) {
      var sorted = values.OrderBy(v => v).ToList();
      var index = (int)Math.Ceiling(percentile * sorted.Count) - 1;
      return sorted[Math.Max(0, Math.Min(index, sorted.Count - 1))];
    }
  }
}

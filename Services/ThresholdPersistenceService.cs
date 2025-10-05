using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using EWIM.Models;

namespace EWIM.Services {
  public class ThresholdPersistenceService {
    private readonly string _thresholdsFilePath;
    private readonly string _baselineHistoryFilePath;
    private readonly JsonSerializerSettings _jsonSettings;

    public ThresholdPersistenceService(string configDirectory = "Config") {
      // Ensure config directory exists
      if (!Directory.Exists(configDirectory)) {
        Directory.CreateDirectory(configDirectory);
      }

      _thresholdsFilePath = Path.Combine(configDirectory, "dynamic_thresholds.json");
      _baselineHistoryFilePath = Path.Combine(configDirectory, "baseline_history.json");

      _jsonSettings = new JsonSerializerSettings {
        Formatting = Formatting.Indented,
        DateFormatHandling = DateFormatHandling.IsoDateFormat
      };
    }

    public Dictionary<IndicatorName, Threshold> LoadThresholds() {
      try {
        if (!File.Exists(_thresholdsFilePath)) {
          return GetDefaultThresholds();
        }

        var json = File.ReadAllText(_thresholdsFilePath);
        var thresholdData = JsonConvert.DeserializeObject<ThresholdConfiguration>(json, _jsonSettings);

        if (thresholdData?.Thresholds == null) {
          return GetDefaultThresholds();
        }

        return thresholdData.Thresholds;
      } catch (Exception ex) {
        Console.WriteLine($"Error loading thresholds: {ex.Message}");
        Console.WriteLine("Using default thresholds.");
        return GetDefaultThresholds();
      }
    }

    public void SaveThresholds(Dictionary<IndicatorName, Threshold> thresholds, BaselineData baselineData = null) {
      try {
        var thresholdConfig = new ThresholdConfiguration {
          LastUpdated = DateTime.Now,
          Thresholds = thresholds,
          BaselineData = baselineData,
          Version = "1.0"
        };

        var json = JsonConvert.SerializeObject(thresholdConfig, _jsonSettings);
        File.WriteAllText(_thresholdsFilePath, json);

        Console.WriteLine($"Thresholds saved to: {_thresholdsFilePath}");
      } catch (Exception ex) {
        Console.WriteLine($"Error saving thresholds: {ex.Message}");
        throw;
      }
    }

    public void SaveBaselineHistory(BaselineData baselineData) {
      try {
        var history = LoadBaselineHistory();
        history.Add(baselineData);

        // Keep only last 10 baselines to prevent file from growing too large
        if (history.Count > 10) {
          history.RemoveAt(0);
        }

        var json = JsonConvert.SerializeObject(history, _jsonSettings);
        File.WriteAllText(_baselineHistoryFilePath, json);

        Console.WriteLine($"Baseline history updated: {_baselineHistoryFilePath}");
      } catch (Exception ex) {
        Console.WriteLine($"Error saving baseline history: {ex.Message}");
      }
    }

    public List<BaselineData> LoadBaselineHistory() {
      try {
        if (!File.Exists(_baselineHistoryFilePath)) {
          return new List<BaselineData>();
        }

        var json = File.ReadAllText(_baselineHistoryFilePath);
        return JsonConvert.DeserializeObject<List<BaselineData>>(json, _jsonSettings) ?? new List<BaselineData>();
      } catch (Exception ex) {
        Console.WriteLine($"Error loading baseline history: {ex.Message}");
        return new List<BaselineData>();
      }
    }

    public void BackupCurrentConfiguration() {
      try {
        if (File.Exists(_thresholdsFilePath)) {
          var backupPath = $"{_thresholdsFilePath}.backup.{DateTime.Now:yyyyMMdd_HHmmss}";
          File.Copy(_thresholdsFilePath, backupPath);
          Console.WriteLine($"Configuration backed up to: {backupPath}");
        }
      } catch (Exception ex) {
        Console.WriteLine($"Error creating backup: {ex.Message}");
      }
    }

    public void ResetToDefaults() {
      try {
        BackupCurrentConfiguration();

        var defaultThresholds = GetDefaultThresholds();
        SaveThresholds(defaultThresholds);

        Console.WriteLine("Thresholds reset to default values.");
      } catch (Exception ex) {
        Console.WriteLine($"Error resetting to defaults: {ex.Message}");
        throw;
      }
    }

    private Dictionary<IndicatorName, Threshold> GetDefaultThresholds() {
      return new Dictionary<IndicatorName, Threshold>
      {
                { IndicatorName.Rop, new Threshold { GreenMax = 100, YellowMax = 200 } },
                { IndicatorName.Wob, new Threshold { GreenMax = 20000, YellowMax = 40000 } },
                { IndicatorName.ReturnFlowPercent, new Threshold { GreenMax = 0.20, YellowMax = 0.3 } },
                { IndicatorName.PitGainBbl, new Threshold { GreenMax = 205, YellowMax = 210 } },
                { IndicatorName.StandpipePressure, new Threshold { GreenMax = 100, YellowMax = 200 } },
                { IndicatorName.HookLoad, new Threshold { GreenMax = 300000, YellowMax = 500000 } },
                { IndicatorName.MudWeight, new Threshold { GreenMax = 10.1, YellowMax = 11 } }
            };
    }
  }
}

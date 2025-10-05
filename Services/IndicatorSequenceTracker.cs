using System;
using System.Collections.Generic;
using EWIM.Models;

namespace EWIM.Services {
  public class IndicatorSequenceTracker {
    private static IndicatorSequenceTracker _instance;
    private Dictionary<IndicatorName, int> _orangeSequence; // Now tracks both orange and red indicators
    private Dictionary<IndicatorName, RiskLevel> _previousRiskLevels;
    private int _sequenceCounter;

    private IndicatorSequenceTracker() {
      _orangeSequence = new Dictionary<IndicatorName, int>(); // Now tracks both orange and red indicators
      _previousRiskLevels = new Dictionary<IndicatorName, RiskLevel>();
      _sequenceCounter = 0;
    }

    public static IndicatorSequenceTracker Instance {
      get {
        if (_instance == null) {
          _instance = new IndicatorSequenceTracker();
        }
        return _instance;
      }
    }

    public void UpdateIndicatorStatus(IndicatorName name, RiskLevel currentRiskLevel) {
      // Get previous risk level for this indicator
      var previousRiskLevel = _previousRiskLevels.ContainsKey(name)
        ? _previousRiskLevels[name]
        : RiskLevel.Green;

      // Check if indicator just turned orange (Yellow) or red (Red) - whichever comes first gets the sequence number
      if ((currentRiskLevel == RiskLevel.Yellow || currentRiskLevel == RiskLevel.Red) &&
          previousRiskLevel == RiskLevel.Green &&
          !_orangeSequence.ContainsKey(name)) {
        // Indicator just turned orange or red from green - assign next sequence number
        _sequenceCounter++;
        _orangeSequence[name] = _sequenceCounter;
      }
      // Check if indicator returned to green - remove from sequence tracking
      else if (currentRiskLevel == RiskLevel.Green && _orangeSequence.ContainsKey(name)) {
        // Remove from orange sequence tracking
        _orangeSequence.Remove(name);

        // Check if all indicators are now green - if so, reset sequence
        if (_orangeSequence.Count == 0) {
          _sequenceCounter = 0;
          Console.WriteLine("All indicators are green - sequence numbering automatically reset!");
        }
      }

      // Update previous risk level
      _previousRiskLevels[name] = currentRiskLevel;
    }

    public int? GetSequenceNumber(IndicatorName name) {
      if (_orangeSequence.ContainsKey(name)) {
        return _orangeSequence[name];
      }
      return null;
    }

    // Keep the old method for backward compatibility
    public int? GetOrangeSequenceNumber(IndicatorName name) {
      return GetSequenceNumber(name);
    }

    public void ResetSequence() {
      _orangeSequence.Clear();
      _previousRiskLevels.Clear();
      _sequenceCounter = 0;
    }

    public Dictionary<IndicatorName, int> GetCurrentSequence() {
      return new Dictionary<IndicatorName, int>(_orangeSequence);
    }

    // Keep the old method for backward compatibility
    public Dictionary<IndicatorName, int> GetCurrentOrangeSequence() {
      return GetCurrentSequence();
    }

    public int GetTotalAbnormalIndicators() {
      return _orangeSequence.Count;
    }

    // Keep the old method for backward compatibility
    public int GetTotalOrangeIndicators() {
      return GetTotalAbnormalIndicators();
    }
  }
}

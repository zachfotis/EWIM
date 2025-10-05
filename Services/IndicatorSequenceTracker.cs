using System;
using System.Collections.Generic;
using EWIM.Models;

namespace EWIM.Services {
  public class IndicatorSequenceTracker {
    private static IndicatorSequenceTracker _instance;
    private Dictionary<IndicatorName, int> _orangeSequence;
    private Dictionary<IndicatorName, RiskLevel> _previousRiskLevels;
    private int _sequenceCounter;

    private IndicatorSequenceTracker() {
      _orangeSequence = new Dictionary<IndicatorName, int>();
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

      // Check if indicator just turned orange/yellow
      if (currentRiskLevel == RiskLevel.Yellow && previousRiskLevel != RiskLevel.Yellow) {
        // Indicator just turned orange - assign next sequence number
        _sequenceCounter++;
        _orangeSequence[name] = _sequenceCounter;
      }
      // Check if indicator is no longer orange/yellow
      else if (currentRiskLevel != RiskLevel.Yellow && _orangeSequence.ContainsKey(name)) {
        // Remove from orange sequence tracking
        _orangeSequence.Remove(name);
      }

      // Update previous risk level
      _previousRiskLevels[name] = currentRiskLevel;
    }

    public int? GetOrangeSequenceNumber(IndicatorName name) {
      if (_orangeSequence.ContainsKey(name)) {
        return _orangeSequence[name];
      }
      return null;
    }

    public void ResetSequence() {
      _orangeSequence.Clear();
      _previousRiskLevels.Clear();
      _sequenceCounter = 0;
    }

    public Dictionary<IndicatorName, int> GetCurrentOrangeSequence() {
      return new Dictionary<IndicatorName, int>(_orangeSequence);
    }

    public int GetTotalOrangeIndicators() {
      return _orangeSequence.Count;
    }
  }
}

using System;
using System.Threading.Tasks;
using EWIM.Classes;
using EWIM.Engine;
using EWIM.System;

namespace EWIM.Services {
  public class DynamicThresholdOrchestrator {
    private readonly BaselineCaptureService _baselineCapture;
    private readonly ThresholdCalibrationService _calibration;
    private readonly ThresholdPersistenceService _persistence;
    private readonly ConsoleInterfaceService _consoleInterface;
    private DSAPI _dsapi;

    public DynamicThresholdOrchestrator() {
      _baselineCapture = new BaselineCaptureService();
      _calibration = new ThresholdCalibrationService();
      _persistence = new ThresholdPersistenceService();
      _consoleInterface = new ConsoleInterfaceService(_baselineCapture, _calibration, _persistence);
    }

    public void Initialize(DSAPI dsapi = null) {
      _dsapi = dsapi; // Store reference for package control

      Console.WriteLine("=== EWIM Dynamic Threshold System Initialized ===");
      Console.WriteLine("Dynamic threshold system loaded successfully.");

      // Load existing thresholds into the engine
      ThresholdEngine.LoadThresholds();

      Console.WriteLine("Press '?' for help with calibration commands.");
      Console.WriteLine("=================================================\n");
    }

    public void ProcessIndicators(Indicators indicators) {
      // Capture baseline samples if capture is active
      _baselineCapture.CaptureSample(indicators);
    }

    public bool HandleUserInput(Indicators indicators) {
      if (Console.KeyAvailable) {
        var keyInfo = Console.ReadKey(true);
        return _consoleInterface.HandleInput(keyInfo, indicators, _dsapi);
      }

      return true; // Continue running
    }

    public void ShowStartupInfo() {
      _consoleInterface.ShowHelp();
    }
  }
}

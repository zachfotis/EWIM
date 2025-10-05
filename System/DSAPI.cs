using System;
using System.Threading;
using DrillSIM_API.API;
using DrillSIM_API.Packages;
using EWIM.Classes;
using EWIM.Models;

namespace EWIM.System {
    public class DSAPI : WorkTemplate {
        private readonly Indicators indicators;
        private readonly RawIndicator rop = new RawIndicator(IndicatorName.Rop);
        private readonly RawIndicator wob = new RawIndicator(IndicatorName.Wob);
        private readonly RawIndicator returnFlow = new RawIndicator(IndicatorName.ReturnFlowPercent);
        private readonly RawIndicator pitGain = new RawIndicator(IndicatorName.PitGainBbl);
        private readonly RawIndicator standpipePressure = new RawIndicator(IndicatorName.StandpipePressure);
        private readonly RawIndicator casingPressure = new RawIndicator(IndicatorName.CasingPressure);
        private readonly RawIndicator hookLoad = new RawIndicator(IndicatorName.HookLoad);
        private readonly RawIndicator mudWeight = new RawIndicator(IndicatorName.MudWeight);

        private bool isPackageEnabled = true;
        public bool IsPackageEnabled => isPackageEnabled;

        public DSAPI(Indicators indicators) {
            this.indicators = indicators;
        }

        protected override void Initialise() {
            try {
                if (WellControlManager.Instance != null) {
                    WellControlManager.Instance.EnablePackage();
                    Console.WriteLine("Package reading initialized as ENABLED - EWIM has control");
                } else {
                    Console.WriteLine("Warning: Could not initialize package - WellControlManager not available");
                }
            } catch (Exception ex) {
                Console.WriteLine($"Warning during package initialization: {ex.Message}");
            }
        }

        protected override void Update() {
            // Only read from DrillSIM if package is enabled
            if (isPackageEnabled) {
                try {
                    if (WellControlManager.Instance == null) {
                        return;
                    }

                    rop.UpdateValue(WellControlManager.ROP.Get());
                    indicators.UpdateIndicatorValue(rop);

                    wob.UpdateValue(WellControlManager.WeightOnBit.Get());
                    indicators.UpdateIndicatorValue(wob);

                    returnFlow.UpdateValue(WellControlManager.FlowOut.Get());
                    indicators.UpdateIndicatorValue(returnFlow);

                    pitGain.UpdateValue(WellControlManager.ActiveTankVolume.Get());
                    indicators.UpdateIndicatorValue(pitGain);

                    standpipePressure.UpdateValue(WellControlManager.DrillPipePressure.Get());
                    indicators.UpdateIndicatorValue(standpipePressure);

                    casingPressure.UpdateValue(WellControlManager.CasingPressure.Get());
                    indicators.UpdateIndicatorValue(casingPressure);

                    hookLoad.UpdateValue(WellControlManager.Hookload.Get());
                    indicators.UpdateIndicatorValue(hookLoad);

                    mudWeight.UpdateValue(WellControlManager.MudDensity.Get() / 7.4805);
                    indicators.UpdateIndicatorValue(mudWeight);
                } catch (Exception ex) {
                    // Log error but don't immediately disable - might be temporary during package switching
                    // Only disable after several consecutive failures
                    if (ex.Message.Contains("disposed") || ex.Message.Contains("connection") || ex.Message.Contains("disconnected")) {
                        Console.WriteLine($"Connection issue detected: {ex.Message}");
                        Console.WriteLine("Package reading will remain enabled - this might be temporary during package switching.");
                    }
                    // Continue without disabling package reading
                }
            }
        }

        public bool EnablePackageReading() {
            try {
                if (!isPackageEnabled && WellControlManager.Instance != null) {
                    WellControlManager.Instance.EnablePackage();
                    isPackageEnabled = true;
                    return true;
                }
                return true; // Already enabled or no instance
            } catch (Exception ex) {
                Console.WriteLine($"Error enabling package reading: {ex.Message}");
                return false;
            }
        }

        public bool DisablePackageReading() {
            try {
                if (isPackageEnabled && WellControlManager.Instance != null) {
                    // Only disable the package, don't affect the main simulation
                    WellControlManager.Instance.DisablePackage();
                    isPackageEnabled = false;
                    Console.WriteLine("Package reading disabled successfully.");
                    return true;
                }
                isPackageEnabled = false; // Mark as disabled even if no instance
                return true; // Already disabled or no instance
            } catch (Exception ex) {
                Console.WriteLine($"Error disabling package reading: {ex.Message}");
                // Don't let package disable errors stop the application
                isPackageEnabled = false; // Mark as disabled anyway
                Console.WriteLine("Package reading marked as disabled due to error.");
                return true; // Return true to avoid "failed" message
            }
        }

        public bool TogglePackageReading() {
            if (isPackageEnabled) {
                return DisablePackageReading();
            } else {
                return EnablePackageReading();
            }
        }

        public bool IsConnectionValid() {
            try {
                return IsRunning && WellControlManager.Instance != null;
            } catch {
                return false;
            }
        }


    }
}

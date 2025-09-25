using DrillSIM_API.API;
using DrillSIM_API.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EWIM.Classes;
using EWIM.Models;

namespace EWIM.System {
    class DSAPI : WorkTemplate {
        private readonly Indicators indicators;
        private readonly RawIndicator rop = new RawIndicator(IndicatorName.Rop);
        private readonly RawIndicator wob = new RawIndicator(IndicatorName.Wob);
        private readonly RawIndicator returnFlow = new RawIndicator(IndicatorName.ReturnFlowPercent);
        private readonly RawIndicator pitGain = new RawIndicator(IndicatorName.PitGainBbl);
        private readonly RawIndicator standpipePressure = new RawIndicator(IndicatorName.StandpipePressure);
        private readonly RawIndicator hookLoad = new RawIndicator(IndicatorName.HookLoad);
        private readonly RawIndicator mudWeight = new RawIndicator(IndicatorName.MudWeight);

        public DSAPI(Indicators indicators) {
            this.indicators = indicators;
        }

        protected override void Initialise() {
            WellControlManager.Instance.EnablePackage();
        }

        protected override void Update() {
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

            hookLoad.UpdateValue(WellControlManager.Hookload.Get());
            indicators.UpdateIndicatorValue(hookLoad);

            mudWeight.UpdateValue(WellControlManager.MudDensity.Get());
            indicators.UpdateIndicatorValue(mudWeight);
        }
    }
}

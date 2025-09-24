using EWIM.Classes;
using EWIM.Models;
using EWIM.Utilities;
using EWIM.System;

namespace EWIM;

internal static class Program
{
  private static readonly Indicators indicators = new();

  static void Main(string[] args)
  {
    DSAPI Project = new DSAPI();

    Project.Simulate();

    while (Project.IsRunning)
    {
      // Run info data
    }

    Project.Exit();
  }

  private static async Task RunEWIM()
  {
    RawIndicator returnFlow = new RawIndicator(IndicatorName.ReturnFlowPercent);
    RawIndicator pitGain = new RawIndicator(IndicatorName.PitGainBbl);
    RawIndicator standpipePressure = new RawIndicator(IndicatorName.StandpipePressure);
    RawIndicator rop = new RawIndicator(IndicatorName.Rop);
    RawIndicator hookLoad = new RawIndicator(IndicatorName.HookLoad);
    RawIndicator mudWeight = new RawIndicator(IndicatorName.MudWeight);

    var rnd = new Random();

    while (true)
    {
      returnFlow.UpdateValue(rnd.NextDouble() * 10);
      indicators.UpdateIndicatorValue(returnFlow);

      pitGain.UpdateValue(rnd.NextDouble() * 3);
      indicators.UpdateIndicatorValue(pitGain);

      standpipePressure.UpdateValue(rnd.Next(300, 1200));
      indicators.UpdateIndicatorValue(standpipePressure);

      rop.UpdateValue(rnd.Next(50, 300));
      indicators.UpdateIndicatorValue(rop);

      hookLoad.UpdateValue(rnd.Next(-1500, 0));
      indicators.UpdateIndicatorValue(hookLoad);

      mudWeight.UpdateValue(rnd.NextDouble() * 1.0);
      indicators.UpdateIndicatorValue(mudWeight);

      Logger.Log(indicators);

      await Task.Delay(2000);
    }
  }
}

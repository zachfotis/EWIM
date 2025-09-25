using System;
using System.Threading.Tasks;
using EWIM.Classes;
using EWIM.Utilities;
using EWIM.System;

namespace EWIM {
  internal static class Program {
    private static readonly Indicators indicators = new Indicators();

    static async Task Main() {
      DSAPI Simulation = new DSAPI(indicators);

      Simulation.Simulate();

      while (Simulation.IsRunning) {
        Logger.Log(indicators);
        await Task.Delay(2000);
      }

      Simulation.Exit();
    }

  }
}

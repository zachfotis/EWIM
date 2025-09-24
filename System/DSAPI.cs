using DrillSIM_API.API;

namespace EWIM.System;

class DSAPI : WorkTemplate
{
    public WorkMain Sim = new WorkMain();

    protected override void Initialise()
    {
        Sim.Initialise();
    }

    protected override void Update()
    {
        Sim.Update();
    }
}

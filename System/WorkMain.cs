using DrillSIM_API.Packages;

namespace EWIM.System;

class WorkMain
{
    public float WellDepth;

    public void Initialise()
    {
        // Do all initialisation here
        RateOfPenetration.Instance.EnablePackage();
        WellDepth = RateOfPenetration.WellTVD.Get();
    }

    public void Update()
    {
        // Do all updates here
        WellDepth = RateOfPenetration.WellTVD.Get();
        Console.WriteLine("Well Depth is {0}", WellDepth);
    }
}

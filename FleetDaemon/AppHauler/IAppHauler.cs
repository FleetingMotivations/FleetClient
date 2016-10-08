using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetDaemon.Hauler
{
    public interface IAppHauler
    {
        Dictionary<String, FleetRunningApplication> RunningApplications { get; }
        Dictionary<String, FleetKnownApplication> KnownApplications { get; }

        Boolean LaunchApplication(String identifier);
        Boolean CloseApplication(String identifier);
        Boolean IsRunning(String identifier);
        Boolean IsRunningOrLaunch(String identifier);

        Boolean IsKnown(String identifier);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetDaemon.Hauler
{
    /// <summary>
    /// Public interface for the AppHauler
    /// Exposes known and runnig applications
    /// Methods to control lifecycle of applicatipons
    /// </summary>
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

/* 
 * Description: AppHaulerModel
 *              Data classes for AppHauler management
 * Project: Fleet/FleetDaemon
 * Last modified: 11 October 2016
 * Last Author: Hayden Cheers
 * 
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetDaemon.Hauler
{
    /// <summary>
    /// Record of a running application
    /// </summary>
    public class FleetRunningApplication
    {
        public Process Process { get; set; }
        public String Identifier { get; set; }
        public String Name { get; set; }
    }

    /// <summary>
    /// Record of a known application
    /// </summary>
    public class FleetKnownApplication
    {
        public String Name { get; set; }
        public String Path { get; set; }
        public String Identifier { get; set; }
        public Boolean Visible { get; set; } = true;
    }
}

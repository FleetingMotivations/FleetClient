using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetDaemon.Hauler
{
    public class FleetRunningApplication
    {
        public Process Process { get; set; }
        public String Identifier { get; set; }
        public String Name { get; set; }
    }

    public class FleetKnownApplication
    {
        public String Name { get; set; }
        public String Path { get; set; }
        public String Identifier { get; set; }
        public Boolean Visible { get; set; } = true;
    }
}

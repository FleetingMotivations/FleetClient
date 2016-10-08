using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetServer;

namespace FleetDaemon
{
    interface IControlMessageManager
    {
        void HandleControlMessageUpdate(FleetClientToken token);
    }
}

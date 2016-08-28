﻿using FleetServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetDaemon.Heartbeat
{
    public interface IRemoteFileManager
    {
        void HandleFileAvailable(FleetClientToken token);
    }
}

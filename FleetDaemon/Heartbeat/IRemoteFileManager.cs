﻿using FleetServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetDaemon
{
    /// <summary>
    /// Public itnerface for the file manager object
    /// </summary>
    public interface IRemoteFileManager
    {
        /// <summary>
        /// Instruct the amnager to handle any files avaibale.
        /// </summary>
        /// <param name="token"></param>
        void HandleFileAvailable(FleetClientToken token);
    }
}

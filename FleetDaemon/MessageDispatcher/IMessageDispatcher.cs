using FleetIPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetDaemon.MessageDispatcher
{
    /// <summary>
    /// Interface for message dispatcher objects.
    /// Quite literally allows messages to be dispatched.
    /// </summary>
    public interface IMessageDispatcher
    {      
        void Dispatch(IPCMessage message);
    }
}

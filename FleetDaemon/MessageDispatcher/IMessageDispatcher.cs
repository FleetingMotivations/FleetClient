using FleetIPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetDaemon.MessageDispatcher
{
    public interface IMessageDispatcher
    {      
        void Dispatch(IPCMessage message);
    }
}

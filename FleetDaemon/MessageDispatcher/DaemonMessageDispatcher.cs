using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetIPC;

namespace FleetDaemon.MessageDispatcher
{
    class DaemonMessageDispatcher: IMessageDispatcher
    {
        // Shared instance
        private static IMessageDispatcher instance;
        public static IMessageDispatcher Instance
        {
            set { instance = value; }

            get
            {
                if (instance == null)
                {
                    instance = new DaemonMessageDispatcher();
                }
                return instance;
            }
        }

        public void Dispatch(IPCMessage message)
        {
            throw new NotImplementedException();
        }
    }
}

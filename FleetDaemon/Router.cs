using FleetDaemon.Storage.Interfaces;
using FleetIPC;
using FleetServer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FleetDaemon.MessageDispatcher;

namespace FleetDaemon
{
    public interface IRouter
    {
        void HandleMessage(IPCMessage message);
    }

    class Router:IRouter
    {
        private FleetClientToken ClientToken;
        private ISimpleStorage Storage;

        public Router(ISimpleStorage storage, FleetClientToken token)
        {
            this.ClientToken = token;
            this.Storage = storage;
        }

        public void HandleMessage(IPCMessage message)
        {
            try
            {
                switch (message.Target)
                {
                    case IPCMessage.MessageTarget.Remote:
                        RemoteMessageDispatcher.Instance.Dispatch(message);
                        break;

                    case IPCMessage.MessageTarget.Local:
                        LocalMessageDispatcher.Instance.Dispatch(message);
                        break;

                    case IPCMessage.MessageTarget.Daemon:
                        DaemonMessageDispatcher.Instance.Dispatch(message);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
 
        }

    }

}

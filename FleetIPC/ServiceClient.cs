using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace FleetIPC
{
    // Client for FleetDaemon
    public class FleetDaemonClient: ClientBase<IDaemonIPC>, IDaemonIPC
    {
        // 1. Create constructor (all you have to do is copy this for all clients and rename)
        public FleetDaemonClient(Binding binding, EndpointAddress address): base(binding, address) { }

        // 1. Implement the client method.
        //    Every client method will call the corresponding method on the Channel member.
        //    Code is automagically generated.
        public void Request(IPCMessage message)
        {
            this.Channel.Request(message);
        }
    }

    // Will need one for the ApplicationCient so the daemon can talk to it
}

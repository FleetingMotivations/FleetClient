using FleetServer;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace WorkstationSelectorIPC
{
    [ServiceContract]
    public interface IWorkstationSelectIPC
    {
        [OperationContract]
        List<FleetClientIdentifier> SelectWorkstations(List<FleetClientIdentifier> clients);
    }

    public class WorkstationSelectIPCClient : ClientBase<IWorkstationSelectIPC>, IWorkstationSelectIPC
    {
        public WorkstationSelectIPCClient(Binding binding, EndpointAddress address) : base(binding, address) { }

        public List<FleetClientIdentifier> SelectWorkstations(List<FleetClientIdentifier> clients)
        {
            return this.Channel.SelectWorkstations(clients);
        }
    }
}

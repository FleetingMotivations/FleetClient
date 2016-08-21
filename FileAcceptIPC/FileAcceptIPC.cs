using FleetServer;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;


namespace FileAcceptIPC
{
    [ServiceContract]
    public interface IFileAcceptIPC
    {
        [OperationContract]
        Boolean RequestAcceptFile(FleetFileIdentifier ident);
    }

    public class FileAcceptIPCClient: ClientBase<IFileAcceptIPC>, IFileAcceptIPC
    {
        public FileAcceptIPCClient(Binding binding, EndpointAddress address): base(binding, address) { }

        public Boolean RequestAcceptFile(FleetFileIdentifier ident)
        {
            return this.Channel.RequestAcceptFile(ident);
        }
    }
}

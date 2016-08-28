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

    public static class FileAcceptIPCUtils
    {
        public static FileAcceptIPCClient MakeClient()
        {
            var address = new EndpointAddress("net.pipe://localhost/fileaccept");
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            binding.MaxBufferPoolSize = Int64.MaxValue;
            binding.MaxBufferSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int64.MaxValue;

            var client = new FileAcceptIPCClient(binding, address);
            return client;
        }
    }
}

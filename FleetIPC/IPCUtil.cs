using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace FleetIPC
{
    public static class IPCUtil
    {
        public static FleetDaemonClient MakeDaemonClient()
        {
            var address = new EndpointAddress("net.pipe://localhost/fleetdaemon");
            //var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            binding.MaxBufferPoolSize = Int32.MaxValue;
            binding.MaxBufferSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int32.MaxValue;

            var client = new FleetDaemonClient(binding, address);
            return client;
        }

        public static ApplicationClient MakeApplicationClient(String pipename)
        {
            var address = new EndpointAddress("net.pipe://localhost/" + pipename);
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            binding.MaxBufferPoolSize = Int32.MaxValue;
            binding.MaxBufferSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int32.MaxValue;

            var client = new ApplicationClient(binding, address);
            return client;
        } 
        
        public static ServiceHost MakeApplicationService(String pipename)
        {
            var address = new Uri("net.pipe://localhost/" + pipename);
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            binding.MaxBufferPoolSize = Int32.MaxValue;
            binding.MaxBufferSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int32.MaxValue;

            var service = new ServiceHost(typeof(ApplicationService));
            service.AddServiceEndpoint(typeof(IApplicationIPC), binding, address);
            return service;
        }
    }
}

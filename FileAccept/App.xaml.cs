using FleetIPC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;

namespace FileAccept
{
    public partial class App : Application
    {
        /**
        private ServiceHost service;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Console.WriteLine("Starting up...");

            var address = new Uri("net.pipe://localhost/fileaccept");
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            this.service.Open();

            var cAddress = new EndpointAddress("net.pipe://localhost/fleetdaemon");
            var cBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            var daemon = new FleetDaemonClient(cBinding, cAddress);

            // Create a dummy message
            var message = new IPCMessage();
            message.ApplicaitonSenderID = "sendId";
            message.ApplicationRecipientID = "recipId";
            message.Content["response"] = "accept-or-reject";

            daemon.Request(message);
        }**/
    }
}

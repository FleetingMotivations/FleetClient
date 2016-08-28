using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using FleetIPC;

namespace FileInbox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceHost service;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Service Events
            ApplicationService.OnInform += ApplicationService_OnInform;
            ApplicationService.OnDeliver += ApplicationService_OnDeliver;
            
            // Create service
            this.service = IPCUtil.MakeApplicationService("fileinbox");
            this.service.Open();

            // Register with client
            var daemon = IPCUtil.MakeDaemonClient();

            // Register with Daemon
            // TODO(hd):

            daemon.Close();
        }

        private void ApplicationService_OnDeliver(IPCMessage message)
        {
            throw new NotImplementedException();
        }

        private void ApplicationService_OnInform(List<IPCMessage> message)
        {
            throw new NotImplementedException();
        }
    }
}

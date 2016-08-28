using FleetIPC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;

namespace FileShare
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceHost service;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Console.WriteLine("Starting Up");

            // Set the service events
            ApplicationService.OnInform += ApplicationService_OnInform;
            ApplicationService.OnDeliver += ApplicationService_OnDeliver;

            // Create and open the service
            this.service = IPCUtil.MakeApplicationService("fileshare");
            this.service.Open();

            // Communicate with the daemon. Obviously will not have to do this here (maybe registration though?)
            // 1. Create binding and address.
            //    Note that the address for a client is a different type
            //    Then create the client
            var daemon = IPCUtil.MakeDaemonClient();

            // Create a dummy message
            /*var message = new IPCMessage();
            message.ApplicaitonSenderID = "fileshare";
            message.ApplicationRecipientID = "friendface";
            message.Content["type"] = "Register Application";
            message.Content["register"] = "Yo bud I exist aight!?";
            */
            //daemon.Request(message);
        }

        private void ApplicationService_OnDeliver(IPCMessage message)
        {
            // Have received
        }

        private void ApplicationService_OnInform(List<IPCMessage> message)
        {
            // Do some even more fun stuff
        }
    }
}

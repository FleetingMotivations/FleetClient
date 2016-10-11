/* 
 * Description: FleetShelf application.
 *              This application is the entry point for the users and consistents of popout 'shelf' (similar to a horizontal dock)
 *              which contains each of the applications of the Fleet system.
 * Project: Fleet/FleetClient
 * Project Member: Jordan Collins, Hayden Cheers, Alistair Woodcock, Tristan Newmann
 * Last modified: 11 October 2016
 * Last Author: Hayden Cheers
 * 
*/

using FleetIPC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace FleetShelf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public const String ApplicationIdentifier = "fleetshelf";

        private ServiceHost service;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            
            // Event handlers
            ApplicationService.OnDeliver += ApplicationService_OnDeliver;
            ApplicationService.OnInform += ApplicationService_OnInform;

            // Create Service
            this.service = IPCUtil.MakeApplicationService(ApplicationIdentifier);
            this.service.Open();

            // Request the applications (potentially shift this to a background threat in future)
            Task.Run(() => RequestApplications());
        }

        private void RequestApplications()
        {
            // 1. Request the known application from the daemon
            var message = new IPCMessage();
            message.ApplicaitonSenderID = ApplicationIdentifier;
            message.ApplicationRecipientID = "daemon";
            message.Target = IPCMessage.MessageTarget.Daemon;
            message.Type = "knownApplications";

            var client = IPCUtil.MakeDaemonClient();

            try
            {
                client.Open();
                client.Request(message);
                client.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                client.Abort();
            }
        }

        private void ApplicationService_OnInform(List<IPCMessage> messages)
        {
            /*
             * 2. Here we will receive a list of IPC messages that tell where 
             * all the known applications are/what they are etc. Each message
             * will contain:
             * * Name
             * * Identifier (for pipe)
             * * Path (to the exe)
             * * If it is or is not visible to the user (I might scrap that actually)
             * Then do something with it
             */

            var applications = new List<FleetShelfApplication>();

            foreach (var message in messages)
            {
                var name = message.Content["name"];
                var identifier = message.Content["identifier"];
                var path = message.Content["path"];

                var app = new FleetShelfApplication
                {
                    Name = name,
                    Identifier = identifier,
                    Path = path
                };

                applications.Add(app);
            }

            var window = App.Current.MainWindow as MainWindow;
            window.UpdateApplications(applications);
        }

        private void ApplicationService_OnDeliver(IPCMessage message)
        {
            // Possibly messages informing if and application was opened or closed?
        }
    }

    class FleetShelfApplication
    {
        public String Name { get; set; }
        public String Identifier { get; set; }
        public String Path { get; set; }
    }
}

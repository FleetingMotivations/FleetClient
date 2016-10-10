/* 
 * Description: FileInbox application.
 *              This application is the inbox containing the received files.
 * Project: Fleet/FleetClient
 * Last modified: 11 October 2016
 * Last Author: Hayden Cheers
 * 
*/

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
            // TODO(hc):

            daemon.Close();
        }

        private void ApplicationService_OnDeliver(IPCMessage message)
        {
            var window = App.Current.MainWindow as MainWindow;

            var props = message.Content;
            var path = props["filepath"];

            window?.Storage.StoreFile(path, props);
            window?.RefreshFiles();
        }

        private void ApplicationService_OnInform(List<IPCMessage> message)
        {
            Console.WriteLine("ApplicationService_OnInform");
        }
    }
}

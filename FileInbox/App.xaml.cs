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
            
            // Create IPC service
            this.service = IPCUtil.MakeApplicationService("fileinbox");
            this.service.Open();
        }

        /// <summary>
        /// Event to handle a IPC message being delivered.
        /// </summary>
        /// <param name="message"></param>
        private void ApplicationService_OnDeliver(IPCMessage message)
        {
            // Get window
            var window = App.Current.MainWindow as MainWindow;

            // Get file properties and path
            var props = message.Content;
            var path = props["filepath"];

            // Store the file and update the list
            window?.Storage.StoreFile(path, props);
            window?.RefreshFiles();
        }
        
        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="message"></param>
        private void ApplicationService_OnInform(List<IPCMessage> message)
        {
            Console.WriteLine("ApplicationService_OnInform");
        }
    }
}

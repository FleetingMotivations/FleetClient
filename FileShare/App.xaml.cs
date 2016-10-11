/* 
 * Description: FileShare application.
 *              This application provides a file delivery service for the Fleet system.
 * Project: Fleet/FleetClient
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
        }

        private void ApplicationService_OnDeliver(IPCMessage message)
        {
            // Not used, this application does not receive files
        }

        private void ApplicationService_OnInform(List<IPCMessage> message)
        {
            // Not used, this application does not receive files
        }
    }
}

/* 
 * Description: FleetAccept application.
 *              This application is used as a notifcation for the receiving workstation, asking for a response whethere
 *              they wish to accept or decline the incoming file.
 * Project: Fleet/FleetClient
 * Last modified: 11 October 2016
 * Last Author: Jordan Collins
 * 
*/

using FileAcceptIPC;
using FleetIPC;
using FleetServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace FileAccept
{
    public partial class App : System.Windows.Application
    {
        private ServiceHost service;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // create and start IPC service
            var address = new Uri("net.pipe://localhost/fileaccept");
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            this.service = new ServiceHost(typeof(FileAcceptService));
            this.service.AddServiceEndpoint(typeof(IFileAcceptIPC), binding, address);
            this.service.Open();

            Console.WriteLine("Service started");
        }
    }

    public class FileAcceptService : IFileAcceptIPC
    {
        public Boolean RequestAcceptFile(FleetFileIdentifier ident)
        {
            // Create winodw. show dialog and return the accept flag
            var window = new MainWindow();
            window.ShowRequestDialog(ident);
            return window.DidAccept;
        }
    }
}

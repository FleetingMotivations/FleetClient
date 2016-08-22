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
            var trayIcon = new NotifyIcon();
            trayIcon.Visible = true;
            trayIcon.Icon = new System.Drawing.Icon("../../jordan_the_tool.ico");

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
            // Create new client wait for close and return
            return true;
        }
    }
}

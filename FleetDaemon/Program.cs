using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetIPC;
using System.ServiceModel;
using System.Diagnostics;
using System.IO;

namespace FleetDaemon
{
    class Program
    {
        static void Main(string[] args)
        {
            var daemon = new Daemon();
            daemon.Run();
            Console.ReadLine();
        }
    }

    class Daemon
    {
        private ServiceHost service;

        public Daemon()
        {
            DaemonService.OnRequest += DaemonService_OnRequest;
        }

        private void DaemonService_OnRequest(IPCMessage message)
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            var address = new Uri("net.pipe://localhost/fleetdaemon");
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);

            this.service = new ServiceHost(typeof(DaemonService));
            this.service.AddServiceEndpoint(typeof(IDaemonIPC), binding, address);
            this.service.Open();
            Console.WriteLine("Daemon running. Press the any key to exit.");
            Console.WriteLine(Directory.GetCurrentDirectory());

            Process.Start(@"..\..\..\FileShare\bin\Debug\FileShare.exe");
            
        }
    }
}

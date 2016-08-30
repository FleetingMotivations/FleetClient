using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetIPC;
using System.ServiceModel;
using System.Diagnostics;
using System.IO;
using FleetServer;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using WorkstationSelectorIPC;
using FleetDaemon.Storage;
using FleetDaemon.Storage.Interfaces;
using System.Threading;

namespace FleetDaemon
{
    class Program
    {
        static void Main(string[] args)
        {
            var storage = new SimpleStorage("./filestore.json");
            var serverResourceName = "BasicHttpBinding_IFleetService";

            var handshakeWaitTime = storage.Get<int>("ServerHandshakeWaitTime");

            if (handshakeWaitTime == 0)
            {
                handshakeWaitTime = 5000;
            }

            // Create registration token
            var clientReg = new FleetClientRegistration();
            clientReg.FriendlyName = System.Environment.MachineName;

            // Register with server
            var client = new FleetServiceClient(serverResourceName);

            FleetClientToken clientToken = null;

            while(true)
            {
                try
                {
                    client.Open();
                    clientToken = client.RegisterClient(clientReg);
                    client.Close();
                    break;
                }
                catch (Exception e)
                {
                    client.Abort();
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Couldn't connect to server. Waiting...");
                }

                Thread.Sleep(handshakeWaitTime);
            }

            Console.WriteLine("Received registration token");
            
            // Dependancy Injection
            var appHauler = new AppHauler(storage);
            var router = new Router(appHauler, storage, clientToken);

            var daemon = new Daemon(storage, router, clientToken);
            daemon.Run();
            Console.ReadLine();
        }
    }

    class Daemon
    {
        // Static instance handling
        private ServiceHost Service;
        private ISimpleStorage Storage { get; set; }
        private IRouter Router { get; set; }

        private FleetClientToken ClientToken { get; set; }
        

        public Daemon(ISimpleStorage Store, IRouter router, FleetClientToken token)
        {
            this.Storage = Store;
            this.Router = router;
            this.ClientToken = token;
            DaemonService.OnRequest += DaemonService_OnRequest;
        }

        private void DaemonService_OnRequest(IPCMessage message)
        {
            Console.WriteLine(String.Format("Received message from: {0}, to: {1}", message.ApplicaitonSenderID, message.ApplicationRecipientID));

            this.Router.HandleMessage(message);
        }

        public void HandleFileReceive(String filename)
        {
            var message = new IPCMessage {
                    
            };
            
            this.Router.HandleMessage(message);
        }

        public void Run()
        {
            // Service initialisation
            var address = new Uri("net.pipe://localhost/fleetdaemon");
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            this.Service = new ServiceHost(typeof(DaemonService));
            this.Service.AddServiceEndpoint(typeof(IDaemonIPC), binding, address);
            this.Service.Open();

            Console.WriteLine("Daemon IPC service listening");

            // Start heartbeat
            HearbeatManager.WaitLength = 2000;
            HearbeatManager.Instance.StartHeartbeat(this.ClientToken);

            Console.WriteLine("Heartbeat is running");

            // Other loading
            // ????

            // Daemon is running
            Console.WriteLine("Daemon running. Press the any key to exit.");
            Console.ReadLine();

            this.Service.Close();
        }
    }


    
}

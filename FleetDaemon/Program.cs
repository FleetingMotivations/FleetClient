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

            var handshakeWaitTime = storage.Get<int>("ServerHandshakeWaitTime") ?? 5000;

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
                    Console.WriteLine("Couldn't connect to server. Waiting...");
                }

                Thread.Sleep(handshakeWaitTime);
            }

            Console.WriteLine("Received registration token");
            
            // Dependancy Injection
            var appHauler = new AppHauler(storage);
            var router = new Router(appHauler, storage, clientToken);

            var daemon = Daemon.CreateInstance(storage, router);
            daemon.Run();
            Console.ReadLine();
        }
    }

    class Daemon
    {
        // Static instance handling
        private static Daemon _instance;

        private ServiceHost service;
        private ISimpleStorage Storage { get; set; }
        private IRouter Router { get; set; }

        private FleetClientToken ClientToken { get; set; }
        

        private Daemon()
        {
            DaemonService.OnRequest += DaemonService_OnRequest;
        }

        public static Daemon CreateInstance(ISimpleStorage Store, IRouter router)
        {
            return new Daemon {
                Storage = Store,
                Router = router
            };
        }

       

        private void DaemonService_OnRequest(IPCMessage message)
        {
            Console.WriteLine(String.Format("Received message from: {0}, to: {1}", message.ApplicaitonSenderID, message.ApplicationRecipientID));

            this.Router.HandleMessage(message);
        }

        public void HandleFileReceive(String filename)
        {
            // Console.WriteLine(String.Format("Received new file from: {0}, to {1}, filename: {2}"));
            var message = new IPCMessage {
                    
            };
            
            this.Router.HandleMessage(message);
            //TODO(AL): Calll Router to handle this and pass it to the appropriate client process
        }

        public void Run()
        {
            // Service initialisation
            var address = new Uri("net.pipe://localhost/fleetdaemon");
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            var service = new ServiceHost(typeof(DaemonService));
            service.AddServiceEndpoint(typeof(IDaemonIPC), binding, address);
            service.Open();

            Console.WriteLine("Daemon IPC service listening");

            // Start heartbeat
            HearbeatManager.WaitLength = 2000;
            HearbeatManager.Instance.StartHeartbeat(ClientToken);

            Console.WriteLine("Heartbeat is running");

            // Other loading
            // ????

            // Daemon is running
            Console.WriteLine("Daemon running. Press the any key to exit.");
            Console.ReadLine();
        }
    }


    
}

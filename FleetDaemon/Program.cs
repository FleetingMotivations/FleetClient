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
using FleetDaemon.MessageDispatcher;
using FleetDaemon.Hauler;
using System.Net;

namespace FleetDaemon
{
    class Program
    {
        static void Main(string[] args)
        {
            // Accept any ssl certificate (this is extremely bad, but allows self-signed SSL certificated to beused
            // Reccomended by Bradley Lindsley
            ServicePointManager.ServerCertificateValidationCallback += (server, certificate, chain, errors) =>
            {
                return true;
            };

            // Create storage manager
            var storage = new SimpleStorage("./filestore.json");

            // Name of the resource to create service connectio with
            var serverResourceName = "BasicHttpBinding_IFleetService";

            // Load config
            var handshakeWaitTime = storage.Get<int>("ServerHandshakeWaitTime");

            if (handshakeWaitTime == 0)
            {
                handshakeWaitTime = 5000;
            }

            // Create registration token
            var clientReg = new FleetClientRegistration();
            clientReg.FriendlyName = System.Environment.MachineName;
            clientReg.RoomIdentifier = "ES205";

            // Register with server
            var client = new FleetServiceClient(serverResourceName);
            FleetClientToken clientToken = null;

            // Try to register in n-second intervals
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
            RemoteMessageDispatcher.Token = clientToken;
            var router = new Router(storage, clientToken);

            // Create and run daemon
            var daemon = new Daemon(storage, router, clientToken);
            ControlMessageManager.DaemonInstance = daemon;
            RemoteFileManager.DaemonInstance = daemon;
            daemon.Run();
        }
    }
}

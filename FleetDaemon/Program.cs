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
        private SimpleStorage Storage;
        private IFleetService FleetServer; // This will need to be populated with the
                                           // actual client or whatever
        public Daemon()
        {
            DaemonService.OnRequest += DaemonService_OnRequest;
            this.FleetServer = new FleetServerStub();
            this.Storage = new SimpleStorage("./filestore.json");
        }

        private void DaemonService_OnRequest(IPCMessage message)
        {
            Console.WriteLine(String.Format("Received message from: {0}, to: {1}", message.ApplicaitonSenderID, message.ApplicationRecipientID));

            //Accept-reject goes here... Receiving a message from a process and handling it

            Console.WriteLine(String.Format("Message Type: {0}", message.Content["type"]));

            if(message.Content["type"] == "sendFile")
            {
                Console.WriteLine("We got a file.");
                Console.WriteLine(String.Format("File URL: {0}", message.Content["fileurl"]));
            }

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

            var clientReg = new FleetClientRegistration();
            clientReg.FriendlyName = System.Environment.MachineName;
            clientReg.IpAddress = "boop boop we gotta implement";
            clientReg.MacAddress = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                                    where nic.OperationalStatus == OperationalStatus.Up
                                    select nic.GetPhysicalAddress().ToString()
                                    ).FirstOrDefault(); ;

            //var clientToken = this.FleetServer.RegisterClient(clientReg);
            //this.Storage.store("token", clientToken);
            this.Storage.store("token", "test_token_cool");
            Console.WriteLine("Client registered to Server.");

            Process.Start(@"..\..\..\FileShare\bin\Debug\FileShare.exe");
            
        }
        
    }

    public class FleetServerStub : IFleetService
    {
        public FleetFile GetFile(FleetClientToken token, FleetFileIdentifier fileId)
        {
            throw new NotImplementedException();
        }

        public FleetMessage GetMessage(FleetClientToken token, FleetMessageIdentifier fileId)
        {
            throw new NotImplementedException();
        }

        public FleetHearbeatEnum Heartbeat(FleetClientToken token, FleetClientIdentifier[] knownClients)
        {
            throw new NotImplementedException();
        }

        public FleetFileIdentifier[] QueryFiles(FleetClientToken token)
        {
            throw new NotImplementedException();
        }

        public FleetMessageIdentifier[] QueryMessages(FleetClientToken token)
        {
            throw new NotImplementedException();
        }

        public FleetClientToken RegisterClient(FleetClientRegistration registrationModel)
        {
            throw new NotImplementedException();
        }

        public bool SendFileMultipleRecipient(FleetClientToken token, FleetClientIdentifier[] recipients, FleetFile file)
        {
            throw new NotImplementedException();
        }

        public bool SendFileSingleRecipient(FleetClientToken token, FleetClientIdentifier recipient, FleetFile file)
        {
            throw new NotImplementedException();
        }

        public bool SendMessageMultipleRecipient(FleetClientToken token, FleetClientIdentifier[] recipients, FleetMessage msg)
        {
            throw new NotImplementedException();
        }

        public bool SendMessageSingleRecipient(FleetClientToken token, FleetClientIdentifier recipient, FleetMessage msg)
        {
            throw new NotImplementedException();
        }
    }


    class SimpleStorage
    {
        public String filePath;
        public Dictionary<String, Object> storage;
        public SimpleStorage(String filePath)
        {
            this.filePath = filePath;

            if(File.Exists(filePath))
            {
                try
                {
                    using (StreamReader file = File.OpenText(filePath))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        this.storage = (Dictionary<String, Object>)serializer.Deserialize(file, typeof(Dictionary<String, Object>));
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    this.storage = new Dictionary<String, Object>();
                }
            }
            else
            {
                this.storage = new Dictionary<String, Object>();
            }
        }

        public Object get(String key)
        {
            return storage[key];
        }

        public bool store(Dictionary<String, Object> dict)
        {
            this.storage = new Dictionary<String, Object>(dict);
            return writeToFile();
        }

        public bool store(String key, Object value)
        {
            this.storage[key] = value;
            return writeToFile();
        }

        private bool writeToFile()
        {
            try
            {
               using (StreamWriter file = File.CreateText(this.filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, storage);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }
    }
}

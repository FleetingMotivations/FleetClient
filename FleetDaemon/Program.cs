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
        private Dictionary<String, Object> MessageStore;

        private IFleetService FleetServer; // This will need to be populated with the
                                           // actual client or whatever
        public Daemon()
        {
            DaemonService.OnRequest += DaemonService_OnRequest;
            this.FleetServer = new FleetServerStub();
            this.Storage = new SimpleStorage("./filestore.json");

            var processes = new Dictionary<String, String>();
            processes.Add("drag_drop", @"..\..\..\FileShare\bin\Debug\FileShare.exe");
            this.Storage.Store("process_list", processes);
            this.MessageStore = new Dictionary<String, Object>();
        }

        private void DaemonService_OnRequest(IPCMessage message)
        {
            Console.WriteLine(String.Format("Received message from: {0}, to: {1}", message.ApplicaitonSenderID, message.ApplicationRecipientID));
            Console.WriteLine(String.Format("Message Type: {0}", message.Content["type"]));

            switch (message.LocationHandle)
            {
                case IPCMessage.MessageLocationHandle.REMOTE:
                    //TODO(AL+JORDAN): Check if communication is granted access
                    // Que process send request so that only one workstation_selector runs at a time

                    var workstationSelector = RunProcess("workstation_selector");

                    //Selection process is running, setup wait for send
                    // Okay so now that's showin lets setup to wait for a reply


                    if (message.Type.Equals("sendFile"))
                    {
                        Console.WriteLine("We got a file.");
                        Console.WriteLine(String.Format("File URL: {0}", message.Content["fileurl"]));
                    }
                    /* Example: 
                    else if (message.Type.Equals("quiz"))
                    {

                    }
                    */
                    break;

                case IPCMessage.MessageLocationHandle.LOCAL:
                    //TODO(AL+JORDAN): Check if the process name given exists
                    //                 Check if communication is granted access
                    //                 Check if it is running
                    //                 
                    break;

                case IPCMessage.MessageLocationHandle.DAEMON:
                    //NOTE(AL+JORDAN): These are all the messages directed for the daemon to handle
                    
                    //TODO(AL+JORDAN): Worstations selected
                    //                 Accept or rejection of file
                    //                 
                    // workstationShareList, fileAccepted, 

                    if(message.Type.Equals("workstationShareList"))
                    {
                        //TODO(AL+JORDAN): 
                    }
                    else if(message.Type.Equals("fileAccepted"))
                    {
                        // message.Content["accepted"] = true | false;
                        // message.Content["some other such stuff"]
                        //TODO(AL+JORDAN): Download the file and store it somewhere
                        //                 Tell the server we downloaded it 
                        //                 and whatever else needs doing
                        //                 finally open the file in the default thing
                    }

                break;
                default:
                    Console.WriteLine("SADFACE please figure out what to do here");
                break;
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
            this.Storage.Store("token", "test_token_cool");
            Console.WriteLine("Client registered to Server.");

            var dragDrop = RunProcess("drag_drop");
            Console.WriteLine(dragDrop.Id);
            //Process.Start(@"..\..\..\FileShare\bin\Debug\FileShare.exe");
            
        }

        private Process RunProcess(String processName)
        {
            var processes = Storage.Get<Dictionary<String, String>>("process_list");
            Console.WriteLine(processes);
            if(processes.ContainsKey(processName))
            {
                var p = Process.Start(processes[processName]);
                return p;
            }
            return null;            
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

        public T Get<T>(String key)
        {
            return (T)storage[key];
        }

        public bool Store(Dictionary<String, Object> dict)
        {
            this.storage = new Dictionary<String, Object>(dict);
            return WriteToFile();
        }

        public bool Store(String key, Object value)
        {
            this.storage[key] = value;
            return WriteToFile();
        }

        private bool WriteToFile()
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

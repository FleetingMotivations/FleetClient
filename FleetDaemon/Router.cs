using FleetDaemon.Storage.Interfaces;
using FleetIPC;
using FleetServer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FleetDaemon
{
    public interface IRouter
    {
        void HandleMessage(IPCMessage message);
    }

    class Router:IRouter
    {
        private FleetClientToken ClientToken;
        private ISimpleStorage Storage;
        private IAppHauler AppHauler;
        private List<ApplicationBinding> AppBindings;

        private IPCMessage FileShareMessage;

        private struct ApplicationBinding {
            public FleetApplication App;
            public ApplicationClient Client;
            public bool Registered;
        } 

        public Router(IAppHauler appHauler, ISimpleStorage storage, FleetClientToken token)
        {
            this.AppHauler = appHauler;
            this.ClientToken = token;
            this.Storage = storage;
            this.AppBindings = new List<ApplicationBinding>();
        }

        public void HandleMessage(IPCMessage message)
        {
            try
            {
                switch (message.Target)
                {
                    case IPCMessage.MessageTarget.Remote:
                        HandleRemoteMessage(message);
                        break;
                    case IPCMessage.MessageTarget.Local:
                        HandleLocalMessage(message);
                        break;
                    case IPCMessage.MessageTarget.Daemon:
                        HandleDaemonMessage(message);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
 
        }

        private void LaunchApplication(string applicationName)
        {
            var app = this.AppHauler.GetAppByName(applicationName);

            if(app == null)
            {
                throw new Exception($"Application '{applicationName}' does not exist.");
            }
            else
            {
                if(app.SingleInstance && app.Running)
                {
                    throw new Exception($"Applicaiton already running");
                }
                else
                {
                    this.AppHauler.LaunchApp(app);
                    CreateBinding(app);
                }
            }
        }

        private void CreateBinding(FleetApplication app)
        {
            var appBinding = new ApplicationBinding {
                App = app,
                Client = null,
                Registered = false
            };

            this.AppBindings.Add(appBinding);
        }

        private void CloseBinding(string id)
        {
            var binding = AppBindings.FirstOrDefault<ApplicationBinding>(b => b.App.Id == id);
            binding.Registered = false;
            AppBindings.Remove(binding);
        }


        private void HandleRemoteMessage(IPCMessage message)
        {
            if(message.SkipSelector)
            {
                //TODO(AL): Send message to server (we need to get the server connection)
                //var message = new FleetMessage();
                
                //Server.SendMessage(server_message);
                var Server = new FleetServiceClient("BasicHttpBinding_IFleetService");
                try
                {
                    Server.Open();
                    //Server.SendMessageSingleRecipient(this.ClientToken, , message)
                    Server.Close();
                }
                catch (Exception e)
                {
                    Server.Abort();
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                var app = AppBindings.FirstOrDefault<ApplicationBinding>(binding => binding.App.Name == "WorkstationSelector").App;

                if (app == null)
                {
                    LaunchApplication("WorkstationSelector");
                    this.FileShareMessage = message;
                }
                else
                {
                    //TODO(AL): Send message to application to wait
                    //          maybe add it to queue and tell it when it can call again or something?
                    //          maybe better to just queue the message and open WorkstationSelector later
                }
            }
        }

        private void HandleLocalMessage(IPCMessage message)
        {
            var binding = AppBindings.First<ApplicationBinding>(b => b.App.Id == message.ApplicationRecipientID);

            // TODO(AL): Allow for messages to launch an application if it's not running

            if (binding.App == null || binding.Client == null)
            {
                // Send message to message.ApplicationSenderID informing it that there exists no running
                // instance of the target application
            }
            else
            {
                if(binding.App.Running)
                {
                    binding.Client.Deliver(message);
                }
                else
                {
                    // the target application is not running (really this shouldn't occur since
                    // the bindings should only be for running applications but it's good to check)
                    // send message to message.ApplicationSenderID
                }
            }
        }

        private void HandleDaemonMessage(IPCMessage message)
        {
            if(message.Type == "Register")
            {
                var binding = AppBindings.First<ApplicationBinding>(b => b.App.Id == message.ApplicaitonSenderID);

                var proccessAddress = new EndpointAddress($"net.pipe://localhost/{binding.App.PipeName}");
                var proccessBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                
                binding.Registered = true;
                binding.Client = new ApplicationClient(proccessBinding, proccessAddress);

            }
            else if (message.Type == "SenderClose")
            {
                CloseBinding(message.ApplicaitonSenderID);
                this.AppHauler.CloseApp(message.ApplicaitonSenderID);
            }
            else if (message.Type == "WorkstationShareList")
            {
                var workstations = JsonConvert
                    .DeserializeObject<string[]>(message.Content["workstations"]);

                var filePath = (string)this.FileShareMessage.Content["filePath"];

                var fileContent = File.ReadAllBytes(filePath);
                string fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

                var file = new FleetFile
                {
                    FileContents = fileContent,
                    FileName = fileName
                };

                var selectedClients = workstations.Select(s => new FleetClientIdentifier
                {
                    Identifier = s,
                    WorkstationName = ""
                });

                var Server = new FleetServiceClient("BasicHttpBinding_IFleetService");
                try
                {
                    Server.Open();
                    Server.SendFileMultipleRecipient(this.ClientToken, selectedClients.ToArray(), file);
                    Server.Close();
                }
                catch(Exception e)
                {
                    Server.Abort();
                    Console.WriteLine(e.Message);
                }

            }
            else if (message.Type == "FileAccepted")
            {
                // message.Content["accepted"] = true | false;
                // message.Content["filePath"]
                //TODO(AL+JORDAN):  open the file in the default thing
            }
        }
    }

}

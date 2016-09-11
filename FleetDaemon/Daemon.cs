using FleetDaemon.Storage.Interfaces;
using FleetIPC;
using FleetServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FleetDaemon.Hauler;
using System.IO;

namespace FleetDaemon
{
    public class Daemon
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

            //this.Storage.Store("process_list", processes);
        }

        private void DaemonService_OnRequest(IPCMessage message)
        {
            Console.WriteLine(String.Format("Received message from: {0}, to: {1}", message.ApplicaitonSenderID, message.ApplicationRecipientID));

            this.Router.HandleMessage(message);
        }

        /// <summary>
        /// File handling interface called from the RemoteFileManager object
        /// Converts the passed path and attributes to an IPC message before
        /// dispatching to the recipient application
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="attributes"></param>
        public void HandleFileReceive(String filepath, Dictionary<String, String> attributes)
        {
            var message = new IPCMessage();
            message.ApplicaitonSenderID = "fileshare";  // TODO(hc): Change this to the actual sender
            message.ApplicationRecipientID = "fileinbox";
            message.Target = IPCMessage.MessageTarget.Local;
            message.Content["filepath"] = filepath;
            message.Type = "sendFile";

            foreach (var pair in attributes)
            {
                message.Content[pair.Key] = pair.Value;
            }

            this.Router.HandleMessage(message);
        }

        public void Run()
        {
            // Service initialisation
            StartService();

            // Start heartbeat
            StartHeartbeat();

            // App Hauler
            InitialiseAppHauler();

            // Platform Services
            StartPlatformServices();

            // Other loading
            // ????

            // Daemon is running
            Console.WriteLine("Daemon running. Press the any key to exit.");
            Console.ReadLine();
            this.Service.Close();
        }

        private void StartService()
        {
            var address = new Uri("net.pipe://localhost/fleetdaemon");
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            this.Service = new ServiceHost(typeof(DaemonService));
            this.Service.AddServiceEndpoint(typeof(IDaemonIPC), binding, address);
            this.Service.Open();

            Console.WriteLine("Daemon IPC service listening");
        }

        private void StartHeartbeat()
        {
            RemoteFileManager.DaemonInstance = this;
            HeartbeatManager.WaitLength = 3000;
            HeartbeatManager.Instance.StartHeartbeat(ClientToken);

            Console.WriteLine("Heartbeat is running");
        }

        private void StartPlatformServices()
        {
            Console.WriteLine(Directory.GetCurrentDirectory());

            // Start Workstation Selector
            AppHauler.Instance.LaunchApplication("workstationselector");

            // Start Accept Dialog
            AppHauler.Instance.LaunchApplication("fileaccept");

            // Start Dock
            //AppHauler.Instance.LaunchApplication("fleetshelf");
        }

        private void InitialiseAppHauler()
        {
            var apphauler = AppHauler.Instance;

            // For Testing
            apphauler.KnownApplications["fileshare"] = new FleetKnownApplication
            {
                Name = "File Share",
                Path = @"..\..\..\FileShare\bin\Debug\FileShare.exe",
                Identifier = "fileshare"
            };

            apphauler.KnownApplications["fileinbox"] = new FleetKnownApplication
            {
                Name = "File Inbox",
                Path = @"..\..\..\FileInbox\bin\Debug\FileInbox.exe",
                Identifier = "fileinbox"
            };

            apphauler.KnownApplications["fileaccept"] = new FleetKnownApplication
            {
                Name = "File Accept",
                Path = @"..\..\..\FileAccept\bin\Debug\FileAccept.exe",
                Identifier = "fileaccept",
                Visible = false
            };

            apphauler.KnownApplications["workstationselector"] = new FleetKnownApplication
            {
                Name = "Workstation Selector",
                Path = @"..\..\..\WorkstationSelector\bin\Debug\WorkstationSelector.exe",
                Identifier = "workstationselector",
                Visible = false
            };

            /*apphauler.KnownApplications["fleetshelf"] = new FleetKnownApplication
            {
                Name = "File Inbox",
                Path = @"..\..\..\FileShare\bin\Debug\FileInbox.exe",
                Identifier = "fileinbox",
                Visible = false
            };*/

        }
    }
}

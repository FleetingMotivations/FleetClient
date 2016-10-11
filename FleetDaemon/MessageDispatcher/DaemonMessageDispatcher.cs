using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetIPC;
using FleetDaemon.Hauler;
using FleetServer;

namespace FleetDaemon.MessageDispatcher
{
    class DaemonMessageDispatcher: IMessageDispatcher
    {
        // Shared instance
        private static IMessageDispatcher instance;
        public static IMessageDispatcher Instance
        {
            set { instance = value; }

            get
            {
                if (instance == null)
                {
                    instance = new DaemonMessageDispatcher();
                }
                return instance;
            }
        }

        /// <summary>
        /// Interface to displatch message. First validates then sends
        /// </summary>
        /// <param name="message"></param>
        public void Dispatch(IPCMessage message)
        {
            if (ValidateSender(message))
            {
                HandleMessage(message);

            } else
            {
                Console.WriteLine("Message Security Error. Application does not have privilege to send daemon messages");
            }
        }

        /// <summary>
        /// Ensure the sender is a correct application (ie is the the list of known applciations)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private Boolean ValidateSender(IPCMessage message)
        {
            // Check if application is registered to send daemon messages
            return true;
        }

        /// <summary>
        /// Handle sendign the message to the target application
        /// </summary>
        /// <param name="message"></param>
        private void HandleMessage(IPCMessage message)
        {
            switch (message.Type)
            {
                case "knownApplications":
                    HandleKnownApplicationsMessage(message);
                    break;
                case "launchApplication":
                    HandleLaunchApplicationMessage(message);
                    break;
                case "control":
                    HandleControlMessage(message);
                    break;
                default:
                    Console.WriteLine("Unhandled message type");
                    break;
            }
        }

        /// <summary>
        /// Handle diaptching a control message. 
        /// These are context change events
        /// </summary>
        /// <param name="message"></param>
        private void HandleControlMessage(IPCMessage message)
        {
            var content = message.Content["message"];

            switch (content)
            {
                case "FleetClientContext:Room":
                    DaemonContext.CurrentContext = FleetClientContext.Room;
                    break;
                case "FleetClientContext:Building":
                    DaemonContext.CurrentContext = FleetClientContext.Building;
                    break;
                case "FleetClientContext:Campus":
                    DaemonContext.CurrentContext = FleetClientContext.Campus;
                    break;
                case "FleetClientContext:Workgroup":
                    DaemonContext.CurrentContext = FleetClientContext.Workgroup;
                    break;
            }

            // TODO FileAccept and FileReject messages
            if (content.Contains("FileAccepted:"))
            {
                try
                {
                    var components = content.Split(':');

                   // DISPLAY NOTIFICATION HERE

                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            } else if (content.Contains("FileRejected:"))
            {
                try
                {
                    var components = content.Split(':');

                    // DISPLAY NOTIFICATION HERE

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Handle sending a list of known applications to the requesing application
        /// </summary>
        /// <param name="message"></param>
        private void HandleKnownApplicationsMessage(IPCMessage message)
        {
            // Get known apps
            var knownApps = AppHauler.Instance.KnownApplications.Values;
            var knownAppMessages = new List<IPCMessage>();

            // Convert to IPC messages
            foreach (var app in knownApps)
            {
                var m = new IPCMessage();
                m.Content["name"] = app.Name;
                m.Content["identifier"] = app.Identifier;
                m.Content["path"] = app.Path;

                knownAppMessages.Add(m);
            }

            // Create client
            var client = IPCUtil.MakeApplicationClient(message.ApplicaitonSenderID);

            try
            {
                // Snd to recipient
                client.Open();
                client.Inform(knownAppMessages);
                client.Close();

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                client.Abort();
            }
        }

        private void HandleLaunchApplicationMessage(IPCMessage message)
        {
            var appID = message.Content["application"];
            AppHauler.Instance.LaunchApplication(appID);
        }
    }
}

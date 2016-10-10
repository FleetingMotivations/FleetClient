using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetIPC;
using FleetDaemon.Hauler;
using FleetServer;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

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

        private Boolean ValidateSender(IPCMessage message)
        {
            // Check if application is registered to send daemon messages
            return true;
        }

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

                    var xml = "<toast>";
                    xml += "<visual>";
                    xml += "<binding template='ToastGeneric'>";
                    xml += "<text>File Accepted</text>";
                    xml += "<text>" + components[1] + " - " + components[2] + "</text>";
                    xml += "</binding>";
                    xml += "</visual>";
                    xml += "</toast>";

                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xml);

                    var notification = new ToastNotification(xmlDoc);
                    notification.Tag = "tag";
                    notification.Group = "FleetDaemon";

                    ToastNotificationManager.CreateToastNotifier("FleetDaemon").Show(notification);
                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            } else if (content.Contains("FileRejected:"))
            {
                try
                {
                    var components = content.Split(':');

                    var xml = "<toast>";
                        xml += "<visual>";
                            xml += "<binding template='ToastGeneric'>";
                                xml += "<text>File Rejected</text>";
                                xml += "<text>" + components[1] + " - " + components[2] + "</text>";
                            xml += "</binding>";
                        xml += "</visual>";
                    xml += "</toast>";

                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xml);

                    var notification = new ToastNotification(xmlDoc);
                    notification.Tag = "tag";
                    notification.Group = "FleetDaemon";

                    ToastNotificationManager.CreateToastNotifier("FleetDaemon").Show(notification);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void HandleKnownApplicationsMessage(IPCMessage message)
        {
            var knownApps = AppHauler.Instance.KnownApplications.Values;
            var knownAppMessages = new List<IPCMessage>();

            foreach (var app in knownApps)
            {
                var m = new IPCMessage();
                m.Content["name"] = app.Name;
                m.Content["identifier"] = app.Identifier;
                m.Content["path"] = app.Path;

                knownAppMessages.Add(m);
            }

            var client = IPCUtil.MakeApplicationClient(message.ApplicaitonSenderID);

            try
            {
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

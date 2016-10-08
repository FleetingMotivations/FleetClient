using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetIPC;
using FleetServer;
using System.ServiceModel;
using WorkstationSelectorIPC;
using System.IO;

namespace FleetDaemon.MessageDispatcher
{
    public class RemoteMessageDispatcher: IMessageDispatcher
    {
        // Data
        public static FleetClientToken Token { get; set; }

        // Shared instance
        private static IMessageDispatcher instance;
        public static IMessageDispatcher Instance
        {
            set { instance = value; }

            get {
                if (instance == null)
                {
                    instance = new RemoteMessageDispatcher();
                }
                return instance;
            }
        }

        public void Dispatch(IPCMessage message)
        {
            if (ValidateMessage(message))
            {
                HandleMessageDispatch(message);

            }
            else
            {
                Console.WriteLine("IPCMessage could not be validated");
                Console.WriteLine(message);
            }
        }

        private Boolean ValidateMessage(IPCMessage message)
        {
            // Check format
            // Check sending restrictions
            // etc
            return true;
        }

        private void HandleMessageDispatch(IPCMessage message)
        {
            switch (message.Type)
            {
                case "sendFile":
                    HandleSendFileDispatch(message);
                    break;
                default:
                    break;
            }
        }

        private void HandleSendFileDispatch(IPCMessage message)
        {

            if (message.SkipSelector)
            {
                // Use the recipient id
                var recipient = new FleetClientIdentifier();
                recipient.Identifier = message.ApplicationRecipientID;

                var recipients = new List<FleetClientIdentifier>();
                recipients.Add(recipient);

                SendFileMessage(message, recipients);

            } else
            {
                // Get clients
                var serviceClient = new FleetServiceClient("BasicHttpBinding_IFleetService");
                FleetClientIdentifier[] clients = null;

                try
                {
                    serviceClient.Open();
                    clients = serviceClient.QueryClients(Token, DaemonContext.CurrentContext, 1);
                    serviceClient.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Aborting HandleSendFileDispatch");

                    serviceClient.Abort();
                    return;
                }

                // Make selector client
                var address = new EndpointAddress("net.pipe://localhost/workstationselector");
                var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                var selectorClient = new WorkstationSelectIPCClient(binding, address);
                List<FleetClientIdentifier> targets = null;

                try
                {
                    // Get selection
                    selectorClient.Open();
                    targets = selectorClient.SelectWorkstations(new List<FleetClientIdentifier>(clients));
                    selectorClient.Close();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Aborting HandleSendFileDispatch");

                    selectorClient.Abort();
                    return;
                }

                // Send
                SendFileMessage(message, targets);
            }
        }

        private void SendFileMessage(IPCMessage message, List<FleetClientIdentifier> clients)
        {
            var serviceClient = new FleetServiceClient("BasicHttpBinding_IFleetService");

            // Serialise file
            var fPath = message.Content["filePath"];
            var fFile = new FleetFile();
            fFile.FileContents = File.ReadAllBytes(fPath);
            fFile.FileName = Path.GetFileName(fPath);

            try
            {
                serviceClient.Open();
                serviceClient.SendFileMultipleRecipient(Token, clients.ToArray(), fFile);
                serviceClient.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Aborting HandleSendFileDispatch");

                serviceClient.Abort();
                return;
            }
        }
    }
}

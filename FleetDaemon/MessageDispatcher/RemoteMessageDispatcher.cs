/* 
 * Description: RemoteMessageDispatcher
 *              Dispatches file messages to a remote workstation
 * Project: Fleet/FleetDaemon
 * Last modified: 11 October 2016
 * Last Author: Hayden Cheers
 * 
*/

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

        /// <summary>
        /// Validates the message then dispatches
        /// </summary>
        /// <param name="message"></param>
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

        /// <summary>
        /// Valudate hte message before sending
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private Boolean ValidateMessage(IPCMessage message)
        {
            //  todo
            // Check format
            // Check sending restrictions
            // etc
            return true;
        }

        /// <summary>
        /// Handles sending the message based on type
        /// </summary>
        /// <param name="message"></param>
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

        /// <summary>
        /// Handle for sending file messages
        /// Retreives clients from server, prompts user to select, then send to server
        /// </summary>
        /// <param name="message"></param>
        private void HandleSendFileDispatch(IPCMessage message)
        {

            if (message.SkipSelector)
            {
                // Just send to the designated recipient
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
                    // Get the context id. if zero, set to 1 (this is flag)
                    // Do not ask what this does.
                    var id = DaemonContext.CurrentWorkgroupId;
                    if (id == 0)
                    {
                        id = 1;
                    }

                    // debug
                    Console.WriteLine(id);
                    Console.WriteLine(DaemonContext.CurrentContext);

                    // get clients
                    serviceClient.Open();
                    clients = serviceClient.QueryClients(Token, DaemonContext.CurrentContext, id);
                    Console.WriteLine("X" + clients);
                    serviceClient.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Aborting HandleSendFileDispatch");

                    serviceClient.Abort();
                    return;
                }

                // Make workstation selector client
                var address = new EndpointAddress("net.pipe://localhost/workstationselector");
                var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                var selectorClient = new WorkstationSelectIPCClient(binding, address);
                List<FleetClientIdentifier> targets = null;

                try
                {
                    // Get selection (IPC call to selector, then await for the results)
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
            // Create client
            var serviceClient = new FleetServiceClient("BasicHttpBinding_IFleetService");

            // Serialise file
            var fPath = message.Content["filePath"];
            var fFile = new FleetFile();
            fFile.FileContents = File.ReadAllBytes(fPath);
            fFile.FileName = Path.GetFileName(fPath);

            try
            {
                // Send
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

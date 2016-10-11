using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetIPC;
using System.Diagnostics;
using FleetDaemon.Hauler;

namespace FleetDaemon.MessageDispatcher
{
    public class LocalMessageDispatcher : IMessageDispatcher
    {
        // Shared instance
        private static IMessageDispatcher instance;
        public static IMessageDispatcher Instance {
            set
            {
                instance = value;
            }

            get
            {
                if (instance == null) {
                    instance = new LocalMessageDispatcher();
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
            if (ValidateMessage(message))
            {
                HandleMessageDispatch(message);

            } else
            {
                Console.WriteLine("IPCMessage could not be validated");
                Console.WriteLine(message);

                //  TODO(hc): Notify somebody that it couldn't be sent.
            }
        }

        /// <summary>
        /// Validat the content of an IPC message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private Boolean ValidateMessage(IPCMessage message)
        {
            Boolean valid = false;

            var recipient = message.ApplicationRecipientID;

            // Check that it is targeted to a valid application
            if (AppHauler.Instance.KnownApplications.ContainsKey(recipient))
            {
                valid = AppHauler.Instance.IsRunningOrLaunch(recipient);
            }

            // TODO(hc): Check that sender is a valid process

            // TODO(hc): Check for message integrity

            //  TODO(hc): Checl tja tjte application can be opened

            return valid;
        }

        /// <summary>
        /// Handle sendign the message to the target application
        /// </summary>
        /// <param name="message"></param>
        private void HandleMessageDispatch(IPCMessage message)
        {
            // Get client to process
            var pipeIdent = message.ApplicationRecipientID;
            var client = IPCUtil.MakeApplicationClient(pipeIdent);
            
            try
            {
                // Send
                client.Open();
                client.Deliver(message);
                client.Close();

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                client.Abort();
            }
        }
    }
}

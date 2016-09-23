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

        public static Dictionary<String, Process> Processes { get; set; }

        public void Dispatch(IPCMessage message)
        {
            if (ValidateMessage(message) || true)       // Override stopgap
            {
                HandleMessageDispatch(message);

            } else
            {
                Console.WriteLine("IPCMessage could not be validated");
                Console.WriteLine(message);
            }
        }

        private Boolean ValidateMessage(IPCMessage message)
        {
            Boolean valid = false;

            var recipient = message.ApplicationRecipientID;

            if (AppHauler.Instance.KnownApplications.ContainsKey(recipient))
            {
                valid = AppHauler.Instance.IsRunningOrLaunch(recipient);
            }

            // TODO(hc): Check that sender is a valid process

            // Check that recipeint process is running
            /*var recipient = Processes[message.ApplicationRecipientID];
            if (recipient != null && !recipient.HasExited)
            {
                valid = false;
            }*/

            // TODO(hc): Check for message integrity

            return valid;
        }

        private void HandleMessageDispatch(IPCMessage message)
        {

            var pipeIdent = message.ApplicationRecipientID;
            var client = IPCUtil.MakeApplicationClient(pipeIdent);
            
            try
            {
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

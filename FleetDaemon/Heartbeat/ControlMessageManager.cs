using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetServer;

namespace FleetDaemon
{
    class ControlMessageManager: IControlMessageManager
    {
        // Shared instance
        private static IControlMessageManager instance;
        public static IControlMessageManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ControlMessageManager();
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        // Refernece to daemon
        public static Daemon DaemonInstance { get; set; }

        // Synchronicity lock
        private Object @lock = new Object();

        public ControlMessageManager()
        {

        }

        /// <summary>
        /// Event to handle control messages
        /// Spawns a new task to handle in the background
        /// </summary>
        /// <param name="token"></param>
        public void HandleControlMessageUpdate(FleetClientToken token)
        {
            Task.Run(() => this.DoControlMessageAvailable(token));
        }

        /// <summary>
        /// Handles to retreive message identifiers of pending control messages
        /// and then spawn backgorund tasks to handle their execution
        /// </summary>
        /// <param name="token"></param>
        private void DoControlMessageAvailable(FleetClientToken token)
        {
            Console.WriteLine("DoControlMessageAvailable");
            lock(@lock)
            {
                // Get ids
                var identifiers = GetMessageIds(token);

                // Foreach create background task to jandle retreiving
                foreach (var identifier in identifiers)
                {
                    Task.Run(() => this.HandleRetreiveMessage(token, identifier));
                }
            }
        }

        /// <summary>
        /// Utility to retreive all ids from server
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private List<FleetMessageIdentifier> GetMessageIds(FleetClientToken token)
        {
            List<FleetMessageIdentifier> identifiers = null;
            var client = new FleetServiceClient("BasicHttpBinding_IFleetService");

            try
            {
                client.Open();

                // Get ids of messages waiting to be retreived
                var ids = client.QueryMessages(token);
                identifiers = new List<FleetMessageIdentifier>(ids);

                client.Close();
            } catch (Exception e)
            {
                client.Abort();
                identifiers = new List<FleetMessageIdentifier>();
                Console.WriteLine(e.Message);
            }

            return identifiers;
        }

        /// <summary>
        /// Handler for retreiving a single message from the server
        /// Passes message to the daemon for execution.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="identifier"></param>
        private void HandleRetreiveMessage(FleetClientToken token, FleetMessageIdentifier identifier)
        {
            var client = new FleetServiceClient("BasicHttpBinding_IFleetService");

            try
            {
                client.Open();

                //Get message and pass to the daemon
                var message = client.GetMessage(token, identifier);
                Task.Run(() => DaemonInstance.HandleControlMessageReceive(message));

                client.Close();
            } catch (Exception e)
            {
                client.Abort();
                Console.WriteLine(e.Message);
            }
        }
    }
}

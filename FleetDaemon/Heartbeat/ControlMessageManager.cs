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

        public static Daemon DaemonInstance { get; set; }

        private Object @lock = new Object();

        public ControlMessageManager()
        {

        }

        public void HandleControlMessageUpdate(FleetClientToken token)
        {
            Task.Run(() => this.DoControlMessageAvailable(token));
        }

        private void DoControlMessageAvailable(FleetClientToken token)
        {
            Console.WriteLine("DoControlMessageAvailable");
            lock(@lock)
            {
                var identifiers = GetMessageIds(token);

                foreach (var identifier in identifiers)
                {
                    Task.Run(() => this.HandleRetreiveMessage(token, identifier));
                }
            }
        }

        private List<FleetMessageIdentifier> GetMessageIds(FleetClientToken token)
        {
            List<FleetMessageIdentifier> identifiers = null;
            var client = new FleetServiceClient("BasicHttpBinding_IFleetService");

            try
            {
                client.Open();

                var ids = client.QueryMessages(token);
                identifiers = new List<FleetMessageIdentifier>();

                client.Close();
            } catch (Exception e)
            {
                client.Abort();
                identifiers = new List<FleetMessageIdentifier>();
                Console.WriteLine(e.Message);
            }

            return identifiers;
        }

        private void HandleRetreiveMessage(FleetClientToken token, FleetMessageIdentifier identifier)
        {
            var client = new FleetServiceClient("BasicHttpBinding_IFleetService");

            try
            {
                client.Open();

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

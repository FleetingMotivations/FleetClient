using FleetServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FleetDaemon
{
    public class HearbeatManager
    {
        // Configuration
        public static Int16 WaitLength { get; set; } = 5000;

        // Singleton instance
        private static HearbeatManager instance;
        public static HearbeatManager Instance { get
            {
                if (instance == null)
                {
                    instance = new HearbeatManager();
                }
                return instance;
            }
        }

        // Members
        private Boolean isRunning;
        private Thread worker;
        private FleetClientToken token;

        private HearbeatManager()
        {
            this.isRunning = false;
        }

        public Boolean StartHeartbeat(FleetClientToken token)
        {
            if (token == null)
            {
                throw new Exception("Hearbeat token cannot be null");
            }

            if (!this.isRunning)
            {
                this.token = token;
                this.worker = new Thread(new ThreadStart(RunLoop));
                this.worker.Start();
                Console.WriteLine("Heartbeat Thread Started");

                this.isRunning = true;

                return true;
            }

            return false;
        }

        public Boolean StopHeartbeat()
        {
            if (this.isRunning)
            {
                this.worker.Interrupt();
                this.worker = null;
                this.token = null;
                this.isRunning = false;

                return true;
            }

            return false;
        }

        private void RunLoop()
        {
            var client = new FleetServiceClient("BasicHttpBinding_IFleetService");
            client.Open();

            while (true)
            {
                var flags = client.Heartbeat(token);

                if (flags.HasFlag(FleetHearbeatEnum.ClientUpdate))
                {

                }

                if (flags.HasFlag(FleetHearbeatEnum.ControlUpdate))
                {

                }

                if (flags.HasFlag(FleetHearbeatEnum.ManageUpdate))
                {

                }

                if (flags.HasFlag(FleetHearbeatEnum.FileAvailable))
                {
                    RemoteFileManager.Instance.HandleFileAvailable(this.token);
                }

                Thread.Sleep(HearbeatManager.WaitLength);
            }
        }
    }
}

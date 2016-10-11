using FleetServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FleetDaemon
{
    public class HeartbeatManager
    {
        // Configuration
        public static Int16 WaitLength { get; set; } = 5000;

        // Singleton instance
        private static HeartbeatManager instance;
        public static HeartbeatManager Instance { get {
                if (instance == null)
                {
                    instance = new HeartbeatManager();
                }
                return instance;
            }
        }

        // Members
        private Thread worker;

        // registration token
        public FleetClientToken Token { get; set; }

        // Flag for running
        private Boolean isRunning;
        public Boolean IsRunning { get {
                return isRunning;
            }
        }

        private HeartbeatManager()
        {
            this.isRunning = false;
        }

        /// <summary>
        /// Starts the Heartbeat task
        /// Returns true if thread dispatched successfully, otherwise false
        /// </summary>
        /// <param name="token">Workstation token</param>
        /// <returns></returns>
        public Boolean StartHeartbeat()
        {
            // Requires a token
            if (this.Token == null)
            {
                throw new Exception("Hearbeat token cannot be null");
            }

            // Ensure not running then start on background thread
            if (!this.isRunning)
            {
                this.worker = new Thread(new ThreadStart(RunLoop));
                this.worker.Start();
                Console.WriteLine("Heartbeat Thread Started");

                this.isRunning = true;
                return true;
            }

            Console.WriteLine("Cannot start Heartbeat. Heartbeat is already running.");
            return false;
        }

        /// <summary>
        /// Stops the Heartbeat task if running
        /// Returns true if successfully stopped.
        /// </summary>
        /// <returns></returns>
        public Boolean StopHeartbeat()
        {
            if (this.isRunning)
            {
                this.worker.Interrupt();
                this.worker = null;
                this.isRunning = false;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Run loop for Heartbeat worker thread.
        /// Every n seconds sends the heartbeat to the server, 
        /// and then dispatches any jobs based on the returned flags
        /// </summary>
        private void RunLoop()
        {
            var client = new FleetServiceClient("BasicHttpBinding_IFleetService");
            client.Open();

            while (true)
            {
                try
                {
                    Console.WriteLine("Heartbeat");
                    var flags = client.Heartbeat(this.Token);
                    
                    // Handle control satus changes
                    Task.Run(() => HandleControlStatusChange(this.Token));

                    // Handle control message updates
                    if (flags.HasFlag(FleetHearbeatEnum.ManageUpdate))
                    {
                        Console.WriteLine("MessageAvailable");
                        ControlMessageManager.Instance.HandleControlMessageUpdate(this.Token);
                    }

                    // Handle new file messages
                    if (flags.HasFlag(FleetHearbeatEnum.FileAvailable))
                    {
                        Console.WriteLine("FileAvailable");
                        RemoteFileManager.Instance.HandleFileAvailable(this.Token);
                    }

                    // Wait between polls
                    Thread.Sleep(HeartbeatManager.WaitLength);

                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            client.Close();
        }

        /// <summary>
        /// Handle any mnessages for change in control status
        /// </summary>
        /// <param name="token"></param>
        private void HandleControlStatusChange(FleetClientToken token)
        {
            // Make client
            var client = new FleetServiceClient("BasicHttpBinding_IFleetService");
            FleetControlStatus status;

            try
            {
                // Get status
                client.Open();
                status = client.QueryControlStatus(token);
                client.Close();
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            // If not status, reset to default
            if (status == null)
            {
                DaemonContext.CanShare = true;
                DaemonContext.CurrentContext = FleetClientContext.Room;
                DaemonContext.CurrentWorkgroupId = 0;
                return;
            }

            // Otherwise set context changes.
            DaemonContext.CanShare = status.CanShare;
            DaemonContext.CurrentWorkgroupId = status.WorkgroupId;
            DaemonContext.CurrentContext = FleetClientContext.Workgroup;

        }
    }
}

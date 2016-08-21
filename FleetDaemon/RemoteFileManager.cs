using FleetServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FleetDaemon
{
    public class RemoteFileManager
    {
        // Shared Instance
        private static RemoteFileManager instance;
        public static RemoteFileManager Instance { get {
                if (instance == null)
                {
                    instance = new RemoteFileManager();
                }
                return instance;
            }
        }

        private Object @lock = new Object();

        private RemoteFileManager()
        {

        }

        public void HandleFileAvailable(FleetClientToken token)
        {
            Task.Run(() => this.HandleFileAvailable(token));
        }

        private void DoFileAvailable(FleetClientToken token)
        {
            lock (@lock)
            {
                var fileIds = GetFileIds(token);

                foreach (var ids in fileIds)
                {
                    // Request download
                    // If so, do in background
                }
            }
        }

        private List<FleetFileIdentifier> GetFileIds(FleetClientToken token)
        {
            var client = new FleetServiceClient("BasicHttpBinding_IFleetService");
            client.Open();

            var ids = client.QueryFiles(token);
            return new List<FleetFileIdentifier>(ids);
        }
    }
}

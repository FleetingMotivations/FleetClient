using FileAcceptIPC;
using FleetServer;
using System;
using System.IO;
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

        public void HandleFileAvailable(FleetClientToken token)
        {
            Task.Run(() => this.HandleFileAvailable(token));
        }

        private void DoFileAvailable(FleetClientToken token)
        {
            lock (@lock)
            {
                var fileIds = GetFileIds(token);

                foreach (var id in fileIds)
                {
                    var address = new EndpointAddress("net.pipe://localhost/fileaccept");
                    var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                    var client = new FileAcceptIPCClient(binding, address);

                    var accepted = client.RequestAcceptFile(id);
                    if (accepted)
                    {
                        Task.Run(() => this.RetreiveFile(token, id));
                    }
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

        private void RetreiveFile(FleetClientToken token, FleetFileIdentifier id)
        {
            var client = new FleetServiceClient("BasicHttpBinding_IFleetService");
            client.Open();

            var file = client.GetFile(token, id);

            this.saveFile(file);
        }

        private void saveFile(FleetFile file)
        {
            var filename = Path.GetTempPath() + file.FileName;
            File.WriteAllBytes(filename, file.FileContents);

            Task.Run(() => Daemon.Instance.HandleFileReceive(filename));
        }
    }
}

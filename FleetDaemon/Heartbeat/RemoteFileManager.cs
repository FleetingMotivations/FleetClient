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
    public class RemoteFileManager: IRemoteFileManager
    {
        // Shared Instance
        private static IRemoteFileManager instance;
        public static IRemoteFileManager Instance {
            set
            {
                instance = value;
            }

            get
            {
                if (instance == null)
                {
                    instance = new RemoteFileManager();
                }
                return instance;
            }
        }

        public static Daemon DaemonInstance { get; set; }
        
        // Synchronicity lock
        private Object @lock = new Object();

        /// <summary>
        /// External interface for the RemoteFileManager object
        /// Runs an async task to handle any available files
        /// </summary>
        /// <param name="token">The workstations assigned access token</param>
        public void HandleFileAvailable(FleetClientToken token)
        {
            Task.Run(() => this.DoFileAvailable(token));
        }

        /// <summary>
        /// Handles retreiving of files and passes to daemon
        /// Requests file ids from server, requests approval from client,
        /// retrieves file from server, and saves to disk
        /// </summary>
        /// <param name="token">The workstations assigned access token</param>
        private void DoFileAvailable(FleetClientToken token)
        {
            Console.Write("DoFileAvailable");
            lock (@lock)
            {
                // Get ids
                var fileIds = GetFileIds(token);
                Console.WriteLine("Received " + fileIds.Count() + " file ids");

                // Request approval from client for each id and then handle retreiving
                foreach (var id in fileIds)
                {
                    Console.WriteLine("Requesting file: " + id.FileName);
                    
                    var client = FileAcceptIPCUtils.MakeClient();
                    var accepted = client.RequestAcceptFile(id);

                    Console.WriteLine("Client accepted file: " + accepted);

                    if (accepted)
                    {
                        // Spawn background task to retreive file
                        Task.Run(() => this.HandleRetreiveFile(token, id));
                    } else
                    {
                        Task.Run(() => this.NotifyRefusal(token, id));
                    }
                }
            }
        }

        private void NotifyRefusal(FleetClientToken token, FleetFileIdentifier id)
        {

        }

        /// <summary>
        /// Retreives a list of pending file identifiers awaiting receipt
        /// </summary>
        /// <param name="token">The workstations assigned access token</param>
        /// <returns>A List of identifiers for sent files pending receipt</returns>
        private List<FleetFileIdentifier> GetFileIds(FleetClientToken token)
        {
            var client = new FleetServiceClient("BasicHttpBinding_IFleetService");
            List<FleetFileIdentifier> ids = null;

            try
            {
                client.Open();
                var idents = client.QueryFiles(token);
                ids = new List<FleetFileIdentifier>(idents);
                client.Close();

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                client.Abort();
            }

            return ids;
        }

        /// <summary>
        /// Handles the retreiving of a file, then saves to disk
        /// </summary>
        /// <param name="token">The workstations assigned access token</param>
        /// <param name="id">Identifier of the file to be retreived</param>
        private void HandleRetreiveFile(FleetClientToken token, FleetFileIdentifier id)
        {
            var client = new FleetServiceClient("BasicHttpBinding_IFleetService");

            try
            {
                client.Open();
                var file = client.GetFile(token, id);
                client.Close();
                
                var attributes = new Dictionary<String, String>();
                attributes["filesize"] = "" + id.FileSize;
                attributes["sender"] = id.SenderName;

                this.SaveFileAndNotify(file, attributes);

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                client.Abort();
            }
        }

        /// <summary>
        /// Save the file to a temp directory and notify the daemon
        /// </summary>
        /// <param name="file">File retrieved from the server</param>
        private void SaveFileAndNotify(FleetFile file, Dictionary<String, String> attributes)
        {
            var filename = Path.GetTempPath() + file.FileName;
            File.WriteAllBytes(filename, file.FileContents);
            
            // Handle notification as background task
            Task.Run(() => DaemonInstance.HandleFileReceive(filename, attributes));
        }
    }
}

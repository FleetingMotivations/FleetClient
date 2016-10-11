/* 
 * Description: FileStore
 *              class for managing the storage of files for the FileInbox application
 *              Manages files in entire directory + handles moving files to the owned directory.
 * Project: Fleet/FileInbox
 * Last modified: 11 October 2016
 * Last Author: Hayden Cheers
 * 
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileInbox
{
    public delegate void FileStoreDidChangeEvent();

    public class FileStore
    {
        /// <summary>
        /// File change events
        /// </summary>
        public FileStoreDidChangeEvent OnChange;
        public FileStoreDidChangeEvent OnCreate;
        public FileStoreDidChangeEvent OnRename;
        public FileStoreDidChangeEvent OnDelete;

        // File tracking
        private String rootFolder;
        private List<StoredFile> files;
        public List<StoredFile> Files { get
            {
                return files;
            }
        }

        // File monitor
        private FileSystemWatcher folderWatcher;

        public FileStore() : this(FileStoreUtils.GetStorePath()) { }
        public FileStore(String rootFolder)
        {
            // Initialise the file store
            this.rootFolder = rootFolder;
            this.files = new List<StoredFile>();
            this.LoadStore();

            // Create & link the file watcher (for any changes in the directory
            var watcher = new FileSystemWatcher(rootFolder);
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastAccess;
            watcher.Filter = "*.*";
            watcher.Changed += Watcher_Changed;
            watcher.Created += Watcher_Created;
            watcher.Renamed += Watcher_Renamed;
            watcher.Deleted += Watcher_Deleted;
            watcher.EnableRaisingEvents = true;
            this.folderWatcher = watcher;
        }

        ~FileStore()
        {
            this.folderWatcher.Dispose();
        }

        //  File System Watch Events

            /// <summary>
            /// On change event ahdnler
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            //MakeRecord(e.FullPath);
            Console.WriteLine("File changed: " + e.FullPath);
            this.OnChange();
        }

        /// <summary>
        /// On delete event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            RemoveRecord(e.FullPath);
            this.OnDelete();
        }

        /// <summary>
        /// On rename event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            RemoveRecord(e.OldFullPath);
            MakeRecord(e.FullPath);
            this.OnRename();
        }

        /// <summary>
        /// On create event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            MakeRecord(e.FullPath);
            this.OnCreate();
        }

        /// <summary>
        /// Makes the record for the specified filename
        /// </summary>
        /// <param name="filename"></param>
        private void MakeRecord(String filename)
        {
            var record = new StoredFile();
            record.IconURL = "";
            record.Filepath = filename;
            record.Sender = "";
            record.Received = "";

            this.files.Add(record);
        }

        /// <summary>
        /// Remove the record for the specified file name
        /// </summary>
        /// <param name="filename"></param>
        private void RemoveRecord(String filename)
        {
            var index = this.files.FindIndex(file => file.Filepath == filename);
            this.files.RemoveAt(index);
        }

        /// <summary>
        /// Store the file in the sotrage directory
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="attributes">(unused)</param>
        public void StoreFile(String filepath, Dictionary<String, String> attributes)
        {
            var filename = this.MakeOwnedFilename(filepath);

            File.Move(filepath, filename);

            /*var record = new StoredFile();
            record.IconURL = "";
            record.Filepath = filename;
            record.Sender = "";
            record.Received = "";*/

            //this.files.Add(record);*/
        }

        /// <summary>
        /// Create the filename for the file once moved to the storage directory
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private String MakeOwnedFilename(String filepath)
        {
            // Get full path
            var newPath = Path.Combine(this.rootFolder, Path.GetFileName(filepath));

            // If filename alraedy taken, keep incrementing a suffix
            if (File.Exists(newPath))
            {
                var i = 1;
                var filename = Path.GetFileNameWithoutExtension(filepath);
                var fileextn = Path.GetExtension(filepath);

                while (File.Exists(newPath))
                {
                    var newName = filename + "-" + i + fileextn;
                    newPath = Path.Combine(this.rootFolder, newName);
                    i++;
                }
            }

            return newPath;
        }

        /// <summary>
        /// Load the files in the directory. Create directory if not exists
        /// </summary>
        private void LoadStore()
        {
            if (!Directory.Exists(this.rootFolder))
            {
                Directory.CreateDirectory(this.rootFolder);
            }

            // Get contents of storage
            var directoryContents = Directory.GetFiles(this.rootFolder);
            var fileStorage = new List<StoredFile>();

            // Create recirds
            foreach (var file in directoryContents)
            {
                var record = new StoredFile();
                record.IconURL = "";
                record.Filepath = file;
                record.Sender = "";
                record.Received = "";

                fileStorage.Add(record);
            }

            // Update list
            this.files = fileStorage;
        }
    }

    internal static class FileStoreUtils
    {
        /// <summary>
        /// Utility to find oath of the store
        /// </summary>
        /// <returns></returns>
        public static String GetStorePath()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = Path.Combine(path, "FileInbox");
            return path;
        }
    }

    /// <summary>
    /// Utility for each record displayed int eh list
    /// </summary>
    public class StoredFile
    {
        public String IconURL { get; set; }
        public String Filepath { get; set; }
        public String Sender { get; set; }
        public String Received { get; set; }

        public String Filename { get {
                return Path.GetFileName(this.Filepath);
            }
        }
    }
}

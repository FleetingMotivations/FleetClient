using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileInbox
{
    public class FileStore
    {
        private String rootFolder;
        private List<StoredFile> files;
        public List<StoredFile> Files { get
            {
                return files;
            }
        }

        public FileStore() : this(FileStoreUtils.GetStorePath()) { }
        public FileStore(String rootFolder)
        {
            this.rootFolder = rootFolder;
            this.files = new List<StoredFile>();
            this.LoadStore();
        }

        public void StoreFile(String filepath, Dictionary<String, String> attributes)
        {
            var filename = this.MakeOwnedFilename(filepath);

            File.Move(filepath, filename);

            var record = new StoredFile();
            record.IconURL = "";
            record.Filepath = filename;
            record.Sender = "";
            record.Received = "";

            this.files.Add(record);
        }

        private String MakeOwnedFilename(String filepath)
        {
            var newPath = Path.Combine(this.rootFolder, Path.GetFileName(filepath));

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

        private void LoadStore()
        {
            if (!Directory.Exists(this.rootFolder))
            {
                Directory.CreateDirectory(this.rootFolder);
            }

            var directoryContents = Directory.GetFiles(this.rootFolder);
            var fileStorage = new List<StoredFile>();

            foreach (var file in directoryContents)
            {
                var record = new StoredFile();
                record.IconURL = "";
                record.Filepath = file;
                record.Sender = "";
                record.Received = "";

                fileStorage.Add(record);
            }

            this.files = fileStorage;
        }
    }

    internal static class FileStoreUtils
    {
        public static String GetStorePath()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = Path.Combine(path, "FileInbox");
            return path;
        }
    }

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

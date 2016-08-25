using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FleetDaemon.Utils
{
    public class SimpleStorage
    {
        public String filePath;
        public Dictionary<String, Object> storage;
        public SimpleStorage(String filePath)
        {
            this.filePath = filePath;

            if(File.Exists(filePath))
            {
                try
                {
                    using (StreamReader file = File.OpenText(filePath))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        this.storage = (Dictionary<String, Object>)serializer.Deserialize(file, typeof(Dictionary<String, Object>));
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    this.storage = new Dictionary<String, Object>();
                }
            }
            else
            {
                this.storage = new Dictionary<String, Object>();
            }
        }

        public Object Get(String key)
        {
            return storage[key];
        }

        public bool Store(Dictionary<String, Object> dict)
        {
            this.storage = new Dictionary<String, Object>(dict);
            return WriteToFile();
        }

        public bool Store(String key, Object value)
        {
            this.storage[key] = value;
            return WriteToFile();
        }

        private bool WriteToFile()
        {
            try
            {
               using (StreamWriter file = File.CreateText(this.filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, storage);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }
    }
}

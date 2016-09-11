using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetDaemon.Storage.Interfaces;
using Newtonsoft.Json;

namespace FleetDaemon.Storage
{
    public class SimpleStorage : ISimpleStorage
    {
        public string filePath;
        public Dictionary<string, object> storage;
        public SimpleStorage(string filePath)
        {
            this.filePath = filePath;

            if (File.Exists(filePath))
            {
                try
                {
                    using (StreamReader file = File.OpenText(filePath))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        this.storage = (Dictionary<string, object>)serializer.Deserialize(file, typeof(Dictionary<string, object>));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    this.storage = new Dictionary<string, object>();
                }
            }
            else
            {
                this.storage = new Dictionary<string, object>();
            }
        }

        public T Get<T>(string key)
        {
            object val;
            storage.TryGetValue(key, out val);
            if (val == null) return default(T);
            return (T)val;
        }

        public bool Store(Dictionary<string, object> dict)
        {
            this.storage = new Dictionary<string, object>(dict);
            return WriteToFile();
        }

        public bool Store(string key, object value)
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

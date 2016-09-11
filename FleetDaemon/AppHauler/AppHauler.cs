using FleetDaemon.Storage;
using FleetDaemon.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FleetDaemon.Hauler
{
    public class FleetRunningApplication
    {
        public Process Process { get; set; }
        public String Identifier { get; set; }
        public String Name { get; set; }
    }

    public class FleetKnownApplication
    {
        public String Name { get; set; }
        public String Path { get; set; }
        public String Identifier { get; set; }
        public Boolean Visible { get; set; } = true;
    }

    public interface IAppHauler
    {
        Dictionary<String, FleetRunningApplication> RunningApplications { get; }
        Dictionary<String, FleetKnownApplication> KnownApplications { get; }

        Boolean LaunchApplication(String identifier);
        Boolean CloseApplication(String identifier);
        Boolean IsRunning(String identifier);
        Boolean IsRunningOrLaunch(String identifier);

        Boolean IsKnown(String identifier);
    }

    public class AppHauler: IAppHauler
    {
        // Configuration
        public static String StoragePath { get; set; } = "./apphauler.json";

        // Shared Instance
        private static IAppHauler instance;
        public static IAppHauler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AppHauler();
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        // Storage
        private ISimpleStorage storage;

        // Accessible Members
        private Dictionary<String, FleetRunningApplication> runningApplications;
        public Dictionary<String, FleetRunningApplication> RunningApplications {
            get {
                return runningApplications;
            }
        }

        private Dictionary<String, FleetKnownApplication> knownApplications;
        public Dictionary<String, FleetKnownApplication> KnownApplications
        {
            get
            {
                return knownApplications;
            }
        }

        // Construction
        public AppHauler()
        {
            this.storage = new SimpleStorage(StoragePath);

            this.runningApplications = new Dictionary<String, FleetRunningApplication>();
            this.knownApplications = this.storage.Get<Dictionary<String, FleetKnownApplication>>("AppHauler_KnownApplications") ?? new Dictionary<string, FleetKnownApplication>();
        }

        // Methods
        public bool LaunchApplication(string identifier)
        {
            try
            {
                var record = this.knownApplications[identifier];
                
                if (!this.runningApplications.ContainsKey(identifier))
                {
                    var exe = record.Path;

                    var process = Process.Start(exe);
                    process.Exited += Process_Exited;

                    var runningRecord = new FleetRunningApplication();
                    runningRecord.Identifier = identifier;
                    runningRecord.Name = record.Name;
                    runningRecord.Process = process;

                    // Wait upon startup to ensure the service has started
                    Thread.Sleep(1000);

                    return true;
                }

                return false;
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool CloseApplication(string identifier)
        {
            try
            {
                var record = this.knownApplications[identifier];

                if (this.runningApplications.ContainsKey(identifier))
                {
                    var app = this.runningApplications[identifier];

                    app.Process.Kill();
                    this.runningApplications.Remove(identifier);
                }

                return false;
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool IsRunning(string identifier)
        {
            try
            {
                return !this.runningApplications[identifier].Process.HasExited;

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool IsRunningOrLaunch(string identifier)
        {
            if (!this.IsRunning(identifier))
            {
                return LaunchApplication(identifier);
            }
            return true;
        }

        public bool IsKnown(string identifier)
        {
            return this.knownApplications.ContainsKey(identifier);
        }

        // Event Handlers
        private void Process_Exited(object sender, EventArgs e)
        {
            Console.WriteLine("Process has exited");
        }
    }
}

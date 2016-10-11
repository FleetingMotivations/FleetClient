using FleetDaemon.Storage;
using FleetDaemon.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FleetDaemon.Hauler
{
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
            //this.knownApplications = this.storage.Get<Dictionary<String, FleetKnownApplication>>("AppHauler_KnownApplications");
            if (knownApplications == null)
            {
                this.knownApplications =  new Dictionary<string, FleetKnownApplication>();
                InitialiseKnownApplications();
                this.storage.Store("AppHauler_KnownApplications", this.knownApplications);
            }
        }

        /// <summary>
        /// Initialse the known applications list with any possible applications (for testing)
        /// </summary>
        private void InitialiseKnownApplications()
        {
            //var apphauler = AppHauler.Instance;
            var apphauler = this;

            // For Testing, add all known applications
            this.KnownApplications["fileshare"] = new FleetKnownApplication
            {
                Name = "File Share",
                Path = @"..\..\..\FileShare\bin\Debug\FileShare.exe",
                Identifier = "fileshare"
            };

            this.KnownApplications["fileinbox"] = new FleetKnownApplication
            {
                Name = "File Inbox",
                Path = @"..\..\..\FileInbox\bin\Debug\FileInbox.exe",
                Identifier = "fileinbox"
            };

            this.KnownApplications["fileaccept"] = new FleetKnownApplication
            {
                Name = "File Accept",
                Path = @"..\..\..\FileAccept\bin\Debug\FileAccept.exe",
                Identifier = "fileaccept",
                Visible = false
            };

            this.KnownApplications["workstationselector"] = new FleetKnownApplication
            {
                Name = "Workstation Selector",
                Path = @"..\..\..\WorkstationSelector\bin\Debug\WorkstationSelector.exe",
                Identifier = "workstationselector",
                Visible = false
            };

            this.KnownApplications["fleetshelf"] = new FleetKnownApplication
            {
                Name = "Fleet Shelf",
                Path = @"..\..\..\FleetShelf\bin\Debug\FleetShelf.exe",
                Identifier = "fleetshelf",
                Visible = false
            };

            this.KnownApplications["fleethelp"] = new FleetKnownApplication
            {
                Name = "Fleet Help",
                Path = @"..\..\..\FleetHelp\bin\Debug\FleetHelp.exe",
                Identifier = "fleethelp",
                Visible = false
            };
        }

        // Methods

            /// <summary>
            /// Launch the application with the passed id if valid. Record execution. 
            /// If not return false
            /// </summary>
            /// <param name="identifier"></param>
            /// <returns></returns>
        public bool LaunchApplication(string identifier)
        {
            try
            {
                // Get record
                var record = this.knownApplications[identifier];
                
                if (!this.runningApplications.ContainsKey(identifier))
                {
                    // Get executable
                    var exe = record.Path;

                    // Start process
                    var process = Process.Start(exe);
                    process.SetMainWindowFocus();
                    process.Exited += Process_Exited;

                    // Record execution
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

        /// <summary>
        /// Close the application wiht the passed id, or retiurn false if not running
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Check if the applciation with the passed id is running
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Check if the application with the passed id is runnig, if not launch it
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public bool IsRunningOrLaunch(string identifier)
        {
            if (!this.IsRunning(identifier))
            {
                return LaunchApplication(identifier);
            }
            return true;
        }

        /// <summary>
        /// Determines if the application with passed application is known
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public bool IsKnown(string identifier)
        {
            return this.knownApplications.ContainsKey(identifier);
        }

        // Event Handlers - not used
        private void Process_Exited(object sender, EventArgs e)
        {
            Console.WriteLine("Process has exited");
        }
    }

    /// <summary>
    /// Utility to make the main window of a process the main window
    /// This is not used.
    /// </summary>
    internal static class ProcessUtils
    {
        [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        internal static extern IntPtr SetFocus(HandleRef handle);

        internal static void SetMainWindowFocus(this Process p)
        {
            SetFocus(new HandleRef(null, p.MainWindowHandle));
        }
    }
}

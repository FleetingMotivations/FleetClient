using FleetDaemon.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

/*namespace FleetDaemon
{

    public class FleetAppConfig
    {
        public string Id { get; }
        public string Name { get; }
        public string Location { get; }
        public string PipeName { get; }
        public bool SingleInstance { get; }
    }

    public class FleetApplication
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string PipeName { get; set; }
        public bool SingleInstance { get; set; }
        public bool Running { get; set; }
        public List<Process> Processes { get; set; }
    }

    public interface IAppHauler
    {
        void ReloadConfig();

        FleetApplication GetAppByName(string applicationName);
        FleetApplication GetAppById(string id);

        bool LaunchApp(string id);
        bool LaunchApp(FleetApplication app);
        
        bool CloseApp(string id);
        bool CloseApp(FleetApplication app);
    }

    public class AppHauler : IAppHauler
    {
        private ISimpleStorage Storage;

        private List<FleetAppConfig> AppConfigs;
        private List<FleetApplication> Applications;

        public AppHauler(ISimpleStorage storage)
        {
            

            // "drag_drop":"..\..\..\FileShare\bin\Debug\FileShare.exe"

            this.Storage = storage;

            this.AppConfigs = this.Storage.Get<List<FleetAppConfig>>("applications");
            this.Applications = new List<FleetApplication>();
            
        }

        public void ReloadConfig()
        {
            this.AppConfigs = this.Storage.Get<List<FleetAppConfig>>("applications");
        }

        public FleetApplication GetAppByName(string appName)
        {
            var app = this.Applications.FirstOrDefault<FleetApplication>(a => a.Name == appName);

            if(app == null)
            {
                var appConf = this.AppConfigs.FirstOrDefault(a => a.Name == appName);
                app = new FleetApplication
                {
                    Id = appConf.Id,
                    Name = appConf.Name,
                    Location = appConf.Location,
                    PipeName = appConf.PipeName,
                    SingleInstance = appConf.SingleInstance,
                    Running = false,
                    Processes = new List<Process>()
                };
            }

            return app;
        }

        public FleetApplication GetAppById(string id)
        {
            var app = this.Applications.FirstOrDefault<FleetApplication>(a => a.Id == id);

            if (app == null)
            {
                var appConf = this.AppConfigs.FirstOrDefault(a => a.Id == id);
                app = new FleetApplication
                {
                    Id = appConf.Id,
                    Name = appConf.Name,
                    Location = appConf.Location,
                    SingleInstance = appConf.SingleInstance,
                    Running = false
                };
            }

            return app;
        }
        
        public bool LaunchApp(string id)
        {
            var app = GetAppById(id);
            return LaunchApp(app);
        }

        public bool LaunchApp(FleetApplication app)
        {
            //TODO(AL): Implement launching and returning a bool for success
            try
            {
                var process = Process.Start(app.Location);
                app.Running = true;


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public bool CloseApp(string id)
        {
            var app = GetAppById(id);
            return CloseApp(app);
        }

        public bool CloseApp(FleetApplication app)
        {
            try
            {
                for(var i = 0; i < app.Processes.Count(); ++i)
                {
                    app.Processes[i].Close();
                }

                app.Processes.Clear();
                app.Running = false;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return true;
        }
    }
}
*/
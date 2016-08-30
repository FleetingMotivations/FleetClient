using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.ServiceModel;
using FleetIPC;
using Newtonsoft.Json;

namespace WorkstationSelector
{

    class WorkstationDisplayModel
    {
        public string FriendlyName { get; set; }
        public string Identifier { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private List<Workstation> selectedWorkstations = new List<Workstation>();
        private List<String> selectedWorkstations = new List<String>();
        private FleetDaemonClient FleetDaemon { get; set; }
        private ServiceHost service;
        private IEnumerable<WorkstationDisplayModel> AvailableWorkstations { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            /*
            //Campus:
            ChecksModel campus = new ChecksModel("Callaghan");

            //Builldings:
            ChecksModel es = new ChecksModel("ES");
            ChecksModel ee = new ChecksModel("EE");
            ChecksModel ef = new ChecksModel("EF");
            campus.AddChild(es);
            campus.AddChild(ee);

            //Rooms:
            List<ChecksModel> rooms = new List<ChecksModel>();

            ChecksModel es123 = new ChecksModel("ES123");
            ChecksModel es124 = new ChecksModel("ES124");
            rooms.Add(es123);
            rooms.Add(es124);
            es.AddChildren(rooms);
            
            rooms.Clear();

            ChecksModel ee123 = new ChecksModel("EE123");
            ChecksModel ee124 = new ChecksModel("EE124");
            rooms.Add(ee123);
            rooms.Add(ee124);
            ee.AddChildren(rooms);
            rooms.Clear();

            //Workstations:
            List<ChecksModel> workstations = new List<ChecksModel>();
            for (int i = 1; i <= 10; i++)
            {
                workstations.Add(new ChecksModel("Workstation " + i));
            }
            es123.AddChildren(workstations);
            workstations.Clear();

            for (int i = 1; i <= 10; i++)
            {
                workstations.Add(new ChecksModel("Workstation " + i));
            }
            es124.AddChildren(workstations);
            workstations.Clear();

            for (int i = 1; i <= 10; i++)
            {
                workstations.Add(new ChecksModel("Workstation " + i));
            }
            ee123.AddChildren(workstations);
            workstations.Clear();

            for (int i = 1; i <= 10; i++)
            {
                workstations.Add(new ChecksModel("Workstation " + i));
            }
            ee124.AddChildren(workstations);
            workstations.Clear();
            

            DataContext = campus;
            */

            // Set the service events
            ApplicationService.OnInform += ApplicationService_OnInform;
            ApplicationService.OnDeliver += ApplicationService_OnDeliver;

            // Might want to do this as a background task?
            // Define address & binding for this applications service
            //var address = new Uri("net.pipe://localhost/WorkstationSelector");
            //var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);

            // Create and open the service
            /*this.service = new ServiceHost(typeof(ApplicationService));
            this.service.AddServiceEndpoint(typeof(IApplicationIPC), binding, address);
            this.service.Open();
            
            var cAddress = new EndpointAddress("net.pipe://localhost/fleetdaemon");
            var cBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            this.FleetDaemon = new FleetDaemonClient(cBinding, cAddress);

            var message = new IPCMessage();
            message.ApplicaitonSenderID = "WorkstationSelector";
            message.ApplicationRecipientID = "FleetDaemon";
            message.LocationHandle = IPCMessage.MessageLocationHandle.DAEMON;
            message.Type = "registration";
            
            this.FleetDaemon.Request(message);*/
        }

        private void ApplicationService_OnDeliver(IPCMessage message)
        {
            // Do some fun stuff
            /*
             [{Identifer:"asdfasdfasdf", FriendlyName: "sadfasdfasdf"}]
             */
            if (message.Type == "availableWorkstations")
            {
                AvailableWorkstations = JsonConvert.DeserializeObject<List<WorkstationDisplayModel>>(message.Content["workstations"]);
                var parent = new ChecksModel("Available Workstations");
                parent.AddChildren(AvailableWorkstations.Select(w => new ChecksModel {
                    Label = w.FriendlyName
                }).ToList());
            }
        }

        private void ApplicationService_OnInform(List<IPCMessage> message)
        {
            // Do some even more fun stuff
        }

        private void WorkstationSelected(object sender, RoutedEventArgs e)
        {
            var content = (e.Source as CheckBox).Content.ToString();
            var workstation = AvailableWorkstations.First(w => w.FriendlyName == content);
            selectedWorkstations.Add(workstation.Identifier);
            //MessageBox.Show(content + " Selected");
        }

        private void WorkstationUnselected(object sender, RoutedEventArgs e)
        {
            var content = (e.Source as CheckBox).Content.ToString();
            var workstation = AvailableWorkstations.First(w => w.FriendlyName == content);
            selectedWorkstations.Remove(workstation.Identifier);
            //MessageBox.Show(content + " Removed");
        }

        private void SendToDaemon(object sender, RoutedEventArgs e)
        {
            JsonSerializer serializer = new JsonSerializer();
            var message = new IPCMessage();

            message.ApplicaitonSenderID = "sendId";
            message.ApplicationRecipientID = "recipId";
            message.Target = IPCMessage.MessageTarget.Daemon;
            message.Type = "workstationShareList";

            message.Content["workstations"] = JsonConvert.SerializeObject(selectedWorkstations.ToArray());

            this.FleetDaemon.Request(message);
            
            String recipients = String.Join(", ", selectedWorkstations.ToArray());
            MessageBox.Show("Sent to: " + recipients);
        }
    }

    class ChecksModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<ChecksModel> _Children;
        private bool _IsChecked;
        private string _Label;

        public ChecksModel(string label, bool status)
        {
            Children = new ObservableCollection<ChecksModel>();
            Label = label;
            IsChecked = status;
        }

        public ChecksModel(string label) : this(label, false) { }

        public ChecksModel()
        {
            Children = new ObservableCollection<ChecksModel>();
        }

        public void AddChild(ChecksModel child)
        {
            Children.Add(child);
        }
        
        public void AddChildren(List<ChecksModel> children)
        {
            foreach (ChecksModel child in children)
            {
                Children.Add(child);
            }
        }
                
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<ChecksModel> Children
        {
            get { return _Children; }
            set
            {
                _Children = value;
                OnPropertyChanged("Children");
            }
        }
        
        public string Label
        {
            get { return _Label; }
            set
            {
                _Label = value;
                OnPropertyChanged("Label");
            }

        }

        public bool IsChecked
        {
            get {return _IsChecked;}
            set
            {
                _IsChecked = value;
                OnPropertyChanged("IsChecked");
                CheckNodes(value);
            }
        }
        private void CheckNodes(bool value)
        {
            foreach (ChecksModel m in _Children)
            {
                m.IsChecked = value;
            }
        }
    }
}
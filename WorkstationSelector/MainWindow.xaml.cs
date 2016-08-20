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

namespace WorkstationSelector
{
    public partial class MainWindow : Window
    {
        //private List<Workstation> selectedWorkstations = new List<Workstation>();
        private List<String> selectedWorkstations = new List<String>();

        public MainWindow()
        {
            InitializeComponent();

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
        }

        private void WorkstationSelected(object sender, RoutedEventArgs e)
        {
            var content = (e.Source as CheckBox).Content.ToString();
            selectedWorkstations.Add(content);
            MessageBox.Show(content + " Selected");
        }

        private void WorkstationUnselected(object sender, RoutedEventArgs e)
        {
            var content = (e.Source as CheckBox).Content.ToString();
            selectedWorkstations.Remove(content);
            MessageBox.Show(content + " Removed");
        }

        private void SendToDaemon(object sender, RoutedEventArgs e)
        {
            var cAddress = new EndpointAddress("net.pipe://localhost/fleetdaemon");
            var cBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            var daemon = new FleetDaemonClient(cBinding, cAddress);

            var message = new IPCMessage();
            foreach (String recipient in selectedWorkstations)
            {
                message.ApplicaitonSenderID = "sendId";
                message.ApplicationRecipientID = "recipId";
                message.Content["type"] = "sometype";
                message.Content["Sender"] = "this";
                message.Content["Receiver"] = recipient;
                daemon.Request(message);

            }
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
using FleetIPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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

namespace FileAccept
{
    public partial class MainWindow : Window
    {
        String temp_appId = "1jg1234j432";
        String temp_response = "response!";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            String response = "accept";
            //Change button colour when pushed:
            AcceptButton.Background = Brushes.LawnGreen;
            //Reply to Daemon:
            respond(response);
            //Close window:
            this.Close();
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            String response = "reject";
            //Change button colour when pushed:
            RejectButton.Background = Brushes.OrangeRed;
            //Reply to Daemon:
            respond(response);
            //Close window:
            this.Close();
        }

        private void respond(String response)
        {
            var cAddress = new EndpointAddress("net.pipe://localhost/fleetdaemon");
            var cBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            var daemon = new FleetDaemonClient(cBinding, cAddress);

            var message = new IPCMessage();
            message.ApplicaitonSenderID = "sendId";
            message.ApplicationRecipientID = "recipId";
            message.Content["response"] = response;

            daemon.Request(message);

        }
    }
}

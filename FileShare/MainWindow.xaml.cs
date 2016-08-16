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

namespace FileShare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void StackPanel_Drop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                //HandleFileOpen(files[0]);
                Console.WriteLine("Got mah bois");

                var cAddress = new EndpointAddress("net.pipe://localhost/fleetdaemon");
                var cBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                var client = new FleetIPC.FleetDaemonClient(cBinding, cAddress);

                var message = new IPCMessage();
                message.ApplicaitonSenderID = "fileshare";
                message.ApplicationRecipientID = "friendface";
                message.Content["file"] = "Yo bud I got chu a file aight!?";
                message.Content["filelocation"] = files[0];

                client.Request(message);
            }
        }

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {

        }

        private void textBox_PreviewDragEnter(object sender, DragEventArgs e)
        {

        }

        private void textBox_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void textBox_PreviewDragOver(object sender, DragEventArgs e)
        {

        }

        private void textBox_DragOver(object sender, DragEventArgs e)
        {

        }

        private void textBox_Drop(object sender, DragEventArgs e)
        {

        }
    }
}

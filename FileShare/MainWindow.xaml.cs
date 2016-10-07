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
using System.Windows.Interop;
using System.Reflection;

using MahApps.Metro.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace FileShare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private const string SendMessage = "Sending Files to Workstation(s)";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnFileDragOver(object sender, DragEventArgs e)
        {
            this.AddFilePanel.Background = Brushes.Gray;
            this.AddFilePanel.Opacity = 0.5;
        }

        private void OnFileDragLeave(object sender, DragEventArgs e)
        {
            this.AddFilePanel.Background = Brushes.Gray;
            this.AddFilePanel.Opacity = 0.1;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            //Style the StackPanel to provide user feedback:
            this.AddFilePanel.Background = Brushes.Gray;
            this.AddFilePanel.Opacity = 1;

            //TODO: Maybe add an icon of the file, or something else rather than just simply changing the background to grey.
            //      This adds more user feedback which is always nice

            String[] droppedFiles = null;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                droppedFiles = e.Data.GetData(DataFormats.FileDrop, true) as String[];

                this.AttachedFiles.Items.Add(droppedFiles[0]);
                this.RemoveFileButton.IsEnabled = true;
                this.SelectWorkstationsButton.IsEnabled = true;
            }

            if ((null == droppedFiles) || (!droppedFiles.Any())) { return; }

            var daemonClient = IPCUtil.MakeDaemonClient();

            try
            {
                daemonClient.Open();

                var message = new IPCMessage();
                message.ApplicaitonSenderID = "FileShare";
                message.ApplicationRecipientID = "friendface";
                message.Target = IPCMessage.MessageTarget.Remote;
                message.Type = "sendFile";
                message.Content["filePath"] = droppedFiles[0];      // TODO: Handle multiple files (seperate by character? or encode as JSON?)

                daemonClient.Request(message);
                daemonClient.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                daemonClient.Abort();
            }
        }

        private void AddFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog exp = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = exp.ShowDialog();

            if(result == true)
            {
                String filename = exp.FileName;
                this.AttachedFiles.Items.Add(filename);
            }

            this.SelectWorkstationsButton.IsEnabled = true;
            this.RemoveFileButton.IsEnabled = true;
        }

        private void RemoveFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.AttachedFiles.SelectedItems.Count > 0)
            {
                foreach (var item in this.AttachedFiles.SelectedItems)
                {
                    Console.WriteLine("DELETING " + item);
                    this.AttachedFiles.Items.Remove(item);

                    if(this.AttachedFiles.SelectedItems.Count == 0) { break; }
                }

                if (this.AttachedFiles.Items.Count == 0)
                {
                    this.RemoveFileButton.IsEnabled = false;
                    this.SelectWorkstationsButton.IsEnabled = false;
                }
            }

        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            int delayTime = 5000;
            
            var flyout = this.SendingFlyout;
            flyout.Visibility = Visibility.Visible;
            ((Storyboard)FindResource("SendSpinner")).Begin();

            await Task.Delay(delayTime);

            flyout.Visibility = Visibility.Collapsed;

            //Send this.AttachedFiles.Items - requires Workstation Selections


        }
    }
}

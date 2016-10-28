/* 
 * Description: FileShare application interface.
 *              This application is used to provide help for users of the Fleet system. Including FAQs,
 *              tutorials and information about Fleet as a system.
 * Project: Fleet/FleetClient
 * Project Member: Jordan Collins, Hayden Cheers, Alistair Woodcock, Tristan Newmann
 * Last modified: 11 October 2016
 * Last Author: Jordan Collins
 * 
*/

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
        private String fileToSend = null;

        /*
         * MainWindow(): initialise FileShare
        */
        public MainWindow()
        {
            InitializeComponent();
        }

        /*
         * OnFileDragOver(object, DragEventArgs): Change the dragzone appearance when a file is dragged over it
        */
        private void OnFileDragOver(object sender, DragEventArgs e)
        {
            this.AddFilePanel.Background = Brushes.Gray;
            this.AddFilePanel.Opacity = 0.5;
        }

        /*
         * OnFileDragLeave(object, DragEventArgs): Change the dragzone appearance when a file is held over it
        */
        private void OnFileDragLeave(object sender, DragEventArgs e)
        {
            this.AddFilePanel.Background = Brushes.Gray;
            this.AddFilePanel.Opacity = 0.1;
        }

        /*
         * Window_Drop(object, DragEventArgs): Store the file which has been dragged and dropped into the dropzone.
        */
        private void Window_Drop(object sender, DragEventArgs e)
        {
            //Style the StackPanel to provide user feedback:
            this.AddFilePanel.Background = Brushes.Gray;
            this.AddFilePanel.Opacity = 1;

            //collect the data from the dropped file:
            String[] droppedFile = null;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //add to dropped files:
                droppedFile = e.Data.GetData(DataFormats.FileDrop, true) as String[];

                ////update interface to match:
                //this.AttachedFiles.Items.Add(droppedFile[0]);
                //this.RemoveFileButton.IsEnabled = true;
                //this.SelectWorkstationsButton.IsEnabled = true;
            }

            //ensure valid files exist in droppedFile:
            if ((null == droppedFile) || (!droppedFile.Any())) { return; }

            //create a Daemon instance:
            var daemonClient = IPCUtil.MakeDaemonClient();

            //create messages from the files:
            try
            {
                //open the client 
                daemonClient.Open();

                //for all files in the list to send:
                foreach (var file in droppedFile)
                {
                    // Create message and send file
                    var message = new IPCMessage();
                    message.ApplicaitonSenderID = "FileShare";
                    message.ApplicationRecipientID = "FileInbox";
                    message.Target = IPCMessage.MessageTarget.Remote;
                    message.Type = "sendFile";
                    message.Content["filePath"] = file;

                    //send message to daemon:
                    daemonClient.Request(message);
                }

                daemonClient.Close();

            }
            catch (Exception ex)
            {
                //print error and abort:
                Console.WriteLine(ex.Message);
                daemonClient.Abort();
            }
        }

        ///*
        // * AddFileButton_Click(object, RoutedEventArgs): Add a file to the list of files to share, through the use
        // *                                               of a button and a windows explorer dialog
        //*/
        //private void AddFileButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //open the windows explorer dialog:
        //    Microsoft.Win32.OpenFileDialog exp = new Microsoft.Win32.OpenFileDialog();

        //    Nullable<bool> result = exp.ShowDialog();

        //    //add the file selected to the AttachedFiles list:
        //    if(result == true)
        //    {
        //        fileToSend = exp.FileName;
        //        this.AttachedFiles.Items.Add(fileToSend);
        //    }

        //    //enable the SelectWorkstation and RemoveFile buttons:
        //    this.SelectWorkstationsButton.IsEnabled = true;
        //    this.RemoveFileButton.IsEnabled = true;
        //}

        ///*
        // * RemoveFileButton_Click(object, RoutedEventArgs): Remove the selected file from the attached files listbox
        //*/
        //private void RemoveFileButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //check if files have been selected that wish to be deleted:
        //    if (this.AttachedFiles.SelectedItems.Count > 0)
        //    {
        //        //remove each attached file selected
        //        foreach (var item in this.AttachedFiles.SelectedItems)
        //        {
        //            this.AttachedFiles.Items.Remove(item);

        //            //stop once all the files have been removed
        //            if(this.AttachedFiles.SelectedItems.Count == 0) { break; }
        //        }

        //        //disable the SelectWorkstation and RemoveFile buttons:
        //        if (this.AttachedFiles.Items.Count == 0)
        //        {
        //            this.RemoveFileButton.IsEnabled = false;
        //            this.SelectWorkstationsButton.IsEnabled = false;
        //        }
        //    }

        //}

        ///*
        // * SendButton_Click(object, RoutedEventArgs): Send the files to the selected workstations
        //*/
        //private void SendButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //int delayTime = 5000;

        //    //For future reference. This will be used to provide a loading screen for usability:

        //    //popup a flyout that informs the user that the files are being delivered
        //    //var flyout = this.SendingFlyout;
        //    //flyout.Visibility = Visibility.Visible;
        //    //((Storyboard)FindResource("SendSpinner")).Begin();

        //    //await Task.Delay(delayTime);

        //    //create an instance of Daemon:
        //    var daemonClient = IPCUtil.MakeDaemonClient();

        //    try
        //    {
        //        daemonClient.Open();
                
        //        // Create message:

        //        var message = new IPCMessage();
        //        message.ApplicaitonSenderID = "FileShare";
        //        message.ApplicationRecipientID = "FileInbox";
        //        message.Target = IPCMessage.MessageTarget.Remote;
        //        message.Type = "sendFile";
        //        message.Content["filePath"] = fileToSend;

        //        //send file to daemon
        //        daemonClient.Request(message);

        //        daemonClient.Close();

        //    }
        //    catch (Exception ex)
        //    {
        //        //print error and abort:
        //        Console.WriteLine(ex.Message);
        //        daemonClient.Abort();
        //    }

        //    //flyout.Visibility = Visibility.Collapsed;
        //}
    }
}

/* 
 * Description: FileInbox application interface.
 *              This application is the inbox containing the received files.
 * Project: Fleet/FleetClient
 * Project Member: Jordan Collins, Hayden Cheers, Alistair Woodcock, Tristan Newmann
 * Last modified: 11 October 2016
 * Last Author: Hayden Cheers
 * 
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using MahApps.Metro.Controls;

namespace FileInbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /*
         * MainWindow(): initialise FileInbox
        */
        public MainWindow()
        {
            InitializeComponent();

            // Initialise file store and link events to handler
            this.storage = new FileStore();
            this.storage.OnCreate += FilesDidChangeEvent;
            this.storage.OnChange += FilesDidChangeEvent;
            this.storage.OnDelete += FilesDidChangeEvent;
            this.storage.OnRename += FilesDidChangeEvent;
            this.filesTable.ItemsSource = this.Storage.Files;            
        }

        /// <summary>
        /// Called when UI is initialised
        /// Load the list of files to display
        /// </summary>
        public override void BeginInit()
        {
            base.BeginInit();
            
            // Load the files to display
            this.RefreshFiles();
        }

        /// <summary>
        /// On files did change, refresh the file list
        /// </summary>
        private void FilesDidChangeEvent()
        {
            Dispatcher.Invoke(() =>
            {
                this.RefreshFiles();
            });
        }

        // read-only property for file store
        private FileStore storage;
        public FileStore Storage { get
            {
                return storage;
            }
        }

        /// <summary>
        /// Refresh the file table (if exists)
        /// </summary>
        public void RefreshFiles()
        {
            this.filesTable?.Items?.Refresh();
        }

        // Button Events

            /// <summary>
            /// Open the selected file
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the item
                var index = this.filesTable.SelectedIndex;
                var item = this.storage.Files[index];

                // Start the item (this will cause windows to open it using the associated application based on file type
                Process.Start(item.Filepath);
            }
            catch(Exception ex)
            { }

        }

        /// <summary>
        /// Copy the selected file to a defined location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyButton_Click(object sender, RoutedEventArgs e)
        {
            //  Todo: Implement
        }

        /// <summary>
        /// Refresh button handler. Refreshes the file list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.RefreshFiles();
        }
    }
}

/* 
 * Description: FleetAccept application interface.
 *              This application is used as a notifcation for the receiving workstation, asking for a response whethere
 *              they wish to accept or decline the incoming file.
 * Project: Fleet/FleetClient
 * Project Member: Jordan Collins, Hayden Cheers, Alistair Woodcock, Tristan Newmann
 * Last modified: 11 October 2016
 * Last Author: Jordan Collins
 * 
*/

using FleetServer;
using System;
using System.Windows;
using System.Windows.Media;

using MahApps.Metro.Controls;

namespace FileAccept
{
    public partial class MainWindow : MetroWindow
    {
        // Flag if the user accepted the file
        public Boolean DidAccept { get; set; } = false;

        /*
         * MainWindow(): initialise FleetAccept
        */
        public MainWindow()
        {
            InitializeComponent();
        }

        /*
         * AcceptButton_Click(object, RoutedEventArgs): Accept the incoming file
        */
        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            AcceptButton.Background = Brushes.LawnGreen;
            this.DidAccept = true;
            this.Close();
        }

        /*
         * RejectButton_Click(object, RoutedEventArgs): Reject the incoming file
        */
        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            RejectButton.Background = Brushes.OrangeRed;
            this.DidAccept = false;
            this.Close();
        }

        /*
         * Boolean? ShowRequestDialog(FleetFileIdentifier): Show the incoming message details, awaiting a reply
        */
        public Boolean? ShowRequestDialog(FleetFileIdentifier ident)
        {
            // Set file attributes
            this.Name.Text = ident.SenderName;
            this.Filename.Text = ident.FileName;
            this.Size.Text = ident.FileSize + "kb";

            return this.ShowDialog();
        }
    }
}

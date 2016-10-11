/* 
 * Description: FleetShelf application interface.
 *              This application is the entry point for the users and consistents of popout 'shelf' (similar to a horizontal dock)
 *              which contains each of the applications of the Fleet system.
 * Project: Fleet/FleetClient
 * Last modified: 11 October 2016
 * Last Author: Jordan Collins
 * 
*/

using System;
using System.Collections.Generic;
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
using FleetIPC;

namespace FleetShelf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //get the list of Fleet applications:
        private List<FleetShelfApplication> applications;
        //initialise the Fleet notification tray icon:
        private System.Windows.Forms.NotifyIcon trayIcon = new System.Windows.Forms.NotifyIcon();

        /*
         * MainWindow(): initialise the FleetShelf
        */
        public MainWindow()
        {
            InitializeComponent();
            
            //TrayIcon
            trayIcon.Visible = true;
            trayIcon.Icon = new System.Drawing.Icon("../../Assets/fleet.ico");
            trayIcon.Text = "Fleet";
            trayIcon.BalloonTipClicked += TrayIconBalloonTip_Clicked;

            //ContexMenu:
            var fleetContextMenu = new System.Windows.Forms.ContextMenu();

            //Menu Items:
            var help = new System.Windows.Forms.MenuItem();
            help.Index = 0;
            help.Text = "Help";
            help.Click += new EventHandler(this.TrayHelp_Click);
            var exit = new System.Windows.Forms.MenuItem();
            exit.Index = 1;
            exit.Text = "Exit";
            exit.Click += new EventHandler(this.TrayExit_Click);

            //Add MenuItems to the ContextMenu:
            fleetContextMenu.MenuItems.Add(help);
            fleetContextMenu.MenuItems.Add(exit);
            //Map ContextMenu to trayIcon:
            trayIcon.ContextMenu = fleetContextMenu;

            this.AllowsTransparency = true;
            this.Focus();
        }

        /*
         * Button_Click(object, RoutedEventArgs): Change the FleetShelf state when the Fleet icon is selected.
        */
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //if the FleetShelf is closed, open it
            if (FleetShelf.Visibility == Visibility.Collapsed)
            {
                FleetShelf.Visibility = Visibility.Visible;
                (sender as Button).Margin = new Thickness(45, 0, 0, 0);
            }
            else //if the FleetShelf is open, close it
            {
                FleetShelf.Visibility = Visibility.Collapsed;
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(@"/FleetShelf;component/Assets/fleet.ico", UriKind.Relative));
                (sender as Button).Content = img;
                (sender as Button).Margin = new Thickness(0, 0, 0, 0);
            }
        }

        /*
         * Inbox_Click(object, RoutedEventArgs): Open the FleetInbox application
        */
        private void Inbox_Click(object sender, RoutedEventArgs e)
        {
            this.LaunchApplication("fileinbox");
        }

        /*
         * ScreenCapture_Click(object, RoutedEventArgs): Open the ScreenCapture application
        */
        private void ScreenCapture_Click(object sender, RoutedEventArgs e)
        {
            //this.LaunchApplication("screencapture");
        }

        /*
         * WorkstationShare_Click(object, RoutedEventArgs): Open the FileShare application
        */
        private void WorkstationShare_Click(object sender, RoutedEventArgs e)
        {
            this.LaunchApplication("fileshare");
        }

        /*
         * FacilitatorControls_Click(object, RoutedEventArgs): Open the FacilitatorControls application
        */
        private void FacilitatorControls_Click(object sender, RoutedEventArgs e)
        {
            //this.LaunchApplication("facilitatorcontrols");
        }

        /*
         * Help_Click(object, RoutedEventArgs): Open the FleetHelp application
        */
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            this.LaunchApplication("fleethelp");
        }

        // 3. We have received a list of applications
        // We can render them and stuff.

        internal void UpdateApplications(List<FleetShelfApplication> applications)
        {
            this.applications = applications;
        }

        /*
         * LaunchApplication(String): Launch the Fleet application based on the application id
        */
        private void LaunchApplication(String identifier)
        {
            var message = new IPCMessage();
            message.ApplicaitonSenderID = App.ApplicationIdentifier;
            message.Target = IPCMessage.MessageTarget.Daemon;
            message.Type = "launchApplication";
            message.Content["application"] = identifier;

            var client = IPCUtil.MakeDaemonClient();

            try
            {
                client.Open();
                client.Request(message);
                client.Abort();

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                client.Abort();
            }
        }

        /*
         * TrayHelp_Click(object, EventArgs): Open the FleetHelp application
        */
        private void TrayHelp_Click(object sender, EventArgs e)
        {
            //Redirect users to a help document/window/webpage
            //this.LaunchApplication("fleethelp");

            //Balloon Sample
            string title = "Fleet Files Delivered";
            string text = "X Files Accepted\nY Files Rejected";
            trayIcon.ShowBalloonTip(1000, title, text, System.Windows.Forms.ToolTipIcon.Info);
        }

        /*
         * TrayHelp_Click(object, EventArgs): Open the FleetHelp application
        */
        private void TrayIconBalloonTip_Clicked(object sender, EventArgs e)
        {
            //Once the BalloonTip is selected, open a file delivery summary
        }

        /*
         * TrayExit_Click(object, EventArgs): Close FleetShelf
        */
        private void TrayExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

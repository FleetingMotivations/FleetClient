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

        private List<FleetShelfApplication> applications;
        private System.Windows.Forms.NotifyIcon trayIcon = new System.Windows.Forms.NotifyIcon();

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

            // TODO: Fit to screen without taskbar

            this.AllowsTransparency = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (FleetShelf.Visibility == Visibility.Collapsed)
            {
                FleetShelf.Visibility = Visibility.Visible;
                (sender as Button).Margin = new Thickness(45, 0, 0, 0);
            }
            else
            {
                FleetShelf.Visibility = Visibility.Collapsed;
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(@"/FleetShelf;component/Assets/fleet.ico", UriKind.Relative));
                (sender as Button).Content = img;
                (sender as Button).Margin = new Thickness(0, 0, 0, 0);
            }
        }

        private void UnFocusDock(object sender, RoutedEventArgs e)
        {
            (sender as DockPanel).Visibility = Visibility.Collapsed;
        }

        /*
         *  Might want to replace these with a generic on click method
         *  that send the correct message based on a tag or something?
         */

        private void Inbox_Click(object sender, RoutedEventArgs e)
        {
            this.LaunchApplication("fileinbox");
        }

        private void ScreenCapture_Click(object sender, RoutedEventArgs e)
        {
            this.LaunchApplication("fileshare");
        }

        private void WorkstationShare_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        private void FacilitatorControls_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        // 3. We have received a list of applications
        // We can render them and stuff.

        internal void UpdateApplications(List<FleetShelfApplication> applications)
        {
            this.applications = applications;
        }

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

        private void TrayHelp_Click(object sender, EventArgs e)
        {
            //Redirect users to a help document/window/webpage

            //Balloon Sample
            string title = "Fleet Files Delivered";
            string text = "X Files Accepted\nY Files Rejected";
            
            trayIcon.ShowBalloonTip(1000, title, text, System.Windows.Forms.ToolTipIcon.Info);
        }

        private void TrayIconBalloonTip_Clicked(object sender, EventArgs e)
        {
            //Once the BalloonTip is selected, open a file delivery summary
        }

        private void TrayExit_Click(object sender, EventArgs e)
        {
            //Close the FleetShelf
            this.Close();
            
        }
    }
}

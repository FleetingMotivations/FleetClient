/* 
 * Description: Workstation Selector Window
 *              Main window for workstation slector
 * Project: Fleet/WorkstationSelector
 * Last modified: 11 October 2016
 * Last Author: Hayden Cheers
*/

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using FleetServer;
using System.Windows.Media.Animation;
using System.Threading.Tasks;

namespace WorkstationSelector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private List<FleetClientIdentifier> selectedClients = new List<FleetClientIdentifier>();
        private List<FleetClientIdentifier> availableClients = null;

        public List<FleetClientIdentifier> ShowSelectorDialog(List<FleetClientIdentifier> clients)
        {
            // Update UI with list of clients
            this.availableClients = clients;
            this.RenderWorkstations();
            
            // Show dialog
            this.ShowDialog();

            // Return the selected clients
            return selectedClients;
        }

        private void RenderWorkstations()
        {
            //TODO:
            //If unavailable: Change Opacity for usability:
            //img.Opacity = 0.1;

            //TODO:
            //If unavailable: Add ToolTip for why it is unavailable:
            //tile.ToolTip = tile.Title.ToString() + " unavailable. (Offline)";

            //populate the WorkstationSelectorPanel with workstations
            foreach (var client in availableClients)
            {
                var tile = new FleetWorkstationTile(client);
                tile.Click += SelectWorkstation_Click;
                WorkstationSelectorPanel.Children.Add(tile);
            }
        }

        private void SelectWorkstation_Click(object sender, RoutedEventArgs e)
        {
            //identify the selected tile:
            var tile = e.OriginalSource as FleetWorkstationTile;

            //toggle the status of the tile:
            if (tile.Tag.Equals("0")) //Not selected
            {
                //change tile appearance:
                tile.Tag = "1"; //Activate button
                tile.Background = (SolidColorBrush)Resources["SelectedWorkstation"];

                //add the workstation to the selected clients list
                this.selectedClients.Add(tile.Identifier);
            }
            else //The Tile was selected, so deselect it
            {
                //change tile appearance:
                tile.Tag = "0";
                tile.Background = (SolidColorBrush)Resources["FleetBlue"];
                this.AllButton.Content = "\xE8B3"; //Select All
                this.AllButton.ToolTip = "Select all workstations";
                this.AllButton.Click += new RoutedEventHandler(SelectAllWorkstations_Click);

                //remove the workstation from the selected clients list
                this.selectedClients.Remove(tile.Identifier);
            }
        }

        private void AllWorkstations_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as Button).Tag.Equals("Select")) //Select All
            {
                //Change all the workstations appearance and select all workstations
                SelectAllWorkstations_Click(sender, e);
                //update the 'all' button to deselect
                this.AllButton.Content = "\xE1C5"; //Deselect All
                this.AllButton.Tag = "Deselect";
                this.AllButton.ToolTip = "Deselect all workstations";
                this.AllButton.Click += new RoutedEventHandler(DeselectAllWorkstations_Click);
            }
            else //"Deselect All"
            {
                //Change all the workstations appearance and deselect all workstations:
                DeselectAllWorkstations_Click(sender, e);
                //update the 'all' button to select
                this.AllButton.Content = "\xE8B3"; //Select All
                this.AllButton.Tag = "Select";
                this.AllButton.ToolTip = "Select all workstations";
                this.AllButton.Click += new RoutedEventHandler(SelectAllWorkstations_Click);
            }
        }

        private void SelectAllWorkstations_Click(object sender, RoutedEventArgs e)
        {
            //Get all workstations, 
            var workstations = FindAllWorkstations(this, new List<Tile>());
            //change appearance of all workstations:
            foreach (Tile t in workstations)
            {
                t.Tag = "1"; //Activate button
                t.Background = (SolidColorBrush)Resources["SelectedWorkstation"];
            }

            //update selected clients to be all:
            this.selectedClients.Clear();
            this.selectedClients.AddRange(this.availableClients);
        }

        private void DeselectAllWorkstations_Click(object sender, RoutedEventArgs e)
        {
            //Get all workstations
            var workstations = FindAllWorkstations(this, new List<Tile>());
            //change appearance of all workstations:
            foreach (Tile t in workstations)
            {
                t.Tag = "0"; //Deactivate button
                t.Background = (SolidColorBrush)Resources["FleetBlue"];
            }

            //remove all selected clients:
            this.selectedClients.Clear();
        }

        /*
         * IList<Tile> FindAllWorkstations(object, IList<Tile>): finds all the workstations in the interface and returns
         *                                                       them as an IList. This is performed recursively.
         */
        private IList<Tile> FindAllWorkstations(object uiElement, IList<Tile> tiles)
        {
            if (uiElement is Tile)
            {
                //If its a tile add it to the list of tiles
                var tile = (Tile)uiElement;
                tiles.Add(tile);
            }
            else if (uiElement is Grid)
            {
                //If its a Grid, search for the workstations inside the grid
                var uiElementAsCollection = (Grid)uiElement;
                foreach (var element in uiElementAsCollection.Children)
                {
                    FindAllWorkstations(element, tiles);
                }
            }
            else if (uiElement is Button)
            {
                //If its a Button, recurse through again
                FindAllWorkstations(this.WorkstationSelectorPanel, tiles);
            }
            else if (uiElement is WrapPanel)
            {
                //IF its a WrapPanel, recurse through its children elements
                var element = (WrapPanel)uiElement;
                for (int i = 0; i < element.Children.Count; i++)
                {
                    FindAllWorkstations(element.Children[i], tiles);
                }
            }
            else if (uiElement is UserControl)
            {
                //If its a UserControl element, then recurse through
                var uiElementAsUserControl = (UserControl)uiElement;
                FindAllWorkstations(uiElementAsUserControl.Content, tiles);
            }
            else if (uiElement is ContentControl)
            {
                //IF its a ContentControl element, then recurse through
                var uiElementAsContentControl = (ContentControl)uiElement;
                FindAllWorkstations(uiElementAsContentControl.Content, tiles);
            }

            //return the list of tiles after the recursive function has completed:
            return tiles;

        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            int delayTime = 2000;

            this.SendingFlyout.Visibility = Visibility.Visible;
            ((Storyboard)FindResource("SendSpinner")).Begin();

            await Task.Delay(delayTime);

            ((Storyboard)FindResource("SendSpinner")).Stop();
            this.Sending.Text = "\xf05d";
            this.Sending.Foreground = (SolidColorBrush)Resources["FleetGreen"];

            this.SendingMessage.FontSize = 32;
            this.SendingMessage.Text = "File(s) Sent!";
            await Task.Delay(1000);

            //Close the interface:
            this.Close();
        }

        private void WorkstationScope_Click(object sender, RoutedEventArgs e)
        {
            var button = (sender as Button);
            WorkstationFlyout.IsOpen = true;
        }

        private void CloseButton_Click()
        {
            WorkstationFlyout.IsOpen = false;
        }
    }
}
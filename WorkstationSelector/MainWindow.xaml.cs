using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using FleetServer;

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
            this.availableClients = clients;
            this.RenderWorkstations();
            this.ShowDialog();
            return selectedClients;
        }

        private void RenderWorkstations()
        {

            foreach (var client in availableClients)
            {
                var tile = new FleetWorkstationTile(client);
                tile.Click += SelectWorkstation_Click;
                WorkstationSelectorPanel.Children.Add(tile);
            }

            /*for (int i = 0; i < 9; i++) //TODO: Change for all Workstations in the context
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(@"/WorkstationSelector;component/Assets/workstation.png", UriKind.Relative));
                img.Stretch = Stretch.Fill;
                img.Margin = new Thickness(0, 10, 0, 20);

                //TODO:
                //If unavailable: Change Opacity for usability:
                //img.Opacity = 0.1;

                Tile tile = new Tile();
                tile.Content = img;
                tile.Title = "Workstation " + (i + 1);

                //TODO:
                //If unavailable: Add ToolTip for why it is unavailable:
                //tile.ToolTip = tile.Title.ToString() + " unavailable. (Offline)";

                tile.Style = (Style)Resources["SmallTileStyle"];
                tile.Click += new RoutedEventHandler(SelectWorkstation_Click);
                tile.Tag = "0"; //Not selected

                WorkstationSelectorPanel.Children.Add(tile);
            }*/
        }

        private void SelectWorkstation_Click(object sender, RoutedEventArgs e)
        {
            var tile = e.OriginalSource as FleetWorkstationTile;

            if (tile.Tag.Equals("0")) //Not selected
            {
                tile.Tag = "1"; //Activate button
                tile.Background = (SolidColorBrush)Resources["SelectedWorkstation"];

                this.selectedClients.Add(tile.Identifier);
            }
            else //The Tile was selected, so deselect it
            {
                tile.Tag = "0";
                tile.Background = (SolidColorBrush)Resources["AvailableWorkstation"];
                this.AllButton.Content = "\xE8B3"; //Select All
                this.AllButton.ToolTip = "Select all workstations";
                this.AllButton.Click += new RoutedEventHandler(SelectAllWorkstations_Click);

                this.selectedClients.Remove(tile.Identifier);
            }
        }

        private void AllWorkstations_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as Button).Tag.Equals("Select")) //Select All
            {
                SelectAllWorkstations_Click(sender, e);
                this.AllButton.Content = "\xE1C5"; //Deselect All
                this.AllButton.Tag = "Deselect";
                this.AllButton.ToolTip = "Deselect all workstations";
                this.AllButton.Click += new RoutedEventHandler(DeselectAllWorkstations_Click);
            }
            else //"Deselect All"
            {
                DeselectAllWorkstations_Click(sender, e);
                this.AllButton.Content = "\xE8B3"; //Select All
                this.AllButton.Tag = "Select";
                this.AllButton.ToolTip = "Select all workstations";
                this.AllButton.Click += new RoutedEventHandler(SelectAllWorkstations_Click);
            }
        }

        private void SelectAllWorkstations_Click(object sender, RoutedEventArgs e)
        {
            var workstations = FindAllWorkstations(this, new List<Tile>());
            foreach (Tile t in workstations)
            {
                t.Tag = "1"; //Activate button
                t.Background = (SolidColorBrush)Resources["SelectedWorkstation"];
            }

            this.selectedClients.Clear();
            this.selectedClients.AddRange(this.availableClients);
        }

        private void DeselectAllWorkstations_Click(object sender, RoutedEventArgs e)
        {
            var workstations = FindAllWorkstations(this, new List<Tile>());
            foreach (Tile t in workstations)
            {
                t.Tag = "0"; //Deactivate button
                t.Background = (SolidColorBrush)Resources["AvailableWorkstation"];
            }

            this.selectedClients.Clear();
        }

        private IList<Tile> FindAllWorkstations(object uiElement, IList<Tile> tiles)
        {
            if (uiElement is Tile)
            {
                var tile = (Tile)uiElement;
                tiles.Add(tile);
            }
            else if (uiElement is Grid)
            {
                var uiElementAsCollection = (Grid)uiElement;
                foreach (var element in uiElementAsCollection.Children)
                {
                    FindAllWorkstations(element, tiles);
                }
            }
            else if (uiElement is Button)
            {
                FindAllWorkstations(this.WorkstationSelectorPanel, tiles);
            }
            else if (uiElement is WrapPanel)
            {
                var element = (WrapPanel)uiElement;
                for (int i = 0; i < element.Children.Count; i++)
                {
                    FindAllWorkstations(element.Children[i], tiles);
                }
            }
            else if (uiElement is UserControl)
            {
                var uiElementAsUserControl = (UserControl)uiElement;
                FindAllWorkstations(uiElementAsUserControl.Content, tiles);
            }
            else if (uiElement is ContentControl)
            {
                var uiElementAsContentControl = (ContentControl)uiElement;
                FindAllWorkstations(uiElementAsContentControl.Content, tiles);
            }

            return tiles;

        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Integrate with system for Sending
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
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;
using FleetServer;

namespace WorkstationSelector
{
    /// <summary>
    /// Utility class for displaying a single workstation per tile
    /// </summary>
    public class FleetWorkstationTile : Tile
    {
        // Identifier of workstation
        private FleetClientIdentifier identifier;
        public FleetClientIdentifier Identifier
        {
            get
            {
                return identifier;
            }
        }

        public FleetWorkstationTile(FleetClientIdentifier identifier)
        {
            // Set attributes
            this.identifier = identifier;

            // Handle tile image
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(@"/WorkstationSelector;component/Assets/workstation_icon.ico", UriKind.Relative));
            img.Stretch = Stretch.Fill;
            img.Margin = new Thickness(0, 10, 0, 20);
            this.Content = img;

            // Set appearance
            this.Title = identifier.WorkstationName;
            this.Style = (Style)Resources["SmallTileStyle"];
            this.Tag = "0";
        }
    }
}

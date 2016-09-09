using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;
using FleetServer;

namespace WorkstationSelector
{
    public class FleetWorkstationTile : Tile
    {
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
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(@"/WorkstationSelector;component/Assets/workstation.png", UriKind.Relative));
            img.Stretch = Stretch.Fill;
            img.Margin = new Thickness(0, 10, 0, 20);
            this.Content = img;

            this.Title = identifier.WorkstationName;
            this.Style = (Style)Resources["SmallTileStyle"];
            this.Tag = "0";
        }
    }
}

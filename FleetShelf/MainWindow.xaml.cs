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

namespace FleetShelf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
                img.Source = new BitmapImage(new Uri(@"/FleetShelf;component/Assets/Fleet_Logo.png", UriKind.Relative));
                (sender as Button).Content = img;
                (sender as Button).Margin = new Thickness(0, 0, 0, 0);
            }
        }

        private void UnFocusDock(object sender, RoutedEventArgs e)
        {
            (sender as DockPanel).Visibility = Visibility.Collapsed;
        }

        private void Inbox_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        private void ScreenCapture_Click(object sender, RoutedEventArgs e)
        {
            //
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
    }
}

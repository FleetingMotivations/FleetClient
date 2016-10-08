using FleetServer;
using System;
using System.Windows;
using System.Windows.Media;

using MahApps.Metro.Controls;

namespace FileAccept
{
    public partial class MainWindow : MetroWindow
    {
        public Boolean DidAccept { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            AcceptButton.Background = Brushes.LawnGreen;
            this.DidAccept = true;
            this.Close();
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            RejectButton.Background = Brushes.OrangeRed;
            this.DidAccept = false;
            this.Close();
        }

        public Boolean? ShowRequestDialog(FleetFileIdentifier ident)
        {
            this.Name.Text = "<Unknown>";
            this.Filename.Text = ident.FileName;
            this.Size.Text = ident.FileSize + "kb";

            return this.ShowDialog();
        }
    }
}

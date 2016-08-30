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

namespace FileInbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.storage = new FileStore();
            this.filesTable.ItemsSource = this.Storage.Files;            
        }

        public override void BeginInit()
        {
            base.BeginInit();
            
            this.RefreshFiles();
        }

        private FileStore storage;
        public FileStore Storage { get
            {
                return storage;
            }
        }

        public void RefreshFiles()
        {
            this.filesTable?.Items?.Refresh();
        }

        // Button Events

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            var index = this.filesTable.SelectedIndex;
            var item = this.storage.Files[index];
            try
            {
                Process.Start(item.Filepath);
            }
            catch(Exception ex)
            { }

        }

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.RefreshFiles();
        }
    }
}

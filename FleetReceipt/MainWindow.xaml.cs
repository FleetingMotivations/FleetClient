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

namespace FleetReceipt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            PopulateListBox();
        }

        private void PopulateListBox()
        {
            //TODO: Logic to populate the lists based on accept/reject results
            //In the mean time, dummy data is being used

            for (int i = 0; i < 10; i++) //To be replaced with a loop through all workstations that the file has been sent to
            {
                String workstation = "Workstation " + (i+1);
                if(i % 2 == 0) //To be replaced with "if delivered else rejected"
                {
                    DeliveredList.Items.Add(workstation);
                }
                else
                {
                    if(i % 3 == 0) //To be replaced with a message of rejection (cause behind the rejection)
                    {
                        workstation += " (Timeout)";
                    }
                    RejectedList.Items.Add(workstation);
                }
            }

        }
    }
}

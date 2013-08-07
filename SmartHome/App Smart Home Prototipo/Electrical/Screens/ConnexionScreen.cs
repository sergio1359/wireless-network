using ServiceLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App_Smart_Home_Prototipo.Electrical.Screens
{
    public partial class ConnexionScreen : Form
    {
        public ConnexionScreen()
        {
            InitializeComponent();

            comboBoxNode.Items.AddRange(Services.NodeService.GetNodes());

            UpdateForm();
        }

        private void UpdateForm()
        {
            listBoxFreeHomeDevices.Items.Clear();
            listBoxFreeHomeDevices.Items.AddRange(Services.HomeDeviceService.GetHomeDevices(false));

            listBoxHomeDevicesConnected.Items.Clear();
            listBoxHomeDevicesConnected.Items.AddRange(Services.HomeDeviceService.GetHomeDevices(true));

            listBoxCapableFreeConnector.Items.Clear();
        }
    }
}

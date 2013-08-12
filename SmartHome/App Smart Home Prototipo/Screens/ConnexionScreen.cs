using ServiceLayer;
using ServiceLayer.DTO;
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

            buttonLinkHomeDevice.Enabled = false;
            buttonUnilinkHomeDevice.Enabled = false;
        }

        private void LinkHomeDeviceToConnector(object sender, EventArgs e)
        {
            HomeDeviceDTO homeDev = (HomeDeviceDTO)listBoxFreeHomeDevices.SelectedItem;

            ConnectorDTO connector = (ConnectorDTO)listBoxCapableFreeConnector.SelectedItem;

            Services.NodeService.LinkHomeDevice(connector.Id, homeDev.Id);

            UpdateForm();
        }

        private void UnlinkHomeDevice(object sender, EventArgs e)
        {
            HomeDeviceDTO homeDev = (HomeDeviceDTO)listBoxHomeDevicesConnected.SelectedItem;

            Services.NodeService.UnlinkHomeDevice(homeDev.Id);

            UpdateForm();
        }

        private void UpdateCapableFreeConnector(object sender, EventArgs e)
        {
            HomeDeviceDTO homeDev = (HomeDeviceDTO)listBoxFreeHomeDevices.SelectedItem;

            ConnectorDTO connector = (ConnectorDTO)listBoxCapableFreeConnector.SelectedItem;

            listBoxCapableFreeConnector.Items.Clear();
            listBoxCapableFreeConnector.Items.AddRange(Services.NodeService.GetConnectorsCapable(homeDev.Id, connector.Id));

            buttonLinkHomeDevice.Enabled = true;
        }

        private void SelectNewHomeDevice(object sender, EventArgs e)
        {
            buttonUnilinkHomeDevice.Enabled = true;
        }

        private void SelectCapableFreeConnector(object sender, EventArgs e)
        {
            buttonLinkHomeDevice.Enabled = true;
        }
    }
}

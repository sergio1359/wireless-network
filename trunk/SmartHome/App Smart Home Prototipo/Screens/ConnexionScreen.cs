using ServiceLayer;
using ServiceLayer.DTO;
using System;
using System.Windows.Forms;
using System.Linq;

namespace App_Smart_Home_Prototipo.Electrical.Screens
{
    public partial class ConnexionScreen : Form
    {
        public ConnexionScreen()
        {
            InitializeComponent();

            comboBoxNode.Items.AddRange(Services.NodeService.GetNodes());

            //Products
            comboBoxListProduct.Items.AddRange(Services.HomeDeviceService.GetNameProducts());
            comboBoxNodeProduct.Items.AddRange(Services.NodeService.GetNodes());
            

            UpdateForm();
        }

        private void UpdateForm()
        {
            listBoxFreeHomeDevices.Items.Clear();
            listBoxFreeHomeDevices.Items.AddRange(Services.HomeDeviceService.GetHomeDevices(false).ToArray());

            listBoxHomeDevicesConnected.Items.Clear();
            listBoxHomeDevicesConnected.Items.AddRange(Services.HomeDeviceService.GetHomeDevices(true).ToArray());

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
            if (listBoxFreeHomeDevices.SelectedItem != null && comboBoxNode.SelectedItem != null)
            {
                HomeDeviceDTO homeDev = (HomeDeviceDTO)listBoxFreeHomeDevices.SelectedItem;

                NodeDTO node = (NodeDTO)this.comboBoxNode.SelectedItem;

                listBoxCapableFreeConnector.Items.Clear();
                listBoxCapableFreeConnector.Items.AddRange(Services.NodeService.GetConnectorsCapable(homeDev.Id, node.Id));

                buttonLinkHomeDevice.Enabled = true;
            }
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

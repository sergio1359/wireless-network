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

            comboBoxNode.Items.AddRange(Services.NodeService.GetNodes().ToArray());

            //Products
            comboBoxListProduct.Items.AddRange(Services.HomeDeviceService.GetNameProducts());
            comboBoxNodeProduct.Items.AddRange(Services.NodeService.GetNodes().ToArray());
            

            UpdateForm();
        }

        private void UpdateForm()
        {
            listBoxFreeHomeDevices.Items.Clear();
            listBoxFreeHomeDevices.Items.AddRange(Services.HomeDeviceService.GetAllHomeDevices(false).ToArray());

            listBoxHomeDevicesConnected.Items.Clear();
            listBoxHomeDevicesConnected.Items.AddRange(Services.HomeDeviceService.GetAllHomeDevices(true).ToArray());

            listBoxCapableFreeConnector.Items.Clear();

            buttonLinkHomeDevice.Enabled = false;
            buttonUnilinkHomeDevice.Enabled = false;
        }

        private void LinkHomeDeviceToConnector(object sender, EventArgs e)
        {
            HomeDeviceDTO homeDev = (HomeDeviceDTO)listBoxFreeHomeDevices.SelectedItem;

            ConnectorDTO connector = (ConnectorDTO)listBoxCapableFreeConnector.SelectedItem;

            Services.HomeDeviceService.LinkHomeDevice(connector.Id, homeDev.Id);

            UpdateForm();
        }

        private void UnlinkHomeDevice(object sender, EventArgs e)
        {
            HomeDeviceDTO homeDev = (HomeDeviceDTO)listBoxHomeDevicesConnected.SelectedItem;

            Services.HomeDeviceService.UnlinkHomeDevice(homeDev.Id);

            UpdateForm();
        }

        private void UpdateCapableFreeConnector(object sender, EventArgs e)
        {
            if (listBoxFreeHomeDevices.SelectedItem != null && comboBoxNode.SelectedItem != null)
            {
                HomeDeviceDTO homeDev = (HomeDeviceDTO)listBoxFreeHomeDevices.SelectedItem;

                NodeDTO node = (NodeDTO)this.comboBoxNode.SelectedItem;

                listBoxCapableFreeConnector.Items.Clear();
                listBoxCapableFreeConnector.Items.AddRange(Services.NodeService.GetConnectorsCapable(homeDev.Id, node.Id).ToArray());

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

        private void ConnectorProductAvailable(object sender, EventArgs e)
        {
            if (comboBoxNodeProduct.SelectedItem != null && comboBoxListProduct.SelectedItem != null)
            {
                string product = (string)comboBoxListProduct.SelectedItem;

                NodeDTO node = (NodeDTO)comboBoxNodeProduct.SelectedItem;

                listBoxConnectorsAvailable.Items.Clear();
                listBoxConnectorsAvailable.Items.AddRange(Services.HomeDeviceService.GetConnectorsCapableProduct(node.Id, product).ToArray());

                buttonLinkProduct.Enabled = true;
            }
        }
    }
}

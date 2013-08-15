using ServiceLayer;
using ServiceLayer.DTO;
using System;
using System.Windows.Forms;

namespace App_Smart_Home_Prototipo.Electrical.Screens
{
    public partial class NodeScreen : Form
    {

        public NodeScreen()
        {
            InitializeComponent();

            UpdateNodes();
        }

        private void UpdateNodes()
        {
            listBoxMACs.Items.Clear();
            listBoxMACs.Items.AddRange(Services.NodeService.GetPendingNodes());

            listBoxNodes.Items.Clear();
            listBoxNodes.Items.AddRange(Services.NodeService.GetNodes());
        }


        private async void AcceptNode(object sender, EventArgs e)
        {
            //TODO GENERATE A MOCK NODE
            if(listBoxMACs.SelectedIndex >= 0)
            {
                await Services.NodeService.AllowPendingNode((string)listBoxMACs.SelectedItem);
                UpdateNodes();
            }
        }

        private void RemoveNode(object sender, EventArgs e)
        {
            int id = ((NodeDTO)listBoxNodes.SelectedItem).Id;
            Services.HomeService.UnlinkNode(id);

            UpdateNodes();
        }

        private void SelectNode(object sender, EventArgs e)
        {
            //update textboxs
            NodeDTO node = (NodeDTO)listBoxNodes.SelectedItem;
            textBoxNameNode.Text = node.Name;
            textBoxBaseNode.Text = node.Base;
            textBoxAddressNode.Text = node.Address.ToString();
            textBoxShieldNode.Text = node.Shield;

            //update connectors
            listBoxConnectors.Items.Clear();
            listBoxConnectors.Items.AddRange(Services.NodeService.GetConnectors(node.Id));
        }

        private void ChangeNodeConfiguration(object sender, EventArgs e)
        {
            if (listBoxNodes.SelectedIndex >= 0)
            {
                NodeDTO node = (NodeDTO)listBoxNodes.SelectedItem;

                Services.NodeService.SetNameNode(node.Id, textBoxNameNode.Text);

                ushort newAddress = 0;
                if (ushort.TryParse(textBoxAddressNode.Text, out newAddress))
                    Services.NodeService.SetAddressNode(node.Id, newAddress);

                UpdateNodes();
            }
        }



    }
}

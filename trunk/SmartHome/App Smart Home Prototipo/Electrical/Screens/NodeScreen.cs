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
    public partial class NodeScreen : Form
    {

        public NodeScreen()
        {
            InitializeComponent();

            //listBoxMACs.Items.AddRange(Services.HomeService.GetPendingNodes());

            listBoxNodes.Items.AddRange(Services.NodeService.GetNodes());
        }

        private void RemoveNode(object sender, EventArgs e)
        {
            int id = ((NodeDTO)listBoxNodes.SelectedItem).Id;
            Services.HomeService.UnlinkNode(id);
        }

        private void ChangeNode(object sender, EventArgs e)
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

        private void ChangeNodeName(object sender, EventArgs e)
        {
            NodeDTO node = (NodeDTO)listBoxNodes.SelectedItem;

            Services.NodeService.SetNameNode(node.Id, textBoxNameNode.Text);
        }

        private void ChangeAddress(object sender, EventArgs e)
        {
            NodeDTO node = (NodeDTO)listBoxNodes.SelectedItem;

            ushort newAddress = 0;
            if (ushort.TryParse(textBoxAddressNode.Text, out newAddress))
                Services.NodeService.SetAddressNode(node.Id, newAddress);
        }


    }
}

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
    public partial class NodeScreen : Form
    {


        
        public NodeScreen()
        {
            InitializeComponent();

            comboBoxNewBase.Items.AddRange(NodeService.GetTypeBases());
            comboBoxNewShield.Items.AddRange(NodeService.GetTypeShields());

            listBoxNodes.Items.AddRange(NodeService.GetNodes().Keys.ToArray());
        }
    }
}

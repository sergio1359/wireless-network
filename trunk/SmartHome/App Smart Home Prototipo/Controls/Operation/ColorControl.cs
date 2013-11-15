using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App_Smart_Home_Prototipo.Controls
{
    public partial class ColorControl : UserControl
    {
        public string NameProperty
        {
            set
            {
                label1.Text = value;
            }
        }

        public Color Value { get; set; }

        public ColorControl()
        {
            InitializeComponent();
        }

        private void ChangeColor(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Value = colorDialog1.Color;
                label2.Text = colorDialog1.Color.ToString();
                label2.ForeColor = colorDialog1.Color;
            }
        }



    }
}

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
    public partial class ByteControl : UserControl
    {
        public string NameProperty
        {
            set
            {
                label1.Text = value;
            }
        }

        public byte Value { get; set; }

        public ByteControl()
        {
            InitializeComponent();
        }

        private void SetNewValue(object sender, EventArgs e)
        {
            Value = (byte)trackBar1.Value;
            label2.Text = trackBar1.Value.ToString();
        }
    }
}

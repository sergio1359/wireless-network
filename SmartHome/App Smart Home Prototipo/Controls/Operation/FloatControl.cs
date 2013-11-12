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
    public partial class FloatControl : UserControl
    {
        public string NameProperty
        {
            set
            {
                label1.Text = value;
            }
        }

        public float Value { get; set; }

        public FloatControl()
        {
            InitializeComponent();
        }

        private void SetNewValue(object sender, EventArgs e)
        {
            Value = (float)trackBar1.Value/100;
            label2.Text = trackBar1.Value + "%";
        }



    }
}

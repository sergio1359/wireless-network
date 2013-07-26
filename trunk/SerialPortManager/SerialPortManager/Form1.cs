using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialPortManager
{
    public partial class Form1 : Form
    {
        SerialManager manager = new SerialManager();

        public Form1()
        {
            InitializeComponent();
            manager.NodeCollectionChanged += (s,o) =>
                {
                    this.listBox1.Items.Clear();
                    this.listBox1.Items.AddRange(manager.GetAllConections().Select(c => string.Format("{0} \t 0x{1:X4} \t {2}", c.PortName, c.NodeAddress, c.NodeType)).ToArray());
                };
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SerialPortManager.ConnectionManager;
using SmartHome.Comunications;
using SmartHome.Comunications.Messages;

namespace RawOperationSender
{
    public partial class Form1 : Form
    {
        CommunicationManager man = new CommunicationManager();
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var operation = new OperationMessage()
            {
                DestinationAddress = 0x4004,
                OpCode = OperationMessage.OPCodes.MacRead,
            };

            OutputHeader outputMessage = new OutputHeader(1f)
            {
                SecurityEnabled = true,
                RoutingEnabled = true,
                EndPoint = 1,
                Retries = 3,
                Content = operation
            };

            for (int i = 0; i < 8; i++)
            {
                Task.Factory.StartNew(async () =>
                    {
                        Debug.WriteLine((await man.SendMessage(outputMessage)).ToString() + DateTime.Now.Ticks);
                    });
            }
        }
    }
}

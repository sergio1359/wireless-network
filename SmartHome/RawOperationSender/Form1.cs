using SerialPortManager.ConnectionManager;
using SmartHome.Communications.Modules;
using SmartHome.Communications;
using SmartHome.Communications.Messages;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataLayer.Entities;
using SmartHome.Communications.Modules.Network;
using SmartHome.Communications.Modules.Config;
using DataLayer;
using System.Collections.Generic;

namespace RawOperationSender
{
    public partial class Form1 : Form
    {
        NetworkJoin joinMod;
        public Form1()
        {
            InitializeComponent();

            this.joinMod = CommunicationManager.Instance.FindModule<NetworkJoin>();
            joinMod.NetworkJoinReceived += joinMod_NetworkJoinReceived;
            joinMod.NodeJoined += joinMod_NetworkJoinReceived;

            /*var node = Repositories.NodeRespository.GetById(1);
            CommunicationManager.Instance.FindModule<ConfigModule>().SendConfiguration(node);*/
        }

        void joinMod_NetworkJoinReceived(object sender, string e)
        {
            this.UIThread(() =>
            {
                this.listBox1.Items.Clear();
                this.listBox1.Items.AddRange(this.joinMod.PendingNodes.ToArray());
            });
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var operation = OperationMessage.MACRead(0x4004);

            float priotity = 0;
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < 8; i++)
            {
                var task = new Task(async () =>
                    {
                        Debug.WriteLine("Message " + priotity + " enqueued");
                        OutputHeader outputMessage = new OutputHeader(priotity)
                        {
                            SecurityEnabled = true,
                            RoutingEnabled = true,
                            EndPoint = 1,
                            Retries = 3,
                            Content = operation
                        };
                        priotity += 0.1f;
                        Debug.WriteLine(outputMessage.Priority + " Response: " + (await CommunicationManager.Instance.SendMessage(outputMessage)).ToString() + " " + DateTime.Now.Millisecond);
                    });

                task.Start();

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }

        private void UIThread(Action code)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(code);
            }
            else
            {
                code.Invoke();
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex >= 0)
            {
                ushort newAddress = Convert.ToUInt16(this.textBox1.Text, 16);

                await joinMod.AcceptNode((string)listBox1.SelectedItem, newAddress, new Security() { SecurityKey = "TestSecurityKey0" });
            }
        }
    }
}

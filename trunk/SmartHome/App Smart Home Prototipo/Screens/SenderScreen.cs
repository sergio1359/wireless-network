using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using App_Smart_Home_Prototipo.Controls;
using ServiceLayer;
using ServiceLayer.DTO;

namespace App_Smart_Home_Prototipo.Screens
{
    public partial class SenderScreen : Form
    {
        public SenderScreen()
        {
            InitializeComponent();
            RefreshDevices();

            Services.HomeDeviceService.StatusChanged += HomeDeviceService_StatusChanged;
        }

        void HomeDeviceService_StatusChanged(object sender, HomeDeviceDTO hd)
        {
            this.UIThread(() =>
                {
                    this.RefreshDevices();
                });
        }

        private void RefreshDevices()
        {
            var selectedItem = this.listBoxHomeDevices.SelectedItem as HomeDeviceDTO;
            int previusHDId = selectedItem != null ? selectedItem.Id : -1;

            this.listBoxHomeDevices.SelectedIndex = -1;
            this.listBoxHomeDevices.Items.Clear();
            this.listBoxHomeDevices.Items.AddRange(Services.HomeDeviceService.GetAllHomeDevices(true).ToArray());

            if (previusHDId != -1 && this.listBoxHomeDevices.Items.Count > 0)
            {
                this.listBoxHomeDevices.SelectedItem = this.listBoxHomeDevices.Items.OfType<HomeDeviceDTO>().FirstOrDefault(hd => hd.Id == previusHDId);
            }
        }


        private void SetStateHomeDevice(object sender, EventArgs e)
        {
            if (this.listBoxHomeDevices.SelectedItem != null)
            {
                HomeDeviceDTO updateHomeDevice = (HomeDeviceDTO)this.listBoxHomeDevices.SelectedItem;

                // UPDATE STATUS
                this.flowLayoutPanelState.Controls.Clear();

                foreach (var stateVar in updateHomeDevice.State)
                {
                    this.flowLayoutPanelState.Controls.Add(new Label()
                    {
                        Text = stateVar.ToString()
                    });
                }

                //GET OPERATIONS
                this.operationList.Items.Clear();
                this.operationList.Items.AddRange(Services.OperationService.GetExecutableHomeDeviceNameOperations(updateHomeDevice.Id));
            }
        }

        private void SetSenderOperationForm(object sender, EventArgs e)
        {
            if (this.operationList.SelectedItem != null && listBoxHomeDevices.SelectedItem != null)
            {
                HomeDeviceDTO homeDevice = (HomeDeviceDTO)listBoxHomeDevices.SelectedItem;

                string operationName = (string)this.operationList.SelectedItem;

                OperationDefinitionDTO operation = Services.OperationService.GetDefinitionOperation(homeDevice.Id, operationName);

                tableLayoutPanel3.Controls.Clear();
                for (int i = 0; i < operation.Args.Count(); i++)
                {
                    SetControl(operation.Args[i], i);
                }

            }
        }

        private void SetControl(ParamDTO param, int position)
        {
            dynamic control = new Control();
            switch (param.Type)
            {
                case "System.Byte":
                    control = new ByteControl();
                    control.NameProperty = param.Name;
                    break;
                case "System.Single":
                    control = new FloatControl();
                    control.NameProperty = param.Name;
                    break;
                case "System.Drawing.Color":
                    control = new ColorControl();
                    control.NameProperty = param.Name;
                    break;
            }

            this.tableLayoutPanel3.Controls.Add(control, 0, position - 1);
        }

        private void SendOperation(object sender, EventArgs e)
        {
            if (this.operationList.SelectedItem != null && this.listBoxHomeDevices.SelectedItem != null)
            {
                HomeDeviceDTO homeDevice = (HomeDeviceDTO)this.listBoxHomeDevices.SelectedItem;
                string nameOperation = (string)operationList.SelectedItem;

                object[] args = new object[this.tableLayoutPanel3.Controls.Count];

                for (int i = 0; i < this.tableLayoutPanel3.Controls.Count; i++)
                {
                    args[i] = GetValue(this.tableLayoutPanel3.Controls[i]);
                }

                Services.OperationService.ExecuteOperation(homeDevice.Id, nameOperation, args);
            }
        }

        private object GetValue(Control control)
        {
            if (control is FloatControl)
                return (control as FloatControl).Value;
            if (control is ByteControl)
                return (control as ByteControl).Value;
            if (control is ColorControl)
                return (control as ColorControl).Value;

            throw new Exception("Not Implemented or problem DEBUG");
        }

        private void RefreshDevices(object sender, EventArgs e)
        {
            RefreshDevices();
        }
    }
}

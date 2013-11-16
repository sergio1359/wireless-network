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
        }

        private void RefreshDevices()
        {
            this.listBoxHomeDevices.SelectedIndex = -1;
            this.listBoxHomeDevices.Items.Clear();
            this.listBoxHomeDevices.Items.AddRange(Services.HomeDeviceService.GetAllHomeDevices(true).ToArray());

            if (this.listBoxHomeDevices.Items.Count > 0)
                this.listBoxHomeDevices.SelectedIndex = 0;
        }


        private void SetStateHomeDevice(object sender, EventArgs e)
        {
            if (this.listBoxHomeDevices.SelectedItem != null)
            {
                //GET HOME DEVICE
                HomeDeviceDTO updateHomeDevice = (HomeDeviceDTO)this.listBoxHomeDevices.SelectedItem;

                if (updateHomeDevice.State.Count() >= 1)
                    this.labelState1.Text = updateHomeDevice.State.ToList()[0].ToString();

                if (updateHomeDevice.State.Count() >= 2)
                    this.labelState1.Text = updateHomeDevice.State.ToList()[1].ToString();

                if (updateHomeDevice.State.Count() >= 3)
                    this.labelState1.Text = updateHomeDevice.State.ToList()[2].ToString();

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
                for (int i = 1; i < operation.Args.Count(); i++) //1 because we don't add the "this" expression
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
                HomeDeviceDTO homeDevice = (HomeDeviceDTO) this.listBoxHomeDevices.SelectedItem;
                string nameOperation = (string) operationList.SelectedItem;

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

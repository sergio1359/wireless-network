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
            listBoxHomeDevices.Items.Clear();
            listBoxHomeDevices.Items.AddRange(Services.HomeDeviceService.GetAllHomeDevices(true).ToArray());
        }


        private void SetStateHomeDevice(object sender, EventArgs e)
        {
            if (listBoxHomeDevices.SelectedItem != null)
            {
                //GET HOME DEVICE
                HomeDeviceDTO updateHomeDevice = (HomeDeviceDTO)listBoxHomeDevices.SelectedItem;

                if (updateHomeDevice.State.Count() >= 1)
                    labelState1.Text = updateHomeDevice.State.ToList()[0].ToString();

                if (updateHomeDevice.State.Count() >= 2)
                    labelState1.Text = updateHomeDevice.State.ToList()[1].ToString();

                if (updateHomeDevice.State.Count() >= 3)
                    labelState1.Text = updateHomeDevice.State.ToList()[2].ToString();

                //GET OPERATIONS
                operationList.Items.Clear();
                operationList.Items.AddRange(Services.OperationService.GetExecutableHomeDeviceNameOperations(updateHomeDevice.Id));
            }
        }

        private void SetSenderOperationForm(object sender, EventArgs e)
        {
            if (operationList.SelectedItem != null && listBoxHomeDevices.SelectedItem != null)
            {
                HomeDeviceDTO homeDevice = (HomeDeviceDTO)listBoxHomeDevices.SelectedItem;

                string operationName = (string)operationList.SelectedItem;

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
                case "byte":
                    control = new ByteControl();
                    control.NameProperty = param.Name;
                    break;
                case "float":
                    control = new FloatControl();
                    control.NameProperty = param.Name;
                    break;
                case "color":
                    control = new ColorControl();
                    control.NameProperty = param.Name;
                    break;
            }

            tableLayoutPanel3.Controls.Add(control, 0, position);
        }

        private void SendOperation(object sender, EventArgs e)
        {
            if (operationList.SelectedItem != null && listBoxHomeDevices.SelectedItem != null)
            {
                HomeDeviceDTO homeDevice = (HomeDeviceDTO) listBoxHomeDevices.SelectedItem;
                string nameOperation = (string) operationList.SelectedItem;

                object[] args = new object[tableLayoutPanel3.Controls.Count];

                for (int i = 0; i < tableLayoutPanel3.Controls.Count; i++)
                {
                    args[i] = GetValue(tableLayoutPanel3.Controls[i]);
                }

                Services.OperationService.ExecuteOperation(homeDevice.Id, nameOperation, args);
            }
        }

        private object GetValue(Control control)
        {
            if (control is FloatControl)
                return (control as FloatControl).Value;
            else if (control is ByteControl)
                return (control as ByteControl).Value;
            else if (control is ColorControl)
                return (control as ColorControl).Value;
            else
            {
                throw new Exception("DEBUG");
            }
        }

        private void RefreshDevices(object sender, EventArgs e)
        {
            RefreshDevices();
        }
    }
}

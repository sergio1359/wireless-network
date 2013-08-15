using ServiceLayer;
using ServiceLayer.DTO;
using System;
using System.Windows.Forms;
using System.Linq;

namespace App_Smart_Home_Prototipo.Administrator.Screens
{
    public partial class OperationScreen : Form
    {
        public OperationScreen()
        {
            InitializeComponent();

            var homeDevices = Services.HomeDeviceService.GetHomeDevices();

            listBoxHomeDevices.Items.AddRange(homeDevices.ToArray());

            comboBoxToHomeDevice.Items.AddRange(homeDevices.ToArray());
        }

        private void SelectHomeDevice(object sender, EventArgs e)
        {
            HomeDeviceDTO homeDev = (HomeDeviceDTO)listBoxHomeDevices.SelectedItem;

            textBoxNameHomeDevice.Text = homeDev.Name;
            textBoxTypeHomeDevice.Text = homeDev.Type;

            UpdateOperation();
        }

        private void UpdateOperation()
        {
            HomeDeviceDTO homeDev = (HomeDeviceDTO)listBoxHomeDevices.SelectedItem;

            listBoxOperations.Items.Clear();
            listBoxOperations.Items.AddRange(Services.OperationService.GetHomeDeviceOperation(homeDev.Id)); //OJO

            //Clear add operation
            textBoxArgs.Text = "";
            comboBoxOperation.Text = "";
            comboBoxToHomeDevice.Text = "";
        }

        private void RemoveOperation(object sender, EventArgs e)
        {
            OperationDTO operation = (OperationDTO)listBoxOperations.SelectedItem;

            Services.OperationService.RemoveOperation(operation.Id);

            UpdateOperation();
        }

        private void SelectOperation(object sender, EventArgs e)
        {
            OperationDTO operation = (OperationDTO)listBoxOperations.SelectedItem;

            textBoxNameOperation.Text = operation.Name;
            comboBoxToHomeDevice.Text = Services.HomeDeviceService.GetHomeDevices().First(hd => hd.Id == operation.IdHomeDevice).ToString();
            comboBoxOperation.Text = operation.NameOperation;
            textBoxArgs.Text = "hola mundo";

        }

        private void AddOperation(object sender, EventArgs e)
        {

        }

        private void EditOperation(object sender, EventArgs e)
        {

        }
    }
}

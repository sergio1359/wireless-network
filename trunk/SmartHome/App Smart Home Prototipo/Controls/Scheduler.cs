using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceLayer;
using ServiceLayer.DTO;

namespace App_Smart_Home_Prototipo.Controls
{
    public partial class Scheduler : UserControl
    {
        public Scheduler()
        {
            InitializeComponent();

            comboBoxToHomeDevice.Items.AddRange(Services.HomeDeviceService.GetHomeDevices().ToArray());

            UpdateScheduler();
        }

        private void UpdateScheduler()
        {
            listBoxOperation.Items.Clear();
            listBoxOperation.Items.AddRange(Services.OperationService.GetScheduler());
        }

        private void AddOperationScheduler(object sender, EventArgs e)
        {
            //WEEK DAYS
            byte weekDays = 0;

            weekDays |= (byte)(checkBox1.Checked ? 0x1 : 0);
            weekDays |= (byte)(checkBox2.Checked ? 0x2 : 0);
            weekDays |= (byte)(checkBox3.Checked ? 0x4 : 0);
            weekDays |= (byte)(checkBox4.Checked ? 0x8 : 0);
            weekDays |= (byte)(checkBox5.Checked ? 0x10 : 0);
            weekDays |= (byte)(checkBox6.Checked ? 0x20 : 0);
            weekDays |= (byte)(checkBox7.Checked ? 0x40 : 0);

            //TIME
            int hours = int.Parse(textBoxHours.Text);
            int minutes = int.Parse(textBoxMinutes.Text);
            int seconds = int.Parse(textBoxSeconds.Text);

            TimeSpan time = new TimeSpan(hours, minutes, seconds);

            int idHomeDevice = (comboBoxToHomeDevice.SelectedItem as HomeDeviceDTO).Id;

            Services.OperationService.AddScheduler(weekDays, time, textBoxNameOperation.Text, idHomeDevice, comboBoxOperation.Text);

            UpdateScheduler();
        }

        private void DeleteScheduler(object sender, EventArgs e)
        {
            int idTimeOperation = (listBoxOperation.SelectedItem as TimeOperationDTO).Id;

            Services.OperationService.RemoveTimeOperation(idTimeOperation);

            UpdateScheduler();
        }

        private void RefreshOperations(object sender, EventArgs e)
        {
            int idHomeDevice = (comboBoxToHomeDevice.SelectedItem as HomeDeviceDTO).Id;

            comboBoxOperation.Items.Clear();

            comboBoxOperation.Items.AddRange( Services.OperationService.GetHomeDeviceOperation(idHomeDevice));
        }


    }
}

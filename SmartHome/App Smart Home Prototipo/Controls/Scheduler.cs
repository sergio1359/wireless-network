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

namespace App_Smart_Home_Prototipo.Controls
{
    public partial class Scheduler : UserControl
    {
        public Scheduler()
        {
            InitializeComponent();

            comboBoxToHomeDevice.Items.AddRange(Services.HomeDeviceService.GetHomeDevices().ToArray());

            listBoxOperation.Items.Clear();
            listBoxOperation.Items.AddRange(Services.OperationService.GetScheduler());
        }
    }
}

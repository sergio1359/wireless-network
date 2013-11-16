using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App_Smart_Home_Prototipo.Screens
{
    public static class ScreenHelper
    {
        public static void UIThread(this Form form, Action code)
        {
            if (form.InvokeRequired)
            {
                form.BeginInvoke(code);
            }
            else
            {
                code.Invoke();
            }
        }
    }
}

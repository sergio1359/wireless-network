using App_Smart_Home_Prototipo.Administrator.Screens;
using App_Smart_Home_Prototipo.Electrical.Screens;
using System;
using System.Windows.Forms;
using App_Smart_Home_Prototipo.Screens;
using ServiceLayer;
using System.Threading.Tasks;

namespace App_Smart_Home_Prototipo
{
    public partial class MainScreen : Form
    {
        public MainScreen()
        {
            InitializeComponent();

            Services.NodeService.NetworkJoinRequestReceived += NodeService_NetworkJoinRequestReceived;
        }

        async void NodeService_NetworkJoinRequestReceived(object sender, string mac)
        {
            var dialogResult = MessageBox.Show("A node with MAC " + mac + " request a network join. Allow?", "Network Join Request Received", MessageBoxButtons.YesNo);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                if (!await Services.NodeService.AllowPendingNode(mac))
                {
                    MessageBox.Show("A problem occurs trying to allow node " + mac);
                }
            }
        }

        #region ToolBar
        private void OpenNodeScreen(object sender, EventArgs e)
        {
            NodeScreen node = new NodeScreen();
            node.ShowDialog();
            RefreshContentAndResetMainScreen();
        }

        private void OpenHomeDeviceScreen(object sender, EventArgs e)
        {
            HomeDeviceScreen homeD = new HomeDeviceScreen();
            homeD.ShowDialog();
            RefreshContentAndResetMainScreen();
        }

        private void OpenConexionScreen(object sender, EventArgs e)
        {
            ConnexionScreen conS = new ConnexionScreen();
            conS.ShowDialog();
            RefreshContentAndResetMainScreen();
        }

        private void OpenHomeScreen(object sender, EventArgs e)
        {
            ZonaScreen zonaS = new ZonaScreen();
            zonaS.ShowDialog();
            RefreshContentAndResetMainScreen();
        }

        private void OpenConfigOperation(object sender, EventArgs e)
        {
            OperationScreen opers = new OperationScreen();
            opers.ShowDialog();
            RefreshContentAndResetMainScreen();
        }

        private void GenerateEEPROM(object sender, EventArgs e)
        {

        }

        private void OpenSenderOperation(object sender, EventArgs e)
        {
            SenderScreen form = new SenderScreen();
            form.ShowDialog();
        }
        #endregion


        #region Mapa-MainScreen
        /// <summary>
        /// Actualiza el contenido de la pantalla principal y reinicia la pestaña del tabbox
        /// </summary>
        private void RefreshContentAndResetMainScreen()
        {
            //throw new NotImplementedException();
        }


        #endregion


    }
}

using App_Smart_Home_Prototipo.Administrator.Screens;
using App_Smart_Home_Prototipo.Electrical.Screens;
using System;
using System.Windows.Forms;
using ServiceLayer;
using System.Threading.Tasks;

namespace App_Smart_Home_Prototipo
{
    public partial class MainScreen : Form
    {
        public MainScreen()
        {
            InitializeComponent();
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

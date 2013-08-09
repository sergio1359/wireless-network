using ServiceLayer;
using ServiceLayer.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App_Smart_Home_Prototipo.Electrical.Screens
{
    public partial class ZonaScreen : Form
    {
        public ZonaScreen()
        {
            InitializeComponent();

            UpdateZones();
        }

        private void UpdateZones()
        {
            listBoxZones.Items.AddRange(Services.HomeService.GetZones());

        }

        private void AddNewZone(object sender, EventArgs e)
        {
            Services.HomeService.AddZone(textBoxNewNameZona.Text);
            UpdateZones();
        }

        private void RemoveZones(object sender, EventArgs e)
        {
            PlaceDTO zone = (PlaceDTO)listBoxZones.SelectedItem;
            Services.HomeService.RemoveZone(zone.Id);
            UpdateZones();
        }

        private void LoadZone(object sender, EventArgs e)
        {
            PlaceDTO zone = (PlaceDTO)listBoxZones.SelectedItem;
            textBoxNameNode.Text = zone.Name;

            listBoxViews.Items.Clear();
            listBoxViews.Items.AddRange(Services.HomeService.GetViews(zone.Id));
        }

        private void ChangeZoneName(object sender, EventArgs e)
        {
            PlaceDTO zone = (PlaceDTO)listBoxZones.SelectedItem;

            Services.HomeService.SetNameZone(zone.Id, textBoxNameNode.Text);
        }

        private void AddView(object sender, EventArgs e)
        {
            PlaceDTO zone = (PlaceDTO)listBoxZones.SelectedItem;
            Services.HomeService.AddView(zone.Id, textBoxNewViewName.Text);
            LoadZone(this, null);
        }

        private void DeleteView(object sender, EventArgs e)
        {
            PlaceDTO view = (PlaceDTO)listBoxViews.SelectedItem;

            textBoxNameView.Text = view.Name;

            //TODO PRINT PHOTO

            LoadZone(this, null);
        }
    }
}

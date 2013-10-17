using ServiceLayer;
using ServiceLayer.DTO;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace App_Smart_Home_Prototipo.Electrical.Screens
{
    public partial class ZonaScreen : Form
    {
        public ZonaScreen()
        {
            InitializeComponent();

            UpdateScreen();
        }

        private void UpdateScreen()
        {
            textBoxHomeName.Text = Services.HomeService.GetHomeName();

            listBoxZones.Items.Clear();
            listBoxZones.Items.AddRange(Services.ZoneService.GetZones().ToArray());
        }

        private void AddNewZone(object sender, EventArgs e)
        {
            Services.ZoneService.AddZone(textBoxNewNameZona.Text);
            UpdateScreen();
        }

        private void RemoveZones(object sender, EventArgs e)
        {
            if (listBoxZones.SelectedItem != null)
            {
                ZoneDTO zone = (ZoneDTO)listBoxZones.SelectedItem;
                Services.ZoneService.RemoveZone(zone.Id);
                UpdateScreen();
            }
        }

        private void LoadZone(object sender, EventArgs e)
        {
            LoadZone();
        }

        private void LoadZone()
        {
            if (listBoxZones.SelectedItem != null)
            {
                ZoneDTO zone = (ZoneDTO)listBoxZones.SelectedItem;
                textBoxNameNode.Text = zone.Name;

                if (zone.MainView.ImageMap.Length != 0)
                {
                    MemoryStream ms = new MemoryStream(Services.ViewService.GetViewImage(zone.MainView.Id));
                    pictureBox.Image = Image.FromStream(ms);
                }

                listBoxViews.Items.Clear();
                listBoxViews.Items.AddRange(Services.ViewService.GetViews(zone.Id).ToArray());
            }
        }

        private void ChangeZoneName(object sender, EventArgs e)
        {
            if (listBoxZones.SelectedItem != null)
            {
                ZoneDTO zone = (ZoneDTO)listBoxZones.SelectedItem;

                Services.ViewService.SetNameView(zone.MainView.Id, textBoxNameNode.Text);

                UpdateScreen();
            }
        }

        private void AddView(object sender, EventArgs e)
        {
            if (listBoxZones.SelectedItem != null)
            {
                ZoneDTO zone = (ZoneDTO)listBoxZones.SelectedItem;
                Services.ViewService.AddView(zone.Id, textBoxNewViewName.Text);
                LoadZone();
            }
        }

        private void DeleteView(object sender, EventArgs e)
        {
            if (listBoxViews.SelectedItem != null)
            {
                ViewDTO view = (ViewDTO)listBoxViews.SelectedItem;

                Services.ViewService.RemoveView(view.Id);

                LoadZone();
            }
        }

        private void ChangeHomeName(object sender, EventArgs e)
        {
            Services.HomeService.SetHomeName(textBoxHomeName.Text);

            UpdateScreen();
        }

        private void LoadView(object sender, EventArgs e)
        {
            if (listBoxViews.SelectedItem != null)
            {
                ViewDTO view = (ViewDTO)listBoxViews.SelectedItem;

                textBoxNameView.Text = view.Name;

                if (view.ImageMap.Length != 0)
                {
                    MemoryStream ms = new MemoryStream(Services.ViewService.GetViewImage(view.Id));
                    pictureBox.Image = Image.FromStream(ms);
                }
            }
        }

        private void ChangeImageZone(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var zone = (ZoneDTO)listBoxZones.SelectedItem;

                Image image = Image.FromFile(openFileDialog.FileName);

                MemoryStream ms = new MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                Services.ViewService.SetViewImage(zone.MainView.Id, ms.ToArray());

                LoadZone();
            }
        }

        private void ChangeViewImage(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var view = (ZoneDTO)listBoxViews.SelectedItem;

                Image image = Image.FromFile(openFileDialog.FileName);

                MemoryStream ms = new MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                LoadZone();
            }
        }

        private void ChangeViewName(object sender, EventArgs e)
        {
            if (listBoxViews.SelectedItem != null)
            {
                ViewDTO view = (ViewDTO)listBoxViews.SelectedItem;

                Services.ViewService.SetNameView(view.Id, textBoxNameView.Text);

                LoadZone();
            }
        }
    }
}

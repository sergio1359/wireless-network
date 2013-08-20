namespace App_Smart_Home_Prototipo
{
    partial class MainScreen
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainScreen));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateHomeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.configurarNodosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurarHomeDevicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurarConexionesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurarHogarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.configuracionDeOperacionesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.comboBoxMapaVistas = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxMapaZonas = new System.Windows.Forms.ComboBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.scheduler1 = new App_Smart_Home_Prototipo.Controls.Scheduler();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripDropDownButton2,
            this.toolStripDropDownButton3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(832, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.updateHomeToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(61, 22);
            this.toolStripDropDownButton1.Text = "Archivo";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.openToolStripMenuItem.Text = "Connect";
            // 
            // updateHomeToolStripMenuItem
            // 
            this.updateHomeToolStripMenuItem.Name = "updateHomeToolStripMenuItem";
            this.updateHomeToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.updateHomeToolStripMenuItem.Text = "Generate EEPROMs";
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configurarNodosToolStripMenuItem,
            this.configurarHomeDevicesToolStripMenuItem,
            this.configurarConexionesToolStripMenuItem,
            this.configurarHogarToolStripMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(110, 22);
            this.toolStripDropDownButton2.Text = "Técnico Eléctrico";
            // 
            // configurarNodosToolStripMenuItem
            // 
            this.configurarNodosToolStripMenuItem.Name = "configurarNodosToolStripMenuItem";
            this.configurarNodosToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.configurarNodosToolStripMenuItem.Text = "Configurar Nodos";
            this.configurarNodosToolStripMenuItem.Click += new System.EventHandler(this.OpenNodeScreen);
            // 
            // configurarHomeDevicesToolStripMenuItem
            // 
            this.configurarHomeDevicesToolStripMenuItem.Name = "configurarHomeDevicesToolStripMenuItem";
            this.configurarHomeDevicesToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.configurarHomeDevicesToolStripMenuItem.Text = "Configurar HomeDevices";
            this.configurarHomeDevicesToolStripMenuItem.Click += new System.EventHandler(this.OpenHomeDeviceScreen);
            // 
            // configurarConexionesToolStripMenuItem
            // 
            this.configurarConexionesToolStripMenuItem.Name = "configurarConexionesToolStripMenuItem";
            this.configurarConexionesToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.configurarConexionesToolStripMenuItem.Text = "Configurar Conexiones";
            this.configurarConexionesToolStripMenuItem.Click += new System.EventHandler(this.OpenConexionScreen);
            // 
            // configurarHogarToolStripMenuItem
            // 
            this.configurarHogarToolStripMenuItem.Name = "configurarHogarToolStripMenuItem";
            this.configurarHogarToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.configurarHogarToolStripMenuItem.Text = "Configurar Hogar";
            this.configurarHogarToolStripMenuItem.Click += new System.EventHandler(this.OpenHomeScreen);
            // 
            // toolStripDropDownButton3
            // 
            this.toolStripDropDownButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configuracionDeOperacionesToolStripMenuItem});
            this.toolStripDropDownButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton3.Image")));
            this.toolStripDropDownButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton3.Name = "toolStripDropDownButton3";
            this.toolStripDropDownButton3.Size = new System.Drawing.Size(96, 22);
            this.toolStripDropDownButton3.Text = "Administrador";
            // 
            // configuracionDeOperacionesToolStripMenuItem
            // 
            this.configuracionDeOperacionesToolStripMenuItem.Name = "configuracionDeOperacionesToolStripMenuItem";
            this.configuracionDeOperacionesToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.configuracionDeOperacionesToolStripMenuItem.Text = "Configuracion de Operaciones";
            this.configuracionDeOperacionesToolStripMenuItem.Click += new System.EventHandler(this.OpenConfigOperation);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(12, 28);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(808, 476);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(800, 450);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Mapa";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.comboBoxMapaVistas, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxMapaZonas, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.72727F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 52.27273F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(794, 44);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // comboBoxMapaVistas
            // 
            this.comboBoxMapaVistas.FormattingEnabled = true;
            this.comboBoxMapaVistas.Location = new System.Drawing.Point(400, 23);
            this.comboBoxMapaVistas.Name = "comboBoxMapaVistas";
            this.comboBoxMapaVistas.Size = new System.Drawing.Size(391, 21);
            this.comboBoxMapaVistas.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Zona";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(400, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Vista";
            // 
            // comboBoxMapaZonas
            // 
            this.comboBoxMapaZonas.FormattingEnabled = true;
            this.comboBoxMapaZonas.Location = new System.Drawing.Point(3, 23);
            this.comboBoxMapaZonas.Name = "comboBoxMapaZonas";
            this.comboBoxMapaZonas.Size = new System.Drawing.Size(391, 21);
            this.comboBoxMapaZonas.TabIndex = 1;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.scheduler1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(800, 450);
            this.tabPage4.TabIndex = 2;
            this.tabPage4.Text = "Scheduler";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // scheduler1
            // 
            this.scheduler1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scheduler1.Location = new System.Drawing.Point(3, 3);
            this.scheduler1.Name = "scheduler1";
            this.scheduler1.Size = new System.Drawing.Size(797, 444);
            this.scheduler1.TabIndex = 0;
            // 
            // MainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 516);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MainScreen";
            this.Text = "SmartHome Proto 1";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateHomeToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem configurarNodosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configurarHomeDevicesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configurarConexionesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configurarHogarToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton3;
        private System.Windows.Forms.ToolStripMenuItem configuracionDeOperacionesToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox comboBoxMapaVistas;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxMapaZonas;
        private Controls.Scheduler scheduler1;
    }
}


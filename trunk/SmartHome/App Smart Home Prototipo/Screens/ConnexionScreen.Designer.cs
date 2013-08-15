namespace App_Smart_Home_Prototipo.Electrical.Screens
{
    partial class ConnexionScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonLinkHomeDevice = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.listBoxFreeHomeDevices = new System.Windows.Forms.ListBox();
            this.listBoxCapableFreeConnector = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxNode = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonUnilinkHomeDevice = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.listBoxHomeDevicesConnected = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.listBoxUnlinkProducts = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.listBoxConnectorsAvailable = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 43.3095F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.86415F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.9285F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 2, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(979, 433);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.buttonLinkHomeDevice);
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(417, 427);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "LINK HOME_DEVICE -> CONNECTOR";
            // 
            // buttonLinkHomeDevice
            // 
            this.buttonLinkHomeDevice.Location = new System.Drawing.Point(0, 372);
            this.buttonLinkHomeDevice.Name = "buttonLinkHomeDevice";
            this.buttonLinkHomeDevice.Size = new System.Drawing.Size(415, 55);
            this.buttonLinkHomeDevice.TabIndex = 0;
            this.buttonLinkHomeDevice.Text = "LINK HOME DEVICE TO CONNECTOR";
            this.buttonLinkHomeDevice.UseVisualStyleBackColor = true;
            this.buttonLinkHomeDevice.Click += new System.EventHandler(this.LinkHomeDeviceToConnector);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.listBoxFreeHomeDevices, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.listBoxCapableFreeConnector, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.75309F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80.24691F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(407, 347);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "FREE HOME DEVICES";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listBoxFreeHomeDevices
            // 
            this.listBoxFreeHomeDevices.FormattingEnabled = true;
            this.listBoxFreeHomeDevices.Location = new System.Drawing.Point(3, 71);
            this.listBoxFreeHomeDevices.Name = "listBoxFreeHomeDevices";
            this.listBoxFreeHomeDevices.Size = new System.Drawing.Size(197, 264);
            this.listBoxFreeHomeDevices.TabIndex = 0;
            // 
            // listBoxCapableFreeConnector
            // 
            this.listBoxCapableFreeConnector.FormattingEnabled = true;
            this.listBoxCapableFreeConnector.Location = new System.Drawing.Point(206, 71);
            this.listBoxCapableFreeConnector.Name = "listBoxCapableFreeConnector";
            this.listBoxCapableFreeConnector.Size = new System.Drawing.Size(198, 264);
            this.listBoxCapableFreeConnector.TabIndex = 0;
            this.listBoxCapableFreeConnector.SelectedValueChanged += new System.EventHandler(this.SelectCapableFreeConnector);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.comboBoxNode);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(206, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(198, 62);
            this.panel1.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(157, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "CAPABLE FREE CONNECTOR";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxNode
            // 
            this.comboBoxNode.FormattingEnabled = true;
            this.comboBoxNode.Location = new System.Drawing.Point(48, 3);
            this.comboBoxNode.Name = "comboBoxNode";
            this.comboBoxNode.Size = new System.Drawing.Size(147, 21);
            this.comboBoxNode.TabIndex = 1;
            this.comboBoxNode.SelectedValueChanged += new System.EventHandler(this.UpdateCapableFreeConnector);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "NODO";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.buttonUnilinkHomeDevice);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.listBoxHomeDevicesConnected);
            this.groupBox2.Location = new System.Drawing.Point(426, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(256, 427);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "UNLINK HOME_DEVICE";
            // 
            // buttonUnilinkHomeDevice
            // 
            this.buttonUnilinkHomeDevice.Location = new System.Drawing.Point(6, 372);
            this.buttonUnilinkHomeDevice.Name = "buttonUnilinkHomeDevice";
            this.buttonUnilinkHomeDevice.Size = new System.Drawing.Size(247, 55);
            this.buttonUnilinkHomeDevice.TabIndex = 0;
            this.buttonUnilinkHomeDevice.Text = "UNLINK HOMEDEVICE FROM CONNECTOR";
            this.buttonUnilinkHomeDevice.UseVisualStyleBackColor = true;
            this.buttonUnilinkHomeDevice.Click += new System.EventHandler(this.UnlinkHomeDevice);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(158, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "HOME DEVICES CONNECTED";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listBoxHomeDevicesConnected
            // 
            this.listBoxHomeDevicesConnected.FormattingEnabled = true;
            this.listBoxHomeDevicesConnected.Location = new System.Drawing.Point(6, 90);
            this.listBoxHomeDevicesConnected.Name = "listBoxHomeDevicesConnected";
            this.listBoxHomeDevicesConnected.Size = new System.Drawing.Size(247, 264);
            this.listBoxHomeDevicesConnected.TabIndex = 0;
            this.listBoxHomeDevicesConnected.SelectedValueChanged += new System.EventHandler(this.SelectNewHomeDevice);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.listBoxConnectorsAvailable);
            this.groupBox3.Controls.Add(this.comboBox2);
            this.groupBox3.Controls.Add(this.comboBox1);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(688, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(288, 427);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "LINK PRODUCT";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button2);
            this.groupBox4.Controls.Add(this.listBoxUnlinkProducts);
            this.groupBox4.Location = new System.Drawing.Point(0, 198);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(287, 228);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "UNLINK PRODUCT";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 147);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(280, 51);
            this.button1.TabIndex = 1;
            this.button1.Text = "LINK PRODUCT";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 174);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(276, 53);
            this.button2.TabIndex = 1;
            this.button2.Text = "UNLINK PRODUCT";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // listBoxUnlinkProducts
            // 
            this.listBoxUnlinkProducts.FormattingEnabled = true;
            this.listBoxUnlinkProducts.Location = new System.Drawing.Point(6, 22);
            this.listBoxUnlinkProducts.Name = "listBoxUnlinkProducts";
            this.listBoxUnlinkProducts.Size = new System.Drawing.Size(275, 134);
            this.listBoxUnlinkProducts.TabIndex = 0;
            this.listBoxUnlinkProducts.SelectedValueChanged += new System.EventHandler(this.SelectNewHomeDevice);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Product";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Node";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 80);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Connectors";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(70, 16);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(211, 21);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedValueChanged += new System.EventHandler(this.UpdateCapableFreeConnector);
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(70, 48);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(211, 21);
            this.comboBox2.TabIndex = 1;
            this.comboBox2.SelectedValueChanged += new System.EventHandler(this.UpdateCapableFreeConnector);
            // 
            // listBoxConnectorsAvailable
            // 
            this.listBoxConnectorsAvailable.FormattingEnabled = true;
            this.listBoxConnectorsAvailable.Location = new System.Drawing.Point(70, 75);
            this.listBoxConnectorsAvailable.Name = "listBoxConnectorsAvailable";
            this.listBoxConnectorsAvailable.Size = new System.Drawing.Size(211, 69);
            this.listBoxConnectorsAvailable.TabIndex = 0;
            this.listBoxConnectorsAvailable.SelectedValueChanged += new System.EventHandler(this.SelectNewHomeDevice);
            // 
            // ConnexionScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1003, 457);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConnexionScreen";
            this.Text = "ConnexionScreen";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonLinkHomeDevice;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBoxFreeHomeDevices;
        private System.Windows.Forms.ListBox listBoxCapableFreeConnector;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxNode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonUnilinkHomeDevice;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox listBoxHomeDevicesConnected;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBoxConnectorsAvailable;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox listBoxUnlinkProducts;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
    }
}
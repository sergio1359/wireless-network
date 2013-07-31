namespace App_Smart_Home_Prototipo.Electrical.Screens
{
    partial class HomeDeviceScreen
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.textBoxNewName = new System.Windows.Forms.TextBox();
            this.buttonNewHomeDevice = new System.Windows.Forms.Button();
            this.comboBoxNewType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.listBoxHomeDevices = new System.Windows.Forms.ListBox();
            this.buttonRemoveHomeDevice = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.textBoxNameHomeDevice = new System.Windows.Forms.TextBox();
            this.listBoxOperations = new System.Windows.Forms.ListBox();
            this.textBoxTypeHomeDevice = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox7);
            this.groupBox3.Controls.Add(this.groupBox8);
            this.groupBox3.Controls.Add(this.groupBox6);
            this.groupBox3.Location = new System.Drawing.Point(2, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(344, 434);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "HomeDevice";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.textBoxNewName);
            this.groupBox7.Controls.Add(this.buttonNewHomeDevice);
            this.groupBox7.Controls.Add(this.comboBoxNewType);
            this.groupBox7.Controls.Add(this.label1);
            this.groupBox7.Controls.Add(this.label2);
            this.groupBox7.Location = new System.Drawing.Point(6, 19);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(329, 72);
            this.groupBox7.TabIndex = 33;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "ADD HOME DEVICE";
            // 
            // textBoxNewName
            // 
            this.textBoxNewName.Location = new System.Drawing.Point(6, 40);
            this.textBoxNewName.Name = "textBoxNewName";
            this.textBoxNewName.Size = new System.Drawing.Size(113, 20);
            this.textBoxNewName.TabIndex = 20;
            // 
            // buttonNewHomeDevice
            // 
            this.buttonNewHomeDevice.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNewHomeDevice.Location = new System.Drawing.Point(237, 15);
            this.buttonNewHomeDevice.Name = "buttonNewHomeDevice";
            this.buttonNewHomeDevice.Size = new System.Drawing.Size(80, 45);
            this.buttonNewHomeDevice.TabIndex = 8;
            this.buttonNewHomeDevice.Text = "+";
            this.buttonNewHomeDevice.UseVisualStyleBackColor = true;
            // 
            // comboBoxNewType
            // 
            this.comboBoxNewType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxNewType.FormattingEnabled = true;
            this.comboBoxNewType.Location = new System.Drawing.Point(125, 39);
            this.comboBoxNewType.Name = "comboBoxNewType";
            this.comboBoxNewType.Size = new System.Drawing.Size(106, 21);
            this.comboBoxNewType.TabIndex = 27;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(128, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Name";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.listBoxHomeDevices);
            this.groupBox8.Controls.Add(this.buttonRemoveHomeDevice);
            this.groupBox8.Location = new System.Drawing.Point(7, 98);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(328, 116);
            this.groupBox8.TabIndex = 32;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "HOME DEVICES";
            // 
            // listBoxHomeDevices
            // 
            this.listBoxHomeDevices.FormattingEnabled = true;
            this.listBoxHomeDevices.Location = new System.Drawing.Point(6, 19);
            this.listBoxHomeDevices.Name = "listBoxHomeDevices";
            this.listBoxHomeDevices.Size = new System.Drawing.Size(224, 82);
            this.listBoxHomeDevices.TabIndex = 6;
            // 
            // buttonRemoveHomeDevice
            // 
            this.buttonRemoveHomeDevice.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRemoveHomeDevice.Location = new System.Drawing.Point(236, 19);
            this.buttonRemoveHomeDevice.Name = "buttonRemoveHomeDevice";
            this.buttonRemoveHomeDevice.Size = new System.Drawing.Size(80, 82);
            this.buttonRemoveHomeDevice.TabIndex = 10;
            this.buttonRemoveHomeDevice.Text = "-";
            this.buttonRemoveHomeDevice.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.textBoxNameHomeDevice);
            this.groupBox6.Controls.Add(this.listBoxOperations);
            this.groupBox6.Controls.Add(this.textBoxTypeHomeDevice);
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.label3);
            this.groupBox6.Controls.Add(this.label11);
            this.groupBox6.Location = new System.Drawing.Point(7, 221);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(328, 204);
            this.groupBox6.TabIndex = 32;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "HOME DEVICE";
            // 
            // textBoxNameHomeDevice
            // 
            this.textBoxNameHomeDevice.Location = new System.Drawing.Point(6, 36);
            this.textBoxNameHomeDevice.Name = "textBoxNameHomeDevice";
            this.textBoxNameHomeDevice.Size = new System.Drawing.Size(153, 20);
            this.textBoxNameHomeDevice.TabIndex = 20;
            // 
            // listBoxOperations
            // 
            this.listBoxOperations.FormattingEnabled = true;
            this.listBoxOperations.Location = new System.Drawing.Point(5, 77);
            this.listBoxOperations.Name = "listBoxOperations";
            this.listBoxOperations.Size = new System.Drawing.Size(317, 121);
            this.listBoxOperations.TabIndex = 20;
            // 
            // textBoxTypeHomeDevice
            // 
            this.textBoxTypeHomeDevice.Location = new System.Drawing.Point(164, 36);
            this.textBoxTypeHomeDevice.Name = "textBoxTypeHomeDevice";
            this.textBoxTypeHomeDevice.ReadOnly = true;
            this.textBoxTypeHomeDevice.Size = new System.Drawing.Size(152, 20);
            this.textBoxTypeHomeDevice.TabIndex = 29;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 19);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "OPERATIONS";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(161, 19);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(31, 13);
            this.label11.TabIndex = 21;
            this.label11.Text = "Type";
            // 
            // HomeDeviceScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 441);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "HomeDeviceScreen";
            this.Text = "HomeDevices";
            this.groupBox3.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TextBox textBoxNewName;
        private System.Windows.Forms.Button buttonNewHomeDevice;
        private System.Windows.Forms.ComboBox comboBoxNewType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.ListBox listBoxHomeDevices;
        private System.Windows.Forms.Button buttonRemoveHomeDevice;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox textBoxNameHomeDevice;
        private System.Windows.Forms.ListBox listBoxOperations;
        private System.Windows.Forms.TextBox textBoxTypeHomeDevice;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label11;
    }
}
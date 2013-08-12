namespace App_Smart_Home_Prototipo.Administrator.Screens
{
    partial class OperationScreen
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
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.listBoxHomeDevices = new System.Windows.Forms.ListBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.buttonRemoveOperation = new System.Windows.Forms.Button();
            this.textBoxNameHomeDevice = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxArgs = new System.Windows.Forms.TextBox();
            this.comboBoxOperation = new System.Windows.Forms.ComboBox();
            this.buttonAddOperation = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.comboBoxToHomeDevice = new System.Windows.Forms.ComboBox();
            this.listBoxOperations = new System.Windows.Forms.ListBox();
            this.textBoxTypeHomeDevice = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox8.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.listBoxHomeDevices);
            this.groupBox8.Location = new System.Drawing.Point(2, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(328, 116);
            this.groupBox8.TabIndex = 33;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "HOME DEVICES";
            // 
            // listBoxHomeDevices
            // 
            this.listBoxHomeDevices.FormattingEnabled = true;
            this.listBoxHomeDevices.Location = new System.Drawing.Point(6, 19);
            this.listBoxHomeDevices.Name = "listBoxHomeDevices";
            this.listBoxHomeDevices.Size = new System.Drawing.Size(316, 82);
            this.listBoxHomeDevices.TabIndex = 6;
            this.listBoxHomeDevices.SelectedIndexChanged += new System.EventHandler(this.SelectHomeDevice);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.buttonRemoveOperation);
            this.groupBox6.Controls.Add(this.textBoxNameHomeDevice);
            this.groupBox6.Controls.Add(this.groupBox5);
            this.groupBox6.Controls.Add(this.listBoxOperations);
            this.groupBox6.Controls.Add(this.textBoxTypeHomeDevice);
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.label3);
            this.groupBox6.Controls.Add(this.label11);
            this.groupBox6.Location = new System.Drawing.Point(2, 126);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(328, 318);
            this.groupBox6.TabIndex = 34;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "HOME DEVICE";
            // 
            // buttonRemoveOperation
            // 
            this.buttonRemoveOperation.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRemoveOperation.Location = new System.Drawing.Point(242, 77);
            this.buttonRemoveOperation.Name = "buttonRemoveOperation";
            this.buttonRemoveOperation.Size = new System.Drawing.Size(80, 120);
            this.buttonRemoveOperation.TabIndex = 31;
            this.buttonRemoveOperation.Text = "-";
            this.buttonRemoveOperation.UseVisualStyleBackColor = true;
            this.buttonRemoveOperation.Click += new System.EventHandler(this.RemoveOperation);
            // 
            // textBoxNameHomeDevice
            // 
            this.textBoxNameHomeDevice.Location = new System.Drawing.Point(6, 36);
            this.textBoxNameHomeDevice.Name = "textBoxNameHomeDevice";
            this.textBoxNameHomeDevice.Size = new System.Drawing.Size(153, 20);
            this.textBoxNameHomeDevice.TabIndex = 20;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label14);
            this.groupBox5.Controls.Add(this.textBoxArgs);
            this.groupBox5.Controls.Add(this.comboBoxOperation);
            this.groupBox5.Controls.Add(this.buttonAddOperation);
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Controls.Add(this.comboBoxToHomeDevice);
            this.groupBox5.Location = new System.Drawing.Point(5, 210);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(317, 102);
            this.groupBox5.TabIndex = 30;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "ADD OPERATIONS";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(9, 22);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(20, 13);
            this.label14.TabIndex = 23;
            this.label14.Text = "To";
            // 
            // textBoxArgs
            // 
            this.textBoxArgs.Enabled = false;
            this.textBoxArgs.Location = new System.Drawing.Point(68, 73);
            this.textBoxArgs.Name = "textBoxArgs";
            this.textBoxArgs.Size = new System.Drawing.Size(157, 20);
            this.textBoxArgs.TabIndex = 20;
            // 
            // comboBoxOperation
            // 
            this.comboBoxOperation.Enabled = false;
            this.comboBoxOperation.FormattingEnabled = true;
            this.comboBoxOperation.Location = new System.Drawing.Point(68, 46);
            this.comboBoxOperation.Name = "comboBoxOperation";
            this.comboBoxOperation.Size = new System.Drawing.Size(157, 21);
            this.comboBoxOperation.TabIndex = 22;
            // 
            // buttonAddOperation
            // 
            this.buttonAddOperation.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAddOperation.Location = new System.Drawing.Point(232, 19);
            this.buttonAddOperation.Name = "buttonAddOperation";
            this.buttonAddOperation.Size = new System.Drawing.Size(79, 74);
            this.buttonAddOperation.TabIndex = 10;
            this.buttonAddOperation.Text = "+";
            this.buttonAddOperation.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 76);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(28, 13);
            this.label16.TabIndex = 28;
            this.label16.Text = "Args";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 49);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 13);
            this.label15.TabIndex = 26;
            this.label15.Text = "Operation";
            // 
            // comboBoxToHomeDevice
            // 
            this.comboBoxToHomeDevice.FormattingEnabled = true;
            this.comboBoxToHomeDevice.Location = new System.Drawing.Point(68, 19);
            this.comboBoxToHomeDevice.Name = "comboBoxToHomeDevice";
            this.comboBoxToHomeDevice.Size = new System.Drawing.Size(157, 21);
            this.comboBoxToHomeDevice.TabIndex = 25;
            this.comboBoxToHomeDevice.SelectedValueChanged += new System.EventHandler(this.UpdateOperations);
            // 
            // listBoxOperations
            // 
            this.listBoxOperations.FormattingEnabled = true;
            this.listBoxOperations.Location = new System.Drawing.Point(5, 77);
            this.listBoxOperations.Name = "listBoxOperations";
            this.listBoxOperations.Size = new System.Drawing.Size(231, 121);
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
            // OperationScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 447);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "OperationScreen";
            this.Text = "Operations";
            this.groupBox8.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.ListBox listBoxHomeDevices;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button buttonRemoveOperation;
        private System.Windows.Forms.TextBox textBoxNameHomeDevice;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxArgs;
        private System.Windows.Forms.ComboBox comboBoxOperation;
        private System.Windows.Forms.Button buttonAddOperation;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox comboBoxToHomeDevice;
        private System.Windows.Forms.ListBox listBoxOperations;
        private System.Windows.Forms.TextBox textBoxTypeHomeDevice;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label11;
    }
}
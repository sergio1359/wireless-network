namespace App_Smart_Home_Prototipo.Electrical.Screens
{
    partial class NodeScreen
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
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBoxShieldNode = new System.Windows.Forms.TextBox();
            this.textBoxBaseNode = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.listBoxConnectors = new System.Windows.Forms.ListBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxNameNode = new System.Windows.Forms.TextBox();
            this.textBoxAddressNode = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.listBoxNodes = new System.Windows.Forms.ListBox();
            this.buttonRemoveNode = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listBoxMACs = new System.Windows.Forms.ListBox();
            this.buttonNewNode = new System.Windows.Forms.Button();
            this.buttonConfig = new System.Windows.Forms.Button();
            this.groupBox.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.groupBox2);
            this.groupBox.Controls.Add(this.groupBox4);
            this.groupBox.Controls.Add(this.groupBox1);
            this.groupBox.Location = new System.Drawing.Point(3, 3);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(338, 545);
            this.groupBox.TabIndex = 1;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Network";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.buttonConfig);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.textBoxShieldNode);
            this.groupBox2.Controls.Add(this.textBoxBaseNode);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.listBoxConnectors);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBoxNameNode);
            this.groupBox2.Controls.Add(this.textBoxAddressNode);
            this.groupBox2.Location = new System.Drawing.Point(9, 256);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(323, 283);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "NODE";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(236, 33);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(80, 62);
            this.button1.TabIndex = 32;
            this.button1.Text = "Change";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ChangeNodeConfiguration);
            // 
            // textBoxShieldNode
            // 
            this.textBoxShieldNode.Location = new System.Drawing.Point(123, 75);
            this.textBoxShieldNode.Name = "textBoxShieldNode";
            this.textBoxShieldNode.ReadOnly = true;
            this.textBoxShieldNode.Size = new System.Drawing.Size(107, 20);
            this.textBoxShieldNode.TabIndex = 31;
            // 
            // textBoxBaseNode
            // 
            this.textBoxBaseNode.Location = new System.Drawing.Point(123, 33);
            this.textBoxBaseNode.Name = "textBoxBaseNode";
            this.textBoxBaseNode.ReadOnly = true;
            this.textBoxBaseNode.Size = new System.Drawing.Size(107, 20);
            this.textBoxBaseNode.TabIndex = 30;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 102);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "CONNECTORS";
            // 
            // listBoxConnectors
            // 
            this.listBoxConnectors.FormattingEnabled = true;
            this.listBoxConnectors.Location = new System.Drawing.Point(10, 118);
            this.listBoxConnectors.Name = "listBoxConnectors";
            this.listBoxConnectors.Size = new System.Drawing.Size(307, 121);
            this.listBoxConnectors.TabIndex = 10;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(120, 61);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Shield";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(120, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Base";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Address";
            // 
            // textBoxNameNode
            // 
            this.textBoxNameNode.Location = new System.Drawing.Point(6, 33);
            this.textBoxNameNode.Name = "textBoxNameNode";
            this.textBoxNameNode.Size = new System.Drawing.Size(111, 20);
            this.textBoxNameNode.TabIndex = 1;
            // 
            // textBoxAddressNode
            // 
            this.textBoxAddressNode.Location = new System.Drawing.Point(6, 75);
            this.textBoxAddressNode.Name = "textBoxAddressNode";
            this.textBoxAddressNode.Size = new System.Drawing.Size(111, 20);
            this.textBoxAddressNode.TabIndex = 10;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.listBoxNodes);
            this.groupBox4.Controls.Add(this.buttonRemoveNode);
            this.groupBox4.Location = new System.Drawing.Point(9, 97);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(322, 160);
            this.groupBox4.TabIndex = 32;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "NODES";
            // 
            // listBoxNodes
            // 
            this.listBoxNodes.FormattingEnabled = true;
            this.listBoxNodes.Location = new System.Drawing.Point(6, 19);
            this.listBoxNodes.Name = "listBoxNodes";
            this.listBoxNodes.Size = new System.Drawing.Size(224, 134);
            this.listBoxNodes.TabIndex = 6;
            this.listBoxNodes.SelectedIndexChanged += new System.EventHandler(this.SelectNode);
            // 
            // buttonRemoveNode
            // 
            this.buttonRemoveNode.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRemoveNode.Location = new System.Drawing.Point(236, 19);
            this.buttonRemoveNode.Name = "buttonRemoveNode";
            this.buttonRemoveNode.Size = new System.Drawing.Size(80, 134);
            this.buttonRemoveNode.TabIndex = 10;
            this.buttonRemoveNode.Text = "-";
            this.buttonRemoveNode.UseVisualStyleBackColor = true;
            this.buttonRemoveNode.Click += new System.EventHandler(this.RemoveNode);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBoxMACs);
            this.groupBox1.Controls.Add(this.buttonNewNode);
            this.groupBox1.Location = new System.Drawing.Point(9, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(322, 72);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ADD NODES";
            // 
            // listBoxMACs
            // 
            this.listBoxMACs.FormattingEnabled = true;
            this.listBoxMACs.Location = new System.Drawing.Point(6, 18);
            this.listBoxMACs.Name = "listBoxMACs";
            this.listBoxMACs.Size = new System.Drawing.Size(224, 43);
            this.listBoxMACs.TabIndex = 6;
            // 
            // buttonNewNode
            // 
            this.buttonNewNode.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNewNode.Location = new System.Drawing.Point(236, 18);
            this.buttonNewNode.Name = "buttonNewNode";
            this.buttonNewNode.Size = new System.Drawing.Size(80, 43);
            this.buttonNewNode.TabIndex = 8;
            this.buttonNewNode.Text = "+";
            this.buttonNewNode.UseVisualStyleBackColor = true;
            this.buttonNewNode.Click += new System.EventHandler(this.AcceptNode);
            // 
            // buttonConfig
            // 
            this.buttonConfig.Location = new System.Drawing.Point(6, 245);
            this.buttonConfig.Name = "buttonConfig";
            this.buttonConfig.Size = new System.Drawing.Size(310, 32);
            this.buttonConfig.TabIndex = 34;
            this.buttonConfig.Text = "Force Configuration";
            this.buttonConfig.UseVisualStyleBackColor = true;
            this.buttonConfig.Click += new System.EventHandler(this.ForceConfiguration);
            // 
            // NodeScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 553);
            this.Controls.Add(this.groupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "NodeScreen";
            this.Text = "Nodes";
            this.groupBox.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxShieldNode;
        private System.Windows.Forms.TextBox textBoxBaseNode;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ListBox listBoxConnectors;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxNameNode;
        private System.Windows.Forms.TextBox textBoxAddressNode;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListBox listBoxNodes;
        private System.Windows.Forms.Button buttonRemoveNode;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonNewNode;
        private System.Windows.Forms.ListBox listBoxMACs;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonConfig;
    }
}
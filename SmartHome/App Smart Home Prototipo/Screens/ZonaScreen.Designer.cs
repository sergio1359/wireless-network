namespace App_Smart_Home_Prototipo.Electrical.Screens
{
    partial class ZonaScreen
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonChangeImageView = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonChangeImage = new System.Windows.Forms.Button();
            this.buttonRemoveView = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonNewView = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxNewViewName = new System.Windows.Forms.TextBox();
            this.listBoxViews = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxNameView = new System.Windows.Forms.TextBox();
            this.textBoxNameNode = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.listBoxZones = new System.Windows.Forms.ListBox();
            this.buttonRemoveZone = new System.Windows.Forms.Button();
            this.textBoxHomeName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label18 = new System.Windows.Forms.Label();
            this.buttonAddZona = new System.Windows.Forms.Button();
            this.textBoxNewNameZona = new System.Windows.Forms.TextBox();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.textBoxHomeName);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(339, 580);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "HOME";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Name";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.buttonChangeImageView);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.buttonChangeImage);
            this.groupBox2.Controls.Add(this.buttonRemoveView);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.buttonNewView);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.textBoxNewViewName);
            this.groupBox2.Controls.Add(this.listBoxViews);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBoxNameView);
            this.groupBox2.Controls.Add(this.textBoxNameNode);
            this.groupBox2.Location = new System.Drawing.Point(6, 283);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(322, 295);
            this.groupBox2.TabIndex = 33;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ZONE";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(236, 236);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(80, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cambiar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.ChangeViewName);
            // 
            // buttonChangeImageView
            // 
            this.buttonChangeImageView.Location = new System.Drawing.Point(48, 264);
            this.buttonChangeImageView.Name = "buttonChangeImageView";
            this.buttonChangeImageView.Size = new System.Drawing.Size(268, 23);
            this.buttonChangeImageView.TabIndex = 2;
            this.buttonChangeImageView.Text = "Cambiar";
            this.buttonChangeImageView.UseVisualStyleBackColor = true;
            this.buttonChangeImageView.Click += new System.EventHandler(this.ChangeViewImage);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(236, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(80, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Cambiar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ChangeZoneName);
            // 
            // buttonChangeImage
            // 
            this.buttonChangeImage.Location = new System.Drawing.Point(47, 40);
            this.buttonChangeImage.Name = "buttonChangeImage";
            this.buttonChangeImage.Size = new System.Drawing.Size(269, 23);
            this.buttonChangeImage.TabIndex = 2;
            this.buttonChangeImage.Text = "Cambiar";
            this.buttonChangeImage.UseVisualStyleBackColor = true;
            this.buttonChangeImage.Click += new System.EventHandler(this.ChangeImageZone);
            // 
            // buttonRemoveView
            // 
            this.buttonRemoveView.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRemoveView.Location = new System.Drawing.Point(236, 127);
            this.buttonRemoveView.Name = "buttonRemoveView";
            this.buttonRemoveView.Size = new System.Drawing.Size(80, 95);
            this.buttonRemoveView.TabIndex = 10;
            this.buttonRemoveView.Text = "-";
            this.buttonRemoveView.UseVisualStyleBackColor = true;
            this.buttonRemoveView.Click += new System.EventHandler(this.DeleteView);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Name";
            // 
            // buttonNewView
            // 
            this.buttonNewView.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNewView.Location = new System.Drawing.Point(274, 77);
            this.buttonNewView.Name = "buttonNewView";
            this.buttonNewView.Size = new System.Drawing.Size(42, 32);
            this.buttonNewView.TabIndex = 8;
            this.buttonNewView.Text = "+";
            this.buttonNewView.UseVisualStyleBackColor = true;
            this.buttonNewView.Click += new System.EventHandler(this.AddView);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 111);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "VISTAS";
            // 
            // textBoxNewViewName
            // 
            this.textBoxNewViewName.Location = new System.Drawing.Point(48, 83);
            this.textBoxNewViewName.Name = "textBoxNewViewName";
            this.textBoxNewViewName.Size = new System.Drawing.Size(220, 20);
            this.textBoxNewViewName.TabIndex = 1;
            // 
            // listBoxViews
            // 
            this.listBoxViews.FormattingEnabled = true;
            this.listBoxViews.Location = new System.Drawing.Point(9, 127);
            this.listBoxViews.Name = "listBoxViews";
            this.listBoxViews.Size = new System.Drawing.Size(221, 95);
            this.listBoxViews.TabIndex = 10;
            this.listBoxViews.SelectedIndexChanged += new System.EventHandler(this.LoadView);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 241);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Name";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 267);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Image";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Image";
            // 
            // textBoxNameView
            // 
            this.textBoxNameView.Location = new System.Drawing.Point(48, 238);
            this.textBoxNameView.Name = "textBoxNameView";
            this.textBoxNameView.Size = new System.Drawing.Size(182, 20);
            this.textBoxNameView.TabIndex = 1;
            // 
            // textBoxNameNode
            // 
            this.textBoxNameNode.Location = new System.Drawing.Point(47, 16);
            this.textBoxNameNode.Name = "textBoxNameNode";
            this.textBoxNameNode.Size = new System.Drawing.Size(183, 20);
            this.textBoxNameNode.TabIndex = 1;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(242, 17);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(80, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Cambiar";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.ChangeHomeName);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.listBoxZones);
            this.groupBox4.Controls.Add(this.buttonRemoveZone);
            this.groupBox4.Location = new System.Drawing.Point(6, 135);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(322, 147);
            this.groupBox4.TabIndex = 34;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "ZONES";
            // 
            // listBoxZones
            // 
            this.listBoxZones.FormattingEnabled = true;
            this.listBoxZones.Location = new System.Drawing.Point(6, 19);
            this.listBoxZones.Name = "listBoxZones";
            this.listBoxZones.Size = new System.Drawing.Size(224, 121);
            this.listBoxZones.TabIndex = 6;
            this.listBoxZones.SelectedIndexChanged += new System.EventHandler(this.LoadZone);
            // 
            // buttonRemoveZone
            // 
            this.buttonRemoveZone.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRemoveZone.Location = new System.Drawing.Point(236, 19);
            this.buttonRemoveZone.Name = "buttonRemoveZone";
            this.buttonRemoveZone.Size = new System.Drawing.Size(80, 121);
            this.buttonRemoveZone.TabIndex = 10;
            this.buttonRemoveZone.Text = "-";
            this.buttonRemoveZone.UseVisualStyleBackColor = true;
            this.buttonRemoveZone.Click += new System.EventHandler(this.RemoveZones);
            // 
            // textBoxHomeName
            // 
            this.textBoxHomeName.Location = new System.Drawing.Point(53, 19);
            this.textBoxHomeName.Name = "textBoxHomeName";
            this.textBoxHomeName.Size = new System.Drawing.Size(183, 20);
            this.textBoxHomeName.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.buttonAddZona);
            this.groupBox3.Controls.Add(this.textBoxNewNameZona);
            this.groupBox3.Location = new System.Drawing.Point(6, 74);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(322, 57);
            this.groupBox3.TabIndex = 35;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "ADD ZONA";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(15, 27);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(35, 13);
            this.label18.TabIndex = 26;
            this.label18.Text = "Name";
            // 
            // buttonAddZona
            // 
            this.buttonAddZona.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAddZona.Location = new System.Drawing.Point(274, 18);
            this.buttonAddZona.Name = "buttonAddZona";
            this.buttonAddZona.Size = new System.Drawing.Size(42, 32);
            this.buttonAddZona.TabIndex = 8;
            this.buttonAddZona.Text = "+";
            this.buttonAddZona.UseVisualStyleBackColor = true;
            this.buttonAddZona.Click += new System.EventHandler(this.AddNewZone);
            // 
            // textBoxNewNameZona
            // 
            this.textBoxNewNameZona.Location = new System.Drawing.Point(56, 24);
            this.textBoxNewNameZona.Name = "textBoxNewNameZona";
            this.textBoxNewNameZona.Size = new System.Drawing.Size(212, 20);
            this.textBoxNewNameZona.TabIndex = 1;
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(357, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(635, 580);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "Open Image File";
            // 
            // ZonaScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 604);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.groupBox1);
            this.Name = "ZonaScreen";
            this.Text = "ZonaScreen";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonChangeImageView;
        private System.Windows.Forms.Button buttonChangeImage;
        private System.Windows.Forms.Button buttonRemoveView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonNewView;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxNewViewName;
        private System.Windows.Forms.ListBox listBoxViews;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxNameView;
        private System.Windows.Forms.TextBox textBoxNameNode;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListBox listBoxZones;
        private System.Windows.Forms.Button buttonRemoveZone;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button buttonAddZona;
        private System.Windows.Forms.TextBox textBoxNewNameZona;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBoxHomeName;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}
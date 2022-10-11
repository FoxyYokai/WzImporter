namespace WzImporter
{
    partial class MainForm
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
            this.checkBox_Hair = new System.Windows.Forms.CheckBox();
            this.checkBox_Face = new System.Windows.Forms.CheckBox();
            this.checkBox_Cap = new System.Windows.Forms.CheckBox();
            this.checkBox_Coat = new System.Windows.Forms.CheckBox();
            this.checkBox_Pants = new System.Windows.Forms.CheckBox();
            this.checkBox_Longcoat = new System.Windows.Forms.CheckBox();
            this.checkBox_Shoes = new System.Windows.Forms.CheckBox();
            this.checkBox_Cape = new System.Windows.Forms.CheckBox();
            this.checkBox_Shield = new System.Windows.Forms.CheckBox();
            this.checkBox_Ring = new System.Windows.Forms.CheckBox();
            this.checkBox_Accessory = new System.Windows.Forms.CheckBox();
            this.checkBox_PetEquip = new System.Windows.Forms.CheckBox();
            this.checkBox_Glove = new System.Windows.Forms.CheckBox();
            this.checkBox_Weapon = new System.Windows.Forms.CheckBox();
            this.checkBox_CashOnly = new System.Windows.Forms.CheckBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.button_Import = new System.Windows.Forms.Button();
            this.textBox_ImportToFile = new System.Windows.Forms.TextBox();
            this.textBox_ImportFromFile = new System.Windows.Forms.TextBox();
            this.textBox_OutputFolder = new System.Windows.Forms.TextBox();
            this.button_ImportToBrowse = new System.Windows.Forms.Button();
            this.button_ImportFromBrowse = new System.Windows.Forms.Button();
            this.button_OutputFolderBrowse = new System.Windows.Forms.Button();
            this.label_Progress = new System.Windows.Forms.Label();
            this.label_ImportTo = new System.Windows.Forms.Label();
            this.label_ImportFrom = new System.Windows.Forms.Label();
            this.label_OutputFolder = new System.Windows.Forms.Label();
            this.checkBox_IncludeString = new System.Windows.Forms.CheckBox();
            this.checkBox_OnlyThese = new System.Windows.Forms.CheckBox();
            this.textBox_OnlyThese = new System.Windows.Forms.TextBox();
            this.pictureBox_Logo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Logo)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBox_Hair
            // 
            this.checkBox_Hair.AutoSize = true;
            this.checkBox_Hair.Location = new System.Drawing.Point(563, 97);
            this.checkBox_Hair.Name = "checkBox_Hair";
            this.checkBox_Hair.Size = new System.Drawing.Size(45, 17);
            this.checkBox_Hair.TabIndex = 0;
            this.checkBox_Hair.Text = "Hair";
            this.checkBox_Hair.UseVisualStyleBackColor = true;
            // 
            // checkBox_Face
            // 
            this.checkBox_Face.AutoSize = true;
            this.checkBox_Face.Location = new System.Drawing.Point(563, 120);
            this.checkBox_Face.Name = "checkBox_Face";
            this.checkBox_Face.Size = new System.Drawing.Size(50, 17);
            this.checkBox_Face.TabIndex = 1;
            this.checkBox_Face.Text = "Face";
            this.checkBox_Face.UseVisualStyleBackColor = true;
            // 
            // checkBox_Cap
            // 
            this.checkBox_Cap.AutoSize = true;
            this.checkBox_Cap.Location = new System.Drawing.Point(563, 143);
            this.checkBox_Cap.Name = "checkBox_Cap";
            this.checkBox_Cap.Size = new System.Drawing.Size(45, 17);
            this.checkBox_Cap.TabIndex = 2;
            this.checkBox_Cap.Text = "Cap";
            this.checkBox_Cap.UseVisualStyleBackColor = true;
            // 
            // checkBox_Coat
            // 
            this.checkBox_Coat.AutoSize = true;
            this.checkBox_Coat.Location = new System.Drawing.Point(563, 166);
            this.checkBox_Coat.Name = "checkBox_Coat";
            this.checkBox_Coat.Size = new System.Drawing.Size(48, 17);
            this.checkBox_Coat.TabIndex = 3;
            this.checkBox_Coat.Text = "Coat";
            this.checkBox_Coat.UseVisualStyleBackColor = true;
            // 
            // checkBox_Pants
            // 
            this.checkBox_Pants.AutoSize = true;
            this.checkBox_Pants.Location = new System.Drawing.Point(649, 97);
            this.checkBox_Pants.Name = "checkBox_Pants";
            this.checkBox_Pants.Size = new System.Drawing.Size(53, 17);
            this.checkBox_Pants.TabIndex = 4;
            this.checkBox_Pants.Text = "Pants";
            this.checkBox_Pants.UseVisualStyleBackColor = true;
            // 
            // checkBox_Longcoat
            // 
            this.checkBox_Longcoat.AutoSize = true;
            this.checkBox_Longcoat.Location = new System.Drawing.Point(649, 120);
            this.checkBox_Longcoat.Name = "checkBox_Longcoat";
            this.checkBox_Longcoat.Size = new System.Drawing.Size(71, 17);
            this.checkBox_Longcoat.TabIndex = 5;
            this.checkBox_Longcoat.Text = "Longcoat";
            this.checkBox_Longcoat.UseVisualStyleBackColor = true;
            // 
            // checkBox_Shoes
            // 
            this.checkBox_Shoes.AutoSize = true;
            this.checkBox_Shoes.Location = new System.Drawing.Point(649, 143);
            this.checkBox_Shoes.Name = "checkBox_Shoes";
            this.checkBox_Shoes.Size = new System.Drawing.Size(56, 17);
            this.checkBox_Shoes.TabIndex = 6;
            this.checkBox_Shoes.Text = "Shoes";
            this.checkBox_Shoes.UseVisualStyleBackColor = true;
            // 
            // checkBox_Cape
            // 
            this.checkBox_Cape.AutoSize = true;
            this.checkBox_Cape.Location = new System.Drawing.Point(649, 166);
            this.checkBox_Cape.Name = "checkBox_Cape";
            this.checkBox_Cape.Size = new System.Drawing.Size(51, 17);
            this.checkBox_Cape.TabIndex = 7;
            this.checkBox_Cape.Text = "Cape";
            this.checkBox_Cape.UseVisualStyleBackColor = true;
            // 
            // checkBox_Shield
            // 
            this.checkBox_Shield.AutoSize = true;
            this.checkBox_Shield.Location = new System.Drawing.Point(563, 189);
            this.checkBox_Shield.Name = "checkBox_Shield";
            this.checkBox_Shield.Size = new System.Drawing.Size(55, 17);
            this.checkBox_Shield.TabIndex = 8;
            this.checkBox_Shield.Text = "Shield";
            this.checkBox_Shield.UseVisualStyleBackColor = true;
            // 
            // checkBox_Ring
            // 
            this.checkBox_Ring.AutoSize = true;
            this.checkBox_Ring.Location = new System.Drawing.Point(649, 189);
            this.checkBox_Ring.Name = "checkBox_Ring";
            this.checkBox_Ring.Size = new System.Drawing.Size(48, 17);
            this.checkBox_Ring.TabIndex = 9;
            this.checkBox_Ring.Text = "Ring";
            this.checkBox_Ring.UseVisualStyleBackColor = true;
            // 
            // checkBox_Accessory
            // 
            this.checkBox_Accessory.AutoSize = true;
            this.checkBox_Accessory.Location = new System.Drawing.Point(563, 212);
            this.checkBox_Accessory.Name = "checkBox_Accessory";
            this.checkBox_Accessory.Size = new System.Drawing.Size(75, 17);
            this.checkBox_Accessory.TabIndex = 10;
            this.checkBox_Accessory.Text = "Accessory";
            this.checkBox_Accessory.UseVisualStyleBackColor = true;
            // 
            // checkBox_PetEquip
            // 
            this.checkBox_PetEquip.AutoSize = true;
            this.checkBox_PetEquip.Location = new System.Drawing.Point(649, 212);
            this.checkBox_PetEquip.Name = "checkBox_PetEquip";
            this.checkBox_PetEquip.Size = new System.Drawing.Size(69, 17);
            this.checkBox_PetEquip.TabIndex = 11;
            this.checkBox_PetEquip.Text = "PetEquip";
            this.checkBox_PetEquip.UseVisualStyleBackColor = true;
            // 
            // checkBox_Glove
            // 
            this.checkBox_Glove.AutoSize = true;
            this.checkBox_Glove.Location = new System.Drawing.Point(563, 235);
            this.checkBox_Glove.Name = "checkBox_Glove";
            this.checkBox_Glove.Size = new System.Drawing.Size(54, 17);
            this.checkBox_Glove.TabIndex = 12;
            this.checkBox_Glove.Text = "Glove";
            this.checkBox_Glove.UseVisualStyleBackColor = true;
            // 
            // checkBox_Weapon
            // 
            this.checkBox_Weapon.AutoSize = true;
            this.checkBox_Weapon.Location = new System.Drawing.Point(649, 235);
            this.checkBox_Weapon.Name = "checkBox_Weapon";
            this.checkBox_Weapon.Size = new System.Drawing.Size(67, 17);
            this.checkBox_Weapon.TabIndex = 13;
            this.checkBox_Weapon.Text = "Weapon";
            this.checkBox_Weapon.UseVisualStyleBackColor = true;
            // 
            // checkBox_CashOnly
            // 
            this.checkBox_CashOnly.AutoSize = true;
            this.checkBox_CashOnly.Location = new System.Drawing.Point(563, 61);
            this.checkBox_CashOnly.Name = "checkBox_CashOnly";
            this.checkBox_CashOnly.Size = new System.Drawing.Size(71, 17);
            this.checkBox_CashOnly.TabIndex = 14;
            this.checkBox_CashOnly.Text = "CashOnly";
            this.checkBox_CashOnly.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "*.wz";
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "*.wz";
            // 
            // button_Import
            // 
            this.button_Import.Location = new System.Drawing.Point(273, 379);
            this.button_Import.Name = "button_Import";
            this.button_Import.Size = new System.Drawing.Size(215, 23);
            this.button_Import.TabIndex = 15;
            this.button_Import.Text = "Import";
            this.button_Import.UseVisualStyleBackColor = true;
            this.button_Import.Click += new System.EventHandler(this.button_Import_Click);
            // 
            // textBox_ImportToFile
            // 
            this.textBox_ImportToFile.Location = new System.Drawing.Point(123, 163);
            this.textBox_ImportToFile.Name = "textBox_ImportToFile";
            this.textBox_ImportToFile.ReadOnly = true;
            this.textBox_ImportToFile.Size = new System.Drawing.Size(365, 20);
            this.textBox_ImportToFile.TabIndex = 16;
            // 
            // textBox_ImportFromFile
            // 
            this.textBox_ImportFromFile.Location = new System.Drawing.Point(123, 221);
            this.textBox_ImportFromFile.Name = "textBox_ImportFromFile";
            this.textBox_ImportFromFile.ReadOnly = true;
            this.textBox_ImportFromFile.Size = new System.Drawing.Size(365, 20);
            this.textBox_ImportFromFile.TabIndex = 17;
            // 
            // textBox_OutputFolder
            // 
            this.textBox_OutputFolder.Location = new System.Drawing.Point(123, 277);
            this.textBox_OutputFolder.Name = "textBox_OutputFolder";
            this.textBox_OutputFolder.ReadOnly = true;
            this.textBox_OutputFolder.Size = new System.Drawing.Size(365, 20);
            this.textBox_OutputFolder.TabIndex = 18;
            // 
            // button_ImportToBrowse
            // 
            this.button_ImportToBrowse.Location = new System.Drawing.Point(494, 161);
            this.button_ImportToBrowse.Name = "button_ImportToBrowse";
            this.button_ImportToBrowse.Size = new System.Drawing.Size(34, 23);
            this.button_ImportToBrowse.TabIndex = 19;
            this.button_ImportToBrowse.Text = "...";
            this.button_ImportToBrowse.UseVisualStyleBackColor = true;
            this.button_ImportToBrowse.Click += new System.EventHandler(this.button_ImportTo_Click);
            // 
            // button_ImportFromBrowse
            // 
            this.button_ImportFromBrowse.Location = new System.Drawing.Point(494, 219);
            this.button_ImportFromBrowse.Name = "button_ImportFromBrowse";
            this.button_ImportFromBrowse.Size = new System.Drawing.Size(34, 23);
            this.button_ImportFromBrowse.TabIndex = 20;
            this.button_ImportFromBrowse.Text = "...";
            this.button_ImportFromBrowse.UseVisualStyleBackColor = true;
            this.button_ImportFromBrowse.Click += new System.EventHandler(this.button_ImportFrom_Click);
            // 
            // button_OutputFolderBrowse
            // 
            this.button_OutputFolderBrowse.Location = new System.Drawing.Point(494, 275);
            this.button_OutputFolderBrowse.Name = "button_OutputFolderBrowse";
            this.button_OutputFolderBrowse.Size = new System.Drawing.Size(34, 23);
            this.button_OutputFolderBrowse.TabIndex = 21;
            this.button_OutputFolderBrowse.Text = "...";
            this.button_OutputFolderBrowse.UseVisualStyleBackColor = true;
            this.button_OutputFolderBrowse.Click += new System.EventHandler(this.button_OutputFolder_Click);
            // 
            // label_Progress
            // 
            this.label_Progress.AutoSize = true;
            this.label_Progress.Location = new System.Drawing.Point(12, 428);
            this.label_Progress.Name = "label_Progress";
            this.label_Progress.Size = new System.Drawing.Size(38, 13);
            this.label_Progress.TabIndex = 22;
            this.label_Progress.Text = "Ready";
            // 
            // label_ImportTo
            // 
            this.label_ImportTo.AutoSize = true;
            this.label_ImportTo.Location = new System.Drawing.Point(120, 147);
            this.label_ImportTo.Name = "label_ImportTo";
            this.label_ImportTo.Size = new System.Drawing.Size(83, 13);
            this.label_ImportTo.TabIndex = 23;
            this.label_ImportTo.Text = "File to Import To";
            // 
            // label_ImportFrom
            // 
            this.label_ImportFrom.AutoSize = true;
            this.label_ImportFrom.Location = new System.Drawing.Point(120, 205);
            this.label_ImportFrom.Name = "label_ImportFrom";
            this.label_ImportFrom.Size = new System.Drawing.Size(93, 13);
            this.label_ImportFrom.TabIndex = 24;
            this.label_ImportFrom.Text = "File to Import From";
            // 
            // label_OutputFolder
            // 
            this.label_OutputFolder.AutoSize = true;
            this.label_OutputFolder.Location = new System.Drawing.Point(120, 261);
            this.label_OutputFolder.Name = "label_OutputFolder";
            this.label_OutputFolder.Size = new System.Drawing.Size(71, 13);
            this.label_OutputFolder.TabIndex = 25;
            this.label_OutputFolder.Text = "Output Folder";
            // 
            // checkBox_IncludeString
            // 
            this.checkBox_IncludeString.AutoSize = true;
            this.checkBox_IncludeString.Location = new System.Drawing.Point(649, 61);
            this.checkBox_IncludeString.Name = "checkBox_IncludeString";
            this.checkBox_IncludeString.Size = new System.Drawing.Size(107, 17);
            this.checkBox_IncludeString.TabIndex = 26;
            this.checkBox_IncludeString.Text = "Include String.wz";
            this.checkBox_IncludeString.UseVisualStyleBackColor = true;
            // 
            // checkBox_OnlyThese
            // 
            this.checkBox_OnlyThese.AutoSize = true;
            this.checkBox_OnlyThese.Location = new System.Drawing.Point(568, 275);
            this.checkBox_OnlyThese.Name = "checkBox_OnlyThese";
            this.checkBox_OnlyThese.Size = new System.Drawing.Size(112, 17);
            this.checkBox_OnlyThese.TabIndex = 27;
            this.checkBox_OnlyThese.Text = "Import Only These";
            this.checkBox_OnlyThese.UseVisualStyleBackColor = true;
            this.checkBox_OnlyThese.CheckedChanged += new System.EventHandler(this.checkBox_OnlyThese_Changed);
            // 
            // textBox_OnlyThese
            // 
            this.textBox_OnlyThese.Enabled = false;
            this.textBox_OnlyThese.Location = new System.Drawing.Point(568, 298);
            this.textBox_OnlyThese.Multiline = true;
            this.textBox_OnlyThese.Name = "textBox_OnlyThese";
            this.textBox_OnlyThese.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_OnlyThese.Size = new System.Drawing.Size(187, 139);
            this.textBox_OnlyThese.TabIndex = 28;
            // 
            // pictureBox_Logo
            // 
            this.pictureBox_Logo.Location = new System.Drawing.Point(123, 33);
            this.pictureBox_Logo.Name = "pictureBox_Logo";
            this.pictureBox_Logo.Size = new System.Drawing.Size(404, 80);
            this.pictureBox_Logo.TabIndex = 29;
            this.pictureBox_Logo.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 449);
            this.Controls.Add(this.pictureBox_Logo);
            this.Controls.Add(this.textBox_OnlyThese);
            this.Controls.Add(this.checkBox_OnlyThese);
            this.Controls.Add(this.checkBox_IncludeString);
            this.Controls.Add(this.label_OutputFolder);
            this.Controls.Add(this.label_ImportFrom);
            this.Controls.Add(this.label_ImportTo);
            this.Controls.Add(this.label_Progress);
            this.Controls.Add(this.button_OutputFolderBrowse);
            this.Controls.Add(this.button_ImportFromBrowse);
            this.Controls.Add(this.button_ImportToBrowse);
            this.Controls.Add(this.textBox_OutputFolder);
            this.Controls.Add(this.textBox_ImportFromFile);
            this.Controls.Add(this.textBox_ImportToFile);
            this.Controls.Add(this.button_Import);
            this.Controls.Add(this.checkBox_CashOnly);
            this.Controls.Add(this.checkBox_Weapon);
            this.Controls.Add(this.checkBox_Glove);
            this.Controls.Add(this.checkBox_PetEquip);
            this.Controls.Add(this.checkBox_Accessory);
            this.Controls.Add(this.checkBox_Ring);
            this.Controls.Add(this.checkBox_Shield);
            this.Controls.Add(this.checkBox_Cape);
            this.Controls.Add(this.checkBox_Shoes);
            this.Controls.Add(this.checkBox_Longcoat);
            this.Controls.Add(this.checkBox_Pants);
            this.Controls.Add(this.checkBox_Coat);
            this.Controls.Add(this.checkBox_Cap);
            this.Controls.Add(this.checkBox_Face);
            this.Controls.Add(this.checkBox_Hair);
            this.Name = "MainForm";
            this.Text = "Wz Importer";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Logo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_Hair;
        private System.Windows.Forms.CheckBox checkBox_Face;
        private System.Windows.Forms.CheckBox checkBox_Cap;
        private System.Windows.Forms.CheckBox checkBox_Coat;
        private System.Windows.Forms.CheckBox checkBox_Pants;
        private System.Windows.Forms.CheckBox checkBox_Longcoat;
        private System.Windows.Forms.CheckBox checkBox_Shoes;
        private System.Windows.Forms.CheckBox checkBox_Cape;
        private System.Windows.Forms.CheckBox checkBox_Shield;
        private System.Windows.Forms.CheckBox checkBox_Ring;
        private System.Windows.Forms.CheckBox checkBox_Accessory;
        private System.Windows.Forms.CheckBox checkBox_PetEquip;
        private System.Windows.Forms.CheckBox checkBox_Glove;
        private System.Windows.Forms.CheckBox checkBox_Weapon;
        private System.Windows.Forms.CheckBox checkBox_CashOnly;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button button_Import;
        private System.Windows.Forms.TextBox textBox_ImportToFile;
        private System.Windows.Forms.TextBox textBox_ImportFromFile;
        private System.Windows.Forms.TextBox textBox_OutputFolder;
        private System.Windows.Forms.Button button_ImportToBrowse;
        private System.Windows.Forms.Button button_ImportFromBrowse;
        private System.Windows.Forms.Button button_OutputFolderBrowse;
        private System.Windows.Forms.Label label_Progress;
        private System.Windows.Forms.Label label_ImportTo;
        private System.Windows.Forms.Label label_ImportFrom;
        private System.Windows.Forms.Label label_OutputFolder;
        private System.Windows.Forms.CheckBox checkBox_IncludeString;
        private System.Windows.Forms.CheckBox checkBox_OnlyThese;
        private System.Windows.Forms.TextBox textBox_OnlyThese;
        private System.Windows.Forms.PictureBox pictureBox_Logo;
    }
}


namespace INFOIBV
{
    partial class INFOIBV
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
            this.LoadImageButton = new System.Windows.Forms.Button();
            this.openImageDialog = new System.Windows.Forms.OpenFileDialog();
            this.imageFileName = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.saveImageDialog = new System.Windows.Forms.SaveFileDialog();
            this.saveButton = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.algorithmComboBox = new System.Windows.Forms.ComboBox();
            this.clearOutputButton = new System.Windows.Forms.Button();
            this.outputComboBox = new System.Windows.Forms.ComboBox();
            this.debugCheckBox = new System.Windows.Forms.CheckBox();
            this.stepsCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // LoadImageButton
            // 
            this.LoadImageButton.Location = new System.Drawing.Point(12, 11);
            this.LoadImageButton.Name = "LoadImageButton";
            this.LoadImageButton.Size = new System.Drawing.Size(98, 23);
            this.LoadImageButton.TabIndex = 0;
            this.LoadImageButton.Text = "Load image...";
            this.LoadImageButton.UseVisualStyleBackColor = true;
            this.LoadImageButton.Click += new System.EventHandler(this.LoadImageButton_Click);
            // 
            // openImageDialog
            // 
            this.openImageDialog.Filter = "Bitmap files (*.bmp;*.gif;*.jpg;*.png;*.tiff;*.jpeg)|*.bmp;*.gif;*.jpg;*.png;*.ti" +
                "ff;*.jpeg";
            this.openImageDialog.InitialDirectory = "..\\..\\images";
            // 
            // imageFileName
            // 
            this.imageFileName.Location = new System.Drawing.Point(116, 13);
            this.imageFileName.Name = "imageFileName";
            this.imageFileName.ReadOnly = true;
            this.imageFileName.Size = new System.Drawing.Size(326, 20);
            this.imageFileName.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(13, 45);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(512, 512);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(568, 11);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(98, 23);
            this.applyButton.TabIndex = 3;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // saveImageDialog
            // 
            this.saveImageDialog.Filter = "Bitmap file (*.bmp)|*.bmp";
            this.saveImageDialog.InitialDirectory = "..\\..\\images";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(948, 11);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(95, 23);
            this.saveButton.TabIndex = 7;
            this.saveButton.Text = "Save as BMP...";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(531, 45);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(512, 512);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(13, 563);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1030, 20);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 6;
            this.progressBar.Visible = false;
            // 
            // algorithmComboBox
            // 
            this.algorithmComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.algorithmComboBox.FormattingEnabled = true;
            this.algorithmComboBox.Location = new System.Drawing.Point(448, 13);
            this.algorithmComboBox.Name = "algorithmComboBox";
            this.algorithmComboBox.Size = new System.Drawing.Size(114, 21);
            this.algorithmComboBox.TabIndex = 2;
            // 
            // clearOutputButton
            // 
            this.clearOutputButton.Location = new System.Drawing.Point(891, 11);
            this.clearOutputButton.Name = "clearOutputButton";
            this.clearOutputButton.Size = new System.Drawing.Size(51, 23);
            this.clearOutputButton.TabIndex = 6;
            this.clearOutputButton.Text = "Clear";
            this.clearOutputButton.UseVisualStyleBackColor = true;
            this.clearOutputButton.Click += new System.EventHandler(this.clearOutputButton_Click);
            // 
            // outputComboBox
            // 
            this.outputComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.outputComboBox.FormattingEnabled = true;
            this.outputComboBox.Location = new System.Drawing.Point(771, 13);
            this.outputComboBox.Name = "outputComboBox";
            this.outputComboBox.Size = new System.Drawing.Size(114, 21);
            this.outputComboBox.TabIndex = 5;
            this.outputComboBox.SelectedValueChanged += new System.EventHandler(this.outputComboBox_SelectedValueChanged);
            // 
            // debugCheckBox
            // 
            this.debugCheckBox.AutoSize = true;
            this.debugCheckBox.Location = new System.Drawing.Point(672, 7);
            this.debugCheckBox.Name = "debugCheckBox";
            this.debugCheckBox.Size = new System.Drawing.Size(93, 17);
            this.debugCheckBox.TabIndex = 4;
            this.debugCheckBox.Text = "Debug Output";
            this.debugCheckBox.UseVisualStyleBackColor = true;
            // 
            // stepsCheckBox
            // 
            this.stepsCheckBox.AutoSize = true;
            this.stepsCheckBox.Location = new System.Drawing.Point(672, 24);
            this.stepsCheckBox.Name = "stepsCheckBox";
            this.stepsCheckBox.Size = new System.Drawing.Size(83, 17);
            this.stepsCheckBox.TabIndex = 4;
            this.stepsCheckBox.Text = "Show Steps";
            this.stepsCheckBox.UseVisualStyleBackColor = true;
            // 
            // INFOIBV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1052, 591);
            this.Controls.Add(this.stepsCheckBox);
            this.Controls.Add(this.debugCheckBox);
            this.Controls.Add(this.outputComboBox);
            this.Controls.Add(this.algorithmComboBox);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.clearOutputButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.imageFileName);
            this.Controls.Add(this.LoadImageButton);
            this.Location = new System.Drawing.Point(10, 10);
            this.Name = "INFOIBV";
            this.ShowIcon = false;
            this.Text = "INFOIBV 2011/12 - Clock Face Detection - Paul Scharf (3683745) ";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoadImageButton;
        private System.Windows.Forms.OpenFileDialog openImageDialog;
        private System.Windows.Forms.TextBox imageFileName;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.SaveFileDialog saveImageDialog;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ComboBox algorithmComboBox;
        private System.Windows.Forms.Button clearOutputButton;
        private System.Windows.Forms.ComboBox outputComboBox;
        private System.Windows.Forms.CheckBox debugCheckBox;
        private System.Windows.Forms.CheckBox stepsCheckBox;

    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

// This file was modified for INFOIBV 2011/12 by Paul Scharf (3683745)

namespace INFOIBV
{
    public partial class INFOIBV : Form
    {
        private Bitmap InputImage;
        // we dont use outputimage anymore
        //private Bitmap OutputImage;

        public INFOIBV()
        {
            InitializeComponent();
            fillAlgoBoxWithAlgos();
        }

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
           if (openImageDialog.ShowDialog() == DialogResult.OK)             // Open File Dialog
            {
                string file = openImageDialog.FileName;                     // Get the file name
                imageFileName.Text = file;                                  // Show file name
                if (InputImage != null) InputImage.Dispose();               // Reset image
                InputImage = new Bitmap(file);                              // Create new Bitmap from file
                if (InputImage.Size.Height <= 0 || InputImage.Size.Width <= 0 ||
                    InputImage.Size.Height > 512 || InputImage.Size.Width > 512) // Dimension check
                    MessageBox.Show("Error in image dimensions (have to be > 0 and <= 512)");
                else
                    pictureBox1.Image = (Image) InputImage;                 // Display input image
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (InputImage == null) return;                                 // Get out if no input image

            // new code here:
            // make sure the algorithm selector has an algorithm selected
            if (algorithmComboBox.SelectedItem == null || !(algorithmComboBox.SelectedItem is Algorithm))
            {
                MessageBox.Show("Please select an algorithm to apply.");
                return; // exit if no algorithm selected
            }
            // get the algorithm from the combo box
            Algorithm algo = (Algorithm)algorithmComboBox.SelectedItem;

            // we dont use outputimage anymore
            //if (OutputImage != null) OutputImage.Dispose();                 // Reset output image
            //OutputImage = new Bitmap(InputImage.Size.Width, InputImage.Size.Height); // Create new output image

            // Setup progress bar
            progressBar.Visible = true;
            progressBar.Minimum = 1;
            progressBar.Maximum = InputImage.Size.Width * InputImage.Size.Height;
            progressBar.Value = 1;
            progressBar.Step = 1;

            //==========================================================================================
            // give progress bad and picture box to algorithm in case it wants to use them
            // to show progress
            algo.GiveProgressBar(progressBar);
            algo.GivePreviewPictureBox(pictureBox2);
            // execute algorithm on input image with selected parameters
            Output output = algo.Execute(InputImage, debugCheckBox.Checked, stepsCheckBox.Checked);

            // add a headline for the current image into the combobox
            outputComboBox.Items.Add(System.IO.Path.GetFileName(imageFileName.Text) + ":"); 
            // add all output images to the combo box
            output.AddToComboBox(outputComboBox);

            //==========================================================================================
            // outputimage not used anymore
            //pictureBox2.Image = (Image)OutputImage;                         // Display output image
            progressBar.Visible = false;                                    // Hide progress bar#
            outputComboBox.Focus();
        }
        
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image == null) return;                                // Get out if no output image
            if (saveImageDialog.ShowDialog() == DialogResult.OK)
                pictureBox2.Image.Save(saveImageDialog.FileName);                 // Save the output image
        }

        // new code from here:
        private void fillAlgoBoxWithAlgos()
        {
            // add all known algorithms to combo box
            algorithmComboBox.Items.Add(new NegativeAlgorithm());
            algorithmComboBox.Items.Add(new ClockFaceFinderAlgorithm());
            algorithmComboBox.SelectedIndex = 1;
        }

        private void clearOutputButton_Click(object sender, EventArgs e)
        {
            // clears the output combo box
            outputComboBox.SelectedItem = null;
            outputComboBox.Items.Clear();
        }

        private void outputComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            // if no image is selected, set picturebox image to null
            if (outputComboBox.SelectedItem == null || !(outputComboBox.SelectedItem is OutputPart))
            {
                pictureBox2.Image = null;
                return;
            }
            // other wise, set to image
            pictureBox2.Image = (Image)((OutputPart)outputComboBox.SelectedItem).Bitmap;
        }

    }
}

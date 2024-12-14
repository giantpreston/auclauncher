using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AUCLauncher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Create and configure the FolderBrowserDialog
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select the folder where you want to install Among Us Classic";

                // Show the dialog and check if a folder is selected
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    // Set the selected folder path in the text box
                    textBox1.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void button3_Click(object sender, EventArgs e)
    {
        string folderPath = textBox1.Text;

        // Check if the directory is valid
        if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
        {
            MessageBox.Show("Please select a valid folder or check folder permissions.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Define the URL of the zip file
        string zipFileUrl = "https://zipfilelinkhere.com/";

        // Define the path where the zip file will be saved
        string zipFilePath = Path.Combine(folderPath, "au.zip");

        // Create WebClient to download the zip file
        using (WebClient client = new WebClient())
        {
            try
            {
                // Show a progress message to the user
                MessageBox.Show("Starting download...", "Please Wait", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Download the zip file and save it to the selected folder
                client.DownloadFile(zipFileUrl, zipFilePath);

                // Inform the user that the download was successful
                MessageBox.Show($"Download complete! File saved to: {zipFilePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the download
                MessageBox.Show($"An error occurred while downloading the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

}
}

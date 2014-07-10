using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace Gold_File_Mover_1
{
    public partial class Form1 : Form
    {
        string strFolder = "";
        string[] fileHolder;

        public void SplitAndCopy(string inFolder, string[] paths)
        {
            //takes in an output folder string and list of filepaths. 
            //Creates subfolders based on the filenames and copies the files into those subfolders

            foreach (string path in paths)
            {
                if (path.Contains("---"))
                {
                    string filename = Path.GetFileName(path);
                    string fileroot = Path.GetDirectoryName(path);

                    //output file path = the file root directory + the filename with "---" replaced by "\"
                    string outPath = Path.Combine(fileroot, filename.Replace("---", @"\"));

                    CreateAndMove(path, outPath);
                }
            }
        }

        public void CreateAndMove(string file, string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            try
            {
                File.Move(file, path);
                listBox1.Items.Add(Path.GetFileName(file) + " was moved to " + Path.GetDirectoryName(path));
            }
            catch (IOException ioex)
            {
                MessageBox.Show(ioex.Message);
            }
        }
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                strFolder = folderBrowserDialog1.SelectedPath;
                toolStripStatusLabel1.Text = strFolder;
            }
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (strFolder == null || textBox1.Text == String.Empty)
            {
                MessageBox.Show("Something's missing. Did you select a folder and enter what to separate by?");
            }

            else
            {
                //get a list of filepaths in selected folder.
                fileHolder = Directory.GetFiles(strFolder);

                string[] filePaths = Directory.GetFiles(strFolder);

                SplitAndCopy(strFolder, filePaths);

                toolStripStatusLabel1.Text = "Move Complete. Drop another folder of audio files to run again.";
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            strFolder = String.Empty;
            listBox1.Items.Clear();

            foreach (var s in (string[])e.Data.GetData(DataFormats.FileDrop, false))
            {
                if(Directory.Exists(s))
                    fileHolder = Directory.GetFiles(s);
                else
                    MessageBox.Show("Must drag a FOLDER of audio files.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (fileHolder.Length > 0)
            {
                strFolder = Directory.GetParent(fileHolder[0]).ToString();
                toolStripStatusLabel1.Text = String.Format("Folder: {0}", strFolder);
                label1.Hide();
            }
            else
            {
                MessageBox.Show("Folder appears to be empty.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
    }
}
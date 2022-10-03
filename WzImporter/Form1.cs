using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WzImporter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void UpdateProgress(string message)
        {
            this.label1.Text = message;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "wz files (*.wz)|*.wz";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    string filePath = openFileDialog.FileName;
                    textBox1.Text = filePath;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "wz files (*.wz)|*.wz";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    string filePath = openFileDialog.FileName;
                    textBox2.Text = filePath;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Show the FolderBrowserDialog.
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string folderName = folderBrowserDialog1.SelectedPath;
                textBox3.Text = folderName;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
                return;

            if (!checkBox1.Checked && !checkBox2.Checked && !checkBox3.Checked && !checkBox4.Checked
                && !checkBox5.Checked && !checkBox6.Checked && !checkBox7.Checked && !checkBox8.Checked
                && !checkBox9.Checked && !checkBox10.Checked && !checkBox11.Checked && !checkBox12.Checked && !checkBox13.Checked)
                return;

            Import import = new Import();
            import.cashOnly = checkBox15.Checked;
            import.selections[0] = checkBox1.Checked;
            import.selections[1] = checkBox2.Checked;
            import.selections[2] = checkBox3.Checked;
            import.selections[3] = checkBox4.Checked;
            import.selections[4] = checkBox5.Checked;
            import.selections[5] = checkBox6.Checked;
            import.selections[6] = checkBox7.Checked;
            import.selections[7] = checkBox8.Checked;
            import.selections[8] = checkBox9.Checked;
            import.selections[9] = checkBox10.Checked;
            import.selections[10] = checkBox11.Checked;
            import.selections[11] = checkBox12.Checked;
            import.selections[12] = checkBox13.Checked;
            import.selections[13] = checkBox14.Checked;
            import.inputToFileName = textBox1.Text;
            import.inputFromFileName = textBox2.Text;
            import.outputDir = textBox3.Text;

            button1.Enabled = false;

            import.ImportXML(this);

            button1.Enabled = true;
        }
    }
}

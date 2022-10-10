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
            this.Update();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "wz files (*.wz)|Character.wz";
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
                openFileDialog.Filter = "wz files (*.wz)|Character.wz";
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
                && !checkBox9.Checked && !checkBox10.Checked && !checkBox11.Checked && !checkBox12.Checked
                && !checkBox13.Checked && !checkBox14.Checked && !checkBox17.Checked)
                return;

            string message = "Importing is a memory-intensive operation. Depending on selections, importing may take a *very* long time.\r\n" +
                "Please close any open WZ files prior to continuing.\r\n" +
                "Do you wish to continue?";
            string title = "Confirm Operation";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.No)
            {
                return;
            }
            else
            {
                Import import = new Import();
                import.cashOnly = checkBox15.Checked;
                import.includeString = checkBox16.Checked;
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
                import.onlyThese = checkBox17.Checked;

                if (checkBox17.Checked)
                {
                    List<string> list = new List<string>();
                    foreach (string item in textBox4.Lines)
                    {
                        string i = item.Trim().ToLower();
                        if (!Int32.TryParse(i, out _))
                            continue;
                        while (i.Length < 8)
                            i = "0" + i;
                        if (i.Length > 8)
                            continue;
                        if (!i.EndsWith(".img"))
                            i = i + ".img";
                        list.Add(i);
                    }
                    if (list.Count < 1)
                    {
                        MessageBox.Show("Invalid image choices. Please include the ids of each item you wish to import on a separate line.", "Error");
                        return;
                    }
                    import.onlyTheseItems = list;
                }

                button1.Enabled = false;

                import.ImportXML(this);

                button1.Enabled = true;
            }
        }

        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox17.Checked)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                checkBox6.Checked = false;
                checkBox7.Checked = false;
                checkBox8.Checked = false;
                checkBox9.Checked = false;
                checkBox10.Checked = false;
                checkBox11.Checked = false;
                checkBox12.Checked = false;
                checkBox13.Checked = false;
                checkBox14.Checked = false;
                checkBox1.Enabled = false;
                checkBox2.Enabled = false;
                checkBox3.Enabled = false;
                checkBox4.Enabled = false;
                checkBox5.Enabled = false;
                checkBox6.Enabled = false;
                checkBox7.Enabled = false;
                checkBox8.Enabled = false;
                checkBox9.Enabled = false;
                checkBox10.Enabled = false;
                checkBox11.Enabled = false;
                checkBox12.Enabled = false;
                checkBox13.Enabled = false;
                checkBox14.Enabled = false;
                checkBox15.Checked = false;
                checkBox15.Enabled = false;
                textBox4.Enabled = true;
            }
            else
            {
                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
                checkBox3.Enabled = true;
                checkBox4.Enabled = true;
                checkBox5.Enabled = true;
                checkBox6.Enabled = true;
                checkBox7.Enabled = true;
                checkBox8.Enabled = true;
                checkBox9.Enabled = true;
                checkBox10.Enabled = true;
                checkBox11.Enabled = true;
                checkBox12.Enabled = true;
                checkBox13.Enabled = true;
                checkBox14.Enabled = true;
                checkBox15.Enabled = true;
                textBox4.Enabled = false;
            }
        }
    }
}

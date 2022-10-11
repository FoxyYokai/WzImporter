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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public void UpdateProgress(string message)
        {
            this.label_Progress.Text = message;
            this.Update();
        }

        private void button_ImportTo_Click(object sender, EventArgs e)
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
                    textBox_ImportToFile.Text = filePath;
                }
            }
        }

        private void button_ImportFrom_Click(object sender, EventArgs e)
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
                    textBox_ImportFromFile.Text = filePath;
                }
            }
        }

        private void button_OutputFolder_Click(object sender, EventArgs e)
        {
            // Show the FolderBrowserDialog.
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string folderName = folderBrowserDialog1.SelectedPath;
                textBox_OutputFolder.Text = folderName;
            }
        }

        private void button_Import_Click(object sender, EventArgs e)
        {
            if (textBox_ImportToFile.Text == "" || textBox_ImportFromFile.Text == "" || textBox_OutputFolder.Text == "")
                return;

            if (!checkBox_Hair.Checked && !checkBox_Face.Checked && !checkBox_Cap.Checked && !checkBox_Coat.Checked
                && !checkBox_Pants.Checked && !checkBox_Longcoat.Checked && !checkBox_Shoes.Checked && !checkBox_Cape.Checked
                && !checkBox_Shield.Checked && !checkBox_Ring.Checked && !checkBox_Accessory.Checked && !checkBox_PetEquip.Checked
                && !checkBox_Glove.Checked && !checkBox_Weapon.Checked && !checkBox_OnlyThese.Checked)
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
                import.cashOnly = checkBox_CashOnly.Checked;
                import.includeString = checkBox_IncludeString.Checked;
                import.selections[0] = checkBox_Hair.Checked;
                import.selections[1] = checkBox_Face.Checked;
                import.selections[2] = checkBox_Cap.Checked;
                import.selections[3] = checkBox_Coat.Checked;
                import.selections[4] = checkBox_Pants.Checked;
                import.selections[5] = checkBox_Longcoat.Checked;
                import.selections[6] = checkBox_Shoes.Checked;
                import.selections[7] = checkBox_Cape.Checked;
                import.selections[8] = checkBox_Shield.Checked;
                import.selections[9] = checkBox_Ring.Checked;
                import.selections[10] = checkBox_Accessory.Checked;
                import.selections[11] = checkBox_PetEquip.Checked;
                import.selections[12] = checkBox_Glove.Checked;
                import.selections[13] = checkBox_Weapon.Checked;
                import.inputToFileName = textBox_ImportToFile.Text;
                import.inputFromFileName = textBox_ImportFromFile.Text;
                import.outputDir = textBox_OutputFolder.Text;
                import.onlyThese = checkBox_OnlyThese.Checked;

                if (checkBox_OnlyThese.Checked)
                {
                    List<string> list = new List<string>();
                    foreach (string item in textBox_OnlyThese.Lines)
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

                button_Import.Enabled = false;

                import.ImportXML(this);

                button_Import.Enabled = true;
            }
        }

        private void checkBox_OnlyThese_Changed(object sender, EventArgs e)
        {
            if (checkBox_OnlyThese.Checked)
            {
                checkBox_Hair.Checked = false;
                checkBox_Face.Checked = false;
                checkBox_Cap.Checked = false;
                checkBox_Coat.Checked = false;
                checkBox_Pants.Checked = false;
                checkBox_Longcoat.Checked = false;
                checkBox_Shoes.Checked = false;
                checkBox_Cape.Checked = false;
                checkBox_Shield.Checked = false;
                checkBox_Ring.Checked = false;
                checkBox_Accessory.Checked = false;
                checkBox_PetEquip.Checked = false;
                checkBox_Glove.Checked = false;
                checkBox_Weapon.Checked = false;
                checkBox_Hair.Enabled = false;
                checkBox_Face.Enabled = false;
                checkBox_Cap.Enabled = false;
                checkBox_Coat.Enabled = false;
                checkBox_Pants.Enabled = false;
                checkBox_Longcoat.Enabled = false;
                checkBox_Shoes.Enabled = false;
                checkBox_Cape.Enabled = false;
                checkBox_Shield.Enabled = false;
                checkBox_Ring.Enabled = false;
                checkBox_Accessory.Enabled = false;
                checkBox_PetEquip.Enabled = false;
                checkBox_Glove.Enabled = false;
                checkBox_Weapon.Enabled = false;
                checkBox_CashOnly.Checked = false;
                checkBox_CashOnly.Enabled = false;
                textBox_OnlyThese.Enabled = true;
            }
            else
            {
                checkBox_Hair.Enabled = true;
                checkBox_Face.Enabled = true;
                checkBox_Cap.Enabled = true;
                checkBox_Coat.Enabled = true;
                checkBox_Pants.Enabled = true;
                checkBox_Longcoat.Enabled = true;
                checkBox_Shoes.Enabled = true;
                checkBox_Cape.Enabled = true;
                checkBox_Shield.Enabled = true;
                checkBox_Ring.Enabled = true;
                checkBox_Accessory.Enabled = true;
                checkBox_PetEquip.Enabled = true;
                checkBox_Glove.Enabled = true;
                checkBox_Weapon.Enabled = true;
                checkBox_CashOnly.Enabled = true;
                textBox_OnlyThese.Enabled = false;
            }
        }
    }
}

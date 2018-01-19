using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Obfuscate
{
    public partial class Form1 : Form
    {
        string btnText;
        bool blnInclude;
        string[] excludedFileList;
        public Form1()
        {
            InitializeComponent();
            Text += My.Version;
            btnText = btnGo.Text;
            btnGo.Text = btnText.Replace("xxx", "");
			txtPath.Text = Properties.Settings.Default.InitialPath;
#if DEBUG
            btnTest.Visible = true; //V118
#endif
        }

		private void btnPath_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = txtPath.Text;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        public void txtPath_TextChanged(object sender, EventArgs e)
        {
            if (txtPath.Text.Length == 0) return;
            FillListbox();
			txtOutputPath.Text = getOutputFilename(this.checkedListBox1.Items);
        }

		private string getOutputFilename(CheckedListBox.ObjectCollection objectCollection) //V111
		{
			string outputFilename = "Obfuscated";
			foreach (string item in objectCollection)
			{
				if (item.EndsWith(".exe"))
				{
					string exe = item.Substring(item.LastIndexOf('\\'));
					return outputFilename+exe;
				}
			}
            return ""; //outputFilename;
		}

        private void FillListbox()
        {
            status(" start.");
            checkedListBox1.BeginUpdate();
            checkedListBox1.Items.Clear();
            chkCheckAll.Checked = false;
            int start = Environment.TickCount;
            /// do processing
            AddFilesToListbox(txtPath.Text);
            int eind = Environment.TickCount;
            Console.WriteLine("Elapsed time: {0} ms", eind - start);
            chkCheckAll.Checked = true;
            checkedListBox1.EndUpdate();
            if ( checkedListBox1.Items.Count == 0)
            {
                status("No files found: {0}\\{1}", txtPath.Text, txtFileExtension.Text);
            }
            else status("");
        }

        private void SetAllItemsChecked(CheckedListBox checkedListBox1, bool chk)
        {
            //
            // Remove eventhandler to prohibit DisplayCount updates during for loop.
            //
            checkedListBox1.ItemCheck -= new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, chk);
            }
            //
            // Restore eventhandler
            //
            checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);

            DisplayCount(0);
        }

        private void AddFilesToListbox(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                status("Error: Directory doesn't exist.");
            }
            else
            {
                excludedFileList = txtExclude.Text.ToLower().Split(new char[]{';'},StringSplitOptions.RemoveEmptyEntries);
                foreach (string extension in txtFileExtension.Text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                foreach (FileInfo file in di.GetFiles(extension)) if (blnInclude ^ !IsInList(file.FullName))
                {
                    checkedListBox1.Items.Add(file.FullName);
                }
                //
                // recurse through subdirectories.
                //
                if (chkRecursive.Checked) foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    AddFilesToListbox(dir.FullName);
                }
            }
        }

        private bool IsInList(string file)
        {
            foreach (string exclude in excludedFileList)
            {
                if (file.ToLower().Contains(exclude)) return true;
            }
            return false;
        }

        private void chkRecursive_CheckedChanged(object sender, EventArgs e)
        {
            FillListbox();
        }

        private void chkCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            SetAllItemsChecked(checkedListBox1, chkCheckAll.Checked);
        }

        private void txtFileExtension_TextChanged(object sender, EventArgs e)
        {
            FillListbox();
        }

        public void btnGo_Click(object sender, EventArgs e)
        {
            int count = checkedListBox1.CheckedItems.Count;
            toolStripProgressBar1.Maximum = count;
            Dictionary<string, string> badConversions = new Dictionary<string, string>();
            int errorCount = 0;
            Convert convert = new Convert();
            convert.Init(getRootedOutputPath());//V121
            for (int i = 0; i < count; i++)
			{
                toolStripProgressBar1.Value = i;
                string file = checkedListBox1.CheckedItems[i].ToString();
                My.Log("file {0}",file);
                try
                {
                    convert.ConvertFile( file);
                }
                catch(Exception err)
                {
                    MessageBox.Show(file + ": " + err.Message,this.Text); //V116+
                    errorCount++;
                }
            }
            toolStripProgressBar1.Value = 0;
            if ( errorCount > 0 )
            {
                status("{0} file(s) with errors during processing.",errorCount);
            }
            else
            {
                status("All files succesfull processed.");
            }
			//
			// Compile everything.
			//
            string result = convert.Result(this);
            int p = result.IndexOf("Error");
            if (p>=0)
            {
                status("");
                MessageBox.Show(result,this.Text); //V116+
                //status(result.Substring(p));
            }
        }

        //
        // Show message on status bar.
        // 
        // Error message are displayed until a clear is forced with msg: " "  
        //
        private void status(string format, params object[] args)
        {
            status(string.Format(format, args));
        }
        private void status(string msg)
        {
            if (!msg.StartsWith(" ") && toolStripStatusLabel1.Text.StartsWith("Error")) return;//V102
            toolStripStatusLabel1.Text = msg;
            My.Log(msg);
            Application.DoEvents();
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            DisplayCount(e.NewValue==CheckState.Checked?1:-1);
        }

        private void DisplayCount(int offset)
        {
            int count = checkedListBox1.CheckedItems.Count+offset;
            if (count > 0)
                btnGo.Text = btnText.Replace("xxx", count.ToString());
            else
                btnGo.Text = "Check one or more files.";
            btnGo.Enabled = count > 0;
        }

        private void txtExclude_TextChanged(object sender, EventArgs e)
        {
            FillListbox();
        }

        private void label1_Click(object sender, EventArgs e) // V102
        {
            blnInclude = !blnInclude;
            label1.Text =((blnInclude)?"Include":"Exclude")+" files";
            FillListbox();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.InitialPath = txtPath.Text;
            Properties.Settings.Default.Save();
        }

		private void txtOutputPath_MouseDoubleClick(object sender, MouseEventArgs e) //V108+
		{
            string dirPath = Path.GetDirectoryName(((TextBox)sender).Text);
            //string dirPath = ((TextBox)sender).Text;
			if (Path.GetPathRoot(dirPath) == "")
			{
				dirPath = txtPath.Text + "\\"+dirPath;
			}

			Process.Start("Explorer", dirPath);
		}

        private void btnTest_Click(object sender, EventArgs e)
        {
            string assemblyFile = getRootedOutputPath(); //V121
            Directory.SetCurrentDirectory(Path.GetDirectoryName(assemblyFile));
            ល.ល.Main(new string[] {"test",assemblyFile});
        }

        private string getRootedOutputPath()
        {
            string path;
            if (Path.IsPathRooted(txtOutputPath.Text))
                path = txtOutputPath.Text;
            else
                path = txtPath.Text + "\\" + txtOutputPath.Text;
            return path;
        }
    }
    public interface IConvert
    {
        void Init(string outputPath);
        void ConvertFile(string file);
        string Result(Form1 form1);
    }
}

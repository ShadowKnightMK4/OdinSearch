using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using OdinSearchEngine;
using OdinSearchEngine.OdinSearch_OutputConsumerTools;

namespace OdinSearchWin
{

    public partial class LandingForm : Form
    {

        OdinSearch SearchEngine = new();
        SearchComLinkage Linkme = new();

        void ExploreTarget(string FileOrFolder)
        {
            if (Directory.Exists(FileOrFolder) || (File.Exists(FileOrFolder)))
                Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe"), string.Format("/select,\"{0}\"", FileOrFolder));
            else
                throw new FileNotFoundException(FileOrFolder);
        }
        public LandingForm()
        {

            InitializeComponent();
        }

        VisualLogWindow VisualLogWindow = new();
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void newSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var SearchDialog = new NewSearchWindow())
            {
                SearchDialog.ShowDialog(this);
                if (SearchDialog.DialogResult == DialogResult.OK)
                {


                    this.SearchEngine.AddSearchAnchor(SearchDialog.SearchAnchor);
                    this.SearchEngine.AddSearchTarget(SearchDialog.SearchTarget);
                    Linkme.SetCustomParameter(SearchComLinkage.ListBoxToStoreMessages, VisualLogWindow.ListBoxLog);
                    Linkme.SetCustomParameter(SearchComLinkage.ListBoxToStoreResults, this.ListBoxSearchResults);
                    SearchEngine.Search(Linkme);
                }
            }
        }

        private void LandingForm_Load(object sender, EventArgs e)
        {

        }

        private void viewLogStreamToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (viewLogStreamToolStripMenuItem.Checked)
            {
                VisualLogWindow.Show(this);
            }
            else
            {
                VisualLogWindow.Visible = false;
            }

        }

        private void openExplorerToThisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //   FileSystemInfo Item = ListBoxSearchResults.Items[ListBoxSearchResults.IndexFromPoint(Cursor.Position.X, Cursor.Position.Y)] as FileSystemInfo;

            {
                foreach (FileSystemInfo Item in ListBoxSearchResults.SelectedItems)
                {
                    ExploreTarget(Item.FullName);
                }
            }
        }


        bool RightDown = false;
        private void ListBoxSearchResults_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void ListBoxSearchResults_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                RightDown = true;
            }
        }

        private void ListBoxSearchResults_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Right) && (RightDown == true))
            {
                RightDown = false;
                ContextMenuStripRightClickList.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }

        private void TimerUpdateSearchResults_Tick(object sender, EventArgs e)
        {

        }

        private void copyFileOrFolderPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ListBoxSearchResults.SelectedItem != null)
            {
                Clipboard.SetText(ListBoxSearchResults.SelectedItem.ToString());
            }
        }

        private void viewLogStreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VisualLogWindow.Visible = true;
        }
    }


    /// <summary>
    /// Bridges gap between the OdinSearch stuff and the displaying of results
    /// </summary>
    class SearchComLinkage : OdinSearch_OutputConsumerBase
    {
        public const string ListBoxToStoreMessages = "LISTBOXLOG";
        public const string ListBoxToStoreResults = "LISTBOXLOGSEARCHRESULTS";
        ListBox ListBoxLog;
        ListBox ListBoxSearchStuff;
        //public List<FileSystemInfo> ListBoxSearchResults = new();
        public ListBox.ObjectCollection ListBoxSearchResults;
        public SearchComLinkage()
        {
            SetCustomParameter(ListBoxToStoreMessages, null);
            SetCustomParameter(ListBoxToStoreResults, null);
        }

        public override bool SearchBegin(DateTime Start)
        {
            if (GetCustomParameter(ListBoxToStoreMessages) == null)
            {
                throw new ArgumentException("CUSTOMARG: LISTBOXLOG");
            }

            if (GetCustomParameter(ListBoxToStoreResults) == null)
            {
                throw new ArgumentException("CUSTOMARG: LISTBOXSEARCHSTUFF");
            }
            ListBoxLog = (ListBox)GetCustomParameter(ListBoxToStoreMessages);
            ListBoxSearchStuff = (ListBox)GetCustomParameter(ListBoxToStoreResults);

            ListBoxSearchResults = ListBoxSearchStuff.Items;

            if (ListBoxLog.InvokeRequired)
            {
                ListBoxLog.Invoke(AddListBox, new object[] { ListBoxLog, "Starting Search..." });
            }
            else
            {
                ListBoxLog.Items.Add("Starting Search...");
            }
            return true;
        }

        void AddListBoxLog(ListBox.ObjectCollection a, object thing)
        {
            a.Add(thing);
        }

        void AddListBox(ListBox a, object thing)
        {
            a.Items.Add(thing);
        }
        void RefreshListBoX(ListBox a)
        {

            a.DataSource = ListBoxSearchResults;
        }
        public override void Match(FileSystemInfo info)
        {

            lock (ListBoxSearchStuff)
            {
                ListBoxSearchStuff.Invoke(AddListBoxLog, new object[] { ListBoxSearchResults, info });
            }

        }

        public override void Messaging(string Message)
        {
            lock (ListBoxLog)
            {
                ListBoxLog.Invoke(AddListBox, new object[] { ListBoxLog, Message });
            }
        }
    }
}

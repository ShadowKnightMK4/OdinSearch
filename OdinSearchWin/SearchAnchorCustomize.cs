using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OdinSearchEngine;
namespace OdinSearchWin
{
    public partial class SearchAnchorCustomize : Form
    {
        public bool ResultWantSubFolders = false;
        public SearchAnchor Results;

        public SearchAnchorCustomize()
        {
            InitializeComponent();
        }

        private void ButtonAddFolder_Click(object sender, EventArgs e)
        {
            if (FolderBrowserDialogAddAnchor.ShowDialog(this) == DialogResult.OK)
            {
                int index = CheckedListBoxSearchAnchors.Items.IndexOf(FolderBrowserDialogAddAnchor.SelectedPath);
                if (index == -1)
                {
                    CheckedListBoxSearchAnchors.Items.Add(FolderBrowserDialogAddAnchor.SelectedPath);
                    index = CheckedListBoxSearchAnchors.Items.IndexOf(FolderBrowserDialogAddAnchor.SelectedPath);
                    CheckedListBoxSearchAnchors.SetItemChecked(index, true);
                }
            }
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;


            if (!CheckBoxAllLocalDrivesDefault.Checked)
            {
                Results = new SearchAnchor(false);
                for (int step = 0; step < CheckedListBoxSearchAnchors.CheckedItems.Count; step++)
                {
                    Results.AddAnchor(CheckedListBoxSearchAnchors.CheckedItems[step].ToString());
                }
            }
            else
            {
                Results = new SearchAnchor(true);
            }
            Results.EnumSubFolders = CheckBoxLookSubFolders.Checked;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void SearchAnchorCustomize_Load(object sender, EventArgs e)
        {

        }

        private void CheckBoxAllLocalDrivesDefault_CheckedChanged(object sender, EventArgs e)
        {
            CheckedListBoxSearchAnchors.Enabled = !CheckBoxAllLocalDrivesDefault.Checked;
            ButtonAddFolder.Enabled = !CheckBoxAllLocalDrivesDefault.Checked;

        }
    }
}

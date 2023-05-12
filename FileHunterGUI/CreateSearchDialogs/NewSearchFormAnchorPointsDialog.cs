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
namespace FileHunterGUI
{
    public partial class NewSearchFormAnchorPointsDialog : Form
    {
        public NewSearchFormAnchorPointsDialog()
        {
            InitializeComponent();
            DialogResult = DialogResult.No;
        }

        public SearchAnchor SearchAnchor = null;
        
        
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void AddLocationButton_Click(object sender, EventArgs e)
        {
            if (ChooseAnchorFolderDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (!CheckBoxListContainer.Items.Contains(ChooseAnchorFolderDialog.SelectedPath))
                {
                    var loc = CheckBoxListContainer.Items.Add(ChooseAnchorFolderDialog.SelectedPath);
                    CheckBoxListContainer.SetItemChecked(loc, true);
                }
                else
                {
                    var loc = CheckBoxListContainer.Items.Contains(ChooseAnchorFolderDialog.SelectedPath);
                    if (loc)
                    {
                        CheckBoxListContainer.SetItemChecked(CheckBoxListContainer.Items.IndexOf(ChooseAnchorFolderDialog.SelectedPath), true);
                    }
                }

            }

        }

        private void NewSearchForm_Load(object sender, EventArgs e)
        {

        }

        private void ClearSelectedItems_click(object sender, EventArgs e)
        {
            List<object> list = new List<object>();
            for (int step = 0; step < CheckBoxListContainer.Items.Count; step++)
            {
                if ((CheckBoxListContainer.Items[step] != null) && (CheckBoxListContainer.GetItemChecked(step) == true))
                {
                    list.Add(CheckBoxListContainer.Items[step]);
                }
            }
            foreach (object a in list)
            {
                CheckBoxListContainer.Items.Remove(a);
            }


        }

        private void CheckBoxCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int step = 0; step < CheckBoxListContainer.Items.Count; step++)
            {
                if ((CheckBoxListContainer.Items[step] != null))
                {
                    CheckBoxListContainer.SetItemChecked(step, CheckBoxCheckAll.Checked);
                }

            }
        }

        private void NewAnchorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
            {
                e.Cancel = (MessageBox.Show("Are you sure you want to close this form and discard your adjusted settings?", "Close Form?", MessageBoxButtons.YesNo) == DialogResult.No);
            }
        }

        private void ButtonCloseWindow_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            SearchAnchor = null;
            Close();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void ButtonAccept_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            if (this.CheckBoxChooseAllLocalPaths.Checked)
            {
                SearchAnchor = new SearchAnchor();
            }
            else
            {
                SearchAnchor = new SearchAnchor(false);
                if (CheckBoxListContainer.CheckedItems.Count > 0)
                {
                    for (int step = 0; step < CheckBoxListContainer.CheckedItems.Count; step++)
                    {
                        SearchAnchor.AddAnchor(CheckBoxListContainer.CheckedItems[step].ToString());
                    }
                }
            }
            Close();
        }

        private void CheckBoxChooseAllLocalPaths_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxListContainer.Enabled = !CheckBoxChooseAllLocalPaths.Checked;
            CheckBoxCheckAll.Enabled = !CheckBoxChooseAllLocalPaths.Checked;
            ButtonClearChecked.Enabled = !CheckBoxChooseAllLocalPaths.Checked;
            ButtonAddLocation.Enabled = !CheckBoxChooseAllLocalPaths.Checked;
        }
    }
}
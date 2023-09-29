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
    public partial class NewSearchWindow : Form
    {
        public SearchTarget SearchTarget = new SearchTarget();
        public SearchAnchor SearchAnchor;
        public NewSearchWindow()
        {
            InitializeComponent();
            SearchTarget.FileNameMatching = SearchTarget.MatchStyleString.MatchAny;
        }


        private void NewSearchWindow_Load(object sender, EventArgs e)
        {
            LabelCurrentSearchStyle.Text = '[' + SearchTarget.FileNameMatching.ToString() + ']';
        }

        private void ButtonUpdateFileNameMatchStyle_click(object sender, EventArgs e)
        {
            using (var UpdateSearchSetting = new CustomizeMatchStyleString())
            {
                UpdateSearchSetting.MatchStyle = SearchTarget.FileNameMatching;
                UpdateSearchSetting.ShowDialog(this, "File names not included folder");
                if (UpdateSearchSetting.DialogResult == DialogResult.OK)
                {
                    SearchTarget.FileNameMatching = UpdateSearchSetting.MatchStyle;
                    LabelCurrentSearchStyle.Text = '[' + SearchTarget.FileNameMatching.ToString() + ']';
                }
            }
        }

        private void ButtonBeginSearch_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            SearchTarget.FileName.Add(this.TextBoxFileName.Text);
            SearchAnchor.roots.Clear();
            for (int step =0; step < CheckedListBoxUserSearchAnchors.CheckedItems.Count;step++)
            {
                SearchAnchor.roots.Add(new DirectoryInfo(CheckedListBoxUserSearchAnchors.CheckedItems[step].ToString()));
            }
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void TextBoxFileName_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void CheckedListBoxUserSearchAnchors_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ButtonCustomizeSearchAnchor_Click(object sender, EventArgs e)
        {
            using (var AnchorDialog = new SearchAnchorCustomize())
            {
                if (AnchorDialog.ShowDialog(this) == DialogResult.OK)
                {
                    SearchAnchor = AnchorDialog.Results;
                    CheckedListBoxUserSearchAnchors.Items.Clear();
                    foreach (DirectoryInfo i in SearchAnchor.roots)
                    {
                       CheckedListBoxUserSearchAnchors.SetItemChecked(CheckedListBoxUserSearchAnchors.Items.Add(i.ToString()),true);
                    }
                }
            }
        }
    }
}

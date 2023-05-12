using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileHunterGUI.CreateSearchDialogs
{
    public partial class NewSearchLandingPageDialog : Form
    {
        FileHunterGUI.NewSearchFormAnchorPointsDialog NewAnchors = new NewSearchFormAnchorPointsDialog();
        public NewSearchLandingPageDialog()
        {
            InitializeComponent();
        }

        private void ButtonAnchor_Click(object sender, EventArgs e)
        {
            NewAnchors.CheckBoxListContainer.Items.Clear();
            if (ListBoxAnchors.Items.Count > 0)
            {
                foreach (string s in ListBoxAnchors.Items)
                {
                    NewAnchors.CheckBoxListContainer.Items.Add(s, true);
                }
            }
            NewAnchors.ShowDialog();
            if (NewAnchors.DialogResult == DialogResult.OK)
            {
                if (NewAnchors.CheckBoxNoOverrideAnchors.Checked == true)
                {

                }
                else
                {
                    ListBoxAnchors.Items.Clear();
                    ListBoxAnchors.Tag = NewAnchors.Anchor;
                    foreach (DirectoryInfo N in NewAnchors.SearchAnchor.roots)
                    {
                        ListBoxAnchors.Items.Add(N.FullName);
                    }
                }

            }
        }
    }
}

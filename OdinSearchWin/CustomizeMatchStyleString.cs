using OdinSearchEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OdinSearchWin
{
    public partial class CustomizeMatchStyleString : Form
    {
        public OdinSearchEngine.SearchTarget.MatchStyleString MatchStyle;
        public CustomizeMatchStyleString()
        {
            InitializeComponent();
            checkedListBox1.Items.Clear();
            checkedListBox1.Items.Add(SearchTarget.MatchStyleString.MatchAny);
            checkedListBox1.Items.Add(SearchTarget.MatchStyleString.MatchAll);
            checkedListBox1.Items.Add(SearchTarget.MatchStyleString.Invert);
            checkedListBox1.Items.Add(SearchTarget.MatchStyleString.Skip);
            checkedListBox1.Items.Add(SearchTarget.MatchStyleString.CaseImportant);
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            for (int step = 0; step < checkedListBox1.CheckedItems.Count; step++)
            {
                SearchTarget.MatchStyleString AsThis = (SearchTarget.MatchStyleString)checkedListBox1.CheckedItems[step];
                MatchStyle |= AsThis;
            }
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            MatchStyle = 0;
            Close();
        }

        /// <summary>
        /// Show this as a dialog with a title bar text after the usual prompt
        /// </summary>
        /// <param name="Owner"></param>
        /// <param name="TitleBarPostFix"></param>
        public void ShowDialog(IWin32Window Owner, string TitleBarPostFix)
        {
            void SetFlagStatus(SearchTarget.MatchStyleString Flag, int step, SearchTarget.MatchStyleString TestThis)
            {
                checkedListBox1.SetItemChecked(step, (TestThis & Flag) == Flag);
                /*
                if ((TestThis & Flag) == Flag)
                {
                    checkedListBox1.SetItemChecked(step, true);
                }
                else
                {
                    checkedListBox1.SetItemChecked(step, false);
                }*/
            }
            this.Text = "Customize Search String Handling for " + TitleBarPostFix;

            //if ( (MatchStyle & SearchTarget.MatchStyleString.MatchAll) == SearchTarget.MatchStyleString.MatchAll)
            {
                
                {
                    SetFlagStatus(SearchTarget.MatchStyleString.MatchAny, checkedListBox1.Items.IndexOf(SearchTarget.MatchStyleString.MatchAny) , MatchStyle  );
                    SetFlagStatus(SearchTarget.MatchStyleString.MatchAll, checkedListBox1.Items.IndexOf(SearchTarget.MatchStyleString.MatchAll), MatchStyle);
                    SetFlagStatus(SearchTarget.MatchStyleString.Invert, checkedListBox1.Items.IndexOf(SearchTarget.MatchStyleString.Invert), MatchStyle);
                    SetFlagStatus(SearchTarget.MatchStyleString.Skip, checkedListBox1.Items.IndexOf(SearchTarget.MatchStyleString.Skip), MatchStyle);
                    SetFlagStatus(SearchTarget.MatchStyleString.CaseImportant, checkedListBox1.Items.IndexOf(SearchTarget.MatchStyleString.CaseImportant), MatchStyle);
                }
            }
          
            ShowDialog(Owner);
        }

        private void CustomizeMatchStyleString_Load(object sender, EventArgs e)
        {

        }
    }
}

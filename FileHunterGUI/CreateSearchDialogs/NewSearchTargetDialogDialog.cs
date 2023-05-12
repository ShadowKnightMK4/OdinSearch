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
namespace FileHunterGUI.CreateSearchDialogs
{
    public partial class NewSearchTargetDialogDialog : Form
    {
        public NewSearchTargetDialogDialog()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void NewSearchTargetDialogDialog_Load(object sender, EventArgs e)
        {
            var EnumContains1 = Enum.GetNames(typeof(SearchTarget.MatchStyleString));
            foreach (string entry in EnumContains1)
            {
               ComboBoxMatchStyle1.Items.Add(entry);
            }
            var EnumContains2 = Enum.GetNames(typeof(FileAttributes));

            foreach (string entry in EnumContains2)
            {
                CheckedListBoxAttribList1.Items.Add(entry);
            }

        }
    }
}

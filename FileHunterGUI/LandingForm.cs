using FileHunterGUI.CreateSearchDialogs;

namespace FileHunterGUI
{
    public partial class LandingForm : Form
    {
        public LandingForm()
        {
            InitializeComponent();
        }

        NewSearchFormAnchorPointsDialog CreateSearchForm_AnchorPointWindow = null;
        NewSearchTargetDialogDialog CreateSearchForm_SearchTargetWindow = null;
        NewSearchLandingPageDialog CreateSearchForm_LandingWindow = null;
        private void showNewSearchDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAnchorPointWindow(true);
        }

        void ShowSearchTargetWindow(bool Dialog)
        {

            if (CreateSearchForm_SearchTargetWindow == null)
            {
                CreateSearchForm_SearchTargetWindow = new NewSearchTargetDialogDialog();
            }
            if (Dialog)
            {
                CreateSearchForm_SearchTargetWindow.ShowDialog();
                return;
            }
            CreateSearchForm_SearchTargetWindow.Show();
        }
        void ShowAnchorPointWindow(bool Dialog)
        {
            if (CreateSearchForm_AnchorPointWindow == null)
            {
                CreateSearchForm_AnchorPointWindow = new NewSearchFormAnchorPointsDialog();

            }
            if (Dialog)
            {
                CreateSearchForm_AnchorPointWindow.ShowDialog();
                return;
            }
            CreateSearchForm_AnchorPointWindow.Show();
            return;
        }

        void ShowSearchNewSearchWindow(bool Dialog)
        {
            if (CreateSearchForm_LandingWindow == null)
            {
                CreateSearchForm_LandingWindow = new NewSearchLandingPageDialog();
            }
            if (Dialog)
            {
                CreateSearchForm_LandingWindow.ShowDialog();
                return;
            }
            CreateSearchForm_LandingWindow.Show();
            return;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void DebugWindowCollection_ShowAnchorPointWindow_Click(object sender, EventArgs e)
        {
            ShowAnchorPointWindow(true);
        }

        private void DebugWindowCollection_ShowSearchTargetWindow_Click(object sender, EventArgs e)
        {
            ShowSearchTargetWindow(true);
        }

        private void DebugWindowCollection_ShowNewSearchWindow_click(object sender, EventArgs e)
        {
            ShowSearchNewSearchWindow(false);
        }
    }
}
namespace OdinSearchWin
{
    partial class LandingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            ListBoxSearchResults = new ListBox();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exportResultsToolStripMenuItem = new ToolStripMenuItem();
            asListToolStripMenuItem = new ToolStripMenuItem();
            searchToolStripMenuItem = new ToolStripMenuItem();
            newSearchToolStripMenuItem = new ToolStripMenuItem();
            viewLogStreamToolStripMenuItem = new ToolStripMenuItem();
            ContextMenuStripRightClickList = new ContextMenuStrip(components);
            copyFileOrFolderPathToolStripMenuItem = new ToolStripMenuItem();
            openExplorerToThisToolStripMenuItem = new ToolStripMenuItem();
            TimerUpdateSearchResults = new System.Windows.Forms.Timer(components);
            menuStrip1.SuspendLayout();
            ContextMenuStripRightClickList.SuspendLayout();
            SuspendLayout();
            // 
            // ListBoxSearchResults
            // 
            ListBoxSearchResults.FormattingEnabled = true;
            ListBoxSearchResults.ItemHeight = 25;
            ListBoxSearchResults.Location = new Point(12, 72);
            ListBoxSearchResults.Name = "ListBoxSearchResults";
            ListBoxSearchResults.SelectionMode = SelectionMode.MultiExtended;
            ListBoxSearchResults.Size = new Size(776, 354);
            ListBoxSearchResults.TabIndex = 0;
            ListBoxSearchResults.MouseClick += ListBoxSearchResults_MouseClick;
            ListBoxSearchResults.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            ListBoxSearchResults.MouseDown += ListBoxSearchResults_MouseDown;
            ListBoxSearchResults.MouseUp += ListBoxSearchResults_MouseUp;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, searchToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 33);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportResultsToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(54, 29);
            fileToolStripMenuItem.Text = "File";
            // 
            // exportResultsToolStripMenuItem
            // 
            exportResultsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { asListToolStripMenuItem });
            exportResultsToolStripMenuItem.Name = "exportResultsToolStripMenuItem";
            exportResultsToolStripMenuItem.Size = new Size(225, 34);
            exportResultsToolStripMenuItem.Text = "Export Results";
            // 
            // asListToolStripMenuItem
            // 
            asListToolStripMenuItem.Name = "asListToolStripMenuItem";
            asListToolStripMenuItem.Size = new Size(165, 34);
            asListToolStripMenuItem.Text = "As List";
            // 
            // searchToolStripMenuItem
            // 
            searchToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newSearchToolStripMenuItem, viewLogStreamToolStripMenuItem });
            searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            searchToolStripMenuItem.Size = new Size(80, 29);
            searchToolStripMenuItem.Text = "Search";
            searchToolStripMenuItem.Click += searchToolStripMenuItem_Click;
            // 
            // newSearchToolStripMenuItem
            // 
            newSearchToolStripMenuItem.Name = "newSearchToolStripMenuItem";
            newSearchToolStripMenuItem.Size = new Size(270, 34);
            newSearchToolStripMenuItem.Text = "New Search";
            newSearchToolStripMenuItem.Click += newSearchToolStripMenuItem_Click;
            // 
            // viewLogStreamToolStripMenuItem
            // 
            viewLogStreamToolStripMenuItem.CheckOnClick = true;
            viewLogStreamToolStripMenuItem.Name = "viewLogStreamToolStripMenuItem";
            viewLogStreamToolStripMenuItem.Size = new Size(270, 34);
            viewLogStreamToolStripMenuItem.Text = "View Log Stream";
            viewLogStreamToolStripMenuItem.CheckedChanged += viewLogStreamToolStripMenuItem_CheckedChanged;
            viewLogStreamToolStripMenuItem.Click += viewLogStreamToolStripMenuItem_Click;
            // 
            // ContextMenuStripRightClickList
            // 
            ContextMenuStripRightClickList.ImageScalingSize = new Size(24, 24);
            ContextMenuStripRightClickList.Items.AddRange(new ToolStripItem[] { copyFileOrFolderPathToolStripMenuItem, openExplorerToThisToolStripMenuItem });
            ContextMenuStripRightClickList.Name = "contextMenuStrip1";
            ContextMenuStripRightClickList.Size = new Size(274, 68);
            // 
            // copyFileOrFolderPathToolStripMenuItem
            // 
            copyFileOrFolderPathToolStripMenuItem.Name = "copyFileOrFolderPathToolStripMenuItem";
            copyFileOrFolderPathToolStripMenuItem.Size = new Size(273, 32);
            copyFileOrFolderPathToolStripMenuItem.Text = "Copy File or Folder Path";
            copyFileOrFolderPathToolStripMenuItem.Click += copyFileOrFolderPathToolStripMenuItem_Click;
            // 
            // openExplorerToThisToolStripMenuItem
            // 
            openExplorerToThisToolStripMenuItem.Name = "openExplorerToThisToolStripMenuItem";
            openExplorerToThisToolStripMenuItem.Size = new Size(273, 32);
            openExplorerToThisToolStripMenuItem.Text = "Open Explorer to this...";
            openExplorerToThisToolStripMenuItem.Click += openExplorerToThisToolStripMenuItem_Click;
            // 
            // TimerUpdateSearchResults
            // 
            TimerUpdateSearchResults.Enabled = true;
            TimerUpdateSearchResults.Tick += TimerUpdateSearchResults_Tick;
            // 
            // LandingForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(ListBoxSearchResults);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "LandingForm";
            Text = "OdinSearch - File Searcher";
            Load += LandingForm_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ContextMenuStripRightClickList.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox ListBoxSearchResults;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exportResultsToolStripMenuItem;
        private ToolStripMenuItem searchToolStripMenuItem;
        private ToolStripMenuItem newSearchToolStripMenuItem;
        private ToolStripMenuItem viewLogStreamToolStripMenuItem;
        private ToolStripMenuItem asListToolStripMenuItem;
        private ContextMenuStrip ContextMenuStripRightClickList;
        private ToolStripMenuItem copyFileOrFolderPathToolStripMenuItem;
        private ToolStripMenuItem openExplorerToThisToolStripMenuItem;
        private System.Windows.Forms.Timer TimerUpdateSearchResults;
    }
}
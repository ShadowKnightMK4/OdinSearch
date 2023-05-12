namespace FileHunterGUI
{
    partial class LandingForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowCollectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newSearchToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.showSearchTargetCustomizerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showNewSearchWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.windowCollectionToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 33);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSearchToolStripMenuItem,
            this.stopSearchToolStripMenuItem});
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(80, 29);
            this.searchToolStripMenuItem.Text = "&Search";
            // 
            // newSearchToolStripMenuItem
            // 
            this.newSearchToolStripMenuItem.Name = "newSearchToolStripMenuItem";
            this.newSearchToolStripMenuItem.Size = new System.Drawing.Size(208, 34);
            this.newSearchToolStripMenuItem.Text = "&New Search";
            // 
            // stopSearchToolStripMenuItem
            // 
            this.stopSearchToolStripMenuItem.Name = "stopSearchToolStripMenuItem";
            this.stopSearchToolStripMenuItem.Size = new System.Drawing.Size(208, 34);
            this.stopSearchToolStripMenuItem.Text = "&Stop Search";
            // 
            // windowCollectionToolStripMenuItem
            // 
            this.windowCollectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSearchToolStripMenuItem1,
            this.showSearchTargetCustomizerToolStripMenuItem,
            this.showNewSearchWindowToolStripMenuItem});
            this.windowCollectionToolStripMenuItem.Name = "windowCollectionToolStripMenuItem";
            this.windowCollectionToolStripMenuItem.Size = new System.Drawing.Size(177, 29);
            this.windowCollectionToolStripMenuItem.Text = "Window Collection";
            // 
            // newSearchToolStripMenuItem1
            // 
            this.newSearchToolStripMenuItem1.Name = "newSearchToolStripMenuItem1";
            this.newSearchToolStripMenuItem1.Size = new System.Drawing.Size(362, 34);
            this.newSearchToolStripMenuItem1.Text = "Show Anchor Point Customizer";
            this.newSearchToolStripMenuItem1.Click += new System.EventHandler(this.DebugWindowCollection_ShowAnchorPointWindow_Click);
            // 
            // showSearchTargetCustomizerToolStripMenuItem
            // 
            this.showSearchTargetCustomizerToolStripMenuItem.Name = "showSearchTargetCustomizerToolStripMenuItem";
            this.showSearchTargetCustomizerToolStripMenuItem.Size = new System.Drawing.Size(362, 34);
            this.showSearchTargetCustomizerToolStripMenuItem.Text = "Show Search Target Customizer";
            this.showSearchTargetCustomizerToolStripMenuItem.Click += new System.EventHandler(this.DebugWindowCollection_ShowSearchTargetWindow_Click);
            // 
            // showNewSearchWindowToolStripMenuItem
            // 
            this.showNewSearchWindowToolStripMenuItem.Name = "showNewSearchWindowToolStripMenuItem";
            this.showNewSearchWindowToolStripMenuItem.Size = new System.Drawing.Size(362, 34);
            this.showNewSearchWindowToolStripMenuItem.Text = "Show New Search Window";
            this.showNewSearchWindowToolStripMenuItem.Click += new System.EventHandler(this.DebugWindowCollection_ShowNewSearchWindow_click);
            // 
            // LandingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "LandingForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem searchToolStripMenuItem;
        private ToolStripMenuItem newSearchToolStripMenuItem;
        private ToolStripMenuItem stopSearchToolStripMenuItem;
        private ToolStripMenuItem windowCollectionToolStripMenuItem;
        private ToolStripMenuItem newSearchToolStripMenuItem1;
        private ToolStripMenuItem showSearchTargetCustomizerToolStripMenuItem;
        private ToolStripMenuItem showNewSearchWindowToolStripMenuItem;
    }
}
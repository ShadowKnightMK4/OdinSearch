
namespace FileInventoryGUI
{
    partial class EntryPoint
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.openCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excellToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.accessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.searchToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 33);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearResultsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.openCSVToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.toolStripMenuItem2});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // clearResultsToolStripMenuItem
            // 
            this.clearResultsToolStripMenuItem.Name = "clearResultsToolStripMenuItem";
            this.clearResultsToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.clearResultsToolStripMenuItem.Text = "Clear Results";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(267, 6);
            // 
            // openCSVToolStripMenuItem
            // 
            this.openCSVToolStripMenuItem.Name = "openCSVToolStripMenuItem";
            this.openCSVToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.openCSVToolStripMenuItem.Text = "Open CSV";
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.asCSVToolStripMenuItem,
            this.excellToolStripMenuItem,
            this.accessToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // asCSVToolStripMenuItem
            // 
            this.asCSVToolStripMenuItem.Name = "asCSVToolStripMenuItem";
            this.asCSVToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.asCSVToolStripMenuItem.Text = "As CSV";
            // 
            // excellToolStripMenuItem
            // 
            this.excellToolStripMenuItem.Name = "excellToolStripMenuItem";
            this.excellToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.excellToolStripMenuItem.Text = "Excell";
            // 
            // accessToolStripMenuItem
            // 
            this.accessToolStripMenuItem.Name = "accessToolStripMenuItem";
            this.accessToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.accessToolStripMenuItem.Text = "Access";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(267, 6);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSearchToolStripMenuItem});
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(80, 29);
            this.searchToolStripMenuItem.Text = "Search";
            // 
            // newSearchToolStripMenuItem
            // 
            this.newSearchToolStripMenuItem.Name = "newSearchToolStripMenuItem";
            this.newSearchToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.newSearchToolStripMenuItem.Text = "New Search";
            // 
            // EntryPoint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "EntryPoint";
            this.Text = "EntryPoint";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearResultsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openCSVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asCSVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem excellToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem accessToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newSearchToolStripMenuItem;
    }
}
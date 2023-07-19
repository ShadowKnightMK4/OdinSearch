namespace OdinSearchWin
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
            DataGridViewResults = new DataGridView();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            searchToolStripMenuItem = new ToolStripMenuItem();
            beginToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)DataGridViewResults).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // DataGridViewResults
            // 
            DataGridViewResults.AllowUserToOrderColumns = true;
            DataGridViewResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridViewResults.Dock = DockStyle.Fill;
            DataGridViewResults.Location = new Point(0, 33);
            DataGridViewResults.Name = "DataGridViewResults";
            DataGridViewResults.RowHeadersWidth = 62;
            DataGridViewResults.Size = new Size(800, 417);
            DataGridViewResults.TabIndex = 0;
            DataGridViewResults.CellContentClick += dataGridView1_CellContentClick;
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
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(54, 29);
            fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(141, 34);
            exitToolStripMenuItem.Text = "Exit";
            // 
            // searchToolStripMenuItem
            // 
            searchToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { beginToolStripMenuItem });
            searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            searchToolStripMenuItem.Size = new Size(80, 29);
            searchToolStripMenuItem.Text = "Search";
            // 
            // beginToolStripMenuItem
            // 
            beginToolStripMenuItem.Name = "beginToolStripMenuItem";
            beginToolStripMenuItem.Size = new Size(158, 34);
            beginToolStripMenuItem.Text = "Begin";
            beginToolStripMenuItem.Click += beginToolStripMenuItem_Click;
            // 
            // LandingForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(DataGridViewResults);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "LandingForm";
            Text = "OdinSearch - Find it the hard way.";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)DataGridViewResults).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView DataGridViewResults;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem searchToolStripMenuItem;
        private ToolStripMenuItem beginToolStripMenuItem;
    }
}
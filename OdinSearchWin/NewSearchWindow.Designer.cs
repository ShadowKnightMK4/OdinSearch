namespace OdinSearchWin
{
    partial class NewSearchWindow
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
            TextBoxFileName = new TextBox();
            ButtonBeginSearch = new Button();
            ButtonCancel = new Button();
            label1 = new Label();
            ButtonSearchControlFileName = new Button();
            label2 = new Label();
            ButtonCustomizeSearchAnchor = new Button();
            CheckedListBoxUserSearchAnchors = new CheckedListBox();
            label3 = new Label();
            LabelCurrentSearchStyle = new Label();
            SuspendLayout();
            // 
            // TextBoxFileName
            // 
            TextBoxFileName.Location = new Point(22, 85);
            TextBoxFileName.Name = "TextBoxFileName";
            TextBoxFileName.Size = new Size(515, 31);
            TextBoxFileName.TabIndex = 0;
            TextBoxFileName.TextChanged += TextBoxFileName_TextChanged;
            // 
            // ButtonBeginSearch
            // 
            ButtonBeginSearch.Location = new Point(660, 27);
            ButtonBeginSearch.Name = "ButtonBeginSearch";
            ButtonBeginSearch.Size = new Size(112, 34);
            ButtonBeginSearch.TabIndex = 1;
            ButtonBeginSearch.Text = "GO!";
            ButtonBeginSearch.UseVisualStyleBackColor = true;
            ButtonBeginSearch.Click += ButtonBeginSearch_Click;
            // 
            // ButtonCancel
            // 
            ButtonCancel.Location = new Point(660, 67);
            ButtonCancel.Name = "ButtonCancel";
            ButtonCancel.Size = new Size(112, 34);
            ButtonCancel.TabIndex = 2;
            ButtonCancel.Text = "Cancel";
            ButtonCancel.UseVisualStyleBackColor = true;
            ButtonCancel.Click += ButtonCancel_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(22, 32);
            label1.Name = "label1";
            label1.Size = new Size(519, 50);
            label1.TabIndex = 3;
            label1.Text = "Enter Wildcard for the file. Seperator multuple with the ; symbol.\r\nUse the 3 dot button to control how to search.\r\n";
            // 
            // ButtonSearchControlFileName
            // 
            ButtonSearchControlFileName.Location = new Point(542, 85);
            ButtonSearchControlFileName.Name = "ButtonSearchControlFileName";
            ButtonSearchControlFileName.Size = new Size(51, 34);
            ButtonSearchControlFileName.TabIndex = 4;
            ButtonSearchControlFileName.Text = "...";
            ButtonSearchControlFileName.UseVisualStyleBackColor = true;
            ButtonSearchControlFileName.Click += ButtonUpdateFileNameMatchStyle_click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(22, 156);
            label2.Name = "label2";
            label2.Size = new Size(323, 25);
            label2.TabIndex = 5;
            label2.Text = "Search Anchors are Starting Locations.  \r\n";
            label2.Click += label2_Click;
            // 
            // ButtonCustomizeSearchAnchor
            // 
            ButtonCustomizeSearchAnchor.Location = new Point(22, 344);
            ButtonCustomizeSearchAnchor.Name = "ButtonCustomizeSearchAnchor";
            ButtonCustomizeSearchAnchor.Size = new Size(297, 34);
            ButtonCustomizeSearchAnchor.TabIndex = 6;
            ButtonCustomizeSearchAnchor.Text = "Customize Anchors...";
            ButtonCustomizeSearchAnchor.UseVisualStyleBackColor = true;
            ButtonCustomizeSearchAnchor.Click += ButtonCustomizeSearchAnchor_Click;
            // 
            // CheckedListBoxUserSearchAnchors
            // 
            CheckedListBoxUserSearchAnchors.FormattingEnabled = true;
            CheckedListBoxUserSearchAnchors.Location = new Point(22, 194);
            CheckedListBoxUserSearchAnchors.Name = "CheckedListBoxUserSearchAnchors";
            CheckedListBoxUserSearchAnchors.Size = new Size(291, 144);
            CheckedListBoxUserSearchAnchors.TabIndex = 7;
            CheckedListBoxUserSearchAnchors.SelectedIndexChanged += CheckedListBoxUserSearchAnchors_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(22, 119);
            label3.Name = "label3";
            label3.Size = new Size(183, 25);
            label3.TabIndex = 8;
            label3.Text = "Current Search Mode:";
            // 
            // LabelCurrentSearchStyle
            // 
            LabelCurrentSearchStyle.AutoSize = true;
            LabelCurrentSearchStyle.Location = new Point(198, 119);
            LabelCurrentSearchStyle.Name = "LabelCurrentSearchStyle";
            LabelCurrentSearchStyle.Size = new Size(44, 25);
            LabelCurrentSearchStyle.TabIndex = 9;
            LabelCurrentSearchStyle.Text = "TDB";
            // 
            // NewSearchWindow
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 390);
            Controls.Add(LabelCurrentSearchStyle);
            Controls.Add(label3);
            Controls.Add(CheckedListBoxUserSearchAnchors);
            Controls.Add(ButtonCustomizeSearchAnchor);
            Controls.Add(label2);
            Controls.Add(ButtonSearchControlFileName);
            Controls.Add(label1);
            Controls.Add(ButtonCancel);
            Controls.Add(ButtonBeginSearch);
            Controls.Add(TextBoxFileName);
            Name = "NewSearchWindow";
            Text = "New Search - Simple View";
            Load += NewSearchWindow_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox TextBoxFileName;
        private Button ButtonBeginSearch;
        private Button ButtonCancel;
        private Label label1;
        private Button ButtonSearchControlFileName;
        private Label label2;
        private Button ButtonCustomizeSearchAnchor;
        private CheckedListBox CheckedListBoxUserSearchAnchors;
        private Label label3;
        private Label LabelCurrentSearchStyle;
    }
}
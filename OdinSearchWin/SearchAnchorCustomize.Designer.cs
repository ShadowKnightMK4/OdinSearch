namespace OdinSearchWin
{
    partial class SearchAnchorCustomize
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
            button1 = new Button();
            button2 = new Button();
            CheckBoxAllLocalDrivesDefault = new CheckBox();
            CheckBoxLookSubFolders = new CheckBox();
            CheckedListBoxSearchAnchors = new CheckedListBox();
            ButtonAddFolder = new Button();
            FolderBrowserDialogAddAnchor = new FolderBrowserDialog();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(597, 52);
            button1.Name = "button1";
            button1.Size = new Size(112, 34);
            button1.TabIndex = 0;
            button1.Text = "Accept";
            button1.UseVisualStyleBackColor = true;
            button1.Click += ButtonOK_Click;
            // 
            // button2
            // 
            button2.Location = new Point(597, 92);
            button2.Name = "button2";
            button2.Size = new Size(112, 34);
            button2.TabIndex = 1;
            button2.Text = "Cancel";
            button2.UseVisualStyleBackColor = true;
            button2.Click += ButtonCancel_Click;
            // 
            // CheckBoxAllLocalDrivesDefault
            // 
            CheckBoxAllLocalDrivesDefault.AutoSize = true;
            CheckBoxAllLocalDrivesDefault.Location = new Point(597, 146);
            CheckBoxAllLocalDrivesDefault.Name = "CheckBoxAllLocalDrivesDefault";
            CheckBoxAllLocalDrivesDefault.Size = new Size(167, 29);
            CheckBoxAllLocalDrivesDefault.TabIndex = 2;
            CheckBoxAllLocalDrivesDefault.Text = "Every local Drive";
            CheckBoxAllLocalDrivesDefault.UseVisualStyleBackColor = true;
            CheckBoxAllLocalDrivesDefault.CheckedChanged += CheckBoxAllLocalDrivesDefault_CheckedChanged;
            // 
            // CheckBoxLookSubFolders
            // 
            CheckBoxLookSubFolders.AutoSize = true;
            CheckBoxLookSubFolders.Location = new Point(597, 171);
            CheckBoxLookSubFolders.Name = "CheckBoxLookSubFolders";
            CheckBoxLookSubFolders.Size = new Size(190, 29);
            CheckBoxLookSubFolders.TabIndex = 3;
            CheckBoxLookSubFolders.Text = "Look in sub folders";
            CheckBoxLookSubFolders.UseVisualStyleBackColor = true;
            // 
            // CheckedListBoxSearchAnchors
            // 
            CheckedListBoxSearchAnchors.FormattingEnabled = true;
            CheckedListBoxSearchAnchors.Location = new Point(45, 56);
            CheckedListBoxSearchAnchors.Name = "CheckedListBoxSearchAnchors";
            CheckedListBoxSearchAnchors.Size = new Size(454, 144);
            CheckedListBoxSearchAnchors.TabIndex = 4;
            // 
            // ButtonAddFolder
            // 
            ButtonAddFolder.Location = new Point(387, 206);
            ButtonAddFolder.Name = "ButtonAddFolder";
            ButtonAddFolder.Size = new Size(112, 34);
            ButtonAddFolder.TabIndex = 5;
            ButtonAddFolder.Text = "Add...";
            ButtonAddFolder.UseVisualStyleBackColor = true;
            ButtonAddFolder.Click += ButtonAddFolder_Click;
            // 
            // SearchAnchorCustomize
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 285);
            Controls.Add(ButtonAddFolder);
            Controls.Add(CheckedListBoxSearchAnchors);
            Controls.Add(CheckBoxLookSubFolders);
            Controls.Add(CheckBoxAllLocalDrivesDefault);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "SearchAnchorCustomize";
            Text = "Customize your SearchAnchors";
            Load += SearchAnchorCustomize_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Button button2;
        private CheckBox CheckBoxAllLocalDrivesDefault;
        private CheckBox CheckBoxLookSubFolders;
        private CheckedListBox CheckedListBoxSearchAnchors;
        private Button ButtonAddFolder;
        private FolderBrowserDialog FolderBrowserDialogAddAnchor;
    }
}
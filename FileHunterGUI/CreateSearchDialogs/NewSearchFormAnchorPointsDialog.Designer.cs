namespace FileHunterGUI
{
    partial class NewSearchFormAnchorPointsDialog
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
            this.components = new System.ComponentModel.Container();
            this.ButtonAccept = new System.Windows.Forms.Button();
            this.ButtonCloseWindow = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CheckBoxCheckAll = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonClearChecked = new System.Windows.Forms.Button();
            this.ButtonAddLocation = new System.Windows.Forms.Button();
            this.CheckBoxListContainer = new System.Windows.Forms.CheckedListBox();
            this.CheckBoxChooseAllLocalPaths = new System.Windows.Forms.CheckBox();
            this.ChooseAnchorFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.ToolTipSet = new System.Windows.Forms.ToolTip(this.components);
            this.CheckBoxNoOverrideAnchors = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonAccept
            // 
            this.ButtonAccept.Location = new System.Drawing.Point(636, 12);
            this.ButtonAccept.Name = "ButtonAccept";
            this.ButtonAccept.Size = new System.Drawing.Size(112, 34);
            this.ButtonAccept.TabIndex = 0;
            this.ButtonAccept.Text = "OK";
            this.ButtonAccept.UseVisualStyleBackColor = true;
            this.ButtonAccept.Click += new System.EventHandler(this.ButtonAccept_Click);
            // 
            // ButtonCloseWindow
            // 
            this.ButtonCloseWindow.Location = new System.Drawing.Point(636, 52);
            this.ButtonCloseWindow.Name = "ButtonCloseWindow";
            this.ButtonCloseWindow.Size = new System.Drawing.Size(112, 34);
            this.ButtonCloseWindow.TabIndex = 1;
            this.ButtonCloseWindow.Text = "Close Window";
            this.ButtonCloseWindow.UseVisualStyleBackColor = true;
            this.ButtonCloseWindow.Click += new System.EventHandler(this.ButtonCloseWindow_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CheckBoxCheckAll);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ButtonClearChecked);
            this.groupBox1.Controls.Add(this.ButtonAddLocation);
            this.groupBox1.Controls.Add(this.CheckBoxListContainer);
            this.groupBox1.Controls.Add(this.CheckBoxChooseAllLocalPaths);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(533, 250);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Anchor Points";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // CheckBoxCheckAll
            // 
            this.CheckBoxCheckAll.AutoSize = true;
            this.CheckBoxCheckAll.Location = new System.Drawing.Point(201, 101);
            this.CheckBoxCheckAll.Name = "CheckBoxCheckAll";
            this.CheckBoxCheckAll.Size = new System.Drawing.Size(110, 29);
            this.CheckBoxCheckAll.TabIndex = 5;
            this.CheckBoxCheckAll.Text = "Check All";
            this.ToolTipSet.SetToolTip(this.CheckBoxCheckAll, "Check (or clear) all checkboxs on thel ist");
            this.CheckBoxCheckAll.UseVisualStyleBackColor = true;
            this.CheckBoxCheckAll.CheckedChanged += new System.EventHandler(this.CheckBoxCheckAll_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(366, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "An Anchor Point is a starting Search Location";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // ButtonClearChecked
            // 
            this.ButtonClearChecked.Location = new System.Drawing.Point(192, 186);
            this.ButtonClearChecked.Name = "ButtonClearChecked";
            this.ButtonClearChecked.Size = new System.Drawing.Size(189, 34);
            this.ButtonClearChecked.TabIndex = 3;
            this.ButtonClearChecked.Text = "Clear Checked Items";
            this.ToolTipSet.SetToolTip(this.ButtonClearChecked, "Clear any locations you\'ve checked to on the list");
            this.ButtonClearChecked.UseVisualStyleBackColor = true;
            this.ButtonClearChecked.Click += new System.EventHandler(this.ClearSelectedItems_click);
            // 
            // ButtonAddLocation
            // 
            this.ButtonAddLocation.Location = new System.Drawing.Point(192, 146);
            this.ButtonAddLocation.Name = "ButtonAddLocation";
            this.ButtonAddLocation.Size = new System.Drawing.Size(189, 34);
            this.ButtonAddLocation.TabIndex = 2;
            this.ButtonAddLocation.Text = "Add Location...";
            this.ToolTipSet.SetToolTip(this.ButtonAddLocation, "Add A New Location to the Anchor List\r\n");
            this.ButtonAddLocation.UseVisualStyleBackColor = true;
            this.ButtonAddLocation.Click += new System.EventHandler(this.AddLocationButton_Click);
            // 
            // CheckBoxListContainer
            // 
            this.CheckBoxListContainer.FormattingEnabled = true;
            this.CheckBoxListContainer.Location = new System.Drawing.Point(6, 66);
            this.CheckBoxListContainer.Name = "CheckBoxListContainer";
            this.CheckBoxListContainer.Size = new System.Drawing.Size(180, 172);
            this.CheckBoxListContainer.TabIndex = 1;
            this.ToolTipSet.SetToolTip(this.CheckBoxListContainer, "Use the checkboxs to select in this list the entries you want.");
            // 
            // CheckBoxChooseAllLocalPaths
            // 
            this.CheckBoxChooseAllLocalPaths.AutoSize = true;
            this.CheckBoxChooseAllLocalPaths.Location = new System.Drawing.Point(201, 66);
            this.CheckBoxChooseAllLocalPaths.Name = "CheckBoxChooseAllLocalPaths";
            this.CheckBoxChooseAllLocalPaths.Size = new System.Drawing.Size(224, 29);
            this.CheckBoxChooseAllLocalPaths.TabIndex = 0;
            this.CheckBoxChooseAllLocalPaths.Text = "Check Everywhere Local";
            this.ToolTipSet.SetToolTip(this.CheckBoxChooseAllLocalPaths, "If checked, everything else is disabled and the anchor list will be all ready dri" +
        "ves");
            this.CheckBoxChooseAllLocalPaths.UseVisualStyleBackColor = true;
            this.CheckBoxChooseAllLocalPaths.CheckedChanged += new System.EventHandler(this.CheckBoxChooseAllLocalPaths_CheckedChanged);
            // 
            // ChooseAnchorFolderDialog
            // 
            this.ChooseAnchorFolderDialog.AddToRecent = false;
            this.ChooseAnchorFolderDialog.InitialDirectory = "C:\\Windows";
            // 
            // CheckBoxNoOverrideAnchors
            // 
            this.CheckBoxNoOverrideAnchors.AutoSize = true;
            this.CheckBoxNoOverrideAnchors.Location = new System.Drawing.Point(551, 92);
            this.CheckBoxNoOverrideAnchors.Name = "CheckBoxNoOverrideAnchors";
            this.CheckBoxNoOverrideAnchors.Size = new System.Drawing.Size(217, 29);
            this.CheckBoxNoOverrideAnchors.TabIndex = 3;
            this.CheckBoxNoOverrideAnchors.Text = "Don\'t Replace Anchors";
            this.ToolTipSet.SetToolTip(this.CheckBoxNoOverrideAnchors, "If set, the search dialog will add these to its list rather than replace its list" +
        " with thse.");
            this.CheckBoxNoOverrideAnchors.UseVisualStyleBackColor = true;
            // 
            // NewSearchFormAnchorPointsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 302);
            this.Controls.Add(this.CheckBoxNoOverrideAnchors);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ButtonCloseWindow);
            this.Controls.Add(this.ButtonAccept);
            this.Name = "NewSearchFormAnchorPointsDialog";
            this.Text = "Create New Anchor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewAnchorForm_FormClosing);
            this.Load += new System.EventHandler(this.NewSearchForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button ButtonAccept;
        private Button ButtonCloseWindow;
        private GroupBox groupBox1;
        private Label label1;
        private Button ButtonClearChecked;
        private Button ButtonAddLocation;
        private CheckBox CheckBoxChooseAllLocalPaths;
        private FolderBrowserDialog ChooseAnchorFolderDialog;
        private CheckBox CheckBoxCheckAll;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private ToolTip ToolTipSet;
        public CheckBox CheckBoxNoOverrideAnchors;
        public CheckedListBox CheckBoxListContainer;
    }
}
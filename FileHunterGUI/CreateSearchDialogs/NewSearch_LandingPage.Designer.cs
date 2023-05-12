namespace FileHunterGUI.CreateSearchDialogs
{
    partial class NewSearchLandingPageDialog
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
            this.ButtonChooseAnchor = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.ListBoxAnchors = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ButtonChooseAnchor
            // 
            this.ButtonChooseAnchor.Location = new System.Drawing.Point(280, 40);
            this.ButtonChooseAnchor.Name = "ButtonChooseAnchor";
            this.ButtonChooseAnchor.Size = new System.Drawing.Size(209, 34);
            this.ButtonChooseAnchor.TabIndex = 0;
            this.ButtonChooseAnchor.Text = "Pick Anchors...";
            this.ButtonChooseAnchor.UseVisualStyleBackColor = true;
            this.ButtonChooseAnchor.Click += new System.EventHandler(this.ButtonAnchor_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(280, 80);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(205, 34);
            this.button2.TabIndex = 1;
            this.button2.Text = "Pick Search Target...";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // ListBoxAnchors
            // 
            this.ListBoxAnchors.FormattingEnabled = true;
            this.ListBoxAnchors.ItemHeight = 25;
            this.ListBoxAnchors.Location = new System.Drawing.Point(12, 40);
            this.ListBoxAnchors.Name = "ListBoxAnchors";
            this.ListBoxAnchors.Size = new System.Drawing.Size(262, 129);
            this.ListBoxAnchors.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(417, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "An Anchor is a starting location to begin the search";
            // 
            // NewSearchLandingPageDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ListBoxAnchors);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.ButtonChooseAnchor);
            this.Name = "NewSearchLandingPageDialog";
            this.Text = "Create a Search";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button ButtonChooseAnchor;
        private Button button2;
        private ListBox ListBoxAnchors;
        private Label label1;
    }
}
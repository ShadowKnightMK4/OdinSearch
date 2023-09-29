namespace OdinSearchWin
{
    partial class CustomizeMatchStyleString
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
            ButtonOK = new Button();
            ButtonCancel = new Button();
            checkedListBox1 = new CheckedListBox();
            ToolTipSearchControlExplain = new ToolTip(components);
            SuspendLayout();
            // 
            // ButtonOK
            // 
            ButtonOK.Location = new Point(366, 30);
            ButtonOK.Name = "ButtonOK";
            ButtonOK.Size = new Size(112, 34);
            ButtonOK.TabIndex = 0;
            ButtonOK.Text = "Ok";
            ButtonOK.UseVisualStyleBackColor = true;
            ButtonOK.Click += ButtonOK_Click;
            // 
            // ButtonCancel
            // 
            ButtonCancel.Location = new Point(366, 70);
            ButtonCancel.Name = "ButtonCancel";
            ButtonCancel.Size = new Size(112, 34);
            ButtonCancel.TabIndex = 1;
            ButtonCancel.Text = "Cancel";
            ButtonCancel.UseVisualStyleBackColor = true;
            ButtonCancel.Click += ButtonCancel_Click;
            // 
            // checkedListBox1
            // 
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Items.AddRange(new object[] { "Match Any Number of the search item.", "Match All search Items in the List", "Invert your match", "Disable this Match", "Case is important" });
            checkedListBox1.Location = new Point(12, 41);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(307, 144);
            checkedListBox1.TabIndex = 2;
            checkedListBox1.SelectedIndexChanged += checkedListBox1_SelectedIndexChanged;
            // 
            // CustomizeMatchStyleString
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(515, 208);
            Controls.Add(checkedListBox1);
            Controls.Add(ButtonCancel);
            Controls.Add(ButtonOK);
            Name = "CustomizeMatchStyleString";
            Text = "CustomizeSearchHandling";
            Load += CustomizeMatchStyleString_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button ButtonOK;
        private Button ButtonCancel;
        private CheckedListBox checkedListBox1;
        private ToolTip ToolTipSearchControlExplain;
    }
}
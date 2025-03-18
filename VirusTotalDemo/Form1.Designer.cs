namespace VirusTotalDemo
{
    partial class Form1
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
            components = new System.ComponentModel.Container();
            textBox1 = new TextBox();
            checkedListBoxFolderTargets = new CheckedListBox();
            toolTipFolderShow = new ToolTip(components);
            button1 = new Button();
            button2 = new Button();
            checkBox1 = new CheckBox();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 266);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(776, 282);
            textBox1.TabIndex = 0;
            // 
            // checkedListBoxFolderTargets
            // 
            checkedListBoxFolderTargets.FormattingEnabled = true;
            checkedListBoxFolderTargets.Location = new Point(12, 31);
            checkedListBoxFolderTargets.Name = "checkedListBoxFolderTargets";
            checkedListBoxFolderTargets.Size = new Size(340, 144);
            checkedListBoxFolderTargets.TabIndex = 1;
            // 
            // button1
            // 
            button1.Location = new Point(358, 31);
            button1.Name = "button1";
            button1.Size = new Size(112, 34);
            button1.TabIndex = 2;
            button1.Text = "Add Folder";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(358, 71);
            button2.Name = "button2";
            button2.Size = new Size(112, 34);
            button2.TabIndex = 3;
            button2.Text = "Blank List";
            button2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(358, 123);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(143, 29);
            checkBox1.TabIndex = 4;
            checkBox1.Text = "Just all Ready";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 560);
            Controls.Add(checkBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(checkedListBoxFolderTargets);
            Controls.Add(textBox1);
            Name = "Form1";
            Text = "OdinSearch Demo Project";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private CheckedListBox checkedListBoxFolderTargets;
        private ToolTip toolTipFolderShow;
        private Button button1;
        private Button button2;
        private CheckBox checkBox1;
    }
}

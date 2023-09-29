namespace OdinSearchWin
{
    partial class VisualLogWindow
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
            ListBoxLog = new ListBox();
            SuspendLayout();
            // 
            // ListBoxLog
            // 
            ListBoxLog.FormattingEnabled = true;
            ListBoxLog.ItemHeight = 25;
            ListBoxLog.Location = new Point(12, 54);
            ListBoxLog.Name = "ListBoxLog";
            ListBoxLog.Size = new Size(774, 254);
            ListBoxLog.TabIndex = 0;
            // 
            // VisualLogWindow
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(ListBoxLog);
            Name = "VisualLogWindow";
            Text = "VisualLogWindow";
            ResumeLayout(false);
        }

        #endregion

        public ListBox ListBoxLog;
    }
}
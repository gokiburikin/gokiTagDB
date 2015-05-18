namespace gokiTagDB
{
    partial class frmAutoSuggestWindow
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
            this.lstSuggestions = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lstSuggestions
            // 
            this.lstSuggestions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstSuggestions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSuggestions.FormattingEnabled = true;
            this.lstSuggestions.IntegralHeight = false;
            this.lstSuggestions.Location = new System.Drawing.Point(0, 0);
            this.lstSuggestions.Name = "lstSuggestions";
            this.lstSuggestions.ScrollAlwaysVisible = true;
            this.lstSuggestions.Size = new System.Drawing.Size(200, 100);
            this.lstSuggestions.TabIndex = 0;
            // 
            // frmAutoSuggestWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(200, 100);
            this.Controls.Add(this.lstSuggestions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmAutoSuggestWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "AutoSuggestWindow";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ListBox lstSuggestions;
    }
}
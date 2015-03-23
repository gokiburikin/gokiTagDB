namespace gokiTagDB
{
    partial class frmTagEdit
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
            this.btnClose = new System.Windows.Forms.Button();
            this.grpCategories = new System.Windows.Forms.GroupBox();
            this.btnSetCategory = new System.Windows.Forms.Button();
            this.btnRemoveCategory = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblCategoriesStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.txtCategory = new System.Windows.Forms.TextBox();
            this.btnAddCategory = new System.Windows.Forms.Button();
            this.lstCategories = new System.Windows.Forms.ListBox();
            this.grpTags = new System.Windows.Forms.GroupBox();
            this.statusStrip3 = new System.Windows.Forms.StatusStrip();
            this.lblTagStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.grpAliases = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.statusStrip2 = new System.Windows.Forms.StatusStrip();
            this.lblAliasStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.txtAliasGroup = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.lstAliases = new System.Windows.Forms.ListBox();
            this.lstTags = new System.Windows.Forms.ListView();
            this.grpCategories.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.grpTags.SuspendLayout();
            this.statusStrip3.SuspendLayout();
            this.grpAliases.SuspendLayout();
            this.statusStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(235, 394);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // grpCategories
            // 
            this.grpCategories.Controls.Add(this.btnSetCategory);
            this.grpCategories.Controls.Add(this.btnRemoveCategory);
            this.grpCategories.Controls.Add(this.statusStrip1);
            this.grpCategories.Controls.Add(this.txtCategory);
            this.grpCategories.Controls.Add(this.btnAddCategory);
            this.grpCategories.Controls.Add(this.lstCategories);
            this.grpCategories.Location = new System.Drawing.Point(268, 12);
            this.grpCategories.Name = "grpCategories";
            this.grpCategories.Size = new System.Drawing.Size(250, 300);
            this.grpCategories.TabIndex = 4;
            this.grpCategories.TabStop = false;
            this.grpCategories.Text = "Categories";
            // 
            // btnSetCategory
            // 
            this.btnSetCategory.Location = new System.Drawing.Point(9, 181);
            this.btnSetCategory.Name = "btnSetCategory";
            this.btnSetCategory.Size = new System.Drawing.Size(228, 23);
            this.btnSetCategory.TabIndex = 5;
            this.btnSetCategory.Text = "Set for Selected Tags";
            this.btnSetCategory.UseVisualStyleBackColor = true;
            // 
            // btnRemoveCategory
            // 
            this.btnRemoveCategory.Location = new System.Drawing.Point(9, 152);
            this.btnRemoveCategory.Name = "btnRemoveCategory";
            this.btnRemoveCategory.Size = new System.Drawing.Size(228, 23);
            this.btnRemoveCategory.TabIndex = 4;
            this.btnRemoveCategory.Text = "Remove Selected Category";
            this.btnRemoveCategory.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblCategoriesStatus});
            this.statusStrip1.Location = new System.Drawing.Point(3, 275);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(244, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblCategoriesStatus
            // 
            this.lblCategoriesStatus.Name = "lblCategoriesStatus";
            this.lblCategoriesStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // txtCategory
            // 
            this.txtCategory.Location = new System.Drawing.Point(9, 210);
            this.txtCategory.MaxLength = 255;
            this.txtCategory.Name = "txtCategory";
            this.txtCategory.Size = new System.Drawing.Size(228, 20);
            this.txtCategory.TabIndex = 2;
            // 
            // btnAddCategory
            // 
            this.btnAddCategory.Location = new System.Drawing.Point(9, 236);
            this.btnAddCategory.Name = "btnAddCategory";
            this.btnAddCategory.Size = new System.Drawing.Size(228, 23);
            this.btnAddCategory.TabIndex = 1;
            this.btnAddCategory.Text = "Add Category";
            this.btnAddCategory.UseVisualStyleBackColor = true;
            this.btnAddCategory.Click += new System.EventHandler(this.btnAddCategory_Click);
            // 
            // lstCategories
            // 
            this.lstCategories.FormattingEnabled = true;
            this.lstCategories.Location = new System.Drawing.Point(9, 25);
            this.lstCategories.Name = "lstCategories";
            this.lstCategories.Size = new System.Drawing.Size(228, 121);
            this.lstCategories.TabIndex = 0;
            // 
            // grpTags
            // 
            this.grpTags.Controls.Add(this.lstTags);
            this.grpTags.Controls.Add(this.statusStrip3);
            this.grpTags.Location = new System.Drawing.Point(12, 12);
            this.grpTags.Name = "grpTags";
            this.grpTags.Size = new System.Drawing.Size(250, 300);
            this.grpTags.TabIndex = 6;
            this.grpTags.TabStop = false;
            this.grpTags.Text = "Tags";
            // 
            // statusStrip3
            // 
            this.statusStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblTagStatus});
            this.statusStrip3.Location = new System.Drawing.Point(3, 275);
            this.statusStrip3.Name = "statusStrip3";
            this.statusStrip3.Size = new System.Drawing.Size(244, 22);
            this.statusStrip3.SizingGrip = false;
            this.statusStrip3.TabIndex = 4;
            this.statusStrip3.Text = "statusStrip3";
            // 
            // lblTagStatus
            // 
            this.lblTagStatus.Name = "lblTagStatus";
            this.lblTagStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // grpAliases
            // 
            this.grpAliases.Controls.Add(this.button2);
            this.grpAliases.Controls.Add(this.statusStrip2);
            this.grpAliases.Controls.Add(this.txtAliasGroup);
            this.grpAliases.Controls.Add(this.button1);
            this.grpAliases.Controls.Add(this.lstAliases);
            this.grpAliases.Location = new System.Drawing.Point(524, 12);
            this.grpAliases.Name = "grpAliases";
            this.grpAliases.Size = new System.Drawing.Size(250, 300);
            this.grpAliases.TabIndex = 7;
            this.grpAliases.TabStop = false;
            this.grpAliases.Text = "Aliases";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 236);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(228, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Add / Update Alias Group";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // statusStrip2
            // 
            this.statusStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblAliasStatus});
            this.statusStrip2.Location = new System.Drawing.Point(3, 275);
            this.statusStrip2.Name = "statusStrip2";
            this.statusStrip2.Size = new System.Drawing.Size(244, 22);
            this.statusStrip2.SizingGrip = false;
            this.statusStrip2.TabIndex = 7;
            // 
            // lblAliasStatus
            // 
            this.lblAliasStatus.Name = "lblAliasStatus";
            this.lblAliasStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // txtAliasGroup
            // 
            this.txtAliasGroup.Location = new System.Drawing.Point(6, 181);
            this.txtAliasGroup.Multiline = true;
            this.txtAliasGroup.Name = "txtAliasGroup";
            this.txtAliasGroup.Size = new System.Drawing.Size(228, 49);
            this.txtAliasGroup.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 152);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(228, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Remove Selected Alias Group";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // lstAliases
            // 
            this.lstAliases.FormattingEnabled = true;
            this.lstAliases.Location = new System.Drawing.Point(6, 25);
            this.lstAliases.Name = "lstAliases";
            this.lstAliases.Size = new System.Drawing.Size(228, 121);
            this.lstAliases.TabIndex = 0;
            // 
            // lstTags
            // 
            this.lstTags.HideSelection = false;
            this.lstTags.Location = new System.Drawing.Point(6, 19);
            this.lstTags.Name = "lstTags";
            this.lstTags.Size = new System.Drawing.Size(238, 253);
            this.lstTags.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstTags.TabIndex = 5;
            this.lstTags.UseCompatibleStateImageBehavior = false;
            this.lstTags.View = System.Windows.Forms.View.Details;
            // 
            // frmTagEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 452);
            this.Controls.Add(this.grpAliases);
            this.Controls.Add(this.grpTags);
            this.Controls.Add(this.grpCategories);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmTagEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Tags";
            this.grpCategories.ResumeLayout(false);
            this.grpCategories.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.grpTags.ResumeLayout(false);
            this.grpTags.PerformLayout();
            this.statusStrip3.ResumeLayout(false);
            this.statusStrip3.PerformLayout();
            this.grpAliases.ResumeLayout(false);
            this.grpAliases.PerformLayout();
            this.statusStrip2.ResumeLayout(false);
            this.statusStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox grpCategories;
        private System.Windows.Forms.Button btnSetCategory;
        private System.Windows.Forms.Button btnRemoveCategory;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblCategoriesStatus;
        private System.Windows.Forms.TextBox txtCategory;
        private System.Windows.Forms.Button btnAddCategory;
        private System.Windows.Forms.ListBox lstCategories;
        private System.Windows.Forms.GroupBox grpTags;
        private System.Windows.Forms.GroupBox grpAliases;
        private System.Windows.Forms.ListBox lstAliases;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.StatusStrip statusStrip2;
        private System.Windows.Forms.ToolStripStatusLabel lblAliasStatus;
        private System.Windows.Forms.TextBox txtAliasGroup;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.StatusStrip statusStrip3;
        private System.Windows.Forms.ToolStripStatusLabel lblTagStatus;
        private System.Windows.Forms.ListView lstTags;
    }
}
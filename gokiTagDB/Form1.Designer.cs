namespace gokiTagDB
{
    partial class frmMainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMainForm));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.categoriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.entriesPerPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.organizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOrganizeMoveSelectedFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanThumbnailDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettingsSelectionMode = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettingsSelectionModeExplorer = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettingsSelectionModeToggle = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.panel2 = new System.Windows.Forms.Panel();
            this.scrPanelVertical = new System.Windows.Forms.VScrollBar();
            this.pnlThumbnailView = new GokiLibrary.UserInterface.DoubleBufferedPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.RichTextBox();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.pnlTagList = new GokiLibrary.UserInterface.DoubleBufferedPanel();
            this.scrTagListVertical = new System.Windows.Forms.VScrollBar();
            this.txtTagEditor = new System.Windows.Forms.RichTextBox();
            this.btnUpdateTags = new System.Windows.Forms.Button();
            this.btnAddTags = new System.Windows.Forms.Button();
            this.btnRemoveTags = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewZoom = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(198, 49);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 628);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.organizeToolStripMenuItem,
            this.mnuView,
            this.mnuSettings,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(792, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tagsToolStripMenuItem,
            this.entriesPerPageToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // tagsToolStripMenuItem
            // 
            this.tagsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.categoriesToolStripMenuItem});
            this.tagsToolStripMenuItem.Name = "tagsToolStripMenuItem";
            this.tagsToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.tagsToolStripMenuItem.Text = "Tags";
            // 
            // categoriesToolStripMenuItem
            // 
            this.categoriesToolStripMenuItem.Name = "categoriesToolStripMenuItem";
            this.categoriesToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.categoriesToolStripMenuItem.Text = "Categories...";
            // 
            // entriesPerPageToolStripMenuItem
            // 
            this.entriesPerPageToolStripMenuItem.Name = "entriesPerPageToolStripMenuItem";
            this.entriesPerPageToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.entriesPerPageToolStripMenuItem.Text = "Entries Per Page...";
            this.entriesPerPageToolStripMenuItem.Click += new System.EventHandler(this.entriesPerPageToolStripMenuItem_Click);
            // 
            // organizeToolStripMenuItem
            // 
            this.organizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOrganizeMoveSelectedFiles,
            this.cleanThumbnailDatabaseToolStripMenuItem});
            this.organizeToolStripMenuItem.Name = "organizeToolStripMenuItem";
            this.organizeToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.organizeToolStripMenuItem.Text = "Organize";
            // 
            // mnuOrganizeMoveSelectedFiles
            // 
            this.mnuOrganizeMoveSelectedFiles.Name = "mnuOrganizeMoveSelectedFiles";
            this.mnuOrganizeMoveSelectedFiles.Size = new System.Drawing.Size(210, 22);
            this.mnuOrganizeMoveSelectedFiles.Text = "Move selected files...";
            this.mnuOrganizeMoveSelectedFiles.Click += new System.EventHandler(this.mnuOrganizeMoveSelectedFiles_Click);
            // 
            // cleanThumbnailDatabaseToolStripMenuItem
            // 
            this.cleanThumbnailDatabaseToolStripMenuItem.Name = "cleanThumbnailDatabaseToolStripMenuItem";
            this.cleanThumbnailDatabaseToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.cleanThumbnailDatabaseToolStripMenuItem.Text = "Clean thumbnail database...";
            this.cleanThumbnailDatabaseToolStripMenuItem.Click += new System.EventHandler(this.cleanThumbnailDatabaseToolStripMenuItem_Click);
            // 
            // mnuSettings
            // 
            this.mnuSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSettingsSelectionMode});
            this.mnuSettings.Name = "mnuSettings";
            this.mnuSettings.Size = new System.Drawing.Size(58, 20);
            this.mnuSettings.Text = "Settings";
            // 
            // mnuSettingsSelectionMode
            // 
            this.mnuSettingsSelectionMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSettingsSelectionModeExplorer,
            this.mnuSettingsSelectionModeToggle});
            this.mnuSettingsSelectionMode.Name = "mnuSettingsSelectionMode";
            this.mnuSettingsSelectionMode.Size = new System.Drawing.Size(146, 22);
            this.mnuSettingsSelectionMode.Text = "Selection Mode";
            // 
            // mnuSettingsSelectionModeExplorer
            // 
            this.mnuSettingsSelectionModeExplorer.Name = "mnuSettingsSelectionModeExplorer";
            this.mnuSettingsSelectionModeExplorer.Size = new System.Drawing.Size(114, 22);
            this.mnuSettingsSelectionModeExplorer.Text = "Explorer";
            this.mnuSettingsSelectionModeExplorer.Click += new System.EventHandler(this.mnuSettingsSelectionModeExplorer_Click);
            // 
            // mnuSettingsSelectionModeToggle
            // 
            this.mnuSettingsSelectionModeToggle.Name = "mnuSettingsSelectionModeToggle";
            this.mnuSettingsSelectionModeToggle.Size = new System.Drawing.Size(114, 22);
            this.mnuSettingsSelectionModeToggle.Text = "Toggle";
            this.mnuSettingsSelectionModeToggle.Click += new System.EventHandler(this.mnuSettingsSelectionModeToggle_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblStatus3,
            this.lblStatus2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 677);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(792, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(47, 17);
            this.lblStatus.Text = "Status 1";
            // 
            // lblStatus3
            // 
            this.lblStatus3.Name = "lblStatus3";
            this.lblStatus3.Size = new System.Drawing.Size(683, 17);
            this.lblStatus3.Spring = true;
            // 
            // lblStatus2
            // 
            this.lblStatus2.Name = "lblStatus2";
            this.lblStatus2.Size = new System.Drawing.Size(47, 17);
            this.lblStatus2.Text = "Status 2";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(792, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.scrPanelVertical);
            this.panel2.Controls.Add(this.pnlThumbnailView);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(201, 49);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(591, 628);
            this.panel2.TabIndex = 3;
            // 
            // scrPanelVertical
            // 
            this.scrPanelVertical.Dock = System.Windows.Forms.DockStyle.Right;
            this.scrPanelVertical.Location = new System.Drawing.Point(575, 0);
            this.scrPanelVertical.Name = "scrPanelVertical";
            this.scrPanelVertical.Size = new System.Drawing.Size(16, 628);
            this.scrPanelVertical.TabIndex = 6;
            // 
            // pnlThumbnailView
            // 
            this.pnlThumbnailView.BackColor = System.Drawing.Color.White;
            this.pnlThumbnailView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlThumbnailView.Location = new System.Drawing.Point(0, 0);
            this.pnlThumbnailView.Name = "pnlThumbnailView";
            this.pnlThumbnailView.Size = new System.Drawing.Size(591, 628);
            this.pnlThumbnailView.TabIndex = 4;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Left;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 49);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnClear);
            this.splitContainer1.Panel1.Controls.Add(this.btnSearch);
            this.splitContainer1.Panel1.Controls.Add(this.txtSearch);
            this.splitContainer1.Panel1MinSize = 66;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(198, 628);
            this.splitContainer1.SplitterDistance = 66;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 8;
            this.splitContainer1.TabStop = false;
            // 
            // btnClear
            // 
            this.btnClear.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClear.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnClear.Location = new System.Drawing.Point(0, 43);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(198, 23);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "&Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            // 
            // btnSearch
            // 
            this.btnSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSearch.Location = new System.Drawing.Point(0, 20);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(198, 23);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // txtSearch
            // 
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtSearch.Location = new System.Drawing.Point(0, 0);
            this.txtSearch.MaxLength = 32767;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(198, 20);
            this.txtSearch.TabIndex = 3;
            this.txtSearch.Text = "";
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.pnlTagList);
            this.splitContainer3.Panel1.Controls.Add(this.scrTagListVertical);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.txtTagEditor);
            this.splitContainer3.Panel2.Controls.Add(this.btnUpdateTags);
            this.splitContainer3.Panel2.Controls.Add(this.btnAddTags);
            this.splitContainer3.Panel2.Controls.Add(this.btnRemoveTags);
            this.splitContainer3.Panel2.Controls.Add(this.label1);
            this.splitContainer3.Size = new System.Drawing.Size(198, 561);
            this.splitContainer3.SplitterDistance = 423;
            this.splitContainer3.TabIndex = 0;
            this.splitContainer3.TabStop = false;
            // 
            // pnlTagList
            // 
            this.pnlTagList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTagList.Location = new System.Drawing.Point(0, 0);
            this.pnlTagList.Name = "pnlTagList";
            this.pnlTagList.Size = new System.Drawing.Size(182, 423);
            this.pnlTagList.TabIndex = 0;
            this.pnlTagList.TabStop = false;
            // 
            // scrTagListVertical
            // 
            this.scrTagListVertical.Dock = System.Windows.Forms.DockStyle.Right;
            this.scrTagListVertical.Location = new System.Drawing.Point(182, 0);
            this.scrTagListVertical.Name = "scrTagListVertical";
            this.scrTagListVertical.Size = new System.Drawing.Size(16, 423);
            this.scrTagListVertical.TabIndex = 1;
            // 
            // txtTagEditor
            // 
            this.txtTagEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTagEditor.Location = new System.Drawing.Point(0, 23);
            this.txtTagEditor.MaxLength = 32767;
            this.txtTagEditor.Name = "txtTagEditor";
            this.txtTagEditor.Size = new System.Drawing.Size(198, 42);
            this.txtTagEditor.TabIndex = 1;
            this.txtTagEditor.Text = "";
            // 
            // btnUpdateTags
            // 
            this.btnUpdateTags.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnUpdateTags.Location = new System.Drawing.Point(0, 65);
            this.btnUpdateTags.Name = "btnUpdateTags";
            this.btnUpdateTags.Size = new System.Drawing.Size(198, 23);
            this.btnUpdateTags.TabIndex = 2;
            this.btnUpdateTags.Text = "Up&date";
            this.btnUpdateTags.UseVisualStyleBackColor = true;
            // 
            // btnAddTags
            // 
            this.btnAddTags.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnAddTags.Location = new System.Drawing.Point(0, 88);
            this.btnAddTags.Name = "btnAddTags";
            this.btnAddTags.Size = new System.Drawing.Size(198, 23);
            this.btnAddTags.TabIndex = 3;
            this.btnAddTags.Text = "&Add";
            this.btnAddTags.UseVisualStyleBackColor = true;
            // 
            // btnRemoveTags
            // 
            this.btnRemoveTags.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnRemoveTags.Location = new System.Drawing.Point(0, 111);
            this.btnRemoveTags.Name = "btnRemoveTags";
            this.btnRemoveTags.Size = new System.Drawing.Size(198, 23);
            this.btnRemoveTags.TabIndex = 4;
            this.btnRemoveTags.Text = "&Remove";
            this.btnRemoveTags.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Edit Tags";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mnuView
            // 
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewZoom});
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(41, 20);
            this.mnuView.Text = "View";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // mnuViewZoom
            // 
            this.mnuViewZoom.Name = "mnuViewZoom";
            this.mnuViewZoom.Size = new System.Drawing.Size(152, 22);
            this.mnuViewZoom.Text = "Zoom";
            // 
            // frmMainForm
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClear;
            this.ClientSize = new System.Drawing.Size(792, 699);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TagDB";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tagsToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripMenuItem entriesPerPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.Panel panel2;
        private GokiLibrary.UserInterface.DoubleBufferedPanel pnlThumbnailView;
        private System.Windows.Forms.VScrollBar scrPanelVertical;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus2;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private GokiLibrary.UserInterface.DoubleBufferedPanel pnlTagList;
        private System.Windows.Forms.VScrollBar scrTagListVertical;
        private System.Windows.Forms.Button btnUpdateTags;
        private System.Windows.Forms.Button btnAddTags;
        private System.Windows.Forms.Button btnRemoveTags;
        private System.Windows.Forms.RichTextBox txtTagEditor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem mnuSettings;
        private System.Windows.Forms.ToolStripMenuItem mnuSettingsSelectionMode;
        private System.Windows.Forms.ToolStripMenuItem mnuSettingsSelectionModeExplorer;
        private System.Windows.Forms.ToolStripMenuItem mnuSettingsSelectionModeToggle;
        private System.Windows.Forms.ToolStripMenuItem categoriesToolStripMenuItem;
        private System.Windows.Forms.RichTextBox txtSearch;
        private System.Windows.Forms.ToolStripMenuItem organizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuOrganizeMoveSelectedFiles;
        private System.Windows.Forms.ToolStripMenuItem cleanThumbnailDatabaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuViewZoom;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}


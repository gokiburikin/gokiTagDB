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
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.randomlyTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.organizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOrganizeMoveSelectedFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSelectedFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewZoom = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettingsSelectionMode = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettingsSelectionModeExplorer = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettingsSelectionModeToggle = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettingsSortMode = new System.Windows.Forms.ToolStripMenuItem();
            this.fileFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.scrPanelVertical = new System.Windows.Forms.VScrollBar();
            this.pnlThumbnailView = new GokiLibrary.UserInterface.DoubleBufferedPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnRemoveTags = new System.Windows.Forms.Button();
            this.btnAddTags = new System.Windows.Forms.Button();
            this.btnUpdateTags = new System.Windows.Forms.Button();
            this.txtTagEditor = new System.Windows.Forms.RichTextBox();
            this.lblAutoSuggest = new System.Windows.Forms.Label();
            this.pnlTagListContainer = new System.Windows.Forms.Panel();
            this.pnlTagList = new GokiLibrary.UserInterface.DoubleBufferedPanel();
            this.scrTagListVertical = new System.Windows.Forms.VScrollBar();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.panel3.SuspendLayout();
            this.pnlTagListContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(201, 49);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 624);
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
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tagsToolStripMenuItem,
            this.randomlyTagToolStripMenuItem,
            this.selectToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // tagsToolStripMenuItem
            // 
            this.tagsToolStripMenuItem.Name = "tagsToolStripMenuItem";
            this.tagsToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.tagsToolStripMenuItem.Text = "Tags";
            this.tagsToolStripMenuItem.Click += new System.EventHandler(this.tagsToolStripMenuItem_Click);
            // 
            // randomlyTagToolStripMenuItem
            // 
            this.randomlyTagToolStripMenuItem.Name = "randomlyTagToolStripMenuItem";
            this.randomlyTagToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.randomlyTagToolStripMenuItem.Text = "Randomly Tag...";
            this.randomlyTagToolStripMenuItem.Click += new System.EventHandler(this.randomlyTagToolStripMenuItem_Click);
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allToolStripMenuItem});
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            this.selectToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.selectToolStripMenuItem.Text = "Select";
            // 
            // allToolStripMenuItem
            // 
            this.allToolStripMenuItem.Name = "allToolStripMenuItem";
            this.allToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.allToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.allToolStripMenuItem.Text = "All";
            this.allToolStripMenuItem.Click += new System.EventHandler(this.allToolStripMenuItem_Click);
            // 
            // organizeToolStripMenuItem
            // 
            this.organizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOrganizeMoveSelectedFiles,
            this.deleteSelectedFilesToolStripMenuItem});
            this.organizeToolStripMenuItem.Name = "organizeToolStripMenuItem";
            this.organizeToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.organizeToolStripMenuItem.Text = "Organize";
            // 
            // mnuOrganizeMoveSelectedFiles
            // 
            this.mnuOrganizeMoveSelectedFiles.Name = "mnuOrganizeMoveSelectedFiles";
            this.mnuOrganizeMoveSelectedFiles.Size = new System.Drawing.Size(182, 22);
            this.mnuOrganizeMoveSelectedFiles.Text = "Move selected files...";
            this.mnuOrganizeMoveSelectedFiles.Click += new System.EventHandler(this.mnuOrganizeMoveSelectedFiles_Click);
            // 
            // deleteSelectedFilesToolStripMenuItem
            // 
            this.deleteSelectedFilesToolStripMenuItem.Name = "deleteSelectedFilesToolStripMenuItem";
            this.deleteSelectedFilesToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.deleteSelectedFilesToolStripMenuItem.Text = "Delete selected files...";
            this.deleteSelectedFilesToolStripMenuItem.Click += new System.EventHandler(this.deleteSelectedFilesToolStripMenuItem_Click);
            // 
            // mnuView
            // 
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewZoom});
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(41, 20);
            this.mnuView.Text = "View";
            // 
            // mnuViewZoom
            // 
            this.mnuViewZoom.Name = "mnuViewZoom";
            this.mnuViewZoom.Size = new System.Drawing.Size(100, 22);
            this.mnuViewZoom.Text = "Zoom";
            // 
            // mnuSettings
            // 
            this.mnuSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSettingsSelectionMode,
            this.mnuSettingsSortMode,
            this.fileFilterToolStripMenuItem});
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
            // mnuSettingsSortMode
            // 
            this.mnuSettingsSortMode.Name = "mnuSettingsSortMode";
            this.mnuSettingsSortMode.Size = new System.Drawing.Size(146, 22);
            this.mnuSettingsSortMode.Text = "Sort Mode";
            // 
            // fileFilterToolStripMenuItem
            // 
            this.fileFilterToolStripMenuItem.Name = "fileFilterToolStripMenuItem";
            this.fileFilterToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.fileFilterToolStripMenuItem.Text = "File Filter...";
            this.fileFilterToolStripMenuItem.Click += new System.EventHandler(this.fileFilterToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noToolStripMenuItem});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // noToolStripMenuItem
            // 
            this.noToolStripMenuItem.Name = "noToolStripMenuItem";
            this.noToolStripMenuItem.Size = new System.Drawing.Size(87, 22);
            this.noToolStripMenuItem.Text = "No";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblStatus2,
            this.lblStatus4,
            this.lblStatus3});
            this.statusStrip1.Location = new System.Drawing.Point(0, 673);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(792, 26);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = false;
            this.lblStatus.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Right | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.lblStatus.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(120, 21);
            this.lblStatus.Text = "Status 1";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatus2
            // 
            this.lblStatus2.AutoSize = false;
            this.lblStatus2.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Right | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.lblStatus2.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.lblStatus2.Name = "lblStatus2";
            this.lblStatus2.Size = new System.Drawing.Size(120, 21);
            this.lblStatus2.Text = "Status 2";
            this.lblStatus2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatus4
            // 
            this.lblStatus4.AutoSize = false;
            this.lblStatus4.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Right | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.lblStatus4.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.lblStatus4.Name = "lblStatus4";
            this.lblStatus4.Size = new System.Drawing.Size(120, 21);
            this.lblStatus4.Text = "Status 4";
            this.lblStatus4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatus3
            // 
            this.lblStatus3.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Right | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.lblStatus3.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.lblStatus3.Name = "lblStatus3";
            this.lblStatus3.Size = new System.Drawing.Size(417, 21);
            this.lblStatus3.Spring = true;
            this.lblStatus3.Text = "Status 3";
            this.lblStatus3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.scrPanelVertical);
            this.pnlMain.Controls.Add(this.pnlThumbnailView);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(204, 49);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(588, 624);
            this.pnlMain.TabIndex = 3;
            // 
            // scrPanelVertical
            // 
            this.scrPanelVertical.Dock = System.Windows.Forms.DockStyle.Right;
            this.scrPanelVertical.Location = new System.Drawing.Point(572, 0);
            this.scrPanelVertical.Name = "scrPanelVertical";
            this.scrPanelVertical.Size = new System.Drawing.Size(16, 624);
            this.scrPanelVertical.TabIndex = 6;
            // 
            // pnlThumbnailView
            // 
            this.pnlThumbnailView.BackColor = System.Drawing.Color.White;
            this.pnlThumbnailView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlThumbnailView.Location = new System.Drawing.Point(0, 0);
            this.pnlThumbnailView.Name = "pnlThumbnailView";
            this.pnlThumbnailView.Size = new System.Drawing.Size(588, 624);
            this.pnlThumbnailView.TabIndex = 4;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnRemoveTags);
            this.panel3.Controls.Add(this.btnAddTags);
            this.panel3.Controls.Add(this.btnUpdateTags);
            this.panel3.Controls.Add(this.txtTagEditor);
            this.panel3.Controls.Add(this.lblAutoSuggest);
            this.panel3.Controls.Add(this.pnlTagListContainer);
            this.panel3.Controls.Add(this.btnClear);
            this.panel3.Controls.Add(this.btnSearch);
            this.panel3.Controls.Add(this.txtSearch);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 49);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(201, 624);
            this.panel3.TabIndex = 10;
            // 
            // btnRemoveTags
            // 
            this.btnRemoveTags.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRemoveTags.Location = new System.Drawing.Point(0, 366);
            this.btnRemoveTags.Name = "btnRemoveTags";
            this.btnRemoveTags.Size = new System.Drawing.Size(201, 23);
            this.btnRemoveTags.TabIndex = 11;
            this.btnRemoveTags.Text = "&Remove";
            this.btnRemoveTags.UseVisualStyleBackColor = true;
            // 
            // btnAddTags
            // 
            this.btnAddTags.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAddTags.Location = new System.Drawing.Point(0, 343);
            this.btnAddTags.Name = "btnAddTags";
            this.btnAddTags.Size = new System.Drawing.Size(201, 23);
            this.btnAddTags.TabIndex = 10;
            this.btnAddTags.Text = "&Add";
            this.btnAddTags.UseVisualStyleBackColor = true;
            // 
            // btnUpdateTags
            // 
            this.btnUpdateTags.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnUpdateTags.Location = new System.Drawing.Point(0, 320);
            this.btnUpdateTags.Name = "btnUpdateTags";
            this.btnUpdateTags.Size = new System.Drawing.Size(201, 23);
            this.btnUpdateTags.TabIndex = 9;
            this.btnUpdateTags.Text = "Up&date";
            this.btnUpdateTags.UseVisualStyleBackColor = true;
            // 
            // txtTagEditor
            // 
            this.txtTagEditor.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtTagEditor.Location = new System.Drawing.Point(0, 273);
            this.txtTagEditor.MaxLength = 32767;
            this.txtTagEditor.Name = "txtTagEditor";
            this.txtTagEditor.Size = new System.Drawing.Size(201, 47);
            this.txtTagEditor.TabIndex = 8;
            this.txtTagEditor.Text = "";
            // 
            // lblAutoSuggest
            // 
            this.lblAutoSuggest.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAutoSuggest.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAutoSuggest.Location = new System.Drawing.Point(0, 243);
            this.lblAutoSuggest.Name = "lblAutoSuggest";
            this.lblAutoSuggest.Size = new System.Drawing.Size(201, 30);
            this.lblAutoSuggest.TabIndex = 13;
            this.lblAutoSuggest.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlTagListContainer
            // 
            this.pnlTagListContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlTagListContainer.Controls.Add(this.pnlTagList);
            this.pnlTagListContainer.Controls.Add(this.scrTagListVertical);
            this.pnlTagListContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTagListContainer.Location = new System.Drawing.Point(0, 66);
            this.pnlTagListContainer.Name = "pnlTagListContainer";
            this.pnlTagListContainer.Size = new System.Drawing.Size(201, 177);
            this.pnlTagListContainer.TabIndex = 12;
            // 
            // pnlTagList
            // 
            this.pnlTagList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTagList.Location = new System.Drawing.Point(0, 0);
            this.pnlTagList.Name = "pnlTagList";
            this.pnlTagList.Size = new System.Drawing.Size(181, 173);
            this.pnlTagList.TabIndex = 2;
            this.pnlTagList.TabStop = false;
            // 
            // scrTagListVertical
            // 
            this.scrTagListVertical.Dock = System.Windows.Forms.DockStyle.Right;
            this.scrTagListVertical.Location = new System.Drawing.Point(181, 0);
            this.scrTagListVertical.Name = "scrTagListVertical";
            this.scrTagListVertical.Size = new System.Drawing.Size(16, 173);
            this.scrTagListVertical.TabIndex = 3;
            // 
            // btnClear
            // 
            this.btnClear.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClear.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnClear.Location = new System.Drawing.Point(0, 43);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(201, 23);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "&Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            // 
            // btnSearch
            // 
            this.btnSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSearch.Location = new System.Drawing.Point(0, 20);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(201, 23);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // txtSearch
            // 
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtSearch.Location = new System.Drawing.Point(0, 0);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(201, 20);
            this.txtSearch.TabIndex = 14;
            // 
            // frmMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 699);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel3);
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
            this.pnlMain.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.pnlTagListContainer.ResumeLayout(false);
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
        private System.Windows.Forms.Panel pnlMain;
        private GokiLibrary.UserInterface.DoubleBufferedPanel pnlThumbnailView;
        private System.Windows.Forms.VScrollBar scrPanelVertical;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus4;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus3;
        private System.Windows.Forms.ToolStripMenuItem mnuSettings;
        private System.Windows.Forms.ToolStripMenuItem mnuSettingsSelectionMode;
        private System.Windows.Forms.ToolStripMenuItem mnuSettingsSelectionModeExplorer;
        private System.Windows.Forms.ToolStripMenuItem mnuSettingsSelectionModeToggle;
        private System.Windows.Forms.ToolStripMenuItem organizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuOrganizeMoveSelectedFiles;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuViewZoom;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noToolStripMenuItem;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnRemoveTags;
        private System.Windows.Forms.Button btnAddTags;
        private System.Windows.Forms.Button btnUpdateTags;
        private System.Windows.Forms.RichTextBox txtTagEditor;
        private System.Windows.Forms.Panel pnlTagListContainer;
        private GokiLibrary.UserInterface.DoubleBufferedPanel pnlTagList;
        private System.Windows.Forms.VScrollBar scrTagListVertical;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus2;
        private System.Windows.Forms.ToolStripMenuItem randomlyTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuSettingsSortMode;
        private System.Windows.Forms.ToolStripMenuItem fileFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSelectedFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
        private System.Windows.Forms.Label lblAutoSuggest;
        private System.Windows.Forms.TextBox txtSearch;
    }
}


using GokiLibrary;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

// TODO: Documentation
// TODO: Tag categories (artist, character, copyright, default)

namespace gokiTagDB
{
    public partial class frmMainForm : Form
    {
        
        public static int thumbnailWidth = 128;
        public static int thumbnailHeight = 128;
        public static int zoomIndex = 3;
        public static float[] zoomLevels = new float[7] { .25f, .50f, .75f, 1.0f, 1.25f, 1.5f, 2.0f };

        private string autoSuggestEntry = "";
        private int panelPadding = 2;
        private int entryMargin = 4;
        private int entryPadding = 2;
        private int borderSize = 2;
        private int panelOffset = 0;
        private int activeIndex = -1;
        private int tagListEntryHeight = 16;
        private int tagListHoverIndex = -1;
        private int selectionMode = 0;
        private bool controlDown = false;
        private bool shiftDown = false;
        private bool isFormClosing = false;
        private long usedMemory = 0;
        private long allowedMemoryUsage = 100000000;
        private Process process;
        private DBEntry hoverEntry = null;
        private ToolTip suggestTip = new ToolTip();
        private bool isGenerationThreaded = true;
        private bool thumbnailPanelDirty = false;

        private System.Windows.Forms.Timer generationTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer invalidateTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer processTimer = new System.Windows.Forms.Timer();

        private GokiBitmap marchingAntsBitmapOld;
        private GokiBitmap marchingAntsBitmapNew;
        private GokiBitmap marchingAntsBitmapSelectedActive;

        Thread generationThread;
        CancellationTokenSource generationCancelTokenSource = new CancellationTokenSource();
        public static BlockingCollection<DBEntry> thumbnailGenerationQueue = new BlockingCollection<DBEntry>();

        public frmMainForm()
        {
            InitializeComponent();
            Load += frmMainForm_Load;
        }

        void frmMainForm_Load(object sender, EventArgs e)
        {
            process = Process.GetCurrentProcess();

            AllowDrop = true;

            foreach( string entry in Enum.GetNames(typeof(SortType)))
            {
                ToolStripMenuItem sortModeItem = new ToolStripMenuItem(entry);
                sortModeItem.Click += sortModeItem_Click;
                mnuSettingsSortMode.DropDownItems.Add(sortModeItem);
            }

            btnSearch.Click += btnSearch_Click;
            btnClear.Click += btnClear_Click;
            btnAddTags.Click += btnAddTags_Click;
            btnUpdateTags.Click += btnUpdateTags_Click;
            btnRemoveTags.Click += btnRemoveTags_Click;

            pnlThumbnailView.MouseMove += pnlThumbnailView_MouseMove;
            pnlThumbnailView.MouseDown += pnlThumbnailView_MouseDown;
            pnlThumbnailView.MouseDoubleClick += pnlThumbnailView_MouseDoubleClick;
            pnlThumbnailView.Paint += pnlThumbnailView_Paint;
            pnlThumbnailView.Resize += pnlThumbnailView_Resize;

            pnlTagList.Paint += pnlTagList_Paint;
            pnlTagList.MouseMove += pnlTagList_MouseMove;
            pnlTagList.MouseLeave += pnlTagList_MouseLeave;
            pnlTagList.MouseDown += pnlTagList_MouseDown;
            pnlTagList.Resize += pnlTagList_Resize;

            scrPanelVertical.Scroll += scrPanelVertical_Scroll;
            scrTagListVertical.Scroll += scrTagListVertical_Scroll;

            txtSearch.TextChanged += autoSuggestTextboxTextChanged;
            txtTagEditor.Click += txtTagEditor_Click;
            txtTagEditor.TextChanged += autoSuggestTextboxTextChanged;

            for ( int i = 0; i < zoomLevels.Length; i++)
            {
                ToolStripMenuItem zoomLevelItem = new ToolStripMenuItem(String.Format("{0:P0}",zoomLevels[i]));
                zoomLevelItem.Tag = i;
                zoomLevelItem.Click += zoomLevelItem_Click;
                mnuViewZoom.DropDownItems.Add(zoomLevelItem);
            }

            generationTimer.Interval = 35;
            generationTimer.Tick += generationTimer_Tick;
            try
            {
                generationThread = new Thread(handleGenerationQueueTask);
                generationThread.Start();
            }
            catch( Exception ex)
            {
                Console.WriteLine("Could not create task... defaulting to single thread.");
                generationTimer.Interval = 35;
                generationTimer.Tick += generationTimer_Tick;
                generationTimer.Start();
                isGenerationThreaded = false;
            }

            invalidateTimer.Interval = 35;
            invalidateTimer.Tick += invalidateTimer_Tick;
            invalidateTimer.Start();

            processTimer.Interval = 3000;
            processTimer.Tick += processTimer_Tick;
            processTimer.Start();

            KeyDown += frmMainForm_KeyDown;
            KeyUp += frmMainForm_KeyUp;
            MouseWheel += mouseWheel;
            txtTagEditor.MouseWheel += mouseWheel;
            DragDrop += dragDrop;
            DragEnter += dragEnter;
            FormClosing += frmMainForm_FormClosing;

            GokiTagDB.databaseStream = new MemoryStream();
            GokiTagDB.thumbnailIndexStream = new MemoryStream();
            GokiTagDB.thumbnailStream = new FileStream(GokiTagDB.thumbnailsPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            marchingAntsBitmapOld = new GokiBitmap(ThumbnailWidth/2, ThumbnailHeight/2);
            marchingAntsBitmapNew = new GokiBitmap(ThumbnailWidth / 2, ThumbnailHeight / 2);
            marchingAntsBitmapSelectedActive = new GokiBitmap(ThumbnailWidth / 2, ThumbnailHeight / 2);

            marchingAntsBitmapOld.marchingAntsRectangle(0, 0, ThumbnailWidth / 2 - 1, ThumbnailHeight / 2 - 1, (int)(DateTime.Now.Millisecond / 1000f), Color.PowderBlue, Color.DodgerBlue);
            marchingAntsBitmapNew.marchingAntsRectangle(0, 0, ThumbnailWidth / 2 - 1, ThumbnailHeight / 2 - 1, (int)(DateTime.Now.Millisecond / 1000f), Color.Gold, Color.DarkGoldenrod);
            marchingAntsBitmapSelectedActive.marchingAntsRectangle(0, 0, ThumbnailWidth / 2 - 1, ThumbnailHeight / 2 - 1, (int)(DateTime.Now.Millisecond / 1000f), Color.DarkGreen, Color.DarkGoldenrod);

            updateMenuControls();
            updateTitle();
            SaveAndLoad.loadTags();
            SaveAndLoad.loadSettings();
            SaveAndLoad.loadDatabase();
            SaveAndLoad.loadThumbnails();
            updatePanelScrollbar();
        }

        #region Generation

        void handleGenerationQueueTask()
        {
            while (!generationCancelTokenSource.Token.IsCancellationRequested)
            {
                handleGenerationQueue();
            }
        }

        void handleGenerationQueue()
        {
            try
            {
                double totalTime = 0;
                DateTime startTime = DateTime.Now;

                while ( totalTime < 100 && !isFormClosing)
                {
                    DBEntry entry = thumbnailGenerationQueue.Take(generationCancelTokenSource.Token);
                    if (entry != null)
                    {
                        entry.generateThumbnail(GokiTagDB.thumbnailStream);
                        try
                        {
                            SaveAndLoad.addThumbnailToDatabase(entry);
                        }
                        catch( IOException ex)
                        {
                            break;
                        }
                        totalTime += (DateTime.Now - startTime).TotalMilliseconds;
                    }
                }

                queueRedraw();
                if (!isGenerationThreaded)
                {
                    if (thumbnailGenerationQueue.Count == 0)
                    {
                        generationTimer.Stop();
                    }
                    else
                    {
                        generationTimer.Start();
                    }
                }
                else
                {
                    if (thumbnailGenerationQueue.Count == 0)
                    {
                        // generationTask.Wait(250);
                    }
                    else
                    {
                      //  generationTask.Wait(25);
                    }
                }
            }
            catch( Exception ex)
            {
                Console.WriteLine("Issue generating thumbnail: " + ex.Message);
            }
        }

        #endregion Generation

        #region Properties

        int MaxColumns
        {
            get
            {
                return (pnlThumbnailView.Width - panelPadding * 2) / (ThumbnailWidth + (entryMargin + entryPadding) * 2);
            }
        }

        int ThumbnailWidth
        {
            get
            {
                return (int)(thumbnailWidth * Zoom);
            }
        }

        int ThumbnailHeight
        {
            get
            {
                return (int)(thumbnailHeight * Zoom);
            }
        }

        float Zoom
        {
            get
            {
                return zoomLevels[zoomIndex];
            }
        }

        #endregion Properties

        #region Mouse and Keyboard

        bool isMouseOverControl(Control control, int x, int y)
        {
            Point locationOnForm = control.FindForm().PointToClient(control.Parent.PointToScreen(control.Location));
            int left = locationOnForm.X;
            int top = locationOnForm.Y;
            if (x >= left && x <= left + control.Width)
            {
                if (y > top && y <= top + control.Height)
                {
                    return true;
                }
            }
            return false;
        }

        Point controlLocation(Control control)
        {
            return control.FindForm().PointToClient(control.Parent.PointToScreen(control.Location));
        }

        

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Tab:
                    tabKeyPressed();
                    break;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
            return true;
        }

        void tabKeyPressed()
        {
            if (txtTagEditor.Focused || txtSearch.Focused)
            {
                TextBoxBase textbox = null;
                if (txtTagEditor.Focused)
                {
                    textbox = txtTagEditor;
                }
                else
                {
                    textbox = txtSearch;
                }
                string text = textbox.Text;
                int caretIndex = textbox.SelectionStart;
                int previousSpaceIndex = 0;
                int spaceCheck = 0;
                while (spaceCheck < caretIndex)
                {
                    if (text[spaceCheck] == ' ')
                    {
                        previousSpaceIndex = spaceCheck + 1;
                    }
                    spaceCheck++;
                }

                string selectedEntry = autoSuggestEntry;
                text = text.Substring(0, previousSpaceIndex) + selectedEntry + " " + text.Substring(caretIndex, text.Length - caretIndex);
                textbox.Text = text;
                textbox.SelectionStart = previousSpaceIndex + selectedEntry.Length + 1;
            }
        }

        void mouseWheel(object sender, MouseEventArgs e)
        {
            if (isMouseOverControl(pnlTagList, e.X, e.Y))
            {
                scrTagListVertical.Value = Math.Min(Math.Max(scrTagListVertical.Minimum, scrTagListVertical.Value - e.Delta), Math.Max(scrTagListVertical.Maximum - scrTagListVertical.LargeChange, scrTagListVertical.Minimum));
                pnlTagList.Invalidate();
            }
            else if (isMouseOverControl(pnlThumbnailView, e.X, e.Y))
            {
                scrPanelVertical.Value = Math.Min(Math.Max(scrPanelVertical.Minimum, scrPanelVertical.Value - e.Delta), Math.Max(scrPanelVertical.Maximum - scrPanelVertical.LargeChange, scrPanelVertical.Minimum));
                panelOffset = scrPanelVertical.Value;
                queueRedraw();
            }
        }

        void txtTagEditor_Click(object sender, EventArgs e)
        {
            txtTagEditor.SelectAll();
        }

        private void mnuSettingsSelectionModeExplorer_Click(object sender, EventArgs e)
        {
            selectionMode = 0;
            updateMenuControls();
        }
        private void mnuSettingsSelectionModeToggle_Click(object sender, EventArgs e)
        {
            selectionMode = 1;
            updateMenuControls();
        }

        private void entriesPerPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEntriesPerPage entriesForm = new frmEntriesPerPage();
            entriesForm.ShowDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAndLoad.saveTags();
            SaveAndLoad.saveDatabase();
            SaveAndLoad.saveThumbnails();
            SaveAndLoad.saveSettings();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void zoomLevelItem_Click(object sender, EventArgs e)
        {
            zoomIndex = (int)((ToolStripMenuItem)sender).Tag;
            foreach (ToolStripMenuItem item in mnuViewZoom.DropDownItems)
            {
                item.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            updatePanelScrollbar();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            GokiTagDB.queriedEntries.Clear();
            GokiTagDB.tagCounts.Clear();
            txtSearch.Text = "";
            pnlTagList.Invalidate();
            queueRedraw();
        }

        private void btnUpdateTags_Click(object sender, EventArgs e)
        {
            int amountUpdated = 0;
            foreach (DBEntry entry in GokiTagDB.queriedEntries)
            {
                if (entry.Selected)
                {
                    foreach (string checkTag in txtTagEditor.Text.Split(' '))
                    {
                        if (!Tags.tags.ContainsKey(checkTag))
                        {
                            Tags.addTag(checkTag, null);
                        }
                    }

                    SaveAndLoad.editEntry(entry, new DBEntry(entry.Location, txtTagEditor.Text.Trim()));
                    amountUpdated++;
                }
            }
            lblStatus.Text = amountUpdated + " file(s) tags updated.";
        }

        void pnlTagList_MouseDown(object sender, MouseEventArgs e)
        {
            if (tagListHoverIndex >= 0)
            {
                List<KeyValuePair<string, int>> pairs = getSortedTagList();
                if (pairs.Count > tagListHoverIndex)
                {
                    txtSearch.Text = pairs[tagListHoverIndex].Key;
                    btnSearch_Click(null, null);
                }
            }
        }

        void pnlTagList_MouseLeave(object sender, EventArgs e)
        {
            tagListHoverIndex = -1;
            pnlTagList.Invalidate();
        }

        void pnlTagList_MouseMove(object sender, MouseEventArgs e)
        {
            tagListHoverIndex = (e.Y + scrTagListVertical.Value) / tagListEntryHeight;
            pnlTagList.Invalidate();
        }

        void btnRemoveTags_Click(object sender, EventArgs e)
        {
            int amountUpdated = 0;
            foreach (DBEntry entry in GokiTagDB.queriedEntries)
            {
                if (entry.Selected)
                {
                    List<string> existingTags = GokiTagDB.entries[entry.Location].TagString.Split(' ').ToList();
                    foreach (string checkTag in txtTagEditor.Text.Split(' '))
                    {
                        if (existingTags.Contains(checkTag))
                        {
                            existingTags.Remove(checkTag);
                        }
                    }
                    SaveAndLoad.editEntry(entry, new DBEntry(entry.Location, String.Join(" ", existingTags).Trim()));
                    amountUpdated++;
                }
            }
            lblStatus.Text = amountUpdated + " file(s) tags updated. @ " + DateTime.Now.ToString();
            updateTagCounts();
            pnlTagList.Invalidate();
        }

        void btnAddTags_Click(object sender, EventArgs e)
        {
            int amountUpdated = 0;
            foreach (DBEntry entry in GokiTagDB.queriedEntries)
            {
                if (entry.Selected)
                {
                    List<string> existingTags = GokiTagDB.entries[entry.Location].TagString.Split(' ').ToList();
                    foreach (string checkTag in txtTagEditor.Text.Split(' '))
                    {
                        if (!existingTags.Contains(checkTag))
                        {
                            existingTags.Add(checkTag);
                            if (!Tags.tags.ContainsKey(checkTag))
                            {
                                Tags.addTag(checkTag, null);
                            }
                        }
                    }
                    SaveAndLoad.editEntry(entry, new DBEntry(entry.Location, String.Join(" ",existingTags)));
                    amountUpdated++;
                }
            }
            lblStatus.Text = amountUpdated + " file(s) tags updated. @ " + DateTime.Now.ToString();
            updateTagCounts();
            pnlTagList.Invalidate();
        }

        void frmMainForm_KeyUp(object sender, KeyEventArgs e)
        {
            controlDown = e.Control;
            shiftDown = e.Shift;
        }

        void frmMainForm_KeyDown(object sender, KeyEventArgs e)
        {
            controlDown = e.Control;
            shiftDown = e.Shift;
            if ( e.KeyCode == Keys.Delete && !txtTagEditor.Focused && !txtSearch.Focused )
            {
                List<DBEntry> selectedEntries = getSelectedEntries();
                SaveAndLoad.removeEntriesFromDatabase(selectedEntries);
                queueRedraw();
                lblStatus4.Text = selectedEntries.Count + " file(s) removed from the database.";
            }
        }

        void pnlThumbnailView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X <= (ThumbnailWidth + entryMargin + entryPadding * 2) * MaxColumns - panelPadding * 2)
            {
                int index = getPanelIndex(e.X, e.Y);
                if (index < GokiTagDB.queriedEntries.Count && index >= 0)
                {
                    hoverEntry = GokiTagDB.queriedEntries[index];
                }
                else
                {
                    hoverEntry = null;
                }
                if (GokiTagDB.queriedEntries.Count > index)
                {
                    lblStatus3.Text = GokiTagDB.queriedEntries[index].Location;
                }
            }
            else
            {
                hoverEntry = null;
            }
            queueRedraw();
        }

        void pnlThumbnailView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X <= (ThumbnailWidth + entryMargin) * MaxColumns - panelPadding * 2)
            {
                int index = getPanelIndex(e.X, e.Y);
                if (index < GokiTagDB.queriedEntries.Count)
                {
                    if (selectionMode == 0 && (!controlDown && !shiftDown))
                    {
                        deselectAllSelectedEntries();
                        activeIndex = index;
                    }
                    if (selectionMode == 1 || (!shiftDown || activeIndex == -1))
                    {
                        if (GokiTagDB.queriedEntries.Count > index)
                        {
                            GokiTagDB.queriedEntries[index].Selected = !GokiTagDB.queriedEntries[index].Selected;
                        }
                    }
                    else if (shiftDown && activeIndex >= 0)
                    {
                        int startIndex = activeIndex;
                        if (index < activeIndex)
                        {
                            int tempIndex = index;
                            index = startIndex;
                            startIndex = tempIndex;
                        }
                        deselectAllSelectedEntries();
                        for (int i = startIndex; i <= index; i++)
                        {
                            GokiTagDB.queriedEntries[i].Selected = true;
                        }
                    }
                }
            }
            else
            {
                deselectAllSelectedEntries();
            }
            updateTagCounts();
            lblStatus4.Text = getSelectedEntries().Count + " file(s) selected.";
            queueRedraw();
            pnlTagList.Invalidate();
        }

        void pnlThumbnailView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = getPanelIndex(e.X, e.Y);
            if (File.Exists(GokiTagDB.queriedEntries[index].Location) && GokiTagDB.queriedEntries.Count > index)
            {
                Process.Start(GokiTagDB.queriedEntries[index].Location);
            }
            else
            {
                MessageBox.Show("File not found.", "File missing");
            }
        }

        private void mnuOrganizeMoveSelectedFiles_Click(object sender, EventArgs e)
        {
            if (getSelectedEntries().Count > 0)
            {
                // TODO: FIND A BETTER WAY TO REPLACE THIS
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    moveSelectedEntries(folderBrowserDialog.SelectedPath);
                }
            }
            else
            {
                MessageBox.Show("No files selected.", "Move failed");
            }
        }



        private void randomlyTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DBEntry entry in getSelectedEntries())
            {
                SaveAndLoad.editEntry(entry, new DBEntry(entry.Location, GokiRandom.randomIntPositive(10000).ToString()));
            }
        }

        private void fileFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFileFilter fileFilterForm = new frmFileFilter();
            fileFilterForm.ShowDialog();
        }

        private void deleteSelectedFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getSelectedEntries().Count > 0)
            {
                deleteSelectedEntries();
            }
            else
            {
                MessageBox.Show("No files selected.", "Delete failed");
            }
        }

        void sortModeItem_Click(object sender, EventArgs e)
        {
            string entry = ((ToolStripMenuItem)sender).Text;
            GokiTagDB.sortType = (SortType)Enum.Parse(typeof(SortType), entry);
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DBEntry entry in GokiTagDB.queriedEntries)
            {
                entry.Selected = true;
            }
            queueRedraw();
        }

        #endregion Mouse and Keyboard

        #region Controls

        void autoSuggestTextboxTextChanged(object sender, EventArgs e)
        {
            bool isShown = false;
            if (Tags.tags.Count > 0)
            {
                TextBoxBase textbox = (TextBoxBase)sender;
                if (textbox.Text.Length > 0)
                {
                    string wordUnderCaret = "";
                    string text = textbox.Text;
                    int caretIndex = textbox.SelectionStart;
                    int previousSpaceIndex = 0;
                    int spaceCheck = 0;
                    while (spaceCheck < caretIndex)
                    {
                        if (text[spaceCheck] == ' ')
                        {
                            previousSpaceIndex = spaceCheck + 1;
                        }
                        spaceCheck++;
                    }
                    wordUnderCaret = text.Substring(previousSpaceIndex, caretIndex - previousSpaceIndex);
                    if (wordUnderCaret.Length > 0)
                    {
                        List<KeyValuePair<string, int>> matches = new List<KeyValuePair<string, int>>();
                        foreach (string tag in Tags.tags.Keys.ToList())
                        {
                            if (tag.Length > 0)
                            {
                                matches.Add(new KeyValuePair<string, int>(tag, scoreStringCompare(tag, wordUnderCaret)));
                            }
                        }
                        matches.Sort((firstPair, nextPair) =>
                        {
                            return -firstPair.Value.CompareTo(nextPair.Value);
                        });


                        Control control = (Control)sender;
                        Point locationOnForm = control.FindForm().PointToClient(control.Parent.PointToScreen(control.Location));
                        locationOnForm.X += control.Width;
                        string message = matches[0].Key + " (" + matches[0].Value + ")";
                        if (matches.Count > 0)
                        {
                            lblAutoSuggest.Text = message;
                            autoSuggestEntry = matches[0].Key;
                            isShown = true;
                        }

                    }

                }

            }
            if (!isShown)
            {
                suggestTip.Hide(this);
                autoSuggestEntry = "";
            }
        }

        void pnlTagList_Resize(object sender, EventArgs e)
        {
            updateTagListScrollbar();
        }

        void scrTagListVertical_Scroll(object sender, ScrollEventArgs e)
        {
            pnlTagList.Invalidate();
        }

        void scrPanelVertical_Scroll(object sender, ScrollEventArgs e)
        {
            panelOffset = e.NewValue;
            queueRedraw();
        }

        void pnlThumbnailView_Resize(object sender, EventArgs e)
        {
            queueRedraw();
            updatePanelScrollbar();
        }

        void frmMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
           // this.Hide();
            try
            {
                generationCancelTokenSource.Cancel();
                SaveAndLoad.saveTags();
                SaveAndLoad.saveDatabase();
                SaveAndLoad.saveThumbnails();
                SaveAndLoad.saveSettings();
                GokiTagDB.thumbnailIndexStream.Close();
                GokiTagDB.thumbnailStream.Close();
                GokiTagDB.databaseStream.Close();
               // saveThumbnails();
            }
            catch (Exception ex)
            {
                this.Show();
                e.Cancel = true;
            }
            
        }

        #endregion Controls

        #region Methods

        private void deleteSelectedEntries()
        {
            foreach (DBEntry entry in getSelectedEntries())
            {
                if (File.Exists(entry.Location))
                {
                    File.Delete(entry.Location);
                    SaveAndLoad.removeEntriesFromDatabase(getSelectedEntries());
                }
            }
        }

        void updateTitle()
        {
            try
            {
                usedMemory = process.PrivateMemorySize64;
                Text = String.Format("gokiTagDB - {0:N0}KB", usedMemory / 1024f);
            }
            catch (PlatformNotSupportedException ex)
            {
                Text = String.Format("gokiTagDB");
            }
        }

        void processTimer_Tick(object sender, EventArgs e)
        {
            process.Refresh();
            updateTitle();
        }

        void invalidateTimer_Tick(object sender, EventArgs e)
        {
            if ( thumbnailPanelDirty )
            {
                pnlThumbnailView.Invalidate();
                thumbnailPanelDirty = false;
                lblStatus2.Text = String.Format("{0:N0} thumbnails in queue", thumbnailGenerationQueue.Count);

            }
        }

        void queueRedraw()
        {
            thumbnailPanelDirty = true;
        }

        void generationTimer_Tick(object sender, EventArgs e)
        {
            generationTimer.Stop();
            if (!isGenerationThreaded)
            {
                handleGenerationQueue();
                queueRedraw();
            }
        }

        int scoreStringCompare(string target, string entry)
        {
            int score = 0;
            for (int i = 0; i < target.Length; i++)
            {
                if (entry.Contains(target[i]))
                {
                    score++;
                    if (Math.Abs(entry.IndexOf(target[i]) - i) < 3)
                    {
                        score += 3 - Math.Abs(entry.IndexOf(target[i]) - i);
                    }
                }
                else
                {
                    score--;
                }
            }
            for (int i = 0; i < Math.Min(target.Length, entry.Length); i++)
            {
                int consecutive = 0;
                if (target[i].Equals(entry[i]))
                {
                    score += ++consecutive;
                }
                else
                {
                    score--;
                    consecutive = 0;
                }
            }
            for (int i = 0; i < target.Length; i++)
            {
                int checkLength = Math.Min(target.Length - i, entry.Length);
                if (entry.Substring(0, checkLength).Equals(target.Substring(i, checkLength)))
                {
                    score += checkLength * checkLength;
                }
            }

            return score;
        }

        int stringMaximumScore(string target)
        {
            return target.Length * 2 - 1 + target.Length * target.Length;
        }

        void updateMenuControls()
        {
            mnuSettingsSelectionModeExplorer.Checked = selectionMode == 0;
            mnuSettingsSelectionModeToggle.Checked = selectionMode == 1;
        }

        void updateTagCounts()
        {
            GokiTagDB.tagCounts.Clear();

            List<DBEntry> entryPool = GokiTagDB.queriedEntries;
            List<DBEntry> selectedEntries = getSelectedEntries();

            if (selectedEntries.Count > 0)
            {
                entryPool = selectedEntries;
            }

            foreach (DBEntry entry in entryPool)
            {
                string[] tags = entry.TagString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string tag in tags)
                {
                    if (GokiTagDB.tagCounts.ContainsKey(tag))
                    {
                        GokiTagDB.tagCounts[tag]++;
                    }
                    else
                    {
                        GokiTagDB.tagCounts.Add(tag, 1);
                    }
                }
            }

            updateTagListScrollbar();
        }

        void updateTagListScrollbar()
        {
            scrTagListVertical.SmallChange = tagListEntryHeight;
            scrTagListVertical.LargeChange = tagListEntryHeight * 5;
            scrTagListVertical.Minimum = 0;
            scrTagListVertical.Maximum = Math.Max(GokiTagDB.tagCounts.Count * tagListEntryHeight + scrTagListVertical.LargeChange - pnlTagList.Height, scrTagListVertical.Minimum);
            scrTagListVertical.Value = scrTagListVertical.Minimum;
        }

        List<KeyValuePair<string, int>> getSortedTagList()
        {
            List<KeyValuePair<string, int>> pairs = GokiTagDB.tagCounts.ToList();
            pairs.Sort((firstPair, nextPair) =>
            {
                return -(firstPair.Value.CompareTo(nextPair.Value) * 10 - firstPair.Key.CompareTo(nextPair.Key));
            }
            );
            return pairs;
        }

        public int getPanelIndex(int mouseX, int mouseY)
        {
            return mouseX / (ThumbnailWidth + entryMargin + entryPadding * 2) + (mouseY + panelOffset) / (ThumbnailHeight + entryMargin + entryPadding * 2) * MaxColumns;
        }

        List<DBEntry> getSelectedEntries()
        {
            List<DBEntry> selectedEntries = new List<DBEntry>();
            foreach (DBEntry entry in GokiTagDB.queriedEntries)
            {
                if (entry.Selected)
                {
                    selectedEntries.Add(entry);
                }
            }
            return selectedEntries;
        }

        void deselectAllSelectedEntries()
        {
            foreach (DBEntry entry in GokiTagDB.queriedEntries)
            {
                entry.Selected = false;
            }
        }

        void updatePanelScrollbar()
        {
            int totalHeight = (int)Math.Ceiling(GokiTagDB.queriedEntries.Count / (float)MaxColumns) * (ThumbnailHeight + entryMargin + entryPadding * 2) + panelPadding * 2;
            double oldPercentage = scrPanelVertical.Value / (double)scrPanelVertical.Maximum;
            if (totalHeight > 1)
            {
                scrPanelVertical.Minimum = 0;
                scrPanelVertical.Maximum = totalHeight;
                scrPanelVertical.SmallChange = (ThumbnailHeight + entryMargin);
                scrPanelVertical.LargeChange = pnlThumbnailView.Height;
                scrPanelVertical.Enabled = true;
            }
            else
            {
                scrPanelVertical.Minimum = 0;
                scrPanelVertical.Maximum = 0;
                scrPanelVertical.SmallChange = 0;
                scrPanelVertical.LargeChange = 0;
                scrPanelVertical.Enabled = false;
            }
            scrPanelVertical.Value = (int)(scrPanelVertical.Maximum * oldPercentage);
            panelOffset = scrPanelVertical.Value;
        }

        void moveSelectedEntries(string folderPath)
        {
            int errors = 0;
            foreach ( DBEntry entry in getSelectedEntries())
            {
                try
                {
                    String newFilePath = folderPath + @"\" + Path.GetFileName(entry.Location);
                    if (!File.Exists(newFilePath))
                    {
                        File.Move(entry.Location, newFilePath);
                        GokiTagDB.thumbnailInfo[entry.Location].Location = newFilePath;
                        GokiTagDB.entries.Remove(entry.Location);
                        entry.Location = newFilePath;
                        GokiTagDB.entries.Add(newFilePath, entry);
                    }
                    
                }
                catch( Exception ex)
                {
                    errors++;
                }
            }
            GokiTagDB.thumbnailIndexStream.Seek(0, SeekOrigin.Begin);
            GokiTagDB.thumbnailIndexStream.SetLength(0);

            foreach (ThumbnailInfo info in GokiTagDB.thumbnailInfo.Values)
            {
                SaveAndLoad.addThumbnailIndexToDatabase(info);
            }
            if (errors > 0)
            {
                MessageBox.Show("Error moving " + errors + " file(s).");
            }
        }

        #endregion Methods

        #region Paint

        void pnlTagList_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font("Tahoma", 10);

            int x = 0;
            int y = 0;
            using (SolidBrush brush = new SolidBrush(Color.Black))
            {
                List<KeyValuePair<string, int>> pairs = getSortedTagList();
                int index = 0;
                foreach (KeyValuePair<string, int> pair in pairs)
                {
                    brush.Color = Color.Black;
                    if (index == tagListHoverIndex)
                    {
                        brush.Color = Color.Blue;
                    }
                    e.Graphics.DrawString(pair.Key + " " + pair.Value, font, brush, x, y - scrTagListVertical.Value);

                    y += tagListEntryHeight;
                    index++;
                }
            }
        }

        void pnlThumbnailView_Paint(object sender, PaintEventArgs e)
        {
            if (GokiTagDB.queriedEntries.Count > 0)
            {
                int x = panelPadding;
                int y = panelPadding;
                int row = 0;
                int column = 0;
                using (Pen pen = new Pen(Color.LightGray))
                {
                    pen.Width = borderSize;
                    using (SolidBrush brush = new SolidBrush(Color.Salmon))
                    {
                        foreach (DBEntry entry in GokiTagDB.queriedEntries)
                        {
                            pen.Color = Color.LightGray;
                            Rectangle rectangle = new Rectangle(x, y - panelOffset, ThumbnailWidth + entryPadding * 2, ThumbnailHeight + entryPadding * 2);
                            if (rectangle.IntersectsWith(new Rectangle(0, 0, pnlThumbnailView.Width, pnlThumbnailView.Height)))
                            {
                                bool highlighted = false;
                                if (GokiTagDB.queriedEntries.Count > activeIndex && activeIndex >= 0 && GokiTagDB.queriedEntries[activeIndex] == entry)
                                {
                                    if (entry.Selected)
                                    {
                                        pen.Color = Color.DarkCyan;
                                        brush.Color = Color.Cyan;
                                    }
                                    else
                                    {
                                        pen.Color = Color.DarkGoldenrod;
                                        brush.Color = Color.Gold;
                                    }
                                    highlighted = true;
                                }
                                else if (entry.Selected)
                                {
                                    pen.Color = Color.DarkGreen;
                                    brush.Color = Color.MediumSpringGreen;
                                    highlighted = true;
                                }
                                else if (entry == hoverEntry)
                                {
                                    pen.Color = Color.DarkSlateBlue;
                                    brush.Color = Color.LightBlue;
                                    highlighted = true;
                                }

                                if (entry.Thumbnail == null && !thumbnailGenerationQueue.Contains(entry))
                                {
                                    thumbnailGenerationQueue.Add(entry);
                                    if (!isGenerationThreaded && !generationTimer.Enabled)
                                    {
                                        generationTimer.Start();
                                    }
                                }

                                if (highlighted)
                                {
                                    e.Graphics.FillRectangle(brush, rectangle);
                                }
                                
                                if (entry.Thumbnail != null)
                                {
                                    try{

                                    
                                        e.Graphics.DrawImage(entry.Thumbnail, x + (ThumbnailWidth - entry.Thumbnail.Width * Zoom) / 2 + entryPadding, y + (ThumbnailHeight - entry.Thumbnail.Height * Zoom) / 2 - panelOffset + entryPadding, entry.Thumbnail.Width * Zoom, entry.Thumbnail.Height * Zoom);
                                    }
                                    catch (InvalidOperationException ex)
                                    {

                                    }
                                }
                                else
                                {
                                    // Static
                                    //e.Graphics.DrawImage(GokiPixels.fastGenerateStatic(ThumbnailWidth,ThumbnailHeight,2,255), x, y - panelOffset, ThumbnailWidth, ThumbnailHeight);
                                    // Outline
                                    if (!GokiTagDB.thumbnailInfo.ContainsKey(entry.Location))
                                    {
                                        e.Graphics.DrawImage(marchingAntsBitmapNew.Bitmap, x + entryPadding, y + entryPadding - panelOffset, ThumbnailWidth, ThumbnailHeight);
                                    }
                                    else
                                    {
                                        e.Graphics.DrawImage(marchingAntsBitmapOld.Bitmap, x + entryPadding, y + entryPadding - panelOffset, ThumbnailWidth, ThumbnailHeight);
                                    }
                                }
                                if (highlighted)
                                {
                                    e.Graphics.DrawRectangle(pen, GokiUtility.getRectangleContracted(rectangle, 0, 0, -borderSize+2, -borderSize+2));
                                }
                              
                            }
                            else
                            {
                                if (usedMemory > allowedMemoryUsage)
                                {
                                    if (entry.Thumbnail != null)
                                    {
                                        entry.Thumbnail.Dispose();
                                        entry.Thumbnail = null;
                                        usedMemory -= ThumbnailWidth * ThumbnailHeight * 4;
                                    }
                                }
                                try
                                {
                                    if (thumbnailGenerationQueue.Contains(entry))
                                    {
                                        DBEntry selectEntry = entry;
                                        thumbnailGenerationQueue.TryTake(out selectEntry);
                                    }
                                }
                                catch(Exception ex)
                                {

                                }
                            }
                            column++;
                            x += ThumbnailWidth + entryMargin + entryPadding * 2;
                            if (column == MaxColumns)
                            {
                                row++;
                                column = 0;
                                y += ThumbnailHeight + entryMargin + entryPadding * 2;
                                x = panelPadding ;
                            }
                        }
                    }
                }
            }
            else
            {
                using (Font font = new Font("Tahoma", 18))
                {
                    using ( SolidBrush brush = new SolidBrush(Color.Gray))
                    {
                        StringFormat format = new StringFormat();
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;
                        e.Graphics.DrawString(String.Format("{0:N0} files in the database\ndrag and drop files here", GokiTagDB.entries.Count), font, brush, pnlThumbnailView.ClientRectangle, format);
                    }
                }
            }
        }

        #endregion Paint

        #region Drag and Drop

        void dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        void dragDrop(object sender, DragEventArgs e)
        {
            List<string> files = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();
            int added = 0;
            int skipped = 0;
            Regex fileFilterRegex = new Regex(GokiTagDB.fileFilter);
            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];
                try
                {
                    //FileAttributes fileAttributes = File.GetAttributes(file);
                    if (Directory.Exists(file))
                    {
                        IEnumerable<string> subfiles = Directory.EnumerateFiles(file, "*", SearchOption.AllDirectories);
                        foreach (string subfile in subfiles)
                        {
                            files.Add(subfile);
                        }
                    }
                    else
                    {
                        if (!GokiTagDB.entries.ContainsKey(file))
                        {
                            string extension = Path.GetExtension(file);
                            if (fileFilterRegex.IsMatch(extension))
                            {
                                added++;
                                SaveAndLoad.addEntryToDatabase( new DBEntry(file, null));
                            }
                        }
                        else
                        {
                            skipped++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not add file to the database: " + file);
                }
            }
            string message = added + " file(s) added to the database.";
            if (skipped > 0)
            {
                message += "\n " + skipped + "file(s) skipped.";
            }
            lblStatus4.Text = message;
            btnSearch_Click(null, null);
        }

        #endregion Drag and Drop

        #region Search

        private void btnSearch_Click(object sender, EventArgs e)
        {
            int filterTagCount = -1;

            if (txtSearch.Text.Trim().Length == 0)
            {
                GokiTagDB.queriedEntries = GokiTagDB.entries.Values.ToList();
            }
            else
            {
                string[] tags = txtSearch.Text.Trim().Replace("\r\n", " ").Replace("\n", " ").Split(' ');
                List<KeyValuePair<string, bool>> tagPool = new List<KeyValuePair<string, bool>>();

                foreach (string tag in tags)
                {
                    if (tag.Substring(0, 1).Equals("?"))
                    {
                        if (tag.Substring(1, 5).ToLower().Equals("tags,"))
                        {
                            int.TryParse(tag.Substring(7), out filterTagCount);
                        }
                    }
                    else
                    {
                        if (tag.Substring(0, 2).Equals("<>"))
                        {
                            tagPool.Add(new KeyValuePair<string,bool>( tag.Substring(2),true));
                        }
                        else
                        {
                            tagPool.Add(new KeyValuePair<string, bool>(tag, false));
                        }
                    }
                }

                List<DBEntry> entryPool = new List<DBEntry>();
                entryPool.AddRange(GokiTagDB.entries.Values);
                GokiTagDB.queriedEntries = new List<DBEntry>();
                foreach (DBEntry entry in entryPool)
                {
                    string formattedEntryTagString = entry.TagString.Trim().Replace("\r\n", " ").Replace("\n", " ");
                    List<string> entryTags = new List<string>();
                    if (formattedEntryTagString.Length > 0)
                    {
                        entryTags = formattedEntryTagString.Split(' ').ToList();
                    }
                    bool passed = true;
                    if (filterTagCount == -1 || (filterTagCount >= 0 && entryTags.Count == filterTagCount))
                    {
                        for (int j = 0; j < tagPool.Count; j++)
                        {
                            if (tagPool[j].Value == false && !entryTags.Contains(tagPool[j].Key))
                            {
                                passed = false;                               
                                break;
                            }
                            else if (tagPool[j].Value == true && entryTags.Contains(tagPool[j].Key))
                            {
                                passed = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        passed = false;
                    }
                    if (passed)
                    {
                        GokiTagDB.queriedEntries.Add(entry);
                    }
                }
            }
            if (GokiTagDB.sortType == SortType.Location)
            {
                GokiTagDB.queriedEntries.Sort((entryA, entryB) =>
                {
                    return entryA.Location.CompareTo(entryB.Location);
                });
            }
            else if (GokiTagDB.sortType == SortType.Size)
            {
                GokiTagDB.queriedEntries.Sort((entryA, entryB) =>
                {
                    return entryA.FileSize.CompareTo(entryB.FileSize);
                });
            }
            else if (GokiTagDB.sortType == SortType.Extension)
            {
                GokiTagDB.queriedEntries.Sort((entryA, entryB) =>
                {
                    return entryA.FileExtension.CompareTo(entryB.FileExtension);
                });
            }
            else if (GokiTagDB.sortType == SortType.DateCreated)
            {
                GokiTagDB.queriedEntries.Sort((entryA, entryB) =>
                {
                    return entryA.CreationDate.CompareTo(entryB.CreationDate);
                });
            }
            else if (GokiTagDB.sortType == SortType.DateModified)
            {
                GokiTagDB.queriedEntries.Sort((entryA, entryB) =>
                {
                    return entryA.ModifiedDate.CompareTo(entryB.ModifiedDate);
                });
            }
            lblStatus.Text = GokiTagDB.queriedEntries.Count + " result(s).";
            panelOffset = 0;
            scrPanelVertical.Value = 0;
            activeIndex = -1;
            suggestTip.Hide(this);
            deselectAllSelectedEntries();
            updateTagListScrollbar();
            updatePanelScrollbar();
            updateTagCounts();
            queueRedraw();
            pnlTagList.Invalidate();
        }

        #       endregion Search

        private void tagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTagEdit tagEditForm = new frmTagEdit();
            tagEditForm.ShowDialog();
        }

    }
}

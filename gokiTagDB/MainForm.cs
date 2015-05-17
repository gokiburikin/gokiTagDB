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
using Newtonsoft.Json;

// TODO: Documentation
// TODO: Tag categories (artist, character, copyright, default)

namespace gokiTagDB
{
    public partial class frmMainForm : Form
    {

        private string lastSearch = "";
        private string autoSuggestEntry = "";
        private int panelOffset = 0;
        private int activeIndex = -1;
        private int tagListEntryHeight = 16;
        private int tagListHoverIndex = -1;
        private bool controlDown = false;
        private bool shiftDown = false;
        private bool isFormClosing = false;
        private long usedMemory = 0;
        private Process process;
        private DBEntry hoverEntry = null;
        private bool isGenerationThreaded = true;
        private bool thumbnailPanelDirty = false;
        private bool keyboardMultiselect = false;
        private frmAutoSuggestWindow autoSuggestWindow;
        private int calculatedFormBorderSize = 0;
        private PerformanceCounter performanceCounter;

        private System.Windows.Forms.Timer generationTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer invalidateTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer processTimer = new System.Windows.Forms.Timer();

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
            performanceCounter = new PerformanceCounter("Process","% Processor Time",process.ProcessName);
            performanceCounter.NextValue();

            autoSuggestWindow  = new frmAutoSuggestWindow();
            autoSuggestWindow.Owner = this;
            autoSuggestWindow.lstSuggestions.KeyDown += lstSuggestions_KeyDown;
            autoSuggestWindow.lstSuggestions.Click += lstSuggestions_Click;

            SaveAndLoad.loadSettings();
            Location = Properties.Settings.Default.WindowLocation;
            Size = Properties.Settings.Default.WindowSize;
            WindowState = Properties.Settings.Default.WindowState;

            Move += frmMainForm_Move;


            foreach( string entry in Enum.GetNames(typeof(SortType)))
            {
                ToolStripMenuItem sortModeItem = new ToolStripMenuItem(entry);
                sortModeItem.Click += sortModeItem_Click;
                mnuSettingsSortMode.DropDownItems.Add(sortModeItem);
            }

            foreach( int updateInterval in Settings.updateIntervals)
            {
                ToolStripMenuItem dropdownItem = new ToolStripMenuItem(updateInterval + "ms");
                dropdownItem.Tag = updateInterval;
                dropdownItem.Click += dropdownItem_Click;
                mnuViewUpdateInterval.DropDownItems.Add(dropdownItem);
            }

            for (int i = 0; i < Settings.zoomLevels.Length; i++)
            {
                ToolStripMenuItem zoomLevelItem = new ToolStripMenuItem(String.Format("{0:P0}", Settings.zoomLevels[i]));
                zoomLevelItem.Tag = i;
                zoomLevelItem.Click += zoomLevelItem_Click;
                mnuViewZoom.DropDownItems.Add(zoomLevelItem);
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
            pnlThumbnailView.KeyDown += pnlThumbnailView_KeyDown;
            pnlThumbnailView.KeyUp += pnlThumbnailView_KeyUp;

            pnlTagList.Paint += pnlTagList_Paint;
            pnlTagList.MouseMove += pnlTagList_MouseMove;
            pnlTagList.MouseLeave += pnlTagList_MouseLeave;
            pnlTagList.MouseDown += pnlTagList_MouseDown;
            pnlTagList.Resize += pnlTagList_Resize;

            lblStatus4.Click += lblStatus4_Click;

            ToolStripMenuItem aboutGokiburi = new ToolStripMenuItem("by gokiburikin");
            aboutGokiburi.Click += aboutGokiburi_Click;
            mnuAbout.DropDownItems.Add(aboutGokiburi);

            ToolStripMenuItem donate = new ToolStripMenuItem("Donate");
            donate.Click += donate_Click;
            mnuAbout.DropDownItems.Add(donate);

            scrPanelVertical.Scroll += scrPanelVertical_Scroll;
            scrTagListVertical.Scroll += scrTagListVertical_Scroll;

            txtSearch.TextChanged += autoSuggestTextChanged;
            txtTagEditor.TextChanged += autoSuggestTextChanged;

            GokiTagDB.databaseStream = new MemoryStream();
            GokiTagDB.thumbnailIndexStream = new MemoryStream();
            GokiTagDB.thumbnailStream = new FileStream(GokiTagDB.thumbnailsPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

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
                generationTimer.Interval = 100;
                generationTimer.Tick += generationTimer_Tick;
                generationTimer.Start();
                isGenerationThreaded = false;
            }

            invalidateTimer.Interval = GokiTagDB.settings.UpdateInterval;
            invalidateTimer.Tick += invalidateTimer_Tick;
            invalidateTimer.Start();

            processTimer.Interval = 3000;
            processTimer.Tick += processTimer_Tick;
            processTimer.Start();

            KeyDown += frmMainForm_KeyDown;
            KeyUp += frmMainForm_KeyUp;
            MouseWheel += mouseWheel;
            txtTagEditor.MouseWheel += txtTagEditor_MouseWheel;

            AllowDrop = true;
            DragDrop += dragDrop;
            DragEnter += dragEnter;

            FormClosing += frmMainForm_FormClosing;

            SaveAndLoad.loadTags();
            SaveAndLoad.loadDatabase();
            SaveAndLoad.loadThumbnails();

            calculatedFormBorderSize = (Size.Width - ClientSize.Width) / 2;

            updateMenuControls();
            updatePanelScrollbar();
        }

        void frmMainForm_Move(object sender, EventArgs e)
        {
            autoSuggestWindow.Hide();
        }

        void lstSuggestions_Click(object sender, EventArgs e)
        {
            autoCompleteHotkeyPressed();
        }

        void lstSuggestions_KeyDown(object sender, KeyEventArgs e)
        {
            if ( e.KeyCode == Keys.Enter )
            {
                autoCompleteHotkeyPressed();
            }
        }

        void lblStatus4_Click(object sender, EventArgs e)
        {
            if (File.Exists(lblStatus4.Text))
            {
                Process.Start("explorer.exe", @"/select," + lblStatus4.Text);
            }
        }

        void donate_Click(object sender, EventArgs e)
        {
            string url = @"https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=gokiburikin%40gmail%2ecom&lc=CA&item_name=gokiTagDB&amount=3%2e00&currency_code=CAD&bn=PP%2dDonationsBF%3abtn_donate_SM%2egif%3aNonHosted";
            System.Diagnostics.Process.Start(url);
        }

        void aboutGokiburi_Click(object sender, EventArgs e)
        {
            string url = @"https://www.github.com/gokiburikin";
            System.Diagnostics.Process.Start(url);
        }

        void pnlThumbnailView_KeyUp(object sender, KeyEventArgs e)
        {
            keyboardMultiselect = false;
        }

        void pnlThumbnailView_KeyDown(object sender, KeyEventArgs e)
        {
            if ( e.KeyCode == Keys.Space)
            {
                keyboardMultiselect = true;
                if ( GokiTagDB.queriedEntries.Count > activeIndex)
                {
                    GokiTagDB.queriedEntries[activeIndex].Selected = !GokiTagDB.queriedEntries[activeIndex].Selected;
                    updateTagCounts();
                    pnlTagList.Invalidate();
                    queueRedraw();
                }
            }
        }

        void dropdownItem_Click(object sender, EventArgs e)
        {
            GokiTagDB.settings.UpdateInterval = (int)((ToolStripMenuItem)sender).Tag;
            updateMenuControls();
        }

        void txtTagEditor_MouseWheel(object sender, MouseEventArgs e)
        {
            int adjustedX = e.X;
            int adjustedY = e.Y;
            adjustedX -= controlLocation(pnlThumbnailView).X - controlLocation(txtTagEditor).X;
            adjustedY -= controlLocation(pnlThumbnailView).Y - controlLocation(txtTagEditor).Y;
            adjustedX += controlLocation(pnlThumbnailView).X;
            adjustedY += controlLocation(pnlThumbnailView).Y;
            mouseWheel(this, new MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, adjustedX, adjustedY, e.Delta));
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
                        if (!generationCancelTokenSource.IsCancellationRequested)
                        {
                            try
                            {
                                SaveAndLoad.addThumbnailToDatabase(entry);
                            }
                            catch (Exception ex)
                            {
                                break;
                            }
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
                return (pnlThumbnailView.Width - GokiTagDB.settings.PanelPadding.Horizontal) / (ThumbnailWidth + GokiTagDB.settings.EntryMargin.Horizontal + GokiTagDB.settings.EntryPadding.Horizontal + GokiTagDB.settings.BorderSize * 2);
            }
        }

        int ThumbnailWidth
        {
            get
            {
                return (int)(GokiTagDB.settings.ThumbnailWidth * Zoom);
            }
        }

        int ThumbnailHeight
        {
            get
            {
                return (int)(GokiTagDB.settings.ThumbnailHeight * Zoom);
            }
        }

        float Zoom
        {
            get
            {
                return Settings.zoomLevels[GokiTagDB.settings.ZoomIndex];
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
            int previousActiveIndex = activeIndex;
            switch (keyData)
            {
                case Keys.Control | Keys.A:
                    if (pnlThumbnailView.Focused)
                    {
                        selectAllEntries();
                        queueRedraw();
                    }
                    else
                    {
                        return base.ProcessCmdKey(ref msg, keyData);
                    }
                    break;
                case Keys.Control | Keys.D:
                    if (pnlThumbnailView.Focused)
                    {
                        deselectAllEntries();
                        queueRedraw();
                    }
                    else
                    {
                        return base.ProcessCmdKey(ref msg, keyData);
                    }
                    break;
                case Keys.Tab:
                    tabKeyPressed();
                    break;
                case Keys.Enter:
                    enterKeyPressed();
                    break;
                case Keys.Space | Keys.Control:
                    autoCompleteHotkeyPressed();
                    break;
                case Keys.Left:
                    if (pnlThumbnailView.Focused && activeIndex % MaxColumns > 0)
                    {
                        activeIndex--;
                    }
                    queueRedraw();
                    break;
                case Keys.Up:
                    if (pnlThumbnailView.Focused && activeIndex >= MaxColumns)
                    {
                        activeIndex -= MaxColumns;
                    }
                    scrollToActiveIndex();
                    queueRedraw();
                    break;
                case Keys.Right:
                    if (pnlThumbnailView.Focused && activeIndex % MaxColumns < MaxColumns - 1 && activeIndex < GokiTagDB.queriedEntries.Count -1)
                    {
                        activeIndex++;
                    }
                    queueRedraw();
                    break;
                case Keys.Down:
                    if ( txtSearch.Focused || txtTagEditor.Focused)
                    {
                        autoSuggestWindow.Focus();
                    }
                    if (pnlThumbnailView.Focused && activeIndex < GokiTagDB.queriedEntries.Count - MaxColumns)
                    {
                        activeIndex += MaxColumns;
                    }
                    scrollToActiveIndex();
                    queueRedraw();
                    break;
                case Keys.Left | Keys.Shift:
                    if (pnlThumbnailView.Focused && activeIndex % MaxColumns > 0)
                    {
                        activeIndex--;
                    }
                    if (activeIndex < previousActiveIndex)
                    {
                        GokiTagDB.queriedEntries[previousActiveIndex].Selected = !GokiTagDB.queriedEntries[previousActiveIndex].Selected;
                    }
                    scrollToActiveIndex();
                    queueRedraw();
                    break;
                case Keys.Up | Keys.Shift:
                    if (pnlThumbnailView.Focused && activeIndex >= MaxColumns)
                    {
                        activeIndex -= MaxColumns;
                    }
                    if (activeIndex < previousActiveIndex)
                    {
                        for (int i = activeIndex; i < previousActiveIndex; i++)
                        {
                            GokiTagDB.queriedEntries[i].Selected = !GokiTagDB.queriedEntries[i].Selected;
                        }
                    }
                    scrollToActiveIndex();
                    queueRedraw();
                    break;
                case Keys.Right | Keys.Shift:
                    if (pnlThumbnailView.Focused && activeIndex % MaxColumns < MaxColumns - 1 && activeIndex < GokiTagDB.queriedEntries.Count -1)
                    {
                        activeIndex++;
                    }
                    if (activeIndex > previousActiveIndex)
                    {
                        GokiTagDB.queriedEntries[previousActiveIndex].Selected = !GokiTagDB.queriedEntries[previousActiveIndex].Selected;
                    }
                    scrollToActiveIndex();
                    queueRedraw();
                    break;
                case Keys.Down | Keys.Shift:
                    if (pnlThumbnailView.Focused && activeIndex < GokiTagDB.queriedEntries.Count - MaxColumns)
                    {
                        activeIndex += MaxColumns;
                    }
                    if ( activeIndex> previousActiveIndex)
                    {
                        for ( int i = previousActiveIndex; i < activeIndex; i++)
                        {
                            GokiTagDB.queriedEntries[i].Selected = !GokiTagDB.queriedEntries[i].Selected;
                        }
                    }
                    scrollToActiveIndex();
                    queueRedraw();
                    break;
                case Keys.Escape:
                    Close();
                    break;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
            return true;
        }

        void scrollToActiveIndex()
        {
            int topY = GokiTagDB.settings.PanelPadding.Top + (activeIndex / MaxColumns) * (ThumbnailHeight + GokiTagDB.settings.EntryPadding.Vertical + GokiTagDB.settings.EntryMargin.Vertical + GokiTagDB.settings.BorderSize * 2);
            int bottomY = GokiTagDB.settings.PanelPadding.Top + ((activeIndex + MaxColumns) / MaxColumns) * (ThumbnailHeight + GokiTagDB.settings.EntryPadding.Vertical + GokiTagDB.settings.EntryMargin.Vertical + GokiTagDB.settings.BorderSize * 2);
            if ( bottomY > scrPanelVertical.Value + pnlThumbnailView.Height )
            {
                scrPanelVertical.Value = bottomY - pnlThumbnailView.Height;
                scrPanelVertical_Scroll(this, new ScrollEventArgs(ScrollEventType.ThumbPosition, bottomY - pnlThumbnailView.Height ));
                scrPanelVertical.Invalidate();
            }
            else if ( topY < scrPanelVertical.Value  )
            {
                scrPanelVertical.Value = topY;
                scrPanelVertical_Scroll(this, new ScrollEventArgs(ScrollEventType.ThumbPosition, topY ));
                scrPanelVertical.Invalidate();
            }
        }

        void enterKeyPressed()
        {
            if ( txtSearch.Focused )
            {
                search(txtSearch.Text);
            }
            else if ( txtTagEditor.Focused)
            {
                addTags();
                txtTagEditor.Focus();
            }
            else if ( pnlThumbnailView.Focused )
            {
                if ( GokiTagDB.queriedEntries.Count > activeIndex &&  File.Exists(GokiTagDB.queriedEntries[activeIndex].FilePath))
                {
                    Process.Start(GokiTagDB.queriedEntries[activeIndex].FilePath);
                }
            }
        }

        void tabKeyPressed()
        {
            autoSuggestWindow.Hide();
            if ( txtTagEditor.Focused)
            {
                pnlThumbnailView.Focus();
            }
            else if ( txtSearch.Focused)
            {
                txtTagEditor.Focus();
            }
            
            else
            {
                txtSearch.Focus();
            }
        }

        void autoCompleteHotkeyPressed()
        {
            if (txtTagEditor.Focused || txtSearch.Focused || autoSuggestWindow.lstSuggestions.Focused)
            {
                if ( autoSuggestWindow.lstSuggestions.Focused)
                {
                    string suggestion =autoSuggestWindow.lstSuggestions.SelectedItem.ToString(); 
                    autoSuggestEntry = suggestion.Substring(0,suggestion.IndexOf(' '));
                }
                TextBoxBase textbox = null;
                if (txtTagEditor.Focused)
                {
                    textbox = txtTagEditor;
                }
                else if ( autoSuggestWindow.lstSuggestions.Focused)
                {
                    if ( autoSuggestWindow.suggestionControl == txtTagEditor)
                    {
                        textbox = txtTagEditor;
                    }
                    else
                    {
                        textbox = txtSearch;
                    }
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
                autoSuggestWindow.Hide();
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

        private void mnuSettingsSelectionModeExplorer_Click(object sender, EventArgs e)
        {
            GokiTagDB.settings.SelectionMode = SelectionMode.Explorer;
            updateMenuControls();
        }
        private void mnuSettingsSelectionModeToggle_Click(object sender, EventArgs e)
        {
            GokiTagDB.settings.SelectionMode = SelectionMode.Toggle;
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
            GokiTagDB.settings.ZoomIndex = (int)((ToolStripMenuItem)sender).Tag;
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

                    SaveAndLoad.editEntry(entry, new DBEntry(entry.FilePath, txtTagEditor.Text.Trim()));
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
                    search(txtSearch.Text);
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
                    List<string> existingTags = GokiTagDB.entries[entry.FilePath].TagString.Split(' ').ToList();
                    foreach (string checkTag in txtTagEditor.Text.Split(' '))
                    {
                        if (existingTags.Contains(checkTag))
                        {
                            existingTags.Remove(checkTag);
                        }
                    }
                    SaveAndLoad.editEntry(entry, new DBEntry(entry.FilePath, String.Join(" ", existingTags).Trim()));
                    amountUpdated++;
                }
            }
            lblStatus.Text = amountUpdated + " file(s) tags updated. @ " + DateTime.Now.ToString();
            updateTagCounts();
            pnlTagList.Invalidate();
        }

        void btnAddTags_Click(object sender, EventArgs e)
        {
            addTags();
        }

        void addTags()
        {
            int amountUpdated = 0;
            foreach (DBEntry entry in GokiTagDB.queriedEntries)
            {
                if (entry.Selected)
                {
                    List<string> existingTags = GokiTagDB.entries[entry.FilePath].TagString.Split(' ').ToList();
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
                    SaveAndLoad.editEntry(entry, new DBEntry(entry.FilePath, String.Join(" ", existingTags)));
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
                lblStatus3.Text = selectedEntries.Count + " file(s) removed from the database.";
            }
        }

        public int getPanelIndex(int mouseX, int mouseY)
        {
            int x = (mouseX - GokiTagDB.settings.PanelPadding.Left);
            if ( x > ValidPanelWidth - GokiTagDB.settings.PanelPadding.Right * 2 - GokiTagDB.settings.PanelPadding.Left)
            {
                x = ValidPanelWidth - GokiTagDB.settings.PanelPadding.Right * 2 - GokiTagDB.settings.PanelPadding.Left;
            }
            else if ( x < 0)
            {
                x = 0;
            }
            x /= (ThumbnailWidth + GokiTagDB.settings.EntryMargin.Horizontal + GokiTagDB.settings.EntryPadding.Horizontal + GokiTagDB.settings.BorderSize*2);
            int y = (mouseY + panelOffset - GokiTagDB.settings.PanelPadding.Top);
            y /= (ThumbnailHeight + GokiTagDB.settings.EntryMargin.Vertical + GokiTagDB.settings.EntryPadding.Vertical + GokiTagDB.settings.BorderSize * 2);
            if ( y < 0 )
            {
                y = 0;
            }
            return x + y * MaxColumns;
        }

        public int ValidPanelWidth
        {
            get
            {
                return (ThumbnailWidth + GokiTagDB.settings.EntryMargin.Horizontal + GokiTagDB.settings.EntryPadding.Horizontal + GokiTagDB.settings.BorderSize * 2 ) * MaxColumns + GokiTagDB.settings.PanelPadding.Horizontal;
            }
        }

        public int ValidPanelHeight
        {
            get
            {
                return (int)Math.Ceiling(GokiTagDB.queriedEntries.Count / (float)MaxColumns) * (ThumbnailHeight + GokiTagDB.settings.EntryMargin.Vertical + GokiTagDB.settings.EntryPadding.Vertical + GokiTagDB.settings.BorderSize*2) + GokiTagDB.settings.PanelPadding.Vertical;
            }
        }
        
        void pnlThumbnailView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X < ValidPanelWidth)
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
                    if (getSelectedEntries().Count == 1)
                    {
                        lblStatus4.Text = getSelectedEntries()[0].FilePath;
                    }
                    else
                    {
                        lblStatus4.Text = GokiTagDB.queriedEntries[index].FilePath;
                    }
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
            if (e.X < ValidPanelWidth)
            {
                int index = getPanelIndex(e.X, e.Y);
                if (index < GokiTagDB.queriedEntries.Count)
                {
                    if (GokiTagDB.settings.SelectionMode == SelectionMode.Explorer && (!controlDown && !shiftDown))
                    {
                        deselectAllEntries();
                        activeIndex = index;
                    }
                    else if (GokiTagDB.settings.SelectionMode == SelectionMode.Toggle)
                    {
                        activeIndex = index;
                    }
                    if (GokiTagDB.settings.SelectionMode == SelectionMode.Toggle || (!shiftDown || activeIndex == -1))
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
                        deselectAllEntries();
                        for (int i = startIndex; i <= index; i++)
                        {
                            GokiTagDB.queriedEntries[i].Selected = true;
                        }
                    }
                }
            }
            else
            {
                if (GokiTagDB.settings.SelectionMode == SelectionMode.Explorer)
                {
                    deselectAllEntries();
                }
            }
            updateTagCounts();
            lblStatus3.Text = getSelectedEntries().Count + " file(s) selected.";
            queueRedraw();
            pnlTagList.Invalidate();
        }

        void pnlThumbnailView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = getPanelIndex(e.X, e.Y);
            if (e.X < ValidPanelWidth)
            {
                if (index < GokiTagDB.queriedEntries.Count)
                {
                    if (File.Exists(GokiTagDB.queriedEntries[index].FilePath) && GokiTagDB.queriedEntries.Count > index)
                    {
                        Process.Start(GokiTagDB.queriedEntries[index].FilePath);
                    }
                    else
                    {
                        MessageBox.Show("File not found.", "File missing");
                    }
                }
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
                SaveAndLoad.editEntry(entry, new DBEntry(entry.FilePath, GokiRandom.randomIntPositive(10000).ToString()));
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
            GokiTagDB.settings.SortType = (SortType)Enum.Parse(typeof(SortType), entry);
            updateMenuControls();
            search(lastSearch);
        }

        #endregion Mouse and Keyboard

        #region Controls

        void autoSuggestTextChanged(object sender, EventArgs e)
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
                        matches.RemoveRange(10, matches.Count - 10);

                        Control control = (Control)sender;
                        Point locationOnForm = control.FindForm().PointToClient(control.Parent.PointToScreen(control.Location));
                        locationOnForm.Y += Size.Height - ClientSize.Height - calculatedFormBorderSize;
                        locationOnForm.X += calculatedFormBorderSize;
                        string message = matches[0].Key + " (" + matches[0].Value + ")";
                        if (matches.Count > 0)
                        {
                            autoSuggestWindow.suggestionControl = control;
                            autoSuggestWindow.lstSuggestions.Items.Clear();
                            foreach (KeyValuePair<string, int> match in matches)
                            {
                                autoSuggestWindow.lstSuggestions.Items.Add(String.Format("{0} ({1:N0})", match.Key, match.Value));
                                autoSuggestWindow.lstSuggestions.SelectedIndex = 0;
                            }

                            Point offset = Location;
                            autoSuggestWindow.Location = new Point(locationOnForm.X + offset.X, locationOnForm.Y + offset.Y + control.Height);
                            autoSuggestWindow.Show();
                            control.Focus();

                            autoSuggestEntry = matches[0].Key;
                            isShown = true;
                        }
                    }
                }
            }
            if (!isShown)
            {
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
                Properties.Settings.Default.WindowLocation = Location;
                Properties.Settings.Default.WindowSize = Size;
                Properties.Settings.Default.WindowState = WindowState;
                Properties.Settings.Default.Save();
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

        private void copySelectedFilePathsToClipboard()
        {
            List<DBEntry> selectedEntries = getSelectedEntries();
            if ( selectedEntries.Count > 0)
            {
                List<string> filePaths = new List<string>();
                foreach ( DBEntry entry in selectedEntries)
                {
                    filePaths.Add(entry.FilePath);
                }
                Clipboard.SetText(JsonConvert.SerializeObject(filePaths));
                lblStatus.Text = "Copied to clipboard.";
            }
            else
            {
                lblStatus.Text = "No entries selected.";

            }
        }

     

        private void deleteSelectedEntries()
        {
            foreach (DBEntry entry in getSelectedEntries())
            {
                if (File.Exists(entry.FilePath))
                {
                    File.Delete(entry.FilePath);
                    SaveAndLoad.removeEntriesFromDatabase(getSelectedEntries());
                }
            }
        }

        void processTimer_Tick(object sender, EventArgs e)
        {
            process.Refresh();
            usedMemory = process.PrivateMemorySize64;
            double cpuUsage = performanceCounter.NextValue() / Environment.ProcessorCount;
            lblStatus5.Text = String.Format("{0:N0}KB - {1:N2}% CPU", usedMemory / 1024f, cpuUsage);
        }

        void invalidateTimer_Tick(object sender, EventArgs e)
        {
            invalidateTimer.Stop();
            if ( thumbnailPanelDirty )
            {
                pnlThumbnailView.Invalidate();
                thumbnailPanelDirty = false;
                lblStatus2.Text = String.Format("{0:N0} thumbnails in queue", thumbnailGenerationQueue.Count);
            }
            invalidateTimer.Interval = GokiTagDB.settings.UpdateInterval;
            invalidateTimer.Start();
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
            mnuSettingsSelectionModeExplorer.Checked = GokiTagDB.settings.SelectionMode == SelectionMode.Explorer;
            mnuSettingsSelectionModeToggle.Checked = GokiTagDB.settings.SelectionMode == SelectionMode.Toggle;
            foreach (ToolStripMenuItem dropDownItem in mnuSettingsSortMode.DropDownItems)
            {
                SortType sortType = (SortType)Enum.Parse(typeof(gokiTagDB.SortType), dropDownItem.Text);
                if (sortType == GokiTagDB.settings.SortType)
                {
                    dropDownItem.Checked = true;
                }
                else
                {
                    dropDownItem.Checked = false;
                }
            }
            foreach ( ToolStripMenuItem item in mnuViewUpdateInterval.DropDownItems )
            {
                if (GokiTagDB.settings.UpdateInterval == (int)item.Tag)
                {
                    item.Checked = true;
                }
                else
                {
                    item.Checked = false;
                }
            }
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

        void selectAllEntries()
        {
            foreach (DBEntry entry in GokiTagDB.queriedEntries)
            {
                entry.Selected = true;
            }
        }

        void deselectAllEntries()
        {
            foreach (DBEntry entry in GokiTagDB.queriedEntries)
            {
                entry.Selected = false;
            }
        }

        void updatePanelScrollbar()
        {
            
            double oldPercentage = scrPanelVertical.Value / (double)scrPanelVertical.Maximum;
            if ( double.IsNaN(oldPercentage))
            {
                oldPercentage = 0;
            }
            if (ValidPanelHeight > 1)
            {
                scrPanelVertical.Minimum = 0;
                scrPanelVertical.Maximum = ValidPanelHeight;
                scrPanelVertical.SmallChange = (ThumbnailHeight + GokiTagDB.settings.EntryMargin.Vertical + GokiTagDB.settings.EntryPadding.Vertical);
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
                    String newFilePath = folderPath + @"\" + Path.GetFileName(entry.FilePath);
                    if (!File.Exists(newFilePath))
                    {
                        File.Move(entry.FilePath, newFilePath);
                        GokiTagDB.thumbnailInfo[entry.FilePath].Location = newFilePath;
                        GokiTagDB.entries.Remove(entry.FilePath);
                        entry.FilePath = newFilePath;
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
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            
            if (GokiTagDB.queriedEntries.Count > 0)
            {
                int x = GokiTagDB.settings.PanelPadding.Left + GokiTagDB.settings.BorderSize;
                int y = GokiTagDB.settings.PanelPadding.Top + GokiTagDB.settings.BorderSize;
                int row = 0;
                int column = 0;
                using (Pen pen = new Pen(Color.LightGray))
                {
                    if (pnlThumbnailView.Focused)
                    {
                        pen.Color = Color.Black;
                    }

                    pen.Width = GokiTagDB.settings.BorderSize;
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    pen.DashOffset = GokiTagDB.settings.BorderSize;
                    float[] dashPattern = new float[] { (ThumbnailWidth + GokiTagDB.settings.BorderSize * 2 + GokiTagDB.settings.EntryPadding.Horizontal) / GokiTagDB.settings.BorderSize / 20.0f, (ThumbnailWidth + GokiTagDB.settings.BorderSize * 2 + GokiTagDB.settings.EntryPadding.Horizontal) / GokiTagDB.settings.BorderSize / 20.0f };

                    using (SolidBrush brush = new SolidBrush(Color.Salmon))
                    {
                        foreach (DBEntry entry in GokiTagDB.queriedEntries)
                        {
                            bool drawRectangle = true;
                            bool highlighted = false;
                            pen.Color = Color.DarkSlateBlue;
                            Rectangle rectangle = new Rectangle(x + GokiTagDB.settings.EntryMargin.Left, y - panelOffset + GokiTagDB.settings.EntryMargin.Top, ThumbnailWidth + GokiTagDB.settings.EntryPadding.Horizontal, ThumbnailHeight + GokiTagDB.settings.EntryPadding.Vertical);
                            Rectangle borderRectangle = GokiUtility.getRectangleContracted(rectangle, -GokiTagDB.settings.BorderSize / 2- 1, -GokiTagDB.settings.BorderSize / 2-1, -GokiTagDB.settings.BorderSize / 2 , -GokiTagDB.settings.BorderSize / 2 );
                            if (GokiTagDB.settings.BorderSize % 2 == 0)
                            {
                                borderRectangle = GokiUtility.getRectangleContracted(borderRectangle, 1, 1, 0, 0);
                            }
                            // Only continue if the entry is actually within the view of the panel window
                            if (rectangle.IntersectsWith(pnlThumbnailView.ClientRectangle))
                            {
                                if (GokiTagDB.queriedEntries.Count > activeIndex && activeIndex >= 0 && GokiTagDB.queriedEntries[activeIndex] == entry)
                                {
                                    if (entry.Selected)
                                    {
                                        brush.Color = Color.LightSteelBlue;
                                        highlighted = true;
                                    }
                                    else
                                    {
                                        brush.Color = Color.White;
                                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                                        pen.DashPattern = dashPattern;
                                        if (entry == hoverEntry)
                                        {
                                            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                                        }
                                    }

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
                                    brush.Color = Color.White;
                                }
                                else
                                {
                                    drawRectangle = false;
                                }

                                if (entry.Thumbnail == null && !thumbnailGenerationQueue.Contains(entry))
                                {
                                    thumbnailGenerationQueue.Add(entry);
                                    if (!isGenerationThreaded && !generationTimer.Enabled)
                                    {
                                        generationTimer.Start();
                                    }
                                }

                                if (drawRectangle && highlighted)
                                {
                                    e.Graphics.FillRectangle(brush, rectangle);
                                }
                                
                                if (entry.Thumbnail != null)
                                {
                                    try{
                                        float adjustedX = x + (ThumbnailWidth - entry.Thumbnail.Width * Zoom) / 2 + GokiTagDB.settings.EntryPadding.Left + GokiTagDB.settings.EntryMargin.Left;
                                        float adjustedY = y + (ThumbnailHeight - entry.Thumbnail.Height * Zoom) / 2 - panelOffset + GokiTagDB.settings.EntryPadding.Top + GokiTagDB.settings.EntryMargin.Top;
                                        float width = entry.Thumbnail.Width * Zoom;
                                        float height = entry.Thumbnail.Height * Zoom;
                                        e.Graphics.DrawImage(entry.Thumbnail, adjustedX, adjustedY, width, height);
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
                                    if (!GokiTagDB.thumbnailInfo.ContainsKey(entry.FilePath))
                                    {
                                        pen.DashPattern = dashPattern;
                                        pen.Color = Color.Goldenrod;
                                        e.Graphics.DrawRectangle(pen, borderRectangle);
                                        //e.Graphics.DrawImage(marchingAntsBitmapNew.Bitmap, x + GokiTagDB.settings.EntryPadding, y + GokiTagDB.settings.EntryPadding - panelOffset, ThumbnailWidth, ThumbnailHeight);
                                    }
                                    else
                                    {
                                        pen.DashPattern = dashPattern;
                                        pen.Color = Color.LightBlue;
                                        e.Graphics.DrawRectangle(pen, borderRectangle);
                                        //e.Graphics.DrawImage(marchingAntsBitmapOld.Bitmap, x + GokiTagDB.settings.EntryPadding, y + GokiTagDB.settings.EntryPadding - panelOffset, ThumbnailWidth, ThumbnailHeight);
                                    }
                                }
                                if (drawRectangle)
                                {
                                    e.Graphics.DrawRectangle(pen, borderRectangle);
                                }
                                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                            }
                            else
                            {
                                if (usedMemory > GokiTagDB.settings.ApproximateMemoryUsage)
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

                            int borderAdjustment = GokiTagDB.settings.BorderSize;
                            if ( borderAdjustment < 0 )
                            {
                                borderAdjustment = 0;
                            }
                            column++;
                            x += ThumbnailWidth + GokiTagDB.settings.EntryMargin.Horizontal + GokiTagDB.settings.EntryPadding.Horizontal + borderAdjustment * 2;
                            if (column == MaxColumns)
                            {
                                row++;
                                column = 0;
                                y += ThumbnailHeight + GokiTagDB.settings.EntryMargin.Vertical + GokiTagDB.settings.EntryPadding.Vertical + borderAdjustment * 2;
                                x = GokiTagDB.settings.PanelPadding.Left + borderAdjustment;
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
            Regex fileFilterRegex = new Regex(GokiTagDB.settings.FileFilter);
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
                            string extension = Path.GetExtension(file).ToLower();
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
            lblStatus3.Text = message;
            search("");
        }

        #endregion Drag and Drop

        #region Search

        private void search( string text )
        {
            lastSearch = text;
            int filterTagCount = -1;

            if (text.Trim().Length == 0)
            {
                GokiTagDB.queriedEntries = GokiTagDB.entries.Values.ToList();
            }
            else
            {
                string[] tags = text.Trim().Replace("\r\n", " ").Replace("\n", " ").Split(' ');
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
                            tagPool.Add(new KeyValuePair<string, bool>(tag.Substring(2), true));
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
                    if (filterTagCount == -1 || entryTags.Count == filterTagCount)
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
            if (GokiTagDB.settings.SortType == SortType.Location)
            {
                GokiTagDB.queriedEntries.Sort((entryA, entryB) =>
                {
                    return entryA.FilePath.CompareTo(entryB.FilePath);
                });
            }
            else if (GokiTagDB.settings.SortType == SortType.Size)
            {
                GokiTagDB.queriedEntries.Sort((entryA, entryB) =>
                {
                    return entryA.FileSize.CompareTo(entryB.FileSize);
                });
            }
            else if (GokiTagDB.settings.SortType == SortType.Extension)
            {
                GokiTagDB.queriedEntries.Sort((entryA, entryB) =>
                {
                    return entryA.FileExtension.CompareTo(entryB.FileExtension);
                });
            }
            else if (GokiTagDB.settings.SortType == SortType.DateCreated)
            {
                GokiTagDB.queriedEntries.Sort((entryA, entryB) =>
                {
                    return entryA.CreationDate.CompareTo(entryB.CreationDate);
                });
            }
            else if (GokiTagDB.settings.SortType == SortType.DateModified)
            {
                GokiTagDB.queriedEntries.Sort((entryA, entryB) =>
                {
                    return entryA.ModifiedDate.CompareTo(entryB.ModifiedDate);
                });
            }
            lblStatus.Text = GokiTagDB.queriedEntries.Count + " result(s).";
            panelOffset = 0;
            scrPanelVertical.Value = 0;
            activeIndex = 0;
            deselectAllEntries();
            updateTagCounts();
            updateTagListScrollbar();
            updatePanelScrollbar();
            queueRedraw();
            pnlTagList.Invalidate();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            search(txtSearch.Text);
        }

        #endregion Search

        private void tagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTagEdit tagEditForm = new frmTagEdit();
            tagEditForm.ShowDialog();
        }

        private void copySelectedFilepathsToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copySelectedFilePathsToClipboard();
        }

        

        private void exportSelectedEntryTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<DBEntry> selectedEntries = getSelectedEntries();
            if ( selectedEntries.Count > 0 )
            {
                List<KeyValuePair<string, string>> entryTags = new List<KeyValuePair<string, string>>();
                foreach( DBEntry entry in selectedEntries)
                {
                    if (entry.TagString.Trim().Length > 0)
                    {
                        entryTags.Add(new KeyValuePair<string, string>(entry.MD5String, entry.TagString));
                    }
                }
                if ( entryTags.Count > 0)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "JSON Files|*.json";
                    if ( saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        File.WriteAllText(saveFileDialog.FileName, JsonConvert.SerializeObject(entryTags));
                    }
                }
                else
                {
                    MessageBox.Show("No selected entries have tags to export.","Export Selected Entry Tags");
                }
            }
            else
            {
                MessageBox.Show("No entries selected.", "Export Selected Entry Tags");
            }
        }

        private void importMD5TagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "JSON Files|*.json";
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    List<KeyValuePair<string, string>> entryList = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(File.ReadAllText(openFileDialog.FileName));
                    Dictionary<string, DBEntry> md5Dictionary = new Dictionary<string,DBEntry>();
                    foreach ( DBEntry entry in GokiTagDB.entries.Values)
                    {
                        string md5 = entry.MD5String;
                        if (!md5Dictionary.ContainsKey(md5))
                        {
                            md5Dictionary.Add(md5, entry);
                        }
                    }
                    List<string> tags = new List<string>();
                    foreach ( KeyValuePair<string,string> entry in entryList)
                    {
                        if ( md5Dictionary.ContainsKey(entry.Key) )
                        {
                            tags.Clear();
                            tags.AddRange(md5Dictionary[entry.Key].TagString.Split(' '));
                            foreach ( string newTag in entry.Value.Split(' '))
                            {
                                if( !tags.Contains(newTag) )
                                {
                                    tags.Add(newTag);
                                }
                                if (!Tags.tags.ContainsKey(newTag))
                                {
                                    Tags.addTag(newTag, "");
                                }
                            }
                            md5Dictionary[entry.Key].TagString = String.Join(" ",tags);
                        }
                    }
                }
                SaveAndLoad.saveDatabase();
                SaveAndLoad.syncDatabaseStream();
                MessageBox.Show("Importing tags successful.");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error importing tags.");
            }
        }

        private void clearDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the database? This can not be undone.", "Clear the database", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                GokiTagDB.entries.Clear();
                GokiTagDB.thumbnailInfo.Clear();
                Tags.tags.Clear();
                GokiTagDB.thumbnailInfo.Clear();
                foreach (DBEntry entry in GokiTagDB.entries.Values)
                {
                    if (entry.Thumbnail != null)
                    {
                        entry.Thumbnail.Dispose();
                        entry.Thumbnail = null;
                    }
                }
                SaveAndLoad.clearThumbnailStreams();
                SaveAndLoad.syncDatabaseStream();
                SaveAndLoad.saveDatabase();
                SaveAndLoad.saveThumbnails();
                SaveAndLoad.saveTags();
                search("");
            }
        }

        private void clearThumbnailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the thumbnail database? This can not be undone.", "Clear the thumbnail database", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                DialogResult result = System.Windows.Forms.DialogResult.Retry;
                while (thumbnailGenerationQueue.Count > 0 && result == System.Windows.Forms.DialogResult.Retry)
                {
                    result = MessageBox.Show("Thumbnail generation queue is not empty.", "Clear Thumbnail Database", MessageBoxButtons.RetryCancel);
                }
                GokiTagDB.thumbnailInfo.Clear();
                foreach (DBEntry entry in GokiTagDB.entries.Values)
                {
                    if (entry.Thumbnail != null)
                    {
                        entry.Thumbnail.Dispose();
                        entry.Thumbnail = null;
                    }
                }
                SaveAndLoad.clearThumbnailStreams();
                SaveAndLoad.saveThumbnails();
                search("");
            }
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectAllEntries();
            queueRedraw();
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deselectAllEntries();
            queueRedraw();
        }
    }
}

using GokiLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// TODO: Document, Organize
// TODO: Auto-suggest
// TODO: Consider whatever that weird parsing thing was for searching (spaces to underscores, separate with... spaces, sure)
// TODO: Tag categories (artist, character, copyright, default)
// TODO: Negation operator

namespace gokiTagDB
{
    public partial class frmMainForm : Form
    {
        public static int thumbnailWidth = 128;
        public static int thumbnailHeight = 128;
        public static int zoomIndex = 3;
        public static float[] zoomLevels = new float[7] { .25f, .50f, .75f, 1.0f, 1.25f, 1.5f, 2.0f };
        public static string tagsPath = @".\tags.db";
        public static string settingsPath = @".\settings.dat";
        public static string databasePath = @".\entries.db";
        public static string thumbnailsPath = @".\thumbnails.db";
        public static string thumbnailsIndexPath = @".\thumbnails_index.db";
        public static Dictionary<string, DBEntry> entries = new Dictionary<string, DBEntry>();
        public static Dictionary<string, ThumbnailInfo> thumbnailInfo = new Dictionary<string, ThumbnailInfo>();
        public static Dictionary<string, int> tagCounts = new Dictionary<string, int>();
        public static List<DBEntry> thumbnailGenerationQueue = new List<DBEntry>();
        public static int entriesPerPage = 20;

        private int panelPadding = 2;
        private int entryMargin = 4;
        private int entryPadding = 4;
        private int borderSize = 3;
        private int panelOffset = 0;
        private int activeIndex = -1;
        private int tagListEntryHeight = 16;
        private int tagListHoverIndex = -1;
        private int selectionMode = 0;
        private long allowedMemoryUsage = 100000000;

        private System.Windows.Forms.Timer generationTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer invalidateTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer processTimer = new System.Windows.Forms.Timer();

        private List<DBEntry> queriedEntries = new List<DBEntry>();


        private bool controlDown = false;
        private bool shiftDown = false;
        private bool thumbnailPanelDirty = false;

        private string autoSuggestEntry = "";

        private ToolTip suggestTip = new ToolTip();

        private Process process;
        private long usedMemory = 0;

        private string title = "";

        public frmMainForm()
        {
            InitializeComponent();
            Load += frmMainForm_Load;
        }

        void frmMainForm_Load(object sender, EventArgs e)
        {
            process = Process.GetCurrentProcess();

            AllowDrop = true;

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

            generationTimer.Interval = 1000;
            generationTimer.Tick += generationTimer_Tick;
            generationTimer.Start();

            invalidateTimer.Interval = 50;
            invalidateTimer.Tick += invalidateTimer_Tick;
            invalidateTimer.Start();

            processTimer.Interval = 3000;
            processTimer.Tick += processTimer_Tick;
            processTimer.Start();

            KeyDown += frmMainForm_KeyDown;
            KeyUp += frmMainForm_KeyUp;
            MouseWheel += mouseWheel;
            txtTagEditor.MouseWheel += mouseWheel;
            DragDrop += dragDrog;
            DragEnter += dragEnter;
            FormClosing += frmMainForm_FormClosing;

            updateMenuControls();
            updateTitle();
            loadTags();
            loadSettings();
            loadDatabase();
            loadThumbnails();
            updatePanelScrollbar();
        }

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

        void txtTagEditor_Click(object sender, EventArgs e)
        {
            txtTagEditor.SelectAll();
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
                thumbnailPanelDirty = true;
            }
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
            saveTags();
            saveSettings();
            saveDatabase();
            saveThumbnails();
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
            queriedEntries.Clear();
            tagCounts.Clear();
            txtSearch.Text = "";
            pnlTagList.Invalidate();
            thumbnailPanelDirty = true;
        }

        private void btnUpdateTags_Click(object sender, EventArgs e)
        {
            int amountUpdated = 0;
            foreach (DBEntry entry in queriedEntries)
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
                    entries[entry.Location].TagString = txtTagEditor.Text.Trim();
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
            foreach (DBEntry entry in queriedEntries)
            {
                if (entry.Selected)
                {
                    List<string> existingTags = entries[entry.Location].TagString.Split(' ').ToList();
                    foreach (string checkTag in txtTagEditor.Text.Split(' '))
                    {
                        if (existingTags.Contains(checkTag))
                        {
                            existingTags.Remove(checkTag);
                        }
                    }
                    entries[entry.Location].TagString = String.Join(" ", existingTags);
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
            foreach (DBEntry entry in queriedEntries)
            {
                if (entry.Selected)
                {
                    List<string> existingTags = entries[entry.Location].TagString.Split(' ').ToList();
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
                    entries[entry.Location].TagString = String.Join(" ", existingTags);
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
            if ( e.KeyCode == Keys.Delete)
            {
                List<DBEntry> selectedEntries = getSelectedEntries();
                int amountRemoved = 0;
                foreach( DBEntry entry in selectedEntries)
                {
                    entries.Remove(entry.Location);
                    queriedEntries.Remove(entry);
                    amountRemoved++;
                }
                thumbnailPanelDirty = true;
                if ( amountRemoved > 0)
                {
                    lblStatus2.Text = amountRemoved + " file(s) removed from the database.";
                }
            }
        }

        void pnlThumbnailView_MouseMove(object sender, MouseEventArgs e)
        {
            foreach (DBEntry entry in queriedEntries)
            {
                entry.Hover = false;
            }
            if (e.X <= (ThumbnailWidth + entryMargin + entryPadding * 2) * MaxColumns - panelPadding * 2)
            {
                int index = getPanelIndex(e.X, e.Y);
                if (queriedEntries.Count > index)
                {
                    queriedEntries[index].Hover = true;
                    lblStatus3.Text = queriedEntries[index].Location;
                }
            }
            thumbnailPanelDirty = true;
        }

        void pnlThumbnailView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X <= (ThumbnailWidth + entryMargin) * MaxColumns - panelPadding * 2)
            {
                int index = getPanelIndex(e.X, e.Y);
                if (selectionMode == 0 && (!controlDown && !shiftDown))
                {
                    deselectAllSelectedEntries();
                    activeIndex = index;
                }
                if (selectionMode == 1 || (!shiftDown || activeIndex == -1))
                {
                    if (queriedEntries.Count > index)
                    {
                        queriedEntries[index].Selected = !queriedEntries[index].Selected;
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
                        queriedEntries[i].Selected = true;
                    }
                }
            }
            else
            {
                deselectAllSelectedEntries();
            }
            updateTagCounts();
            lblStatus2.Text = getSelectedEntries().Count + " file(s) selected.";
            thumbnailPanelDirty = true;
            pnlTagList.Invalidate();
        }

        void pnlThumbnailView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = getPanelIndex(e.X, e.Y);
            if (File.Exists(queriedEntries[index].Location) && queriedEntries.Count > index)
            {
                Process.Start(queriedEntries[index].Location);
            }
            else
            {
                MessageBox.Show("File not found.", "File missing");
            }
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
                        string tooltip = "Best Guess:\n" + matches[0].Key + " (" + matches[0].Value + ")\n\nCandidates:\n";
                        for (int i = 1; i < Math.Min(5, matches.Count); i++)
                        {
                            tooltip += String.Format("{0} ({1})\n", matches[i].Key, matches[i].Value);
                        }
                        if (matches.Count > 0)
                        {
                            suggestTip.Show(tooltip, this, locationOnForm);
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
            thumbnailPanelDirty = true;
        }

        void pnlThumbnailView_Resize(object sender, EventArgs e)
        {
            thumbnailPanelDirty = true;
            updatePanelScrollbar();
        }

        void frmMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            try
            {
                saveTags();
                saveSettings();
                saveDatabase();
                saveThumbnails();
            }
            catch (Exception ex)
            {
                this.Show();
                e.Cancel = true;
            }
            
        }

        #endregion Controls

        #region Methods

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
            invalidateTimer.Stop();
            if (thumbnailPanelDirty)
            {
                pnlThumbnailView.Invalidate();
                thumbnailPanelDirty = false;
            }
            invalidateTimer.Start();
        }

        void generationTimer_Tick(object sender, EventArgs e)
        {
            generationTimer.Stop();
            handleGenerationQueue();
            thumbnailPanelDirty = true;
            generationTimer.Start();
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

        void updateMenuControls()
        {
            mnuSettingsSelectionModeExplorer.Checked = selectionMode == 0;
            mnuSettingsSelectionModeToggle.Checked = selectionMode == 1;
        }

        void updateTagCounts()
        {
            tagCounts.Clear();

            List<DBEntry> entryPool = queriedEntries;
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
                    if (tagCounts.ContainsKey(tag))
                    {
                        tagCounts[tag]++;
                    }
                    else
                    {
                        tagCounts.Add(tag, 1);
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
            scrTagListVertical.Maximum = Math.Max(tagCounts.Count * tagListEntryHeight + scrTagListVertical.LargeChange - pnlTagList.Height, scrTagListVertical.Minimum);
            scrTagListVertical.Value = scrTagListVertical.Minimum;
        }

        List<KeyValuePair<string, int>> getSortedTagList()
        {
            List<KeyValuePair<string, int>> pairs = tagCounts.ToList();
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
            foreach (DBEntry entry in queriedEntries)
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
            foreach (DBEntry entry in queriedEntries)
            {
                entry.Selected = false;
            }
        }

        void updatePanelScrollbar()
        {
            int totalHeight = (int)Math.Ceiling(queriedEntries.Count / (float)MaxColumns) * (ThumbnailHeight + entryMargin + entryPadding * 2) + panelPadding * 2;
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
        }

        void moveSelectedEntries(string folderPath)
        {
            foreach ( DBEntry entry in getSelectedEntries())
            {
                int errors = 0;
                try
                {
                    string newFilePath = folderPath +@"\"+ Path.GetFileName(entry.Location);
                    File.Move(entry.Location, newFilePath);
                    entry.Location = newFilePath;
                }
                catch( Exception ex)
                {
                    errors++;
                }
                if ( errors > 0 )
                {
                    MessageBox.Show("Error moving " + errors + " file(s).");
                }
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
            if (queriedEntries.Count > 0)
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
                        foreach (DBEntry entry in queriedEntries)
                        {
                            pen.Color = Color.LightGray;
                            Rectangle rectangle = new Rectangle(x, y - panelOffset, ThumbnailWidth + entryPadding * 2, ThumbnailHeight + entryPadding * 2);
                            if (rectangle.IntersectsWith(new Rectangle(0, 0, pnlThumbnailView.Width, pnlThumbnailView.Height)))
                            {
                                bool highlighted = false;
                                if (queriedEntries.Count > activeIndex && activeIndex >= 0 && queriedEntries[activeIndex] == entry)
                                {
                                    pen.Color = Color.DarkGoldenrod;
                                    brush.Color = Color.Gold;
                                    highlighted = true;
                                }
                                else if (entry.Selected)
                                {
                                    pen.Color = Color.DarkGreen;
                                    brush.Color = Color.MediumSpringGreen;
                                    highlighted = true;
                                }
                                else if (entry.Hover)
                                {
                                    pen.Color = Color.DarkSlateBlue;
                                    brush.Color = Color.LightBlue;
                                    highlighted = true;
                                }

                                if (entry.Thumbnail == null && !thumbnailGenerationQueue.Contains(entry))
                                {
                                    thumbnailGenerationQueue.Add(entry);
                                }

                                if (highlighted)
                                {
                                    e.Graphics.FillRectangle(brush, rectangle);
                                }
                                if (entry.Thumbnail != null)
                                {
                                    e.Graphics.DrawImage(entry.Thumbnail, x + (ThumbnailWidth - entry.Thumbnail.Width * Zoom) / 2 + entryPadding, y + (ThumbnailHeight - entry.Thumbnail.Height * Zoom) / 2 - panelOffset + entryPadding, entry.Thumbnail.Width * Zoom, entry.Thumbnail.Height * Zoom);
                                }
                                else
                                {
                                    // Static
                                    //e.Graphics.DrawImage(GokiPixels.fastGenerateStatic(ThumbnailWidth,ThumbnailHeight,2,255), x, y - panelOffset, ThumbnailWidth, ThumbnailHeight);
                                    // Outline
                                    e.Graphics.DrawImage(GokiPixels.marchingAntsRectangle(ThumbnailWidth-1, ThumbnailHeight-1, Color.Yellow,Color.DarkGoldenrod,(int)(DateTime.Now.Millisecond /1000f * thumbnailWidth)), x + entryPadding, y + entryPadding - panelOffset, ThumbnailWidth, ThumbnailHeight);
                                }
                                if (highlighted)
                                {
                                    e.Graphics.DrawRectangle(pen, GokiUtility.getRectangleContracted(rectangle, 0, 0, -borderSize+2, -borderSize+2));
                                }
                            }
                            else
                            {
                                try
                                {
                                    if (usedMemory > allowedMemoryUsage)
                                    {
                                        usedMemory -= ThumbnailWidth * ThumbnailHeight * 4;
                                        entry.Thumbnail = null;
                                    }
                                }
                                catch(PlatformNotSupportedException ex)
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
        }

        #endregion Paint

        void handleGenerationQueue()
        {
            double totalTime = 0;
            DateTime startTime = DateTime.Now;

            while (thumbnailGenerationQueue.Count > 0 && totalTime < 100)
            {
                int count = thumbnailGenerationQueue.Count;
                thumbnailGenerationQueue[count-1].generateThumbnail();
                thumbnailGenerationQueue.RemoveAt(count-1);
                totalTime += (DateTime.Now - startTime).TotalMilliseconds;
            }

            if (thumbnailGenerationQueue.Count == 0)
            {
                generationTimer.Interval = 50;
            }
        }

        #region Drag and Drop

        void dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        void dragDrog(object sender, DragEventArgs e)
        {
            List<string> files = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();
            int added = 0;
            int skipped = 0;
            int failed = 0;
            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];
                try
                {
                    if (!entries.ContainsKey(file))
                    {

                        FileAttributes fileAttributes = File.GetAttributes(file);
                        if (fileAttributes == FileAttributes.Directory)
                        {
                            List<string> subfiles = Directory.EnumerateFiles(file).ToList();
                            foreach (string subfile in subfiles)
                            {
                                files.Add(subfile);
                            }
                        }
                        else
                        {
                            if (!entries.ContainsKey(file))
                            {
                                added++;
                                entries.Add(file, new DBEntry(Path.GetFileNameWithoutExtension(file), file, null));
                            }
                            else
                            {
                                skipped++;
                            }

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
            MessageBox.Show(message);
        }

        #endregion Drag and Drop

        #region Search

        private void btnSearch_Click(object sender, EventArgs e)
        {
            int filterTagCount = -1;

            if (txtSearch.Text.Trim().Length == 0)
            {
                queriedEntries = entries.Values.ToList();
            }
            else
            {
                string[] tags = txtSearch.Text.Trim().Replace("\r\n", " ").Replace("\n", " ").Split(' ');
                List<KeyValuePair<string, bool>> tagPool = new List<KeyValuePair<string, bool>>();

                foreach (string tag in tags)
                {
                    if (tag.Substring(0, 2).Equals("?:"))
                    {
                        if (tag.Substring(2, 5).ToLower().Equals("tags,"))
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
                entryPool.AddRange(entries.Values);
                queriedEntries = new List<DBEntry>();
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
                        queriedEntries.Add(entry);
                    }
                }
            }
            lblStatus.Text = queriedEntries.Count + " result(s).";
            panelOffset = 0;
            scrPanelVertical.Value = 0;
            activeIndex = -1;
            suggestTip.Hide(this);
            deselectAllSelectedEntries();
            updateTagListScrollbar();
            updatePanelScrollbar();
            updateTagCounts();
            thumbnailPanelDirty = true;
            pnlTagList.Invalidate();
        }

        #       endregion Search

        #region Save and Load

        private void saveSettings()
        {
            try
            {
                AutoSizeGokiBytesWriter writer = new AutoSizeGokiBytesWriter();
                writer.write(entriesPerPage);
                File.WriteAllBytes(settingsPath, GokiUtility.getCompressedByteArray(writer.Data));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save settings.\n" + ex.Message);
            }
        }

        private void loadSettings()
        {
            if (File.Exists(settingsPath))
            {
                try
                {
                    GokiBytesReader reader = new GokiBytesReader(GokiUtility.getDecompressedByteArray(File.ReadAllBytes(settingsPath)));
                    entriesPerPage = reader.readInt();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not load settings.\n" + ex.Message);
                }
            }
        }

        private void saveTags()
        {
            try
            {
                File.WriteAllBytes(tagsPath, Tags.toByteArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save tag database.\n" + ex.Message);
            }
        }

        private void loadTags()
        {
            try
            {
                if (File.Exists(tagsPath))
                {
                    Tags.loadFromByteArray(File.ReadAllBytes(tagsPath));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load tag database.\n" + ex.Message);
            }
        }

        private void saveDatabase()
        {
            try
            {
                AutoSizeGokiBytesWriter writer = new AutoSizeGokiBytesWriter();
                writer.write(entries.Values.Count);
                foreach (DBEntry entry in entries.Values.ToList())
                {
                    byte[] data = entry.toByteArray();
                    writer.writeInt(data.Length);
                    writer.write(data);
                }
                File.WriteAllBytes(databasePath, GokiUtility.getCompressedByteArray(writer.Data));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save database.\n" + ex.Message);
            }
        }
        private void loadDatabase()
        {
            if (File.Exists(databasePath))
            {
                try
                {
                    string fullTagString = "";
                    GokiBytesReader reader = new GokiBytesReader(GokiUtility.getDecompressedByteArray(File.ReadAllBytes(databasePath)));
                    entries.Clear();
                    int count = reader.readInt();
                    for (int i = 0; i < count; i++)
                    {
                        byte[] data = new byte[reader.readInt()];
                        DBEntry entry = DBEntry.fromByteArray(reader.readByteArray(data));
                        fullTagString += entry.TagString + " ";
                        entries.Add(entry.Location, entry);
                    }

                    string[] allTags = fullTagString.Split(' ');
                    foreach ( string tag in allTags)
                    {
                        if (!Tags.tags.ContainsKey(tag))
                        {
                            Tags.addTag(tag,null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not load database.\n" + ex.Message);
                }
            }
        }

        private void saveThumbnails()
        {
            try
            {
                List<string> currentLocations = new List<string>();
                if (File.Exists(thumbnailsIndexPath))
                {
                    GokiBytesReader indexReader = new GokiBytesReader(File.ReadAllBytes(thumbnailsIndexPath));
                    while (!indexReader.EOF)
                    {
                        currentLocations.Add(indexReader.readString());
                        indexReader.readInt();
                        indexReader.readInt();
                    }
                }

                AutoSizeGokiBytesWriter indexWriter = new AutoSizeGokiBytesWriter();
                if (File.Exists(thumbnailsIndexPath))
                {
                    indexWriter.write(File.ReadAllBytes(thumbnailsIndexPath));
                }
                AutoSizeGokiBytesWriter thumbnailWriter = new AutoSizeGokiBytesWriter();
                if (File.Exists(thumbnailsPath))
                {
                    thumbnailWriter.write(File.ReadAllBytes(thumbnailsPath));
                }
                foreach (DBEntry entry in entries.Values)
                {
                    if (!currentLocations.Contains(entry.Location) && entry.Thumbnail != null)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            entry.Thumbnail.Save(memoryStream, ImageFormat.Png);
                            byte[] thumbnailData = memoryStream.ToArray();

                            indexWriter.write(entry.Location);
                            indexWriter.write(thumbnailWriter.Index);
                            indexWriter.write(thumbnailData.Length);
                            thumbnailWriter.write(thumbnailData);
                        }
                    }
                }
                File.WriteAllBytes(thumbnailsIndexPath, indexWriter.Data);
                File.WriteAllBytes(thumbnailsPath, thumbnailWriter.Data);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save thumbnails.\n" + ex.Message);
            }
        }

        private void loadThumbnails()
        {
            if (File.Exists(thumbnailsIndexPath) && File.Exists(thumbnailsPath))
            {
                try
                {
                    GokiBytesReader reader = new GokiBytesReader(File.ReadAllBytes(thumbnailsIndexPath));
                    thumbnailInfo.Clear();
                    while (!reader.EOF)
                    {
                        string location = reader.readString();
                        int index = reader.readInt();
                        int size = reader.readInt();
                        thumbnailInfo.Add(location, new ThumbnailInfo(location, index, size));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not load thumbnails.\n" + ex.Message);
                }
            }
        }

        #endregion Save and Load

        private void mnuOrganizeMoveSelectedFiles_Click(object sender, EventArgs e)
        {
            if (getSelectedEntries().Count > 0)
            {
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

        private void cleanThumbnailDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ( MessageBox.Show("This will search through the thumbnail database to clean up any issues such as missing files. This can take some time.\n\nContinue?", "Confirmation", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                cleanThumbnailDatabase();
            }
        }

        private void cleanThumbnailDatabase()
        {
            int missingFiles = 0;
            foreach (KeyValuePair<string, ThumbnailInfo> info in thumbnailInfo)
            {
                if ( !File.Exists(info.Key))
                {
                   /* using (FileStream indexStream = File.OpenWrite(thumbnailsIndexPath))
                    {
                        using (FileStream thumbnailsStream = File.Open(thumbnailsPath, FileMode.Open,FileAccess.ReadWrite))
                        {
                            byte[] data = new byte[thumbnailsStream.Length - info.Value.Size];
                            thumbnailsStream.Read(data,0,(int)info.Value.Index);
                            thumbnailsStream.Seek(info.Value.Size, SeekOrigin.Current);
                            thumbnailsStream.Read(data, (int)info.Value.Index, data.Length - (int)info.Value.Index);
                            thumbnailsStream.Seek(0, SeekOrigin.Begin);
                            thumbnailsStream.SetLength(data.Length);
                            thumbnailsStream.Write(data, 0, data.Length);
                        }
                    }*/
                    missingFiles++;
                }
            }
            Console.WriteLine(missingFiles + " missing file(s).");
        }
    }
}

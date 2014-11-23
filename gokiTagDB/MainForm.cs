using GokiLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gokiTagDB
{
    public partial class frmMainForm : Form
    {
        public static string settingsPath = @".\settings.dat";
        public static string databasePath = @".\entries.db";
        public static string thumbnailsPath = @".\thumbnails.db";
        public static Dictionary<string, DBEntry> entries = new Dictionary<string, DBEntry>();
        public static Dictionary<string, BitmapDataSizeTuple> thumbnailData = new Dictionary<string, BitmapDataSizeTuple>();
        public static int entriesPerPage = 20;
        
        private List<DBEntry> selectedEntries = new List<DBEntry>();
        private int selectedOffset = 0;
        
        public frmMainForm()
        {
            InitializeComponent();
            Load += frmMainForm_Load;
        }

        void frmMainForm_Load(object sender, EventArgs e)
        {
            btnSearch.Click += btnSearch_Click;
            btnClear.Click += btnClear_Click;
            floOutput.MouseWheel += floOutput_MouseWheel;
            AllowDrop = true;
            DragDrop += dragDrog;
            DragEnter += dragEnter;
            numOffset.ValueChanged += numOffset_ValueChanged;
            FormClosing += frmMainForm_FormClosing;
            loadSettings();
            loadDatabase();
            loadThumbnails();
        }

        void floOutput_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0 )
            {
                floOutput.VerticalScroll.Value--;
            }
            else if(  e.Delta > 0 )
            {
                floOutput.VerticalScroll.Value++;
            }
        }

        void frmMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveSettings();
            saveDatabase();
            saveThumbnails();
        }


        void numOffset_ValueChanged(object sender, EventArgs e)
        {
            selectedOffset = numOffset.Value * entriesPerPage;
            updateOutputPanelControls();
            populateOutputPanel();
        }

        void dragEnter(object sender, DragEventArgs e)
        {
            if ( e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        void dragDrog(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            int added = 0;
            foreach( string file in files )
            {
                if ( !entries.ContainsKey(file))
                {
                    added++;
                    entries.Add(file, new DBEntry(Path.GetFileNameWithoutExtension(file), file, null));
                }
            }
            MessageBox.Show(added + " file(s) added to the database");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            int filterTagCount = -1;

            if (txtSearch.Text.Trim().Length == 0)
            {
                selectedEntries = entries.Values.ToList();
            }
            else
            {
                string[] tags = txtSearch.Text.Trim().Replace("\r\n", " ").Replace("\n", " ").Split(' ');
                List<string> tagPool = new List<string>();

                foreach( string tag in tags)
                {
                    if ( tag.Substring(0,2).Equals("?:") )
                    {
                        if ( tag.Substring(2,5).ToLower().Equals("tags,"))
                        {
                            int.TryParse(tag.Substring(7), out filterTagCount);
                        }
                    }
                    else
                    {
                        tagPool.Add(tag);
                    }
                }

                List<DBEntry> entryPool = new List<DBEntry>();
                entryPool.AddRange(entries.Values);
                selectedEntries = new List<DBEntry>();
                foreach( DBEntry entry in entryPool)
                {
                    string formattedEntryTagString = entry.TagString.Trim().Replace("\r\n", " ").Replace("\n", " ");
                    List<string> entryTags = new List<string>();
                    if (formattedEntryTagString.Length > 0)
                    {
                        entryTags = formattedEntryTagString.Split(' ').ToList();
                    }
                    bool passed = true;
                    if (filterTagCount >= 0 && entryTags.Count == filterTagCount)
                    {

                        for (int j = 0; j < tagPool.Count; j++)
                        {
                            if (!entryTags.Contains(tagPool[j]))
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
                    if ( passed )
                    {
                        selectedEntries.Add(entry);
                    }
                }
            }
            populateOutputPanel();
            updateOutputPanelControls();
            numOffset.Minimum = 0;
            numOffset.Maximum = (int)Math.Ceiling(selectedEntries.Count / (double)entriesPerPage)-1;
            numOffset.TickFrequency = 1;
            numOffset.TickStyle = TickStyle.BottomRight;
        }

        private void updateOutputPanelControls()
        {
            lblOffset.Text = (int)Math.Ceiling((selectedOffset + 1)/ (double)entriesPerPage) + "/" + (int)Math.Ceiling(selectedEntries.Count / (double)entriesPerPage);
        }

        private void populateOutputPanel()
        {
            floOutput.SuspendLayout();
            int thumbnailsGenerated = 0;
            int controlIndex = 0;
            int neededControls = Math.Min(entriesPerPage, selectedEntries.Count - selectedOffset);
            while (floOutput.Controls.Count > neededControls)
            {
                floOutput.Controls.RemoveAt(0);
            }
            while ( floOutput.Controls.Count < neededControls )
            {
                DBEntryControl control = new DBEntryControl();
                control.Click += control_Click;
                control.MouseWheel += control_MouseWheel;
                floOutput.Controls.Add(control);
            }
            for (int i = selectedOffset; i < Math.Min(selectedOffset + entriesPerPage, selectedEntries.Count); i++ )
            {
                DBEntryControl control = (DBEntryControl)floOutput.Controls[controlIndex];
                if (selectedEntries[i].Thumbnail == null)
                {
                    selectedEntries[i].generateThumbnail();
                    thumbnailsGenerated++;
                }
                
                control.setEntry(selectedEntries[i]);
                controlIndex ++;
            }
            
            if (thumbnailsGenerated > 0)
            {
                lblStatus.Text = thumbnailsGenerated + " thumbnail(s) generated";
            }
            else
            {
                lblStatus.Text = "No thumbnails generated";
            }
            floOutput.ResumeLayout();
        }

        void control_MouseWheel(object sender, MouseEventArgs e)
        {
            floOutput_MouseWheel(sender, e);
        }

        void control_Click(object sender, EventArgs e)
        {
            DBEntryControl clickedControl = (DBEntryControl)sender;
            foreach (Control control in floOutput.Controls)
            {
                DBEntryControl dbEntryControl = (DBEntryControl)control;
                dbEntryControl.Selected = false;
            }
            clickedControl.Selected = true;
            txtTagEditor.Text = clickedControl.Entry.TagString;
        }

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
            catch(Exception ex)
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
                    GokiBytesReader reader = new GokiBytesReader(GokiUtility.getDecompressedByteArray(File.ReadAllBytes(databasePath)));
                    entries.Clear();
                    int count = reader.readInt();
                    for (int i = 0; i < count; i++)
                    {
                        byte[] data = new byte[reader.readInt()];
                        DBEntry entry = DBEntry.fromByteArray(reader.readByteArray(data));
                        entries.Add(entry.Location, entry);
                    }
                }
                catch(Exception ex )
                {
                    MessageBox.Show("Could not load database.\n" + ex.Message);
                }
            }
        }

        private void saveThumbnails()
        {
            try
            {
                AutoSizeGokiBytesWriter writer = new AutoSizeGokiBytesWriter();
                writer.write(thumbnailData.Values.Count);
                foreach (KeyValuePair<string, BitmapDataSizeTuple> pair in thumbnailData)
                {
                    writer.writeString(pair.Key);
                    writer.writeSizedByteArray(pair.Value.toByteArray());
                }
                File.WriteAllBytes(thumbnailsPath, GokiUtility.getCompressedByteArray(writer.Data));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save thumbnails.\n" + ex.Message);
            }
        }

        private void loadThumbnails()
        {
            if (File.Exists(thumbnailsPath))
            {
                try
                {
                    GokiBytesReader reader = new GokiBytesReader(GokiUtility.getDecompressedByteArray(File.ReadAllBytes(thumbnailsPath)));
                    thumbnailData.Clear();
                    int count = reader.readInt();
                    for (int i = 0; i < count; i++)
                    {
                        string key = reader.readString();
                        BitmapDataSizeTuple tuple = BitmapDataSizeTuple.fromByteArray(reader.readSizedByteArray());
                        thumbnailData.Add(key, tuple);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not load thumbnails.\n" + ex.Message);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            selectedEntries.Clear();
            floOutput.Controls.Clear();
            txtSearch.Text = "";
        }

        private void btnUpdateTags_Click(object sender, EventArgs e)
        {
            int amountUpdated = 0;
            foreach ( Control control in floOutput.Controls )
            {
                DBEntryControl dbEntryControl = (DBEntryControl)control;
                if ( dbEntryControl.Selected )
                {
                    entries[dbEntryControl.Entry.Location].TagString = txtTagEditor.Text.Trim();
                    amountUpdated++;
                }
            }
            lblStatus.Text = amountUpdated + " file(s) tags updated.";
        }

        private void entriesPerPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEntriesPerPage entriesForm = new frmEntriesPerPage();
            entriesForm.ShowDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveSettings();
            saveDatabase();
            saveThumbnails();
        }
    }
}

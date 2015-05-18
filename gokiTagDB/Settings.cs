using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace gokiTagDB
{
    public class Settings
    {
        public static float[] zoomLevels = new float[7] { .25f, .50f, .75f, 1.0f, 1.25f, 1.5f, 2.0f };
        public static int[] updateIntervals = new int[] { 15, 30, 100 };

        private string fileFilter;

        public string FileFilter
        {
            get { return fileFilter; }
            set { fileFilter = value; }
        }
        private SortType sortType;

        public SortType SortType
        {
            get { return sortType; }
            set { sortType = value; }
        }

        private SelectionMode selectionMode;

        public SelectionMode SelectionMode
        {
            get { return selectionMode; }
            set { selectionMode = value; }
        }

        private int thumbnailWidth;

        public int ThumbnailWidth
        {
            get { return thumbnailWidth; }
            set { thumbnailWidth = value; }
        }
        private int thumbnailHeight;

        public int ThumbnailHeight
        {
            get { return thumbnailHeight; }
            set { thumbnailHeight = value; }
        }
        private int zoomIndex;

        public int ZoomIndex
        {
            get { return zoomIndex; }
            set { zoomIndex = value; }
        }

        private int updateInterval;

        public int UpdateInterval
        {
            get { return updateInterval; }
            set { updateInterval = value; }
        }

        private Padding panelPadding ;

        public Padding PanelPadding
        {
            get { return panelPadding; }
            set { panelPadding = value; }
        }
        private Padding entryMargin;

        public Padding EntryMargin
        {
            get { return entryMargin; }
            set { entryMargin = value; }
        }
        private Padding entryPadding;

        public Padding EntryPadding
        {
            get { return entryPadding; }
            set { entryPadding = value; }
        }
        private int borderSize;

        public int BorderSize
        {
            get { return borderSize; }
            set
            {
                borderSize = value;
                if (borderSize < 1)
                {
                    borderSize = 1;
                }
            }
        }

        private long approximateMemoryUsage;

        public long ApproximateMemoryUsage
        {
            get { return approximateMemoryUsage; }
            set { approximateMemoryUsage = value; }
        }

        private int maximumSuggestions;

        public int MaximumSuggestions
        {
            get { return maximumSuggestions; }
            set { maximumSuggestions = value; }
        }

        private ThumbnailGenerationMethod thumbnailGenerationMethod;

        public ThumbnailGenerationMethod ThumbnailGenerationMethod
        {
            get { return thumbnailGenerationMethod; }
            set { thumbnailGenerationMethod = value; }
        }

        public Settings()
        {
            FileFilter = @".*(\.jpeg|\.jpg|\.png|\.gif|\.bmp|\.webm)";
            SortType = SortType.Location;
            SelectionMode = SelectionMode.Explorer;
            ThumbnailWidth = 200;
            ThumbnailHeight = 200;
            ZoomIndex = 3;
            UpdateInterval = updateIntervals[0];
            PanelPadding = new Padding(2);
            EntryMargin = new Padding(1);
            EntryPadding = new Padding(1);
            BorderSize = 1;
            ApproximateMemoryUsage = 100000000;
            MaximumSuggestions = 10;
            ThumbnailGenerationMethod = gokiTagDB.ThumbnailGenerationMethod.Smart;
        }


    }
}

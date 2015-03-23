using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace gokiTagDB
{
    public class GokiTagDB
    {
        // Lists
        internal static Dictionary<string, DBEntry> entries = new Dictionary<string, DBEntry>();
        internal static Dictionary<string, ThumbnailInfo> thumbnailInfo = new Dictionary<string, ThumbnailInfo>();
        internal static Dictionary<string, int> tagCounts = new Dictionary<string, int>();
        internal static List<DBEntry> queriedEntries = new List<DBEntry>();

        // Paths
        internal static string tagsPath = @".\tags.db";
        internal static string settingsPath = @".\settings.dat";
        internal static string databasePath = @".\entries.db";
        internal static string thumbnailsPath = @".\thumbnails.db";
        internal static string thumbnailsIndexPath = @".\thumbnails_index.db";

        // Settings
        internal static SortType sortType = SortType.Location;
        internal static string fileFilter = @".*(\.jpeg|\.jpg|\.png|\.gif|\.bmp|\.webm)";
        internal static int entriesPerPage = 20;

        // Streams
        internal static MemoryStream thumbnailIndexStream;
        internal static MemoryStream databaseStream;
        internal static FileStream thumbnailStream;
    }
}

﻿using GokiLibrary;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace gokiTagDB
{
    public class SaveAndLoad
    {
        public static void saveSettings()
        {
            try
            {
                File.WriteAllText(GokiTagDB.settingsPath, JsonConvert.SerializeObject(GokiTagDB.settings,Formatting.Indented));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save settings.\n" + ex.Message);
            }
        }

        public static void loadSettings()
        {
            if (File.Exists(GokiTagDB.settingsPath))
            {
                try
                {
                    GokiTagDB.settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(GokiTagDB.settingsPath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not load settings.\n" + ex.Message);
                }
            }
        }

        public static void saveTags()
        {
            try
            {
                File.WriteAllBytes(GokiTagDB.tagsPath, Tags.toByteArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save tag database.\n" + ex.Message);
            }
        }

        public static void loadTags()
        {
            try
            {
                if (File.Exists(GokiTagDB.tagsPath))
                {
                    Tags.loadFromByteArray(File.ReadAllBytes(GokiTagDB.tagsPath));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load tag database.\n" + ex.Message);
            }
        }

        public static void saveDatabase()
        {
            using (FileStream databaseFileStream = new FileStream(GokiTagDB.databasePath, FileMode.Create, FileAccess.ReadWrite))
            {
                byte[] data = GokiTagDB.databaseStream.ToArray();
                databaseFileStream.Write(data, 0, data.Length);
            }
        }
        public static void editEntry(DBEntry entry, DBEntry newEntry)
        {
            if (GokiTagDB.entries.ContainsKey(entry.FilePath))
            {
                //byte[] dataBefore = new byte[entry.Index];
                byte[] dataAfter = new byte[GokiTagDB.databaseStream.Length - entry.Index - entry.Length];

                //databaseStream.Seek(0, SeekOrigin.Begin);
                //databaseStream.Read(dataBefore, 0, dataBefore.Length);
                GokiTagDB.databaseStream.Seek(entry.Index + entry.Length, SeekOrigin.Begin);
                GokiTagDB.databaseStream.Read(dataAfter, 0, dataAfter.Length);
                GokiTagDB.databaseStream.Seek(entry.Index, SeekOrigin.Begin);
                AutoSizeGokiBytesWriter writer = new AutoSizeGokiBytesWriter();
                writer.write(newEntry.FilePath);
                writer.write(newEntry.TagString);
                GokiTagDB.databaseStream.Write(writer.Data, 0, writer.Data.Length);

                GokiTagDB.databaseStream.Write(dataAfter, 0, dataAfter.Length);
                GokiTagDB.databaseStream.SetLength(GokiTagDB.databaseStream.Position);

                long entryIndex = entry.Index;
                int entryLength = entry.Length;
                int newEntryLength = newEntry.Length;
                foreach (DBEntry checkEntry in GokiTagDB.entries.Values)
                {
                    if (checkEntry.Index > entryIndex)
                    {
                        checkEntry.Index += newEntryLength - entryLength;
                    }
                }
                entry.TagString = newEntry.TagString;
            }
        }
        public static void removeEntriesFromDatabase(List<DBEntry> removalEntries)
        {
            // Fastest method is indeed just rewriting the entire file... of course
            foreach (DBEntry entry in removalEntries)
            {
                GokiTagDB.entries.Remove(entry.FilePath);
                if (GokiTagDB.queriedEntries.Contains(entry))
                {
                    GokiTagDB.queriedEntries.Remove(entry);
                }
            }
            syncDatabaseStream();
        }

        public static void loadDatabase()
        {
            try
            {
                int errors = 0;
                if (File.Exists(GokiTagDB.databasePath))
                {

                    using (FileStream databaseFileStream = new FileStream(GokiTagDB.databasePath, FileMode.Open, FileAccess.Read))
                    {
                        databaseFileStream.CopyTo(GokiTagDB.databaseStream);
                    }
                    string fullTagString = "";
                    //   GokiBytesReader reader = new GokiBytesReader(GokiUtility.getDecompressedByteArray(File.ReadAllBytes(databasePath)));
                    GokiTagDB.databaseStream.Seek(0, SeekOrigin.Begin);

                    byte[] data = new byte[GokiTagDB.databaseStream.Length];
                    GokiTagDB.databaseStream.Read(data, 0, data.Length);
                    GokiBytesReader reader = new GokiBytesReader(data);
                    GokiTagDB.entries.Clear();
                    while (!reader.EOF)
                    {
                        try
                        {
                            int index = reader.Index;
                            DBEntry entry = new DBEntry(reader.readString(), reader.readString());
                            entry.Index = index;
                            entry.FileSize = new FileInfo(entry.FilePath).Length;
                            if (entry.TagString.Length > 0 && fullTagString.IndexOf(entry.TagString) == -1)
                            {
                                fullTagString += entry.TagString + " ";
                            }
                            GokiTagDB.entries.Add(entry.FilePath, entry);
                        }
                        catch (Exception ex)
                        {
                            errors++;
                        }

                        string[] allTags = fullTagString.Trim().Split(' ');
                        foreach (string tag in allTags)
                        {
                            if (!Tags.tags.ContainsKey(tag))
                            {
                                Tags.addTag(tag, null);
                            }
                        }
                    }
                }
                if ( errors > 0)
                {
                    MessageBox.Show(String.Format("Could not load {0:N0} file(s).", errors));
                    SaveAndLoad.syncDatabaseStream();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load database.\n" + ex.Message);
            }
        }

        public static void loadThumbnails()
        {
            if (File.Exists(GokiTagDB.thumbnailsIndexPath) && File.Exists(GokiTagDB.thumbnailsPath))
            {
                try
                {
                    using (FileStream thumbnailsIndexFileStream = new FileStream(GokiTagDB.thumbnailsIndexPath, FileMode.Open, FileAccess.Read))
                    {
                        thumbnailsIndexFileStream.CopyTo(GokiTagDB.thumbnailIndexStream);
                    }
                    GokiTagDB.thumbnailIndexStream.Seek(0, SeekOrigin.Begin);
                    byte[] data = new byte[GokiTagDB.thumbnailIndexStream.Length];
                    GokiTagDB.thumbnailIndexStream.Read(data, 0, data.Length);
                    GokiBytesReader reader = new GokiBytesReader(data);
                    GokiTagDB.thumbnailInfo.Clear();
                    while (!reader.EOF)
                    {
                        int indexIndex = reader.Index;
                        string location = reader.readString();
                        long index = reader.readLong();
                        long size = reader.readLong();
                        if (!GokiTagDB.thumbnailInfo.ContainsKey(location))
                        {
                            GokiTagDB.thumbnailInfo.Add(location, new ThumbnailInfo(location, indexIndex, index, size));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not load thumbnails.\n" + ex.Message);
                }
            }
        }

        public static void saveThumbnails()
        {
            using (FileStream thumbnailIndexFileStream = new FileStream(GokiTagDB.thumbnailsIndexPath, FileMode.Create, FileAccess.ReadWrite))
            {
                byte[] data = GokiTagDB.thumbnailIndexStream.ToArray();
                thumbnailIndexFileStream.Write(data, 0, data.Length);
            }
        }

        public static void addEntryToDatabase(DBEntry entry)
        {
            if (!GokiTagDB.entries.ContainsKey(entry.FilePath))
            {
                GokiTagDB.databaseStream.Seek(0, SeekOrigin.End);
                entry.Index = GokiTagDB.databaseStream.Position;
                byte[] data = entry.toByteArray();
                GokiTagDB.databaseStream.Write(data, 0, data.Length);
                GokiTagDB.entries.Add(entry.FilePath, entry);
            }
        }

        public static void syncDatabaseStream()
        {
            GokiTagDB.databaseStream.Seek(0, SeekOrigin.Begin);
            GokiTagDB.databaseStream.SetLength(0);
            foreach (DBEntry entry in GokiTagDB.entries.Values)
            {
                entry.Index = GokiTagDB.databaseStream.Position;
                AutoSizeGokiBytesWriter writer = new AutoSizeGokiBytesWriter();
                writer.write(entry.FilePath);
                writer.write(entry.TagString);
                GokiTagDB.databaseStream.Write(writer.Data, 0, writer.Data.Length);
            }
        }

        public static void addThumbnailIndexToDatabase(ThumbnailInfo info)
        {
            byte[] thumbnailInfoData = info.toByteArray();
            GokiTagDB.thumbnailIndexStream.Seek(0, SeekOrigin.End);
            GokiTagDB.thumbnailIndexStream.Write(info.toByteArray(), 0, thumbnailInfoData.Length);
        }

        public static void addThumbnailToDatabase(DBEntry entry)
        {
            try
            {
                if (!GokiTagDB.thumbnailInfo.ContainsKey(entry.FilePath))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        if (entry.Thumbnail == null)
                        {
                            entry.generateThumbnail(GokiTagDB.thumbnailStream);
                        }
                        if (GokiTagDB.settings.ThumbnailGenerationMethod == ThumbnailGenerationMethod.PNG)
                        {
                            entry.Thumbnail.Save(memoryStream, ImageFormat.Png);
                        }
                        else if (GokiTagDB.settings.ThumbnailGenerationMethod == ThumbnailGenerationMethod.Auto)
                        {
                            if ( entry.FileExtension.ToLower() == ".png" ||  entry.FileExtension.ToLower() == ".gif")
                            {
                                entry.Thumbnail.Save(memoryStream, ImageFormat.Png);
                            }
                            else
                            {
                                entry.Thumbnail.Save(memoryStream, ImageFormat.Jpeg);
                            }
                        }
                        else if (GokiTagDB.settings.ThumbnailGenerationMethod == ThumbnailGenerationMethod.Smart)
                        {
                            if ( GokiPixels.usesAlpha(entry.Thumbnail))
                            {
                                entry.Thumbnail.Save(memoryStream, ImageFormat.Png);
                            }
                            else
                            {
                                entry.Thumbnail.Save(memoryStream, ImageFormat.Jpeg);
                            }
                        }
                        else 
                        {
                            entry.Thumbnail.Save(memoryStream, ImageFormat.Jpeg);
                        }
                        byte[] thumbnailData = memoryStream.ToArray();

                        GokiTagDB.thumbnailIndexStream.Seek(0, SeekOrigin.End);
                        GokiTagDB.thumbnailStream.Seek(0, SeekOrigin.End);

                        ThumbnailInfo info = new ThumbnailInfo(entry.FilePath, GokiTagDB.thumbnailIndexStream.Position, GokiTagDB.thumbnailStream.Position, thumbnailData.Length);
                        byte[] thumbnailInfoData = info.toByteArray();
                        GokiTagDB.thumbnailIndexStream.Write(info.toByteArray(), 0, thumbnailInfoData.Length);
                        GokiTagDB.thumbnailStream.Write(thumbnailData, 0, thumbnailData.Length);

                        GokiTagDB.thumbnailInfo.Add(entry.FilePath, info);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void clearThumbnailStreams()
        {
            try
            {
                GokiTagDB.thumbnailStream.Close();
                GokiTagDB.thumbnailStream.Dispose();
                File.Delete(GokiTagDB.thumbnailsPath);
                GokiTagDB.thumbnailStream = new FileStream(GokiTagDB.thumbnailsPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                GokiTagDB.thumbnailIndexStream.Close();
                GokiTagDB.thumbnailIndexStream.Dispose();
                GokiTagDB.thumbnailIndexStream = new MemoryStream();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error clearing thumbnail database");
            }
        }
    }
}

using GokiLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gokiTagDB
{
    public class DBEntry
    {
        private string location;

        public string FilePath
        {
            get { return location; }
            set { location = value; }
        }

        public string FileExtension
        {
            get
            {
                return Path.GetExtension(FilePath);
            }
        }

        private long fileSize = 0;

        public long FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }

        public DateTime CreationDate
        {
            get
            {
                DateTime creationDate = DateTime.MinValue;
                try
                {
                    creationDate = new FileInfo(FilePath).CreationTime;
                }
                catch (Exception ex)
                {
                }
                return creationDate;
            }
        }

        public DateTime ModifiedDate
        {
            get
            {
                DateTime modifiedDate = DateTime.MinValue;
                try
                {
                    modifiedDate = new FileInfo(FilePath).LastWriteTime;
                }
                catch (Exception ex)
                {
                }
                return modifiedDate;
            }
        }

        private string tagString;

        public string TagString
        {
            get {
                if ( tagString == null )
                {
                    tagString = "";
                }
                return tagString;
            }
            set { tagString = value; }
        }

        private Bitmap thumbnail;

        public Bitmap Thumbnail
        {
            get {
                return thumbnail;
            }
            set { thumbnail = value; }
        }

        private byte[] md5 = null;

        private bool selected = false;

        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        private bool active = false;

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        private long index = -1;

        public long Index
        {
            get { return index; }
            set { index = value; }
        }

        public DBEntry(): this("", ""){}

        public DBEntry(string location, string tagString)
        {
            FilePath = location;
            TagString = tagString;
        }

        public void generateThumbnail(FileStream thumbnailStream)
        {
            if (GokiTagDB.thumbnailInfo.ContainsKey(FilePath))
            {
                try
                {
                    thumbnailStream.Seek((int)GokiTagDB.thumbnailInfo[FilePath].ImageIndex, SeekOrigin.Begin);
                    byte[] data = new byte[(int)GokiTagDB.thumbnailInfo[FilePath].Size];
                    thumbnailStream.Read(data, 0, data.Length);
                    using (MemoryStream stream = new MemoryStream(data))
                    {
                        Bitmap bitmap = (Bitmap)Bitmap.FromStream(stream);
                        thumbnail = bitmap;
                    }
                }
                catch(Exception ex)
                {
                    GokiTagDB.thumbnailInfo.Remove(FilePath);
                }
            }
            else if (location != null && File.Exists(location))
            {
                try
                {
                    Image original = Image.FromFile(location);
                    float width = original.Width;
                    float height = original.Height;
                    float xAspectRatio = width / GokiTagDB.settings.ThumbnailWidth;
                    float yAspectRatio = height / GokiTagDB.settings.ThumbnailHeight;
                    if (xAspectRatio > yAspectRatio)
                    {
                        width /= xAspectRatio;
                        height /= xAspectRatio;
                    }
                    else
                    {
                        width /= yAspectRatio;
                        height /= yAspectRatio;
                    }
                    thumbnail = new Bitmap(original, (int)width, (int)height);
                    original.Dispose();
                    original = null;
                }
                catch (Exception ex)
                {

                }
            }
            if ( thumbnail == null)
            {
                thumbnail = Properties.Resources.missing_thumbnail;
            }
        }

        public byte[] MD5
        {
            get
            {
                byte[] output = this.md5;
                if (output == null)
                {
                    if (File.Exists(FilePath))
                    {
                        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                        {
                            using (FileStream fileStream = File.OpenRead(FilePath))
                            {
                                output = md5.ComputeHash(fileStream);
                            }
                        }
                    }
                }
                return output;
            }
        }

        public string MD5String
        {
            get
            {
                string output = "";
                byte[] md5 = MD5;
                if ( md5 != null )
                {
                    output = BitConverter.ToString(md5).Replace("-", "").ToLower();
                }
                return output;
            }
        }

        public int Length
        {
            get
            {
                return (FilePath.Length + TagString.Length) * sizeof(char) + sizeof(int) * 2;
            }
        }

        public byte[] toByteArray()
        {
            GokiBytesWriter writer = new GokiBytesWriter(Length);
            writer.write( FilePath );
            writer.write( TagString );
            return writer.data;
        }

        public static DBEntry fromByteArray( byte[] data )
        {
            GokiBytesReader reader = new GokiBytesReader(data);
            string location = reader.readString();
            string tagString = reader.readString();
            return new DBEntry( location, tagString);
        }
    }
}

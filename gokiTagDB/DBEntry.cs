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
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string location;

        public string Location
        {
            get { return location; }
            set { location = value; }
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

        private bool hover = false;

        public bool Hover
        {
            get { return hover; }
            set { hover = value; }
        }

        private bool active = false;

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }


        public DBEntry(): this("", "", ""){}

        public DBEntry(string name, string location, string tagString)
        {
            Name = name;
            Location = location;
            TagString = tagString;
        }

        public void generateThumbnail()
        {
            if (frmMainForm.thumbnailInfo.ContainsKey(Location))
            {
                using (FileStream fileStream = File.OpenRead(frmMainForm.thumbnailsPath))
                {
                    fileStream.Seek((int)frmMainForm.thumbnailInfo[Location].Index,SeekOrigin.Begin);
                    byte[] data = new byte[(int)frmMainForm.thumbnailInfo[Location].Size];
                    fileStream.Read(data, 0, data.Length);
                    fileStream.Close();
                    using (MemoryStream stream = new MemoryStream(data))
                    {
                        Bitmap bitmap = (Bitmap)Bitmap.FromStream(stream);
                        thumbnail = bitmap;
                    }
                }
            }
            else if ( location != null && File.Exists(location))
            {
                Bitmap bitmap = null;
                try
                {
                    bitmap = (Bitmap)Image.FromFile(location);
                }
                catch(Exception ex)
                {
                    bitmap = Properties.Resources.missing_thumbnail;
                }
                if ( bitmap != null )
                {
                    float width = bitmap.Width;
                    float height = bitmap.Height;
                    float xAspectRatio = width / frmMainForm.thumbnailWidth;
                    float yAspectRatio = height / frmMainForm.thumbnailHeight;
                    if ( xAspectRatio > yAspectRatio )
                    {
                        width /= xAspectRatio;
                        height /= xAspectRatio;
                    }
                    else
                    {
                        width /= yAspectRatio;
                        height /= yAspectRatio;
                    }
                    thumbnail = GokiPixels.resize(bitmap,width,height,0,System.Drawing.Drawing2D.InterpolationMode.Bilinear);
                    
                    bitmap.Dispose();
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
                    if (File.Exists(Location))
                    {
                        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                        {
                            using (FileStream fileStream = File.OpenRead(Location))
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

        public byte[] toByteArray()
        {
            int capacity = (Name.Length + Location.Length + TagString.Length)*sizeof(char) + sizeof(int)  * 3;
            GokiBytesWriter writer = new GokiBytesWriter(capacity);
            writer.write( Name );
            writer.write( Location );
            writer.write( TagString );
            
            return writer.data;
        }

        public static DBEntry fromByteArray( byte[] data )
        {
            GokiBytesReader reader = new GokiBytesReader(data);
            string name = reader.readString();
            string location = reader.readString();
            string tagString = reader.readString();
            return new DBEntry(name, location, tagString);
        }
    }
}

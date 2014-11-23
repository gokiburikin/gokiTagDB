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
            get { return thumbnail; }
            set { thumbnail = value; }
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
            if (frmMainForm.thumbnailData.ContainsKey(Location))
            {
                Thumbnail = frmMainForm.thumbnailData[Location].Bitmap;
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
                    float xAspectRatio = width / 128;
                    float yAspectRatio = height / 128;
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
                    if ( !frmMainForm.thumbnailData.ContainsKey(Location))
                    {
                        frmMainForm.thumbnailData.Add(Location, new BitmapDataSizeTuple(thumbnail.Width, thumbnail.Height, GokiPixels.getDataFromBitmap(thumbnail)));
                    }
                    else
                    {
                        frmMainForm.thumbnailData[Location] = new BitmapDataSizeTuple(thumbnail.Width, thumbnail.Height, GokiPixels.getDataFromBitmap(thumbnail));
                    }
                    bitmap.Dispose();
                }
            }
        }

        public byte[] toByteArray()
        {
            int capacity = (name.Length + location.Length + tagString.Length)*sizeof(char) + sizeof(int)  * 3;
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

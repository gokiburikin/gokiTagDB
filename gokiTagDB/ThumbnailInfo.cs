using GokiLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gokiTagDB
{
    public class ThumbnailInfo
    {
        private string location;

        public string Location
        {
            get { return location; }
            set { location = value; }
        }

        private long indexIndex;

        public long IndexIndex
        {
            get { return indexIndex; }
            set { indexIndex = value; }
        }

        private long imageIndex;

        public long ImageIndex
        {
            get { return imageIndex; }
            set { imageIndex = value; }
        }
        private long size;

        public long Size
        {
            get { return size; }
            set { size = value; }
        }

        public ThumbnailInfo(string location, long indexIndex, long imageIndex, long size)
        {
            Location = location;
            IndexIndex = indexIndex;
            ImageIndex = imageIndex;
            Size = size;
        }

        public long Length
        {
            get
            {
                return sizeof(char) * Location.Length + sizeof(int) + sizeof(long) * 2;
            }
        }

        public byte[] toByteArray()
        {
            GokiBytesWriter writer = new GokiBytesWriter(Length);
            writer.write(Location);
            writer.write(ImageIndex);
            writer.write(Size);
            return writer.data;
        }

        public static ThumbnailInfo fromByteArray(byte[] data)
        {
            GokiBytesReader reader = new GokiBytesReader(data);
            return new ThumbnailInfo(reader.readString(), -1, reader.readLong(), reader.readLong());
        }
    }
}

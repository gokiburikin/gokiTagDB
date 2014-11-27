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
        private long index;

        public long Index
        {
            get { return index; }
            set { index = value; }
        }
        private long size;

        public long Size
        {
            get { return size; }
            set { size = value; }
        }

        public ThumbnailInfo(string location, long index, long size)
        {
            Location = location;
            Index = index;
            Size = size;
        }

        public byte[] toByteArray()
        {
            long capacity = sizeof(char) * Location.Length + sizeof(int) + sizeof(long) * 3;
            GokiBytesWriter writer = new GokiBytesWriter(capacity);
            writer.write(Location);
            writer.write(Index);
            writer.write(Size);
            return writer.data;
        }

        public static ThumbnailInfo fromByteArray(byte[] data)
        {
            GokiBytesReader reader = new GokiBytesReader(data);
            return new ThumbnailInfo(reader.readString(), reader.readLong(), reader.readLong());
        }
    }
}

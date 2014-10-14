using System.IO;

namespace NFGraph.Net.Util
{
    public class SimpleByteArray : IByteData
    {

        private readonly byte[] _data;

        public SimpleByteArray(int length)
        {
            _data = new byte[length];
        }

        public SimpleByteArray(byte[] data)
        {
            _data = data;
        }

        public void Set(long idx, byte b)
        {
            _data[(int) idx] = b;
        }

        public byte Get(long idx)
        {
            return _data[(int) idx];
        }

        public long Length()
        {
            return _data.Length;
        }

        public void WriteTo(BinaryWriter os, long length)
        {
            os.Write(_data, 0, (int) length);
        }
    }
}
using System.IO;

namespace NFGraph.Net.Util
{
    public class ByteArrayBuffer
    {

        private readonly SegmentedByteArray _data;

        private long _pointer;

        public ByteArrayBuffer()
        {
            _data = new SegmentedByteArray(14);
            _pointer = 0;
        }

        /**
         * Copies the contents of the specified buffer into this buffer at the current position.
         */
        public void Write(ByteArrayBuffer buf)
        {
            _data.Copy(buf._data, 0, _pointer, buf.Length());
            _pointer += buf.Length();
        }

        /**
         * Writes a variable-byte encoded integer to the byte array.
         */
        public void WriteVInt(int value)
        {
            if (value < 0)
            {
                WriteByte(0x80);
                return;
            }

            if (value > 0x0FFFFFFF) WriteByte((byte)(0x80 | ((int)((uint)value >> 28))));
            if (value > 0x1FFFFF)   WriteByte((byte)(0x80 | ((int)((uint)value >> 21) & 0x7F)));
            if (value > 0x3FFF)     WriteByte((byte)(0x80 | ((int)((uint)value >> 14) & 0x7F)));
            if (value > 0x7F)       WriteByte((byte)(0x80 | ((int)((uint)value >> 7) & 0x7F)));

            WriteByte((byte) (value & 0x7F));
        }

        /**
         * The current length of the written data, in bytes.
         */
        public long Length()
        {
            return _pointer;
        }

        /**
         * Sets the length of the written data to 0.
         */
        public void Reset()
        {
            _pointer = 0;
        }

        /**
         * @return The underlying SegmentedByteArray containing the written data.
         */
        public SegmentedByteArray GetData()
        {
            return _data;
        }

        /**
         * Writes a byte of data.
         */
        public void WriteByte(byte b)
        {
            _data.Set(_pointer++, b);
        }

        /**
         * Writes each byte of data, in order.
         */
        public void Write(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                WriteByte(data[i]);
            }
        }

        /**
         * Copies the written data to the given <code>OutputStream</code>
         */
        public void CopyTo(BinaryWriter os)
        {
            _data.WriteTo(os, 0, _pointer);
        }

    }

}
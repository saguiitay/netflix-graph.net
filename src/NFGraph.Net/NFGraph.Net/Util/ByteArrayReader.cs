namespace NFGraph.Net.Util
{
    public class ByteArrayReader
    {

        private readonly IByteData _data;

        private long _pointer;
        private long _startByte;
        private long _endByte = int.MaxValue;

        public ByteArrayReader(IByteData data, long pointer)
        {
            _data = data;
            _pointer = pointer;
            _startByte = pointer;
            _endByte = data.Length();
        }

        public ByteArrayReader(IByteData data, long startByte, long endByte)
        {
            _data = data;
            _startByte = startByte;
            _endByte = endByte;
            _pointer = startByte;
        }

        /**
     * @return the byte value at the given offset.
     */

        public byte GetByte(long offset)
        {
            return _data.Get(_startByte + offset);
        }

        /**
     * Set the current offset of this reader.
     */

        public void SetPointer(long pointer)
        {
            _pointer = pointer;
        }

        /**
     * Increment the current offset of this reader by numBytes.
     */

        public void Skip(long numBytes)
        {
            _pointer += numBytes;
        }

        /**
     * @return a variable-byte integer at the current offset.  The offset is incremented by the size of the returned integer.
     */

        public int ReadVInt()
        {
            if (_pointer >= _endByte)
                return -1;

            byte b = ReadByte();

            if (b == 0x80)
                return -1;

            int value = b & 0x7F;
            while ((b & 0x80) != 0)
            {
                b = ReadByte();
                value <<= 7;
                value |= (b & 0x7F);
            }

            return value;
        }

        /**
     * @return the byte at the current offset.  The offset is incremented by one.
     */

        public byte ReadByte()
        {
            return _data.Get(_pointer++);
        }

        /**
     * Sets the start byte of this reader to the current offset, then sets the end byte to the current offset + <code>remainingBytes</code>
     */

        public void SetRemainingBytes(int remainingBytes)
        {
            _startByte = _pointer;
            _endByte = _pointer + remainingBytes;
        }

        /**
     * Sets the current offset of this reader to the start byte.
     */

        public void Reset()
        {
            _pointer = _startByte;
        }

        /**
     * @return the length of this reader.
     */

        public long Length()
        {
            return _endByte - _startByte;
        }

        /**
     * @return a copy of this reader.  The copy will have the same underlying byte array, start byte, and end byte, but the current offset will be equal to the start byte.
     */

        public ByteArrayReader Copy()
        {
            return new ByteArrayReader(_data, _startByte, _endByte);
        }


    }

}
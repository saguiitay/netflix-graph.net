using NFGraph.Net.Util;

namespace NFGraph.Net.Compressed
{
    public class HashSetOrdinalIterator : IOrdinalIterator
    {

        private readonly ByteArrayReader _reader;
        private readonly int _beginOffset;
        private int _offset;
        private bool _firstValue;

        public HashSetOrdinalIterator(ByteArrayReader reader)
        {
            _reader = reader;
            SeekBeginByte();
            _beginOffset = _offset;
            _firstValue = true;
        }

        public int NextOrdinal()
        {
            SeekBeginByte();

            if (_offset == _beginOffset)
            {
                if (!_firstValue)
                    return Consts.NO_MORE_ORDINALS;
                _firstValue = false;
            }

            int value = _reader.GetByte(_offset);
            NextOffset();

            while ((_reader.GetByte(_offset) & 0x80) != 0)
            {
                value <<= 7;
                value |= _reader.GetByte(_offset) & 0x7F;
                NextOffset();
            }

            return value - 1;
        }

        public void Reset()
        {
            _offset = _beginOffset;
            _firstValue = true;
        }

        public IOrdinalIterator Copy()
        {
            return new HashSetOrdinalIterator(_reader);
        }

        public bool IsOrdered()
        {
            return false;
        }

        private void NextOffset()
        {
            _offset++;
            if (_offset >= _reader.Length())
            {
                _offset = 0;
            }
        }

        private void SeekBeginByte()
        {
            while ((_reader.GetByte(_offset) & 0x80) != 0 || _reader.GetByte(_offset) == 0)
                NextOffset();
        }
    }

}
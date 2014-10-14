using NFGraph.Net.Util;

namespace NFGraph.Net.Compressed
{
    public class HashSetOrdinalSet : OrdinalSet
    {

        private readonly ByteArrayReader _reader;
        private int _size = int.MinValue;

        public HashSetOrdinalSet(ByteArrayReader reader)
        {
            _reader = reader;
        }

        public override IOrdinalIterator Iterator()
        {
            return new HashSetOrdinalIterator(_reader.Copy());
        }

        public override bool Contains(int value)
        {
            value += 1;

            int offset = (Mixer.HashInt(value) & ((int)_reader.Length() - 1));

            offset = SeekBeginByte(offset);

            while (_reader.GetByte(offset) != 0)
            {
                int readValue = _reader.GetByte(offset);
                offset = NextOffset(offset);

                while ((_reader.GetByte(offset) & 0x80) != 0)
                {
                    readValue <<= 7;
                    readValue |= _reader.GetByte(offset) & 0x7F;
                    offset = NextOffset(offset);
                }

                if (readValue == value)
                    return true;
            }

            return false;
        }

        public override int Size()
        {
            if (_size == int.MinValue)
                _size = CountHashEntries();
            return _size;
        }

        private int SeekBeginByte(int offset)
        {
            while ((_reader.GetByte(offset) & 0x80) != 0)
                offset = NextOffset(offset);
            return offset;
        }

        private int NextOffset(int offset)
        {
            offset++;
            if (offset >= _reader.Length())
            {
                offset = 0;
            }
            return offset;
        }

        private int CountHashEntries()
        {
            int counter = 0;
            for (int i = 0; i < _reader.Length(); i++)
            {
                byte b = _reader.GetByte(i);
                if (b != 0 && (b & 0x80) == 0)
                    counter++;
            }
            return counter;
        }

    }
}
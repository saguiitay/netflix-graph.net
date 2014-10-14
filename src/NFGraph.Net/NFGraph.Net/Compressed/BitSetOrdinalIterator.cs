using NFGraph.Net.Util;

namespace NFGraph.Net.Compressed
{
    public class BitSetOrdinalIterator : IOrdinalIterator
    {

        private readonly ByteArrayReader _reader;
        public int Offset;

        public BitSetOrdinalIterator(ByteArrayReader reader)
        {
            _reader = reader;
        }

        public int NextOrdinal()
        {
            if ((int)((uint)Offset >> 3) == _reader.Length())
                return Consts.NO_MORE_ORDINALS;

            SkipToNextPopulatedByte();

            while (MoreBytesToRead())
            {
                if (TestCurrentBit())
                {
                    return Offset++;
                }
                Offset++;
            }

            return Consts.NO_MORE_ORDINALS;
        }

        public void Reset()
        {
            Offset = 0;
        }

        public IOrdinalIterator Copy()
        {
            return new BitSetOrdinalIterator(_reader);
        }

        public bool IsOrdered()
        {
            return true;
        }

        private void SkipToNextPopulatedByte()
        {
            if (MoreBytesToRead()
                && (CurrentByte() >> (Offset & 0x07)) == 0)
            {
                Offset += 0x08;
                Offset &= ~0x07;

                while (MoreBytesToRead() && CurrentByte() == 0)
                    Offset += 0x08;
            }
        }

        private bool MoreBytesToRead()
        {
            return (int)((uint)Offset >> 3) < _reader.Length();
        }

        private bool TestCurrentBit()
        {
            int b = CurrentByte();
            return (b & (1 << (Offset & 0x07))) != 0;
        }

        private byte CurrentByte()
        {
            return _reader.GetByte((int)((uint)Offset >> 3));
        }

    }

}
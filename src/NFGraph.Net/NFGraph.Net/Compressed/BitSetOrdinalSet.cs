using NFGraph.Net.Util;

namespace NFGraph.Net.Compressed
{
    public class BitSetOrdinalSet : OrdinalSet
    {

        private readonly ByteArrayReader _reader;

        public BitSetOrdinalSet(ByteArrayReader reader)
        {
            _reader = reader;
        }

        public override bool Contains(int value)
        {
            int offset = value >> 3;
            int mask = 1 << (value & 0x07);

            if (offset >= _reader.Length())
                return false;

            return (_reader.GetByte(offset) & mask) != 0;
        }

        public override IOrdinalIterator Iterator()
        {
            return new BitSetOrdinalIterator(_reader);
        }

        public override int Size()
        {
            int cardinalitySum = 0;
            for (int i = 0; i < (_reader.Length()); i++)
            {
                cardinalitySum += BITS_SET_TABLE[_reader.GetByte(i) & 0xFF];
            }
            return cardinalitySum;
        }

        private static readonly int[] BITS_SET_TABLE = new int[256];

        static BitSetOrdinalSet()
        {
            for (int i = 0; i < 256; i++)
            {
                BITS_SET_TABLE[i] = (i & 1) + BITS_SET_TABLE[i/2];
            }
        }

    }
}
using NFGraph.Net.Util;

namespace NFGraph.Net.Compressor
{
    public class BitSetPropertyBuilder
    {

        private readonly ByteArrayBuffer _buf;

        public BitSetPropertyBuilder(ByteArrayBuffer buf)
        {
            _buf = buf;
        }

        public void BuildProperty(OrdinalSet ordinals, int numBits)
        {
            byte[] data = BuildBitSetData(numBits, ordinals.Iterator());
            _buf.Write(data);
        }

        private byte[] BuildBitSetData(int numBits, IOrdinalIterator iter)
        {
            int numBytes = ((numBits - 1)/8) + 1;
            var data = new byte[numBytes];

            int ordinal = iter.NextOrdinal();

            while (ordinal != Consts.NO_MORE_ORDINALS)
            {
                data[ordinal >> 3] |= (byte) (1 << (ordinal & 0x07));
                ordinal = iter.NextOrdinal();
            }

            return data;
        }

    }

}
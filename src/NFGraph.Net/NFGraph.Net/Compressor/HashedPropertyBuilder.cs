using NFGraph.Net.Util;

namespace NFGraph.Net.Compressor
{
    public class HashedPropertyBuilder
    {

        private readonly ByteArrayBuffer _buf;

        public HashedPropertyBuilder(ByteArrayBuffer buf)
        {
            _buf = buf;
        }

        public void BuildProperty(OrdinalSet ordinals)
        {
            if (ordinals.Size() == 0)
                return;

            byte[] data = BuildHashedPropertyData(ordinals);
            _buf.Write(data);
        }

        private byte[] BuildHashedPropertyData(OrdinalSet ordinals)
        {
            var data = new byte[CalculateByteArraySize(ordinals)];

            IOrdinalIterator iter = ordinals.Iterator();

            int ordinal = iter.NextOrdinal();

            while (ordinal != Consts.NO_MORE_ORDINALS)
            {
                Put(ordinal, data);
                ordinal = iter.NextOrdinal();
            }

            return data;
        }

        private void Put(int value, byte[] data)
        {
            value += 1;

            int bucket = Mixer.HashInt(value) & (data.Length - 1);

            if (data[bucket] != 0)
            {
                bucket = NextEmptyByte(data, bucket);
            }

            WriteKey(value, bucket, data);
        }

        private void WriteKey(int value, int offset, byte[] data)
        {
            int numBytes = CalculateVIntSize(value);

            EnsureSpaceIsAvailable(numBytes, offset, data);

            WriteVInt(value, offset, data, numBytes);
        }

        private void WriteVInt(int value, int offset, byte[] data, int numBytes)
        {
            int b = ((int)((uint)value >> (7*(numBytes - 1)))) & 0x7F;
            data[offset] = (byte) b;
            offset = nextOffset(data.Length, offset);

            for (int i = numBytes - 2; i >= 0; i--)
            {
                b = ((int)((uint)value >> (7*i))) & 0x7F;
                data[offset] = (byte) (b | 0x80);
                offset = nextOffset(data.Length, offset);
            }
        }

        private int nextOffset(int length, int offset)
        {
            offset++;
            if (offset == length)
                offset = 0;
            return offset;
        }

        private int previousOffset(int length, int offset)
        {
            offset--;
            if (offset == -1)
                offset = length - 1;
            return offset;
        }

        private void EnsureSpaceIsAvailable(int requiredSpace, int offset, byte[] data)
        {
            int copySpaces = 0;
            int foundSpace = 1;
            int currentOffset = offset;

            while (foundSpace < requiredSpace)
            {
                currentOffset = nextOffset(data.Length, currentOffset);
                if (data[currentOffset] == 0)
                {
                    foundSpace++;
                }
                else
                {
                    copySpaces++;
                }
            }

            int moveToOffset = currentOffset;
            currentOffset = previousOffset(data.Length, currentOffset);

            while (copySpaces > 0)
            {
                if (data[currentOffset] != 0)
                {
                    data[moveToOffset] = data[currentOffset];
                    copySpaces--;
                    moveToOffset = previousOffset(data.Length, moveToOffset);
                }
                currentOffset = previousOffset(data.Length, currentOffset);
            }
        }

        private int NextEmptyByte(byte[] data, int offset)
        {
            while (data[offset] != 0)
            {
                offset = nextOffset(data.Length, offset);
            }
            return offset;
        }

        private int CalculateByteArraySize(OrdinalSet ordinals)
        {
            int numPopulatedBytes = CalculateNumPopulatedBytes(ordinals.Iterator());

            return CalculateByteArraySizeAfterLoadFactor(numPopulatedBytes);
        }

        private int CalculateNumPopulatedBytes(IOrdinalIterator ordinalIterator)
        {
            int totalSize = 0;
            int ordinal = ordinalIterator.NextOrdinal();

            while (ordinal != Consts.NO_MORE_ORDINALS)
            {
                totalSize += CalculateVIntSize(ordinal + 1);
                ordinal = ordinalIterator.NextOrdinal();
            }

            return totalSize;
        }

        private int CalculateVIntSize(int value)
        {
            int numBitsSet = numBitsUsed(value);
            return ((numBitsSet - 1)/7) + 1;
        }

        private int CalculateByteArraySizeAfterLoadFactor(int numPopulatedBytes)
        {
            int desiredSizeAfterLoadFactor = (numPopulatedBytes*4)/3;

            int nextPowerOfTwo = 1 << numBitsUsed(desiredSizeAfterLoadFactor);
            return nextPowerOfTwo;
        }

        private int numBitsUsed(int value)
        {
            return 32 - IntegerUtils.NumberOfLeadingZeros(value);
        }

    }

}
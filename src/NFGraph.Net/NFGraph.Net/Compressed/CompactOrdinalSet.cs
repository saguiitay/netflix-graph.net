using System;
using NFGraph.Net.Compressor;
using NFGraph.Net.Util;

namespace NFGraph.Net.Compressed
{
    public class CompactOrdinalSet : OrdinalSet
    {

        private readonly ByteArrayReader _reader;
        private int _size = int.MinValue;

        public CompactOrdinalSet(ByteArrayReader reader)
        {
            _reader = reader;
        }

        public override bool Contains(int value)
        {
            IOrdinalIterator iter = Iterator();

            int iterValue = iter.NextOrdinal();

            while (iterValue < value)
            {
                iterValue = iter.NextOrdinal();
            }

            return iterValue == value;
        }

        public override bool ContainsAll(params int[] values)
        {
            IOrdinalIterator iter = Iterator();

            Array.Sort(values);

            int valuesIndex = 0;
            int setValue = iter.NextOrdinal();

            while (valuesIndex < values.Length)
            {
                if (setValue == values[valuesIndex])
                {
                    valuesIndex++;
                }
                else if (setValue < values[valuesIndex])
                {
                    setValue = iter.NextOrdinal();
                }
                else
                {
                    break;
                }
            }

            return valuesIndex == values.Length;
        }

        public override IOrdinalIterator Iterator()
        {
            return new CompactOrdinalIterator(_reader.Copy());
        }

        public override int Size()
        {
            if (SizeIsUnknown())
                _size = CountVInts(_reader.Copy());
            return _size;
        }

        private bool SizeIsUnknown()
        {
            return _size == int.MinValue;
        }

        private int CountVInts(ByteArrayReader myReader)
        {
            int counter = 0;
            while (myReader.ReadVInt() >= 0)
                counter++;
            return counter;
        }

    }

}
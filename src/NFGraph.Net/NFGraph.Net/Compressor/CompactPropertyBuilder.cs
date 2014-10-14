using System;
using NFGraph.Net.Util;

namespace NFGraph.Net.Compressor
{
    public class CompactPropertyBuilder
    {

        private readonly ByteArrayBuffer _buf;

        public CompactPropertyBuilder(ByteArrayBuffer buf)
        {
            _buf = buf;
        }

        public void BuildProperty(OrdinalSet ordinalSet)
        {
            int[] connectedOrdinals = ordinalSet.AsArray();
            Array.Sort(connectedOrdinals);

            int previousOrdinal = 0;

            for (int i = 0; i < connectedOrdinals.Length; i++)
            {
                _buf.WriteVInt(connectedOrdinals[i] - previousOrdinal);
                previousOrdinal = connectedOrdinals[i];
            }
        }

    }

}
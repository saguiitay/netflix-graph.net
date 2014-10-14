using System;
using System.IO;
using NFGraph.Net.Compressed;
using NFGraph.Net.Util;

namespace NFGraph.Net.Serializer
{
    public class NFCompressedGraphPointersSerializer
    {

        private readonly INFCompressedGraphPointers _pointers;
        private readonly long _dataLength;

        public NFCompressedGraphPointersSerializer(INFCompressedGraphPointers pointers, long dataLength)
        {
            _pointers = pointers;
            _dataLength = dataLength;
        }

        public void SerializePointers(BinaryWriter dos)
        {
            var pointersAsMap = _pointers.AsMap();

            int numNodeTypes = pointersAsMap.Count;
            if (_dataLength > 0xFFFFFFFFL)
                numNodeTypes |= int.MinValue;

            // In order to maintain backwards compatibility of produced artifacts,
            // if more than 32 bits is required to represent the pointers, then flag
            // the sign bit in the serialized number of node types.
            dos.Write(numNodeTypes);

            foreach (var entry in pointersAsMap)
            {
                dos.Write(entry.Key);
                serializePointerArray(dos, entry.Value);
            }
        }

        private void serializePointerArray(BinaryWriter dos, long[] pointers)
        {
            var buf = new ByteArrayBuffer();

            long currentPointer = 0;

            for (int i = 0; i < pointers.Length; i++)
            {
                if (pointers[i] == -1)
                {
                    buf.WriteVInt(-1);
                }
                else
                {
                    buf.WriteVInt((int) (pointers[i] - currentPointer));
                    currentPointer = pointers[i];
                }
            }

            dos.Write(pointers.Length);
            dos.Write((int) buf.Length());

            buf.CopyTo(dos);
        }

    }

}
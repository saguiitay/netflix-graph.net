using System;
using System.IO;
using NFGraph.Net.Compressed;
using NFGraph.Net.Util;

namespace NFGraph.Net.Serializer
{
    public class NFCompressedGraphPointersDeserializer
    {
        public INFCompressedGraphPointers DeserializePointers(BinaryReader dis)
        {
            int numTypes = dis.ReadInt32();

            // Backwards compatibility:  The representation of the pointers is encoded as
            // In order to maintain backwards compatibility of produced artifacts,
            // if more than 32 bits is required to represent the pointers, then flag
            // the sign bit in the serialized number of node types.
            if ((numTypes & int.MinValue) != 0)
            {
                numTypes &= int.MaxValue;
                return DeserializeLongPointers(dis, numTypes & int.MaxValue);
            }

            return DeserializeIntPointers(dis, numTypes);
        }

        private NFCompressedGraphLongPointers DeserializeLongPointers(BinaryReader dis, int numTypes)
        {
            var longPointers = new NFCompressedGraphLongPointers();

            for (int i = 0; i < numTypes; i++)
            {
                String nodeType = dis.ReadString();
                longPointers.AddPointers(nodeType, DeserializeLongPointerArray(dis));
            }

            return longPointers;
        }

        private long[] DeserializeLongPointerArray(BinaryReader dis)
        {
            int numNodes = dis.ReadInt32();
            int numBytes = dis.ReadInt32();

            byte[] data = dis.ReadBytes(numBytes);
            var pointers = new long[numNodes];

            var reader = new ByteArrayReader(new SimpleByteArray(data), 0);

            long currentPointer = 0;

            for (int i = 0; i < numNodes; i++)
            {
                int vInt = reader.ReadVInt();
                if (vInt == -1)
                {
                    pointers[i] = -1;
                }
                else
                {
                    currentPointer += vInt;
                    pointers[i] = currentPointer;
                }
            }

            return pointers;
        }

        private NFCompressedGraphIntPointers DeserializeIntPointers(BinaryReader dis, int numTypes)
        {
            var pointers = new NFCompressedGraphIntPointers();

            for (int i = 0; i < numTypes; i++)
            {
                String nodeType = dis.ReadString();
                pointers.AddPointers(nodeType, DeserializeIntPointerArray(dis));
            }

            return pointers;
        }

        private int[] DeserializeIntPointerArray(BinaryReader dis)
        {
            int numNodes = dis.ReadInt32();
            int numBytes = dis.ReadInt32();

            byte[] data = dis.ReadBytes(numBytes);
            var pointers = new int[numNodes];

            var reader = new ByteArrayReader(new SimpleByteArray(data), 0);

            long currentPointer = 0;

            for (int i = 0; i < numNodes; i++)
            {
                int vInt = reader.ReadVInt();
                if (vInt == -1)
                {
                    pointers[i] = -1;
                }
                else
                {
                    currentPointer += vInt;
                    pointers[i] = (int) currentPointer;
                }
            }

            return pointers;
        }



    }

}
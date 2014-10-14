using System;
using System.IO;
using NFGraph.Net.Compressed;
using NFGraph.Net.Compressor;
using NFGraph.Net.Spec;
using NFGraph.Net.Util;

namespace NFGraph.Net.Serializer
{
    public class NFCompressedGraphDeserializer
    {

        private readonly NFCompressedGraphPointersDeserializer _pointersDeserializer = new NFCompressedGraphPointersDeserializer();

        public NFCompressedGraph Deserialize(Stream input)
        {
            var dis = new BinaryReader(input);

            NFGraphSpec spec = DeserializeSpec(dis);
            NFGraphModelHolder models = DeserializeModels(dis);
            INFCompressedGraphPointers pointers = _pointersDeserializer.DeserializePointers(dis);
            long dataLength = DeserializeDataLength(dis);
            IByteData data = DeserializeData(dis, dataLength);

            return new NFCompressedGraph(spec, models, data, dataLength, pointers);
        }



        private NFGraphSpec DeserializeSpec(BinaryReader dis)
        {
            int numNodes = dis.ReadInt32();
            var nodeSpecs = new NFNodeSpec[numNodes];

            for (int i = 0; i < numNodes; i++)
            {
                String nodeTypeName = dis.ReadString();
                int numProperties = dis.ReadInt32();
                var propertySpecs = new NFPropertySpec[numProperties];

                for (int j = 0; j < numProperties; j++)
                {
                    String propertyName = dis.ReadString();
                    String toNodeType = dis.ReadString();
                    bool isGlobal = dis.ReadBoolean();
                    bool isMultiple = dis.ReadBoolean();
                    bool isHashed = dis.ReadBoolean();

                    propertySpecs[j] = new NFPropertySpec(propertyName, toNodeType, isGlobal, isMultiple, isHashed);
                }

                nodeSpecs[i] = new NFNodeSpec(nodeTypeName, propertySpecs);
            }

            return new NFGraphSpec(nodeSpecs);
        }

        private NFGraphModelHolder DeserializeModels(BinaryReader dis)
        {
            int numModels = dis.ReadInt32();
            var modelHolder = new NFGraphModelHolder();

            for (int i = 0; i < numModels; i++)
            {
                modelHolder.GetModelIndex(dis.ReadString());
            }

            return modelHolder;
        }

        /// Backwards compatibility:  If the data length is greater than Integer.MAX_VALUE, then
        /// -1 is serialized as an int before a long containing the actual length.
        private long DeserializeDataLength(BinaryReader dis)
        {
            int dataLength = dis.ReadInt32();
            if (dataLength == -1)
            {
                return dis.ReadInt64();
            }
            return dataLength;
        }

        private IByteData DeserializeData(BinaryReader dis, long dataLength)
        {
            if (dataLength >= 0x20000000)
            {
                var data = new SegmentedByteArray(14);
                data.ReadFrom(dis, dataLength);
                return data;
            }
            else
            {
                byte[] data = dis.ReadBytes((int) dataLength);
                return new SimpleByteArray(data);
            }
        }
    }
}
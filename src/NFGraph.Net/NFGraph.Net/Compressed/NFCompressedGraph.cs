using System;
using System.IO;
using NFGraph.Net.Serializer;
using NFGraph.Net.Spec;
using NFGraph.Net.Util;

namespace NFGraph.Net.Compressed
{
    public class NFCompressedGraph : NFGraph
    {

        private readonly INFCompressedGraphPointers _pointers;
        private readonly IByteData _data;
        private readonly long _dataLength;

        public NFCompressedGraph(NFGraphSpec spec, NFGraphModelHolder modelHolder, IByteData data, long dataLength, INFCompressedGraphPointers pointers)
            : base(spec, modelHolder)
        {
            _data = data;
            _dataLength = dataLength;
            _pointers = pointers;
        }

        protected override int GetConnection(int connectionModelIndex, String nodeType, int ordinal, String propertyName)
        {
            ByteArrayReader reader = Reader(nodeType, ordinal);

            if (reader != null)
            {
                NFPropertySpec propertySpec = PointReaderAtProperty(reader, nodeType, propertyName, connectionModelIndex);

                if (propertySpec != null)
                {
                    if (propertySpec.IsSingle)
                        return reader.ReadVInt();

                    int firstOrdinal = Iterator(nodeType, reader, propertySpec).NextOrdinal();
                    if (firstOrdinal != Consts.NO_MORE_ORDINALS)
                        return firstOrdinal;
                }
            }

            return -1;
        }

        protected override OrdinalSet GetConnectionSet(int connectionModelIndex, String nodeType, int ordinal, String propertyName)
        {
            ByteArrayReader reader = Reader(nodeType, ordinal);

            if (reader != null)
            {
                NFPropertySpec propertySpec = PointReaderAtProperty(reader, nodeType, propertyName, connectionModelIndex);

                if (propertySpec != null)
                {
                    return Set(nodeType, reader, propertySpec);
                }
            }

            return Consts.EmptySet;
        }

        protected override IOrdinalIterator GetConnectionIterator(int connectionModelIndex, String nodeType, int ordinal, String propertyName)
        {
            ByteArrayReader reader = Reader(nodeType, ordinal);

            if (reader != null)
            {
                NFPropertySpec propertySpec = PointReaderAtProperty(reader, nodeType, propertyName, connectionModelIndex);

                if (propertySpec != null)
                {
                    return Iterator(nodeType, reader, propertySpec);
                }
            }

            return Consts.EmptyIterator;
        }

        private INFCompressedGraphPointers GetPointers()
        {
            return _pointers;
        }

        private OrdinalSet Set(String nodeType, ByteArrayReader reader, NFPropertySpec propertySpec)
        {
            if (propertySpec.IsSingle)
                return new SingleOrdinalSet(reader.ReadVInt());

            int size = reader.ReadVInt();

            if (size == -1)
            {
                int numBits = _pointers.NumPointers(propertySpec.ToNodeType);
                int numBytes = ((numBits - 1)/8) + 1;
                reader.SetRemainingBytes(numBytes);
                return new BitSetOrdinalSet(reader);
            }

            if (size == 0)
                return Consts.EmptySet;

            if (propertySpec.IsHashed)
            {
                reader.SetRemainingBytes(1 << (size - 1));
                return new HashSetOrdinalSet(reader);
            }

            reader.SetRemainingBytes(size);
            return new CompactOrdinalSet(reader);
        }

        private IOrdinalIterator Iterator(String nodeType, ByteArrayReader reader, NFPropertySpec propertySpec)
        {
            if (propertySpec.IsSingle)
                return new SingleOrdinalIterator(reader.ReadVInt());

            int size = reader.ReadVInt();

            if (size == -1)
            {
                int numBits = _pointers.NumPointers(propertySpec.ToNodeType);
                int numBytes = ((numBits - 1)/8) + 1;
                reader.SetRemainingBytes(numBytes);
                return new BitSetOrdinalIterator(reader);
            }

            if (size == 0)
                return Consts.EmptyIterator;

            if (propertySpec.IsHashed)
            {
                reader.SetRemainingBytes(1 << size);
                return new HashSetOrdinalIterator(reader);
            }

            reader.SetRemainingBytes(size);
            return new CompactOrdinalIterator(reader);
        }

        private ByteArrayReader Reader(String nodeType, int ordinal)
        {
            long pointer = _pointers.GetPointer(nodeType, ordinal);

            if (pointer == -1)
                return null;

            return new ByteArrayReader(_data, pointer);
        }


        private NFPropertySpec PointReaderAtProperty(ByteArrayReader reader, String nodeType, String propertyName, int connectionModelIndex)
        {
            NFNodeSpec nodeSpec = GraphSpec.GetNodeSpec(nodeType);

            foreach (NFPropertySpec propertySpec in nodeSpec.PropertySpecs)
            {
                if (propertySpec.Name.Equals(propertyName))
                {
                    if (propertySpec.IsConnectionModelSpecific)
                        PositionForModel(reader, connectionModelIndex, propertySpec);
                    return propertySpec;
                }
                SkipProperty(reader, propertySpec);
            }

            throw new Exception("Property " + propertyName + " is undefined for node type " + nodeType);
        }

        private void PositionForModel(ByteArrayReader reader, int connectionModelIndex, NFPropertySpec propertySpec)
        {
            reader.SetRemainingBytes(reader.ReadVInt());

            for (int i = 0; i < connectionModelIndex; i++)
            {
                SkipSingleProperty(reader, propertySpec);
            }
        }

        private void SkipProperty(ByteArrayReader reader, NFPropertySpec propertySpec)
        {
            if (propertySpec.IsConnectionModelSpecific)
            {
                int size = reader.ReadVInt();
                reader.Skip(size);
            }
            else
            {
                SkipSingleProperty(reader, propertySpec);
            }
        }

        private void SkipSingleProperty(ByteArrayReader reader, NFPropertySpec propertySpec)
        {
            if (propertySpec.IsSingle)
            {
                reader.ReadVInt();
                return;
            }

            int size = reader.ReadVInt();

            if (size == 0)
                return;

            if (size == -1)
            {
                int numBits = _pointers.NumPointers(propertySpec.ToNodeType);
                int numBytes = ((numBits - 1)/8) + 1;
                reader.Skip(numBytes);
                return;
            }

            if (propertySpec.IsHashed)
            {
                reader.Skip(1 << (size - 1));
                return;
            }

            reader.Skip(size);
        }

        public void WriteTo(Stream os)
        {
            var serializer = new NFCompressedGraphSerializer(GraphSpec, ModelHolder, _pointers, _data, _dataLength);
            serializer.SerializeTo(os);
        }

        public static NFCompressedGraph ReadFrom(Stream input)
        {
            var deserializer = new NFCompressedGraphDeserializer();
            return deserializer.Deserialize(input);
        }

    }
}
using System;
using System.Collections.Generic;
using NFGraph.Net.Build;
using NFGraph.Net.Compressed;
using NFGraph.Net.Spec;
using NFGraph.Net.Util;

namespace NFGraph.Net.Compressor
{
    public class NFCompressedGraphBuilder
    {

        private readonly NFGraphSpec _graphSpec;
        private readonly NFBuildGraphNodeCache _buildGraphNodeCache;
        private readonly NFGraphModelHolder _modelHolder;

        private readonly ByteArrayBuffer _graphBuffer;
        private readonly ByteArrayBuffer _modelBuffer;
        private readonly ByteArrayBuffer _fieldBuffer;

        private readonly CompactPropertyBuilder _compactPropertyBuilder;
        private readonly HashedPropertyBuilder _hashedPropertyBuilder;
        private readonly BitSetPropertyBuilder _bitSetPropertyBuilder;

        private readonly NFCompressedGraphLongPointers _compressedGraphPointers;

        public NFCompressedGraphBuilder(NFGraphSpec graphSpec, NFBuildGraphNodeCache buildGraphNodeCache, NFGraphModelHolder modelHolder)
        {
            _graphSpec = graphSpec;
            _buildGraphNodeCache = buildGraphNodeCache;
            _modelHolder = modelHolder;

            _graphBuffer = new ByteArrayBuffer();
            _modelBuffer = new ByteArrayBuffer();
            _fieldBuffer = new ByteArrayBuffer();

            _compactPropertyBuilder = new CompactPropertyBuilder(_fieldBuffer);
            _hashedPropertyBuilder = new HashedPropertyBuilder(_fieldBuffer);
            _bitSetPropertyBuilder = new BitSetPropertyBuilder(_fieldBuffer);

            _compressedGraphPointers = new NFCompressedGraphLongPointers();
        }

        public NFCompressedGraph BuildGraph()
        {
            foreach (String nodeType in _graphSpec.GetNodeTypes())
            {
                List<NFBuildGraphNode> nodeOrdinals = _buildGraphNodeCache.GetNodes(nodeType);
                AddNodeType(nodeType, nodeOrdinals);
            }

            return new NFCompressedGraph(_graphSpec, _modelHolder, _graphBuffer.GetData(), _graphBuffer.Length(), _compressedGraphPointers);
        }

        private void AddNodeType(String nodeType, List<NFBuildGraphNode> nodes)
        {
            NFNodeSpec nodeSpec = _graphSpec.GetNodeSpec(nodeType);
            var ordinalPointers = new long[nodes.Count];

            for (int i = 0; i < nodes.Count; i++)
            {
                NFBuildGraphNode node = nodes[i];
                if (node != null)
                {
                    ordinalPointers[i] = _graphBuffer.Length();
                    SerializeNode(node, nodeSpec);
                }
                else
                {
                    ordinalPointers[i] = -1;
                }
            }

            _compressedGraphPointers.AddPointers(nodeType, ordinalPointers);
        }

        private void SerializeNode(NFBuildGraphNode node, NFNodeSpec nodeSpec)
        {
            foreach (NFPropertySpec propertySpec in nodeSpec.PropertySpecs)
            {
                SerializeProperty(node, propertySpec);
            }
        }

        private void SerializeProperty(NFBuildGraphNode node, NFPropertySpec propertySpec)
        {
            if (propertySpec.IsConnectionModelSpecific)
            {
                for (int i = 0; i < _modelHolder.Size(); i++)
                {
                    SerializeProperty(node, propertySpec, i, _modelBuffer);
                }
                CopyBuffer(_modelBuffer, _graphBuffer);
            }
            else
            {
                SerializeProperty(node, propertySpec, 0, _graphBuffer);
            }
        }

        private void SerializeProperty(NFBuildGraphNode node, NFPropertySpec propertySpec, int connectionModelIndex, ByteArrayBuffer toBuffer)
        {
            if (propertySpec.IsMultiple)
            {
                SerializeMultipleProperty(node, propertySpec, connectionModelIndex, toBuffer);
            }
            else
            {
                int connection = node.GetConnection(connectionModelIndex, propertySpec);
                if (connection == -1)
                {
                    toBuffer.WriteByte(0x80);
                }
                else
                {
                    toBuffer.WriteVInt(connection);
                }
            }
        }

        private void SerializeMultipleProperty(NFBuildGraphNode node, NFPropertySpec propertySpec, int connectionModelIndex, ByteArrayBuffer toBuffer)
        {
            OrdinalSet connections = node.GetConnectionSet(connectionModelIndex, propertySpec);

            int numBitsInBitSet = _buildGraphNodeCache.NumNodes(propertySpec.ToNodeType);
            int bitSetSize = ((numBitsInBitSet - 1)/8) + 1;

            if (connections.Size() < bitSetSize)
            {
                if (propertySpec.IsHashed)
                {
                    _hashedPropertyBuilder.BuildProperty(connections);
                    if (_fieldBuffer.Length() < bitSetSize)
                    {
                        int log2BytesUsed = 32 - IntegerUtils.NumberOfLeadingZeros((int) _fieldBuffer.Length());
                        toBuffer.WriteByte((byte) log2BytesUsed);
                        toBuffer.Write(_fieldBuffer);
                        _fieldBuffer.Reset();
                        return;
                    }
                }
                else
                {
                    _compactPropertyBuilder.BuildProperty(connections);
                    if (_fieldBuffer.Length() < bitSetSize)
                    {
                        toBuffer.WriteVInt((int) _fieldBuffer.Length());
                        toBuffer.Write(_fieldBuffer);
                        _fieldBuffer.Reset();
                        return;
                    }
                }

                _fieldBuffer.Reset();
            }

            _bitSetPropertyBuilder.BuildProperty(connections, numBitsInBitSet);
            toBuffer.WriteByte(0x80);
            toBuffer.Write(_fieldBuffer);
            _fieldBuffer.Reset();
        }

        private static void CopyBuffer(ByteArrayBuffer from, ByteArrayBuffer to)
        {
            to.WriteVInt((int) from.Length());
            to.Write(from);
            from.Reset();
        }

    }
}
using System;
using System.Collections.Generic;

namespace NFGraph.Net.Compressed
{
    public class NFCompressedGraphLongPointers : INFCompressedGraphPointers
    {

        private readonly IDictionary<String, long[]> _pointersByOrdinal;

        public NFCompressedGraphLongPointers()
        {
            _pointersByOrdinal = new Dictionary<String, long[]>();
        }

        /**
     * @return the offset into the {@link NFCompressedGraph}'s byte array for the node identified by the given type and ordinal.
     */

        public long GetPointer(String nodeType, int ordinal)
        {
            long[] pointers;

            if (!_pointersByOrdinal.TryGetValue(nodeType, out pointers) || pointers == null)
                throw new Exception("Undefined node type: " + nodeType);
            if (ordinal < pointers.Length)
                return pointers[ordinal];
            return -1;
        }

        public void AddPointers(String nodeType, long[] pointers)
        {
            _pointersByOrdinal[nodeType] = pointers;
        }

        public int NumPointers(String nodeType)
        {
            return _pointersByOrdinal[nodeType].Length;
        }

        public IDictionary<String, long[]> AsMap()
        {
            return _pointersByOrdinal;
        }

    }
}
using System;
using System.Collections.Generic;

namespace NFGraph.Net.Compressed
{
    public class NFCompressedGraphIntPointers : INFCompressedGraphPointers
    {

        private readonly IDictionary<String, int[]> _pointersByOrdinal;

        public NFCompressedGraphIntPointers()
        {
            _pointersByOrdinal = new Dictionary<String, int[]>();
        }

        /**
     * @return the offset into the {@link NFCompressedGraph}'s byte array for the node identified by the given type and ordinal.
     */

        public long GetPointer(String nodeType, int ordinal)
        {
            int[] pointers;
            if (!_pointersByOrdinal.TryGetValue(nodeType, out pointers) || pointers == null)
                throw new Exception("Undefined node type: " + nodeType);
            if (ordinal < pointers.Length)
            {
                if (pointers[ordinal] == -1)
                    return -1;
                return 0xFFFFFFFFL & pointers[ordinal];
            }
            return -1;
        }

        public void AddPointers(String nodeType, int[] pointers)
        {
            _pointersByOrdinal[nodeType] = pointers;
        }

        public int NumPointers(String nodeType)
        {
            return _pointersByOrdinal[nodeType].Length;
        }

        public IDictionary<String, long[]> AsMap()
        {
            IDictionary<String, long[]> map = new Dictionary<String, long[]>();

            foreach (var entry in _pointersByOrdinal)
            {
                map[entry.Key] = ToLongArray(entry.Value);
            }

            return map;
        }

        private long[] ToLongArray(int[] arr)
        {
            var l = new long[arr.Length];

            for (int i = 0; i < arr.Length; i++)
            {
                l[i] = arr[i];
            }

            return l;
        }

    }

}
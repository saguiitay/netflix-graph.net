using System;
using System.Collections.Generic;

namespace NFGraph.Net.Compressed
{
    public interface INFCompressedGraphPointers
    {

        /**
         * @return the offset into the {@link NFCompressedGraph}'s byte array for the node identified by the given type and ordinal.
         */
        long GetPointer(String nodeType, int ordinal);

        int NumPointers(String nodeType);

        IDictionary<String, long[]> AsMap();

    }

}
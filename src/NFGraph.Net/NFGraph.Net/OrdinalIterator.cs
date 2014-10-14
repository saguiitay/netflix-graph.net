namespace NFGraph.Net
{
    public interface IOrdinalIterator {
    

        /**
         * @return the next ordinal in this set.
         */
        int NextOrdinal();
    
        /**
         * Rewinds this <code>OrdinalIterator</code> to the beginning of the set.
         */
        void Reset();
    
        /**
         * Obtain a copy of this <copy>OrdinalIterator</code>.  The returned <code>OrdinalIterator</code> will be reset to the beginning of the set.
         */
        IOrdinalIterator Copy();
    
        /**
         * @return <code>true</code> if the ordinals returned from this set are guaranteed to be in ascending order.  Returns <code>false</code> otherwise.  
         */
        bool IsOrdered();
    }

    
}
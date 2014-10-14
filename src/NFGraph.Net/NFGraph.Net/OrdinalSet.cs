namespace NFGraph.Net
{
    public abstract class OrdinalSet
    {

        /**
         * Returns <code>true</code> when the specified value is contained in this set.  Depending on the implementation,
         * this operation will have one of two performance characteristics:<p/>
         * 
         * <code>O(1)</code> for {@link HashSetOrdinalSet} and {@link BitSetOrdinalSet}<br/>
         * <code>O(n)</code> for {@link CompactOrdinalSet} and {@link NFBuildGraphOrdinalSet}
         */
        public abstract bool Contains(int value);

        /**
         * Returns <code>true</code> when all specified values are contained in this set.  Depending on the implementation,
         * this operation will have one of two performance characteristics:<p/>
         * 
         * <code>O(m)</code> for {@link HashSetOrdinalSet} and {@link BitSetOrdinalSet}, where <code>m</code> is the number of specified elements.<br/>
         * <code>O(n)</code> for {@link CompactOrdinalSet}, where <code>n</code> is the number of elements in the set.<br/>
         * <code>O(n * m)</code> for {@link NFBuildGraphOrdinalSet}.
         */

        public virtual bool ContainsAll(params int[] values)
        {
            foreach (int value in values)
            {
                if (!Contains(value))
                    return false;
            }
            return true;
        }

        /**
	     * Returns an array containing all elements in the set.
	     */

        public virtual int[] AsArray()
        {
            var arr = new int[Size()];
            IOrdinalIterator iter = Iterator();

            int ordinal = iter.NextOrdinal();
            int i = 0;

            while (ordinal != int.MaxValue)
            {
                arr[i++] = ordinal;
                ordinal = iter.NextOrdinal();
            }

            return arr;
        }

        /**
	     * @return an {@link OrdinalIterator} over this set.
	     */
        public abstract IOrdinalIterator Iterator();

        /**
	     * @return the number of ordinals in this set.
	     */
        public abstract int Size();
    }


}
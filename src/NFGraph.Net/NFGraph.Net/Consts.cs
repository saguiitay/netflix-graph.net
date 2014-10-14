namespace NFGraph.Net
{
    public static class Consts
    {
        /**
        * This value will be returned from <code>nextOrdinal()</code> after the iteration is completed.
        */
        public static readonly int NO_MORE_ORDINALS = int.MaxValue;

        public static readonly int[] EMPTY_ORDINAL_ARRAY = new int[0];

        ///**
        // * An iterator which always return <code>OrdinalIterator.NO_MORE_ORDINALS</code>
        // */
        public static IOrdinalIterator EmptyIterator = new EMPTY_ITERATOR();

        ///**
        // * An empty <code>OrdinalSet</code>.
        // */
        public static OrdinalSet EmptySet = new EMPTY_SET();



        private class EMPTY_ITERATOR : IOrdinalIterator
        {

            #region Implementation of IOrdinalIterator

            public int NextOrdinal()
            {
                return Consts.NO_MORE_ORDINALS;
            }

            public void Reset()
            {
            }

            public IOrdinalIterator Copy()
            {
                return this;
            }

            public bool IsOrdered()
            {
                return true;
            }

            #endregion
        }

        private sealed class EMPTY_SET : OrdinalSet
        {
            public override bool Contains(int value) { return false; }

            public override int[] AsArray() { return Consts.EMPTY_ORDINAL_ARRAY; }

            public override IOrdinalIterator Iterator() { return Consts.EmptyIterator; }

            public override int Size() { return 0; }
        };
        
    }
}
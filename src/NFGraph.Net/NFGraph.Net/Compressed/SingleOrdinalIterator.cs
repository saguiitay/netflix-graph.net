namespace NFGraph.Net.Compressed
{
    public class SingleOrdinalIterator : IOrdinalIterator
    {

        private readonly int _ordinal;
        private bool _returned;

        public SingleOrdinalIterator(int ordinal)
        {
            _ordinal = ordinal;
        }

        public int NextOrdinal()
        {
            if (_returned)
                return Consts.NO_MORE_ORDINALS;

            _returned = true;
            return _ordinal;
        }

        public void Reset()
        {
            _returned = false;
        }

        public IOrdinalIterator Copy()
        {
            return new SingleOrdinalIterator(_ordinal);
        }

        public bool IsOrdered()
        {
            return true;
        }

    }

}
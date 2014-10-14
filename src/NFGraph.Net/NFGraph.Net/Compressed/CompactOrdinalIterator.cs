using NFGraph.Net.Util;

namespace NFGraph.Net.Compressed
{
    public class CompactOrdinalIterator : IOrdinalIterator
    {

        private readonly ByteArrayReader _arrayReader;
        private int _currentOrdinal;

        public CompactOrdinalIterator(ByteArrayReader arrayReader)
        {
            _arrayReader = arrayReader;
        }

        public int NextOrdinal()
        {
            int delta = _arrayReader.ReadVInt();
            if (delta == -1)
                return Consts.NO_MORE_ORDINALS;
            _currentOrdinal += delta;
            return _currentOrdinal;
        }

        public void Reset()
        {
            _arrayReader.Reset();
            _currentOrdinal = 0;
        }

        public IOrdinalIterator Copy()
        {
            return new CompactOrdinalIterator(_arrayReader.Copy());
        }

        public bool IsOrdered()
        {
            return true;
        }

    }
}
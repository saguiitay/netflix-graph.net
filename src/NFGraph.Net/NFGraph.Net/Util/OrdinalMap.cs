using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NFGraph.Net.Build;

namespace NFGraph.Net.Util
{
    public class OrdinalMap<T> : IEnumerable<T>
        where T : class
    {

        private int[] _hashedOrdinalArray;
        private T[] _objectsByOrdinal;

        private int _size;

        public OrdinalMap()
            : this(10)
        {
        }

        public OrdinalMap(int expectedSize)
        {
            int mapArraySize = 1 << (32 - IntegerUtils.NumberOfLeadingZeros(expectedSize*4/3));
            int ordinalArraySize = mapArraySize*3/4;

            _hashedOrdinalArray = NewHashedOrdinalArray(mapArraySize);
            _objectsByOrdinal = new T[ordinalArraySize];
        }

        /**
     * Add an object into this <code>OrdinalMap</code>.  If the same object (or an {@link Object#equals(Object)} object) is
     * already in the map, then no changes will be made.
     * 
     * @return the ordinal of <code>obj</code>
     */

        public int Add(T obj)
        {
            int ordinal = Get(obj);
            if (ordinal != -1)
                return ordinal;

            if (_size == _objectsByOrdinal.Length)
                GrowCapacity();

            _objectsByOrdinal[_size] = obj;
            HashOrdinalIntoArray(_size, _hashedOrdinalArray);

            return _size++;
        }

        /**
     * @return the ordinal of an object previously added to the map.  If the object has not been added to the map, returns -1 instead. 
     */

        public int Get(T obj)
        {
            int hash = Mixer.HashInt(obj.GetHashCode());

            int bucket = hash%_hashedOrdinalArray.Length;
            int ordinal = _hashedOrdinalArray[bucket];

            while (ordinal != -1)
            {
                if (_objectsByOrdinal[ordinal].Equals(obj))
                    return ordinal;

                bucket = (bucket + 1)%_hashedOrdinalArray.Length;
                ordinal = _hashedOrdinalArray[bucket];
            }

            return -1;
        }

        /**
     * @return the object for a given ordinal.  If the ordinal does not yet exist, returns null.
     */

        public T Get(int ordinal)
        {
            if (ordinal >= _size)
                return null;
            return _objectsByOrdinal[ordinal];
        }

        /**
     * @return the number of objects in this map.
     */

        public int Size()
        {
            return _size;
        }

        private void GrowCapacity()
        {
            int[] newHashedOrdinalArray = NewHashedOrdinalArray(_hashedOrdinalArray.Length*2);

            for (int i = 0; i < _objectsByOrdinal.Length; i++)
            {
                HashOrdinalIntoArray(i, newHashedOrdinalArray);
            }

            _objectsByOrdinal = _objectsByOrdinal.Copy(_objectsByOrdinal.Length*2);
            _hashedOrdinalArray = newHashedOrdinalArray;
        }

        private void HashOrdinalIntoArray(int ordinal, int[] hashedOrdinalArray)
        {
            int hash = Mixer.HashInt(_objectsByOrdinal[ordinal].GetHashCode());

            int bucket = hash%hashedOrdinalArray.Length;

            while (hashedOrdinalArray[bucket] != -1)
            {
                bucket = (bucket + 1)%hashedOrdinalArray.Length;
            }

            hashedOrdinalArray[bucket] = ordinal;
        }

        private int[] NewHashedOrdinalArray(int length)
        {
            var arr = new int[length];
            arr.Fill(-1);
            return arr;
        }


        #region Implementation of IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            return _objectsByOrdinal.Take(_size).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
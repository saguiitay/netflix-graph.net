using System;

namespace NFGraph.Net.Spec
{
    public class NFPropertySpec
    {

        /**
         * A property spec instantiated with this flag will not be separable into connection models.
        */
        public static readonly int GLOBAL = 0x00;

        /**
        * A property spec instantiated with this flag will be separable into connection models.
        */
        public static readonly int MODEL_SPECIFIC = 0x01;

        /**
        * A property spec instantiated with this flag will be allowed multiple connections.
        */
        public static readonly int MULTIPLE = 0x00;

        /**
        * A property spec instantiated with this flag will be allowed only a single connection.
        */
        public static readonly int SINGLE = 0x02;

        /**
        * A {@link #MULTIPLE} property instantiated with this flag will be represented as a {@link BitSetOrdinalSet} in an {@link NFCompressedGraph}.
        * 
        * @see BitSetOrdinalSet
        */
        public static readonly int HASH = 0x04;

        /**
        * A {@link #MULTIPLE} property instantiated with this flag will be represented as a {@link CompactOrdinalSet} in an {@link NFCompressedGraph}.
        * 
        * @see CompactOrdinalSet
        */
        public static readonly int COMPACT = 0x00;

        private readonly bool _isGlobal;
        private readonly bool _isMultiple;
        private readonly bool _isHashed;

        private readonly String _name;
        private readonly String _toNodeType;

        private int _propertyIndex;

        /**
        * The recommended constructor.
        * 
        * @param name the name of the property.
        * @param toNodeType the node type to which this property connects
        * @param flags a bitwise-or of the various flags defined as constants in {@link NFPropertySpec}.<br/>For example, a global, multiple, compact property would take the value <code>NFPropertySpec.GLOBAL | NFPropertySpec.MULTIPLE | NFPropertySpec.COMPACT</code> 
        * 
        */

        public NFPropertySpec(String name, String toNodeType, int flags)
        {
            _name = name;
            _toNodeType = toNodeType;
            _isGlobal = (flags & MODEL_SPECIFIC) == 0;
            _isMultiple = (flags & SINGLE) == 0;
            _isHashed = (flags & HASH) != 0;
        }

        public NFPropertySpec(String name, String toNodeType, bool isGlobal, bool isMultiple, bool isHashed)
        {
            _name = name;
            _toNodeType = toNodeType;
            _isGlobal = isGlobal;
            _isMultiple = isMultiple;
            _isHashed = isHashed;
        }

        public bool IsConnectionModelSpecific
        {
            get { return !_isGlobal; }
        }

        public bool IsGlobal
        {
            get { return _isGlobal; }
        }

        public bool IsMultiple
        {
            get { return _isMultiple; }
        }

        public bool IsSingle
        {
            get { return  !_isMultiple; }
        }

        public bool IsHashed
        {
            get { return _isHashed; }
        }

        public bool IsCompact
        {
            get { return  !_isHashed; }
        }

        public String Name
        {
            get { return _name; }
        }

        public String ToNodeType
        {
            get { return _toNodeType; }
        }

        public int PropertyIndex
        {
            get { return _propertyIndex; }
            set { _propertyIndex = value; }
        }

    }
}
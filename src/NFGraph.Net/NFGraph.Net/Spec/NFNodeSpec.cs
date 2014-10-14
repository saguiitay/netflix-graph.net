using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NFGraph.Net.Spec
{
    public class NFNodeSpec : IEnumerable<NFPropertySpec> {

        private readonly String _nodeTypeName;
        private readonly NFPropertySpec[] _propertySpecs;
    
        private readonly int _numSingleProperties;
        private readonly int _numMultipleProperties;

        /**
     * The constructor for an <code>NFNodeSpec</code>.
     * 
     * @param nodeTypeName the name of the node type
     * @param propertySpecs a complete listing of the properties available for this node type.
     */
        public NFNodeSpec(String nodeTypeName, params NFPropertySpec[] propertySpecs) {
            _nodeTypeName = nodeTypeName;
            _propertySpecs = propertySpecs;
        
            int numSingleProperties = 0;
            int numMultipleProperties = 0;
        
            foreach (NFPropertySpec propertySpec in propertySpecs) {
                propertySpec.PropertyIndex = (propertySpec.IsSingle ? numSingleProperties++ : numMultipleProperties++);
            }
        
            _numSingleProperties = numSingleProperties;
            _numMultipleProperties = numMultipleProperties;
        }
    
        public String NodeTypeName {
            get { return _nodeTypeName; }
        }

        public NFPropertySpec[] PropertySpecs {
            get { return _propertySpecs; }
        }
    
        public NFPropertySpec GetPropertySpec(String propertyName) {
            foreach (NFPropertySpec spec in _propertySpecs) {
                if(spec.Name.Equals(propertyName))
                    return spec;
            }
            throw new Exception("Property " + propertyName + " is undefined for node type " + _nodeTypeName);
        }
    
        public int NumSingleProperties {
            get { return _numSingleProperties; }
        }
    
        public int NumMultipleProperties {
            get { return _numMultipleProperties; }
        }

        //public override Iterator<NFPropertySpec> iterator() {
        //    return new ArrayIterator<NFPropertySpec>(_propertySpecs);
        //}

        #region Implementation of IEnumerable

        public IEnumerator<NFPropertySpec> GetEnumerator()
        {
            return _propertySpecs.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
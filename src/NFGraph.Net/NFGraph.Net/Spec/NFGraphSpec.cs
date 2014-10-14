using System;
using System.Collections;
using System.Collections.Generic;

namespace NFGraph.Net.Spec
{
    public class NFGraphSpec : IEnumerable<NFNodeSpec>
    {

        private readonly IDictionary<String, NFNodeSpec> _nodeSpecs;

        /**
        * Instantiate a graph specification with no {@link NFNodeSpec}s.
        */
        public NFGraphSpec()
        {
            _nodeSpecs = new Dictionary<String, NFNodeSpec>();
        }

        /**
        * Instantiate a graph specification with the given {@link NFNodeSpec}. 
        */
        public NFGraphSpec(params NFNodeSpec[] nodeTypes)
            : this()
        {
            foreach (NFNodeSpec spec in nodeTypes)
            {
                AddNodeSpec(spec);
            }
        }

        /**
        * @return the {@link NFNodeSpec} for the specified node type.
        */
        public NFNodeSpec GetNodeSpec(String nodeType)
        {
            NFNodeSpec spec;
            if (!_nodeSpecs.TryGetValue(nodeType, out spec) || spec == null)
                throw new Exception("Node spec " + nodeType + " is undefined");

            return spec;
        }

        /**
        * Add a node type to this graph specification. 
        */
        public void AddNodeSpec(NFNodeSpec nodeSpec)
        {
            _nodeSpecs.Add(nodeSpec.NodeTypeName, nodeSpec);
        }

        /**
        * @return the number of node types defined by this graph specification.
        */
        public int Size
        {
            get { return _nodeSpecs.Count; }
        }

        /**
        * @return a {@link List} containing the names of each of the node types.
        */
        public List<String> GetNodeTypes()
        {
            return new List<String>(_nodeSpecs.Keys);
        }

        #region Implementation of IEnumerable

        public IEnumerator<NFNodeSpec> GetEnumerator()
        {
            return _nodeSpecs.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
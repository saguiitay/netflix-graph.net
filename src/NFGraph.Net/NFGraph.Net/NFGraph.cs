using System;
using NFGraph.Net.Spec;

namespace NFGraph.Net
{
    public abstract class NFGraph
    {

        protected readonly NFGraphSpec GraphSpec;
        protected readonly NFGraphModelHolder ModelHolder;


        protected NFGraph(NFGraphSpec graphSpec)
        {
            GraphSpec = graphSpec;
            ModelHolder = new NFGraphModelHolder();
        }

        protected NFGraph(NFGraphSpec graphSpec, NFGraphModelHolder modelHolder)
        {
            GraphSpec = graphSpec;
            ModelHolder = modelHolder;
        }

        /**
         * Retrieve a single connected ordinal, given the type and ordinal of the originating node, and the property by which this node is connected.
         * 
         * @return the connected ordinal, or -1 if there is no such ordinal
         */
        public int GetConnection(String nodeType, int ordinal, String propertyName)
        {
            return GetConnection(0, nodeType, ordinal, propertyName);
        }

        /**
         * Retrieve a single connected ordinal in a given connection model, given the type and ordinal of the originating node, and the property by which this node is connected.
         * 
         * @return the connected ordinal, or -1 if there is no such ordinal
         */
        public int GetConnection(String connectionModel, String nodeType, int ordinal, String propertyName)
        {
            int connectionModelIndex = ModelHolder.GetModelIndex(connectionModel);
            return GetConnection(connectionModelIndex, nodeType, ordinal, propertyName);
        }

        /**
         * Retrieve an {@link OrdinalIterator} over all connected ordinals, given the type and ordinal of the originating node, and the property by which this node is connected.
         * 
         * @return an {@link OrdinalIterator} over all connected ordinals
         */
        public IOrdinalIterator GetConnectionIterator(String nodeType, int ordinal, String propertyName)
        {
            return GetConnectionIterator(0, nodeType, ordinal, propertyName);
        }

        /**
         * Retrieve an {@link OrdinalIterator} over all connected ordinals in a given connection model, given the type and ordinal of the originating node, and the property by which this node is connected.
         * 
         * @return an {@link OrdinalIterator} over all connected ordinals
         */
        public IOrdinalIterator GetConnectionIterator(String connectionModel, String nodeType, int ordinal, String propertyName)
        {
            int connectionModelIndex = ModelHolder.GetModelIndex(connectionModel);
            return GetConnectionIterator(connectionModelIndex, nodeType, ordinal, propertyName);
        }

        /**
         * Retrieve an {@link OrdinalSet} over all connected ordinals, given the type and ordinal of the originating node, and the property by which this node is connected.
         * 
         * @return an {@link OrdinalSet} over all connected ordinals
         */
        public OrdinalSet GetConnectionSet(String nodeType, int ordinal, String propertyName)
        {
            return GetConnectionSet(0, nodeType, ordinal, propertyName);
        }

        /**
         * Retrieve an {@link OrdinalSet} over all connected ordinals in a given connection model, given the type and ordinal of the originating node, and the property by which this node is connected.
         * 
         * @return an {@link OrdinalSet} over all connected ordinals
         */
        public OrdinalSet GetConnectionSet(String connectionModel, String nodeType, int ordinal, String propertyName)
        {
            int connectionModelIndex = ModelHolder.GetModelIndex(connectionModel);
            return GetConnectionSet(connectionModelIndex, nodeType, ordinal, propertyName);
        }

        protected abstract int GetConnection(int connectionModelIndex, String nodeType, int ordinal, String propertyName);

        protected abstract OrdinalSet GetConnectionSet(int connectionModelIndex, String nodeType, int ordinal, String propertyName);

        protected abstract IOrdinalIterator GetConnectionIterator(int connectionModelIndex, String nodeType, int ordinal, String propertyName);

    }
}

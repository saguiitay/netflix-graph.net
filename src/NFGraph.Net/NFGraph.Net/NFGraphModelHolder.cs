using System;
using System.Collections;
using System.Collections.Generic;
using NFGraph.Net.Util;

namespace NFGraph.Net
{
    public class NFGraphModelHolder : IEnumerable<String>
    {

        public static readonly String CONNECTION_MODEL_GLOBAL = "global";

        private readonly OrdinalMap<String> _modelMap;

        public NFGraphModelHolder()
        {
            _modelMap = new OrdinalMap<String> {CONNECTION_MODEL_GLOBAL};
        }

        public int Size()
        {
            return _modelMap.Size();
        }

        public int GetModelIndex(String connectionModel)
        {
            return _modelMap.Add(connectionModel);
        }

        public String GetModel(int modelIndex)
        {
            return _modelMap.Get(modelIndex);
        }

        #region Implementation of IEnumerable

        public IEnumerator<string> GetEnumerator()
        {
            return _modelMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFGraph.Net.Compressed
{
    public class SingleOrdinalSet : OrdinalSet {

    private readonly int _ordinal;
    
    public SingleOrdinalSet(int ordinal) {
        this._ordinal = ordinal;
    }

    public override bool Contains(int value) {
        return _ordinal == value;
    }

    public override int[] AsArray() {
        return new[] { _ordinal };
    }

    public override IOrdinalIterator Iterator() {
        return new SingleOrdinalIterator(_ordinal);
    }

    public override int Size() {
        return 1;
    }
    
}

}

using System.IO;

namespace NFGraph.Net.Util
{
    public interface IByteData {

        void Set(long idx, byte b);

        byte Get(long idx);

        long Length();

        void WriteTo(BinaryWriter os, long length);

    }
}
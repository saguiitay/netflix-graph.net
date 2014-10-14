using System;
using System.IO;
using NFGraph.Net.Compressed;
using NFGraph.Net.Spec;
using NFGraph.Net.Util;

namespace NFGraph.Net.Serializer
{
    public class NFCompressedGraphSerializer {

    private readonly NFGraphSpec _spec;
    private readonly NFGraphModelHolder _modelHolder;
    private readonly NFCompressedGraphPointersSerializer _pointersSerializer;
    private readonly IByteData _data;
    private readonly long _dataLength;

    public NFCompressedGraphSerializer(NFGraphSpec spec, NFGraphModelHolder modelHolder, INFCompressedGraphPointers pointers, IByteData data, long dataLength) {
        _spec = spec;
        _modelHolder = modelHolder;
        _pointersSerializer = new NFCompressedGraphPointersSerializer(pointers, dataLength);
        _data = data;
        _dataLength = dataLength;
    }

    public void SerializeTo(Stream os){
        var dos = new BinaryWriter(os);

        SerializeSpec(dos);
        SerializeModels(dos);
        _pointersSerializer.SerializePointers(dos);
        SerializeData(dos);

        dos.Flush();
    }

    private void SerializeSpec(BinaryWriter dos){
        dos.Write(_spec.Size);

        foreach (NFNodeSpec nodeSpec in _spec) {
            dos.Write(nodeSpec.NodeTypeName);
            dos.Write(nodeSpec.PropertySpecs.Length);

            foreach (NFPropertySpec propertySpec in nodeSpec.PropertySpecs) {
                dos.Write(propertySpec.Name);
                dos.Write(propertySpec.ToNodeType);
                dos.Write(propertySpec.IsGlobal);
                dos.Write(propertySpec.IsMultiple);
                dos.Write(propertySpec.IsHashed);
            }
        }
    }

    private void SerializeModels(BinaryWriter dos){
        dos.Write(_modelHolder.Size());
        foreach(String model in _modelHolder) {
            dos.Write(model);
        }
    }

    private void SerializeData(BinaryWriter dos){
        // In order to maintain backwards compatibility of produced artifacts,
        // if more than Integer.MAX_VALUE bytes are required in the data,
        // first serialize a negative 1 integer, then serialize the number
        // of required bits as a long.
        if(_dataLength > int.MaxValue) {
            dos.Write(-1);
            dos.Write(_dataLength);
        } else {
            dos.Write((int)_dataLength);
        }

        _data.WriteTo(dos, _dataLength);
    }
}
}
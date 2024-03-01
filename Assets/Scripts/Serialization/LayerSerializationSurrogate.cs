using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;

[System.Obsolete()]
public class LayerSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Layer layer = (Layer)obj;

        info.AddValue("layerName", layer.name);
        info.AddValue("visible", layer.visible);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Layer layer = (Layer)obj;

        string layerName = (string)info.GetValue("layerName", typeof(string));
        bool visible = (bool)info.GetValue("visible", typeof(bool));

        obj = layer;
        return obj;
    }
}

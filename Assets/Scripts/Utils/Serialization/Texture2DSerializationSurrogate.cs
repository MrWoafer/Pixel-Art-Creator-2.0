using System.Runtime.Serialization;
using UnityEngine;

namespace PAC.Serialization
{
    [System.Obsolete()]
    public class Texture2DSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Texture2D tex = (Texture2D)obj;

            byte[] byteArray = tex.EncodeToPNG();
            info.AddValue("Length", byteArray.Length);
            for (int i = 0; i < byteArray.Length; i++)
            {
                info.AddValue(i.ToString(), byteArray[i]);
            }
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Texture2D tex = new Texture2D(1, 1);

            byte[] byteArray = new byte[(int)info.GetValue("Length", typeof(int))];
            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] = (byte)info.GetValue(i.ToString(), typeof(byte));
            }

            tex.LoadImage(byteArray);

            obj = tex;
            return obj;
        }
    }
}

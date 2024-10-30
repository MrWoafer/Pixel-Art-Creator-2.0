using PAC.Json;
using UnityEngine;

namespace PAC.Extensions
{
    public static class JsonConverters
    {
        public static readonly JsonConverterList allConverters = new JsonConverterList(new Vector2JsonConverter(), new Vector3JsonConverter(), new ColorJsonConverter());

        public class Vector2JsonConverter : IJsonConverter<Vector2, JsonList>
        {
            public override JsonData ToJson(Vector2 obj)
            {
                return new JsonList { new JsonFloat(obj.x), new JsonFloat(obj.y) };
            }

            public override Vector2 FromJson(JsonList jsonData)
            {
                if (jsonData.Count != 2)
                {
                    throw new System.Exception("Expected a list of length 2 but found one of length " + jsonData.Count);
                }
                return new Vector2(JsonConverter.FromJson<float>(jsonData[0]), JsonConverter.FromJson<float>(jsonData[1]));
            }
        }

        public class Vector22JsonConverter : IJsonConverter<Vector2, JsonList>
        {
            public override JsonData ToJson(Vector2 obj)
            {
                return new JsonList { new JsonFloat(obj.x), new JsonFloat(obj.y) };
            }

            public override Vector2 FromJson(JsonList jsonData)
            {
                if (jsonData.Count != 2)
                {
                    throw new System.Exception("Expected a list of length 2 but found one of length " + jsonData.Count);
                }
                return new Vector2(JsonConverter.FromJson<float>(jsonData[0]), JsonConverter.FromJson<float>(jsonData[1]));
            }
        }

        public class Vector3JsonConverter : IJsonConverter<Vector3, JsonList>
        {
            public override JsonData ToJson(Vector3 obj)
            {
                return new JsonList { new JsonFloat(obj.x), new JsonFloat(obj.y), new JsonFloat(obj.z) };
            }

            public override Vector3 FromJson(JsonList jsonData)
            {
                if (jsonData.Count != 3)
                {
                    throw new System.Exception("Expected a list of length 3 but found one of length " + jsonData.Count);
                }
                return new Vector3(JsonConverter.FromJson<float>(jsonData[0]), JsonConverter.FromJson<float>(jsonData[1]), JsonConverter.FromJson<float>(jsonData[2]));
            }
        }

        public class ColorJsonConverter : IJsonConverter<Color, JsonList>
        {
            public override JsonData ToJson(Color obj)
            {
                return new JsonList { new JsonFloat(obj.r), new JsonFloat(obj.g), new JsonFloat(obj.b), new JsonFloat(obj.a) };
            }

            public override Color FromJson(JsonList jsonData)
            {
                if (jsonData.Count != 4)
                {
                    throw new System.Exception("Expected a list of length 4 but found one of length " + jsonData.Count);
                }
                return new Color(JsonConverter.FromJson<float>(jsonData[0]), JsonConverter.FromJson<float>(jsonData[1]), JsonConverter.FromJson<float>(jsonData[2]),
                    JsonConverter.FromJson<float>(jsonData[3]));
            }
        }
    }
}

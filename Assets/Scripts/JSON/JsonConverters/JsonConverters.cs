using System;
using System.Runtime.Serialization;
using PAC.Json;
using UnityEngine;

namespace PAC.Extensions
{
    public static class JsonConverters
    {
        /// <summary>All the custom JSON converters define in this class.</summary>
        public static readonly JsonConversion.JsonConverterSet allConverters = new JsonConversion.JsonConverterSet(new Vector2JsonConverter(), new Vector3JsonConverter(), new ColorJsonConverter(), new Texture2DJsonConverter());

        /// <summary>
        /// Custom JSON converter for Vector2.
        /// </summary>
        public class Vector2JsonConverter : JsonConversion.JsonConverter<Vector2, JsonData.List>
        {
            public override JsonData.List ToJson(Vector2 vec)
            {
                JsonData.List jsonList = new JsonData.List { new JsonData.Float(vec.x), new JsonData.Float(vec.y) };
                jsonList.separateLinesInPrettyString = false;
                return jsonList;
            }

            public override Vector2 FromJson(JsonData.List jsonData)
            {
                if (jsonData.Count != 2)
                {
                    throw new ArgumentException("Expected a list of length 2 but found one of length " + jsonData.Count, "jsonData");
                }
                return new Vector2(JsonConversion.FromJson<float>(jsonData[0]), JsonConversion.FromJson<float>(jsonData[1]));
            }
        }

        /// <summary>
        /// Custom JSON converter for Vector3.
        /// </summary>
        public class Vector3JsonConverter : JsonConversion.JsonConverter<Vector3, JsonData.List>
        {
            public override JsonData.List ToJson(Vector3 vec)
            {
                JsonData.List jsonList = new JsonData.List { new JsonData.Float(vec.x), new JsonData.Float(vec.y), new JsonData.Float(vec.z) };
                jsonList.separateLinesInPrettyString = false;
                return jsonList;
            }

            public override Vector3 FromJson(JsonData.List jsonData)
            {
                if (jsonData.Count != 3)
                {
                    throw new ArgumentException("Expected a list of length 3 but found one of length " + jsonData.Count, "jsonData");
                }
                return new Vector3(JsonConversion.FromJson<float>(jsonData[0]), JsonConversion.FromJson<float>(jsonData[1]), JsonConversion.FromJson<float>(jsonData[2]));
            }
        }

        /// <summary>
        /// Custom JSON converter for Color.
        /// </summary>
        public class ColorJsonConverter : JsonConversion.JsonConverter<Color, JsonData.List>
        {
            public override JsonData.List ToJson(Color color)
            {
                JsonData.List jsonList = new JsonData.List { new JsonData.Float(color.r), new JsonData.Float(color.g), new JsonData.Float(color.b), new JsonData.Float(color.a) };
                jsonList.separateLinesInPrettyString = false;
                return jsonList;
            }

            public override Color FromJson(JsonData.List jsonData)
            {
                if (jsonData.Count != 4)
                {
                    throw new ArgumentException("Expected a list of length 4 but found one of length " + jsonData.Count, "jsonData");
                }
                return new Color(JsonConversion.FromJson<float>(jsonData[0]), JsonConversion.FromJson<float>(jsonData[1]), JsonConversion.FromJson<float>(jsonData[2]),
                    JsonConversion.FromJson<float>(jsonData[3]));
            }
        }

        /// <summary>
        /// Custom JSON converter for Texture2D.
        /// </summary>
        public class Texture2DJsonConverter : JsonConversion.JsonConverter<Texture2D, JsonData.Object>
        {
            private JsonConversion.JsonConverterSet converterList = new JsonConversion.JsonConverterSet(new ColorJsonConverter());

            public override JsonData.Object ToJson(Texture2D tex)
            {
                JsonData.List pixels = (JsonData.List)JsonConversion.ToJson(tex.GetPixels(), converterList);
                pixels.separateLinesInPrettyString = false;

                JsonData.Object json = new JsonData.Object
                {
                    { "width", tex.width },
                    { "height", tex.height },
                    { "pixels", pixels }
                };

                return json;
            }

            public override Texture2D FromJson(JsonData.Object jsonData)
            {
                int width = JsonConversion.FromJson<int>(jsonData["width"]);
                int height = JsonConversion.FromJson<int>(jsonData["height"]);
                Color[] pixels = JsonConversion.FromJson<Color[]>(jsonData["pixels"], converterList);

                if (width * height != pixels.Length)
                {
                    throw new SerializationException("Number of given pixels is " + pixels.Length + " which does not equal the given width * height, which is " + width * height);
                }

                Texture2D tex = new Texture2D(width, height);
                tex.SetPixels(pixels);
                tex.Apply();

                return tex;
            }
        }
    }
}

using NUnit.Framework;
using PAC.Json;
using UnityEngine;
using PAC.Json.JsonConverters;

namespace PAC.Tests.Json
{
    /// <summary>
    /// Tests the 'JsonConversion.JsonConverter's in the JsonConverters class.
    /// </summary>
    public class JsonConverters_Tests
    {
        /// <summary>
        /// Checks that ToJson() works properly for the custom JSON converter for type Vector2.
        /// </summary>
        [Test]
        [Category("JSON"), Category("Extensions")]
        public void Vector2ToJson()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new JsonConverters.Vector2JsonConverter());

            Vector2 colour = new Vector2(0.2f, 0.4f);
            JsonData.List expected = new JsonData.List(new JsonData.Float(0.2f), new JsonData.Float(0.4f));

            Assert.True(JsonData.HaveSameData(JsonConversion.ToJson(colour, converters, false), expected, 0.05f));
        }

        /// <summary>
        /// Checks that FromJson() works properly for the custom JSON converter for type Vector2.
        /// </summary>
        [Test]
        [Category("JSON"), Category("Extensions")]
        public void Vector2FromJson()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new JsonConverters.Vector2JsonConverter());

            JsonData.List jsonList = new JsonData.List(new JsonData.Float(0.2f), new JsonData.Float(0.4f));
            Vector2 expected = new Vector2(0.2f, 0.4f);

            Assert.AreEqual(expected, JsonConversion.FromJson<Vector2>(jsonList, converters, false));

            // List too short
            Assert.Catch(() => JsonConversion.FromJson<Vector2>(new JsonData.List(new JsonData.Float(0.2f)), converters, false));
            // List too long
            Assert.Catch(() => JsonConversion.FromJson<Vector2>(new JsonData.List(new JsonData.Float(0.2f), new JsonData.Float(0.4f), new JsonData.Float(0.1567f)), converters, false));
        }

        /// <summary>
        /// Checks that ToJson() works properly for the custom JSON converter for type Vector3.
        /// </summary>
        [Test]
        [Category("JSON"), Category("Extensions")]
        public void Vector3ToJson()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new JsonConverters.Vector3JsonConverter());

            Vector3 colour = new Vector3(0.2f, 0.4f, 0.1567f);
            JsonData.List expected = new JsonData.List(new JsonData.Float(0.2f), new JsonData.Float(0.4f), new JsonData.Float(0.1567f));

            Assert.True(JsonData.HaveSameData(JsonConversion.ToJson(colour, converters, false), expected, 0.00005f));
        }

        /// <summary>
        /// Checks that FromJson() works properly for the custom JSON converter for type Vector3.
        /// </summary>
        [Test]
        [Category("JSON"), Category("Extensions")]
        public void Vector3FromJson()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new JsonConverters.Vector3JsonConverter());

            JsonData.List jsonList = new JsonData.List(new JsonData.Float(0.2f), new JsonData.Float(0.4f), new JsonData.Float(0.1567f));
            Vector3 expected = new Vector3(0.2f, 0.4f, 0.1567f);

            Assert.AreEqual(expected, JsonConversion.FromJson<Vector3>(jsonList, converters, false));

            // List too short
            Assert.Catch(() => JsonConversion.FromJson<Vector3>(new JsonData.List(new JsonData.Float(0.2f), new JsonData.Float(0.4f)), converters, false));
            // List too long
            Assert.Catch(() => JsonConversion.FromJson<Vector3>(new JsonData.List(new JsonData.Float(0.2f), new JsonData.Float(0.4f), new JsonData.Float(0.1567f), new JsonData.Float(0.95f)),
                converters, false));
        }

        /// <summary>
        /// Checks that ToJson() works properly for the custom JSON converter for type Color.
        /// </summary>
        [Test]
        [Category("JSON"), Category("Extensions")]
        public void ColorToJson()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new JsonConverters.ColorJsonConverter());

            Color colour = new Color(0.2f, 0.4f, 0.1567f, 0.95f);
            JsonData.List expected = new JsonData.List(new JsonData.Float(0.2f), new JsonData.Float(0.4f), new JsonData.Float(0.1567f), new JsonData.Float(0.95f));

            Assert.True(JsonData.HaveSameData(JsonConversion.ToJson(colour, converters, false), expected, 0.00005f));
        }

        /// <summary>
        /// Checks that FromJson() works properly for the custom JSON converter for type Color.
        /// </summary>
        [Test]
        [Category("JSON"), Category("Extensions")]
        public void ColorFromJson()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new JsonConverters.ColorJsonConverter());

            JsonData.List jsonList = new JsonData.List(new JsonData.Float(0.2f), new JsonData.Float(0.4f), new JsonData.Float(0.1567f), new JsonData.Float(0.95f));
            Color expected = new Color(0.2f, 0.4f, 0.1567f, 0.95f);

            Assert.AreEqual(expected, JsonConversion.FromJson<Color>(jsonList, converters, false));

            // List too short
            Assert.Catch(() => JsonConversion.FromJson<Color>(new JsonData.List(new JsonData.Float(0.2f), new JsonData.Float(0.4f), new JsonData.Float(0.1567f)), converters, false));
            // List too long
            Assert.Catch(() => JsonConversion.FromJson<Color>(new JsonData.List(new JsonData.Float(0.2f), new JsonData.Float(0.4f), new JsonData.Float(0.1567f), new JsonData.Float(0.95f),
                new JsonData.Float(0.3f)), converters, false));
        }

        /// <summary>
        /// Checks that ToJson() works properly for the custom JSON converter for type Texture2D.
        /// </summary>
        [Test]
        [Category("JSON"), Category("Extensions")]
        public void Texture2DToJson()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new JsonConverters.Texture2DJsonConverter());

            Texture2D tex = new Texture2D(2, 2);
            tex.SetPixel(0, 0, new Color(0f, 0f, 0f, 1f));
            tex.SetPixel(1, 0, new Color(0.1f, 0.2f, 0.3f, 0.9f));
            tex.SetPixel(0, 1, new Color(0.5f, 0.43f, 0.2f, 0f));
            tex.SetPixel(1, 1, new Color(1f, 0.8f, 0f, 1f));

            JsonData.Object expected = new JsonData.Object
            {
                { "width", new JsonData.Int(2) },
                { "height", new JsonData.Int(2) },
                { "pixels", new JsonData.List{
                    new JsonData.List(new JsonData.Float(0f), new JsonData.Float(0f), new JsonData.Float(0f), new JsonData.Float(1f)),
                    new JsonData.List(new JsonData.Float(0.1f), new JsonData.Float(0.2f), new JsonData.Float(0.3f), new JsonData.Float(0.9f)),
                    new JsonData.List(new JsonData.Float(0.5f), new JsonData.Float(0.43f), new JsonData.Float(0.2f), new JsonData.Float(0f)),
                    new JsonData.List(new JsonData.Float(1f), new JsonData.Float(0.8f), new JsonData.Float(0f), new JsonData.Float(1f))
                    }
                }
            };

            Assert.True(JsonData.HaveSameData(JsonConversion.ToJson(tex, converters, false), expected, 0.005f));
        }

        /// <summary>
        /// Checks that FromJson() works properly for the custom JSON converter for type Texture2D.
        /// </summary>
        [Test]
        [Category("JSON"), Category("Extensions")]
        public void Texture2DFromJson()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new JsonConverters.Texture2DJsonConverter());

            Texture2D expected = new Texture2D(2, 2);
            expected.SetPixel(0, 0, new Color(0f, 0f, 0f, 1f));
            expected.SetPixel(1, 0, new Color(0.1f, 0.2f, 0.3f, 0.9f));
            expected.SetPixel(0, 1, new Color(0.5f, 0.43f, 0.2f, 0f));
            expected.SetPixel(1, 1, new Color(1f, 0.8f, 0f, 1f));

            JsonData.Object jsonObj = new JsonData.Object
            {
                { "width", new JsonData.Int(2) },
                { "height", new JsonData.Int(2) },
                { "pixels", new JsonData.List{
                    new JsonData.List(new JsonData.Float(0f), new JsonData.Float(0f), new JsonData.Float(0f), new JsonData.Float(1f)),
                    new JsonData.List(new JsonData.Float(0.1f), new JsonData.Float(0.2f), new JsonData.Float(0.3f), new JsonData.Float(0.9f)),
                    new JsonData.List(new JsonData.Float(0.5f), new JsonData.Float(0.43f), new JsonData.Float(0.2f), new JsonData.Float(0f)),
                    new JsonData.List(new JsonData.Float(1f), new JsonData.Float(0.8f), new JsonData.Float(0f), new JsonData.Float(1f))
                    }
                }
            };

            Texture2D converted = JsonConversion.FromJson<Texture2D>(jsonObj, converters, false);

            Assert.AreEqual(converted.width, expected.width);
            Assert.AreEqual(converted.height, expected.height);
            Assert.AreEqual(expected.GetPixel(0, 0), converted.GetPixel(0, 0));
            Assert.AreEqual(expected.GetPixel(1, 0), converted.GetPixel(1, 0));
            Assert.AreEqual(expected.GetPixel(0, 1), converted.GetPixel(0, 1));
            Assert.AreEqual(expected.GetPixel(1, 1), converted.GetPixel(1, 1));

            // Num of pixels != width * height
            jsonObj["width"] = new JsonData.Int(3);
            Assert.Catch(() => JsonConversion.FromJson<Texture2D>(jsonObj, converters, false));
        }
    }
}

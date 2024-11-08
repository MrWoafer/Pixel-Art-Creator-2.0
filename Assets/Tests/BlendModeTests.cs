using NUnit.Framework;
using PAC.Colour;
using PAC.Json;

namespace PAC.Tests
{
    /// <summary>
    /// Tests the BlendMode class.
    /// </summary>
    public class BlendModeTests
    {
        /// <summary>
        /// Checks that ToJson() works properly for the custom JSON converter for type BlendMode.
        /// </summary>
        [Test]
        public void ToJson()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new BlendMode.JsonConverter());

            (BlendMode, JsonData)[] testCases =
            {
                (BlendMode.Add, new JsonString("Add")),
                (BlendMode.Normal, new JsonString("Normal")),
                (BlendMode.Screen, new JsonString("Screen")),
            };

            foreach ((BlendMode blendMode, JsonData expected) in testCases)
            {
                Assert.True(JsonData.HaveSameData(expected, JsonConversion.ToJson(blendMode, converters, false)));
            }
        }

        /// <summary>
        /// Checks that FromJson() works properly for the custom JSON converter for type BlendMode.
        /// </summary>
        [Test]
        public void FromJson()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new BlendMode.JsonConverter());

            (BlendMode, JsonData)[] testCases =
            {
                (BlendMode.Add, new JsonString("Add")),
                (BlendMode.Normal, new JsonString("Normal")),
                (BlendMode.Screen, new JsonString("Screen")),
                (BlendMode.Add, new JsonString("add")),
                (BlendMode.Normal, new JsonString("normal")),
                (BlendMode.Screen, new JsonString("screen")),
            };

            foreach ((BlendMode expected, JsonData jsonData) in testCases)
            {
                Assert.AreEqual(expected, JsonConversion.FromJson<BlendMode>(jsonData, converters, false));
            }

            // No such blend mode
            Assert.Catch(() => JsonConversion.FromJson<BlendMode>(new JsonString("Woafer"), converters, false));
        }
    }
}

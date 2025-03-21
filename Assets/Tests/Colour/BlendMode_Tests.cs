using System.Linq;

using NUnit.Framework;

using PAC.Colour;
using PAC.Extensions;
using PAC.Json;

using UnityEngine;

namespace PAC.Tests.Colour
{
    /// <summary>
    /// Tests for <see cref="BlendMode"/>.
    /// </summary>
    public class BlendMode_Tests
    {
        [Test]
        [Category("Colour"), Category("Compositing")]
        public void Normal_WithSimpleAlphaCompositing_MatchesSimpleAlphaCompositing()
        {
            System.Random random = new System.Random(0);
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                Color source = random.NextRGB().WithAlpha(1f);
                Color destination = random.NextColor();

                Color composited = BlendMode.Normal.Blend(source, destination);
                Assert.True(source.Equals(composited, 0.001f), $"Failed with {nameof(source)} = {source}, {nameof(destination)} = {destination}.");
            }
        }

        /// <summary>
        /// Tests <see cref="BlendMode.Normal"/> with simple alpha compositing, using example values from Krita.
        /// </summary>
        [Test]
        [Category("Colour"), Category("Compositing")]
        public void Normal_WithSimpleAlphaCompositing_Example()
        {
            Color source = new Color(0.39f, 0.31f, 0.32f, 0.55f);
            Color destination = new Color(0.26f, 0.18f, 0.81f, 0.62f);

            Color composited = BlendMode.Normal.Blend(source, destination);
            Color expected = new Color(0.35f, 0.27f, 0.48f, 0.83f);

            Assert.True(
                expected.Equals(composited, 0.01f),
                $"Failed.\nExpected: {expected}\nObserved: {composited}");
        }

        /// <summary>
        /// Tests <see cref="BlendMode.Overlay"/> with simple alpha compositing, using example values from Krita.
        /// </summary>
        [Test]
        [Category("Colour"), Category("Compositing")]
        public void Overlay_WithSimpleAlphaCompositing_Example()
        {
            Color source = new Color(0.21f, 0.31f, 0.41f, 0.51f);
            Color destination = new Color(0.64f, 0.81f, 0.49f, 0.36f);

            Color composited = BlendMode.Overlay.Blend(source, destination);
            Color expected = new Color(0.38f, 0.55f, 0.43f, 0.69f);

            Assert.True(
                expected.Equals(composited, 0.01f),
                $"Failed.\nExpected: {expected}\nObserved: {composited}");
        }

        /// <summary>
        /// Tests <see cref="BlendMode.Multiply"/> with simple alpha compositing, using example values from Krita.
        /// </summary>
        [Test]
        [Category("Colour"), Category("Compositing")]
        public void Multiply_WithSimpleAlphaCompositing_Example()
        {
            Color source = new Color(0.78f, 0.64f, 0.04f, 0.70f);
            Color destination = new Color(0.58f, 0.47f, 0.65f, 0.46f);

            Color composited = BlendMode.Multiply.Blend(source, destination);
            Color expected = new Color(0.63f, 0.48f, 0.13f, 0.84f);

            Assert.True(
                expected.Equals(composited, 0.01f),
                $"Failed.\nExpected: {expected}\nObserved: {composited}");
        }

        /// <summary>
        /// Tests <see cref="BlendMode.Screen"/> with simple alpha compositing, using example values from Krita.
        /// </summary>
        [Test]
        [Category("Colour"), Category("Compositing")]
        public void Screen_WithSimpleAlphaCompositing_Example()
        {
            Color source = new Color(0.08f, 0.13f, 0.60f, 0.97f);
            Color destination = new Color(0.73f, 0.42f, 0.56f, 0.55f);

            Color composited = BlendMode.Screen.Blend(source, destination);
            Color expected = new Color(0.45f, 0.33f, 0.72f, 0.98f);

            Assert.True(
                expected.Equals(composited, 0.01f),
                $"Failed.\nExpected: {expected}\nObserved: {composited}");
        }

        /// <summary>
        /// Tests <see cref="BlendMode.Add"/> with simple alpha compositing, using example values from Krita.
        /// </summary>
        [Test]
        [Category("Colour"), Category("Compositing")]
        public void Add_WithSimpleAlphaCompositing_Example()
        {
            Color source = new Color(0.31f, 0.41f, 0.59f, 0.26f);
            Color destination = new Color(0.73f, 0.09f, 0.27f, 0.89f);

            Color composited = BlendMode.Add.Blend(source, destination);
            Color expected = new Color(0.79f, 0.20f, 0.42f, 0.92f);

            Assert.True(
                expected.Equals(composited, 0.01f),
                $"Failed.\nExpected: {expected}\nObserved: {composited}");
        }

        /// <summary>
        /// Tests <see cref="BlendMode.Subtract"/> with simple alpha compositing, using example values from Krita.
        /// </summary>
        [Test]
        [Category("Colour"), Category("Compositing")]
        public void Subtract_WithSimpleAlphaCompositing_Example()
        {
            Color source = new Color(0.27f, 0.18f, 0.28f, 0.18f);
            Color destination = new Color(0.16f, 0.26f, 0.03f, 0.43f);

            Color composited = BlendMode.Subtract.Blend(source, destination);
            Color expected = new Color(0.15f, 0.21f, 0.07f, 0.53f);

            Assert.True(
                expected.Equals(composited, 0.01f),
                $"Failed.\nExpected: {expected}\nObserved: {composited}");
        }

        [Test]
        [Category("Colour"), Category("Compositing")]
        public void BlendModes_NonEmpty()
        {
            Assert.True(BlendMode.BlendModes.Any());
        }
        [Test]
        [Category("Colour"), Category("Compositing")]
        public void BlendModes_ContainsNoDuplicates()
        {
            Assert.True(BlendMode.BlendModes.AreAllDistinct());
        }
        [Test]
        [Category("Colour"), Category("Compositing")]
        public void BlendModes_ExampleElements()
        {
            CollectionAssert.IsSubsetOf(
                new BlendMode[] { BlendMode.Normal, BlendMode.Overlay, BlendMode.Multiply, BlendMode.Screen, BlendMode.Add, BlendMode.Subtract },
                BlendMode.BlendModes
                );
        }

        /// <summary>
        /// Checks that ToJson() works properly for the custom JSON converter for type BlendMode.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void ToJson()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new BlendMode.JsonConverter());

            (BlendMode, JsonData)[] testCases =
            {
                (BlendMode.Add, new JsonData.String("Add")),
                (BlendMode.Normal, new JsonData.String("Normal")),
                (BlendMode.Screen, new JsonData.String("Screen")),
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
        [Category("JSON")]
        public void FromJson()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new BlendMode.JsonConverter());

            (BlendMode, JsonData)[] testCases =
            {
                (BlendMode.Add, new JsonData.String("Add")),
                (BlendMode.Normal, new JsonData.String("Normal")),
                (BlendMode.Screen, new JsonData.String("Screen")),
                (BlendMode.Add, new JsonData.String("add")),
                (BlendMode.Normal, new JsonData.String("normal")),
                (BlendMode.Screen, new JsonData.String("screen")),
            };

            foreach ((BlendMode expected, JsonData jsonData) in testCases)
            {
                Assert.AreEqual(expected, JsonConversion.FromJson<BlendMode>(jsonData, converters, false));
            }

            // No such blend mode
            Assert.Catch(() => JsonConversion.FromJson<BlendMode>(new JsonData.String("Woafer"), converters, false));
        }
    }
}

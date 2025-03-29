using NUnit.Framework;

using PAC.DataStructures;
using PAC.Patterns;
using PAC.Patterns.Extensions;

using UnityEngine;

namespace PAC.Tests.Extensions
{
    /// <summary>
    /// Tests for <see cref="IPattern2DExtensions"/>.
    /// </summary>
    public class IPattern2DExtensions_Tests
    {
        private struct Pattern_Color : IPattern2D<Color>
        {
            public Color this[IntVector2 point] => point switch
            {
                (0, 0) => Color.red,
                (0, 1) => Color.green,
                (1, 0) => Color.blue,
                (1, 1) => Color.white,
                _ => Color.black
            };
        }

        /// <summary>
        /// Tests <see cref="IPattern2DExtensions.ToTexture(IPattern2D{Color}, IntRect)"/>.
        /// </summary>
        [Test]
        [Category("Extensions")]
        public void ToTexture_Color()
        {
            IntRect textureRect = new IntRect((-1, -1), (2, 1));
            Texture2D texture = new Pattern_Color().ToTexture(textureRect);

            Assert.IsNotNull(texture);
            Assert.AreEqual(4, texture.width);
            Assert.AreEqual(3, texture.height);

            Color[] expectedPixels = new Color[]
            {
                Color.black, Color.black, Color.black, Color.black,
                Color.black, Color.red, Color.blue, Color.black,
                Color.black, Color.green, Color.white, Color.black
            };

            CollectionAssert.AreEqual(expectedPixels, texture.GetPixels());
        }

        private struct Pattern_Color32 : IPattern2D<Color32>
        {
            public Color32 this[IntVector2 point] => point switch
            {
                (0, 0) => Color.red,
                (0, 1) => Color.green,
                (1, 0) => Color.blue,
                (1, 1) => Color.white,
                _ => Color.black
            };
        }

        /// <summary>
        /// Tests <see cref="IPattern2DExtensions.ToTexture(IPattern2D{Color32}, IntRect)"/>.
        /// </summary>
        [Test]
        [Category("Extensions")]
        public void ToTexture_Color32()
        {
            IntRect textureRect = new IntRect((-1, -1), (2, 1));
            Texture2D texture = new Pattern_Color32().ToTexture(textureRect);

            Assert.IsNotNull(texture);
            Assert.AreEqual(4, texture.width);
            Assert.AreEqual(3, texture.height);

            Color32[] expectedPixels = new Color32[]
            {
                Color.black, Color.black, Color.black, Color.black,
                Color.black, Color.red, Color.blue, Color.black,
                Color.black, Color.green, Color.white, Color.black
            };

            CollectionAssert.AreEqual(expectedPixels, texture.GetPixels32());
        }
    }
}

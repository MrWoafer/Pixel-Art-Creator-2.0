using NUnit.Framework;

using PAC.Extensions;

using UnityEngine;

namespace PAC.Tests
{
    /// <summary>
    /// Tests for <see cref="ColorExtensions"/>.
    /// </summary>
    public class ColorExtensions_Tests
    {
        /// <summary>
        /// Tests <see cref="ColorExtensions.Equals(Color, Color, float)"/>.
        /// </summary>
        [Test]
        [Category("Extensions")]
        public void ApproxEquals()
        {
            System.Random rng = new System.Random(0);
            for (int i = 0; i < 1_000; i++)
            {
                Color colour = new Color((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble());

                Assert.True(colour.Equals(colour, 0f), $"Failed with {colour}.");

                Color otherColour = new Color((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble());
                float tolerance = (float)rng.NextDouble();

                if (Mathf.Abs(colour.r - otherColour.r) <= tolerance &&
                    Mathf.Abs(colour.g - otherColour.g) <= tolerance &&
                    Mathf.Abs(colour.b - otherColour.b) <= tolerance &&
                    Mathf.Abs(colour.a - otherColour.a) <= tolerance)
                {
                    Assert.True(colour.Equals(otherColour, tolerance), $"Failed with {colour} and {otherColour} and tolerance {tolerance}.");
                    Assert.True(otherColour.Equals(colour, tolerance), $"Failed with {colour} and {otherColour} and tolerance {tolerance}.");
                }
                else
                {
                    Assert.False(colour.Equals(otherColour, tolerance), $"Failed with {colour} and {otherColour} and tolerance {tolerance}.");
                    Assert.False(otherColour.Equals(colour, tolerance), $"Failed with {colour} and {otherColour} and tolerance {tolerance}.");
                }
            }
        }
    }
}

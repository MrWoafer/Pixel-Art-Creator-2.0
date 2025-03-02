using System.Collections.Generic;

using NUnit.Framework;

using PAC.DataStructures;
using PAC.Geometry;

using UnityEngine;

namespace PAC.Tests.Patterns
{
    /// <summary>
    /// Tests for <see cref="PAC.Patterns.Gradient.Radial"/>.
    /// </summary>
    public class GradientRadial_Tests
    {
        private IEnumerable<PAC.Patterns.Gradient.Radial> testCases
        {
            get
            {
                System.Random random = new System.Random(0);
                foreach (IntVector2 centreCoord in new IntRect((-5, -5), (5, 5)))
                {
                    foreach (IntVector2 onCircumferenceCoord in new IntRect((-5, -5), (5, 5)))
                    {
                        Color centreColour = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                        Color onCircumferenceColour = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());

                        yield return new PAC.Patterns.Gradient.Radial((centreCoord, centreColour), (onCircumferenceCoord, onCircumferenceColour));
                    }
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="Patterns.Gradient.Radial.centre"/> and <see cref="Patterns.Gradient.Radial.onCircumference"/> have the correct colour.
        /// </summary>
        [Test]
        [Category("Patterns")]
        public void ColourOfParameters()
        {
            foreach (PAC.Patterns.Gradient.Radial gradient in testCases)
            {
                Assert.AreEqual(gradient.centre.colour, gradient[gradient.centre.coord], $"Failed with {gradient}.");
                if (gradient.centre.coord != gradient.onCircumference.coord)
                {
                    Assert.True(gradient.onCircumference.colour == gradient[gradient.onCircumference.coord], $"Failed with {gradient}.");
                }
            }
        }

        /// <summary>
        /// Tests that points that are further from <see cref="Patterns.Gradient.Radial.centre"/> than <see cref="Patterns.Gradient.Radial.onCircumference"/> is have the same colour as
        /// <see cref="Patterns.Gradient.Radial.onCircumference"/>.
        /// </summary>
        [Test]
        [Category("Patterns")]
        public void ColourOutsideCircle()
        {
            foreach (PAC.Patterns.Gradient.Radial gradient in testCases)
            {
                float radius = IntVector2.Distance(gradient.centre.coord, gradient.onCircumference.coord);

                IntRect testRegion = new IntRect(gradient.centre.coord, gradient.onCircumference.coord);
                testRegion = new IntRect(testRegion.bottomLeft + IntVector2.downLeft, testRegion.topRight + IntVector2.upRight);
                foreach (IntVector2 point in testRegion)
                {
                    float distance = IntVector2.Distance(gradient.centre.coord, point);
                    if (distance > radius || (distance == radius && gradient.centre.coord != gradient.onCircumference.coord))
                    {
                        Assert.True(gradient.onCircumference.colour == gradient[point], $"Failed with {gradient} and {point}");
                    }
                }
            }
        }

        /// <summary>
        /// Tests that points inside the circle have the correct colour.
        /// </summary>
        [Test]
        [Category("Patterns")]
        public void ColourInsideCircle()
        {
            foreach (PAC.Patterns.Gradient.Radial gradient in testCases)
            {
                float radius = IntVector2.Distance(gradient.centre.coord, gradient.onCircumference.coord);

                IntRect testRegion = new IntRect(gradient.centre.coord, gradient.onCircumference.coord);
                testRegion = new IntRect(testRegion.bottomLeft + IntVector2.downLeft, testRegion.topRight + IntVector2.upRight);
                foreach (IntVector2 point in testRegion)
                {
                    float distance = IntVector2.Distance(gradient.centre.coord, point);
                    if (0 < distance && distance < radius)
                    {
                        Color expectedColour = Color.LerpUnclamped(gradient.centre.colour, gradient.onCircumference.colour, distance / radius);
                        Assert.True(expectedColour == gradient[point], $"Failed with {gradient} and {point}");
                    }
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="Patterns.Gradient.Radial.this[IntVector2]"/> has reflectional symmetry across the horizontal/vertical axis through <see cref="Patterns.Gradient.Radial.centre"/>.
        /// </summary>
        [Test]
        [Category("Patterns")]
        public void ReflectionalSymmetry()
        {
            foreach (PAC.Patterns.Gradient.Radial gradient in testCases)
            {
                IntRect testRegion = new IntRect(gradient.centre.coord, gradient.onCircumference.coord);
                testRegion = new IntRect(testRegion.bottomLeft + IntVector2.downLeft, testRegion.topRight + IntVector2.upRight);
                foreach (IntVector2 point in testRegion)
                {
                    Assert.True(gradient[point] == gradient[gradient.centre.coord + (point - gradient.centre.coord).Flip(FlipAxis.Vertical)], $"Failed with {gradient} and {point}.");
                    Assert.True(gradient[point] == gradient[gradient.centre.coord + (point - gradient.centre.coord).Flip(FlipAxis.Horizontal)], $"Failed with {gradient} and {point}.");
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="Patterns.Gradient.Radial.this[IntVector2]"/> has 90-degree rotational symmetry about <see cref="Patterns.Gradient.Radial.centre"/>.
        /// </summary>
        [Test]
        [Category("Patterns")]
        public void RotationalSymmetry()
        {
            foreach (PAC.Patterns.Gradient.Radial gradient in testCases)
            {
                IntRect testRegion = new IntRect(gradient.centre.coord, gradient.onCircumference.coord);
                testRegion = new IntRect(testRegion.bottomLeft + IntVector2.downLeft, testRegion.topRight + IntVector2.upRight);
                foreach (IntVector2 point in testRegion)
                {
                    Assert.True(gradient[point] == gradient[gradient.centre.coord + (point - gradient.centre.coord).Rotate(QuadrantalAngle._90)], $"Failed with {gradient} and {point}.");
                }
            }
        }
    }
}

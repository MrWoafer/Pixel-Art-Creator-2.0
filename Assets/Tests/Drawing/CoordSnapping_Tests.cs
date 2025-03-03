using System;

using NUnit.Framework;

using PAC.DataStructures;
using PAC.Drawing;
using PAC.Extensions;
using PAC.Geometry;
using PAC.Geometry.Axes;
using PAC.Geometry.Shapes;

namespace PAC.Tests.Drawing
{
    /// <summary>
    /// Tests for <see cref="CoordSnapping"/>.
    /// </summary>
    public class CoordSnapping_Tests
    {
        /// <summary>
        /// Tests <see cref="CoordSnapping.SnapToSquare(IntVector2, IntVector2)"/>.
        /// </summary>
        [Test]
        [Category("Drawing")]
        public void SnapToSquare()
        {
            foreach (IntVector2 fixedPoint in new IntRect((-5, -5), (5, 5)))
            {
                foreach (IntVector2 movablePoint in new IntRect((-5, -5), (5, 5)))
                {
                    IntRect originalRect = new IntRect(fixedPoint, movablePoint);

                    IntVector2 snappedPoint = CoordSnapping.SnapToSquare(fixedPoint, movablePoint);
                    IntRect snappedRect = new IntRect(fixedPoint, snappedPoint);

                    Assert.True(snappedRect.isSquare, $"Failed with {fixedPoint} and {movablePoint}.");
                    Assert.AreEqual(Math.Max(originalRect.width, originalRect.height), snappedRect.width, $"Failed with {fixedPoint} and {movablePoint}.");

                    Assert.True(snappedPoint.x == movablePoint.x || snappedPoint.y == movablePoint.y, $"Failed with {fixedPoint} and {movablePoint}.");

                    if (originalRect.isSquare)
                    {
                        Assert.AreEqual(snappedPoint, movablePoint, $"Failed with {fixedPoint} and {movablePoint}.");
                    }
                    
                    if (movablePoint.x == fixedPoint.x && movablePoint.y != fixedPoint.y)
                    {
                        Assert.True(snappedPoint.x > movablePoint.x, $"Failed with {fixedPoint} and {movablePoint}.");
                    }
                    else if (movablePoint.y == fixedPoint.y && movablePoint.x != fixedPoint.x)
                    {
                        Assert.True(snappedPoint.y > movablePoint.y, $"Failed with {fixedPoint} and {movablePoint}.");
                    }
                    else
                    {
                        Assert.AreEqual(IntVector2.Sign(movablePoint - fixedPoint), IntVector2.Sign(snappedPoint - fixedPoint), $"Failed with {fixedPoint} and {movablePoint}.");
                    }
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="CoordSnapping.SnapToPerfectLine(IntVector2, IntVector2, int)"/> does indeed form a perfect <see cref="Line"/>.
        /// </summary>
        [Test]
        [Category("Drawing")]
        public void SnapToPerfectLine_FormsPerfectLine()
        {
            Random random = new Random(0);
            IntRect testRegion = new IntRect((-20, -20), (20, 20));
            for (int i = 0; i < 10_000; i++)
            {
                IntVector2 fixedPoint = testRegion.RandomPoint(random);
                IntVector2 movablePoint = testRegion.RandomPoint(random);

                IntVector2 snappedPoint = CoordSnapping.SnapToPerfectLine(fixedPoint, movablePoint);
                Line snappedLine = new Line(fixedPoint, snappedPoint);

                Assert.True(snappedLine.isPerfect, $"Failed with {nameof(fixedPoint)} = {fixedPoint} and {nameof(movablePoint)} = {movablePoint}.");
            }
        }

        /// <summary>
        /// Tests that, for <see cref="CoordSnapping.SnapToPerfectLine(IntVector2, IntVector2, int)"/>, the quadrant (relative to the horizontal/vertical axes through the fixed input point) that the
        /// snapped point lies in matches that of the movable input point (including potentially lying on the axes).
        /// </summary>
        [Test]
        [Category("Drawing")]
        public void SnapToPerfectLine_PreservesQuadrant()
        {
            Random random = new Random(0);
            IntRect testRegion = new IntRect((-20, -20), (20, 20));
            for (int i = 0; i < 10_000; i++)
            {
                IntVector2 fixedPoint = testRegion.RandomPoint(random);
                IntVector2 movablePoint = testRegion.RandomPoint(random);

                IntVector2 snappedPoint = CoordSnapping.SnapToPerfectLine(fixedPoint, movablePoint);

                IntVector2 originalSign = (movablePoint - fixedPoint).sign;
                IntVector2 snappedSign = (snappedPoint - fixedPoint).sign;

                Assert.True(
                        originalSign.x == snappedSign.x || snappedSign.x == 0,
                        $"Failed with {nameof(fixedPoint)} = {fixedPoint} and {nameof(movablePoint)} = {movablePoint}."
                        );
                Assert.True(
                        originalSign.y == snappedSign.y || snappedSign.y == 0,
                        $"Failed with {nameof(fixedPoint)} = {fixedPoint} and {nameof(movablePoint)} = {movablePoint}."
                        );
            }
        }

        /// <summary>
        /// Tests that translating the input points of <see cref="CoordSnapping.SnapToPerfectLine(IntVector2, IntVector2, int)"/> (by the same amount), translates the snapped point by the same
        /// amount.
        /// </summary>
        [Test]
        [Category("Drawing")]
        public void SnapToPerfectLine_TranslatingInputsTranslatesOutput()
        {
            Random random = new Random(0);
            IntRect testRegion = new IntRect((-20, -20), (20, 20));
            for (int i = 0; i < 10_000; i++)
            {
                IntVector2 fixedPoint = testRegion.RandomPoint(random);
                IntVector2 movablePoint = testRegion.RandomPoint(random);

                IntVector2 snappedPoint = CoordSnapping.SnapToPerfectLine(fixedPoint, movablePoint);

                IntVector2 translation = testRegion.RandomPoint(random);

                IntVector2 snappedPointTranslatedInputs = CoordSnapping.SnapToPerfectLine(fixedPoint + translation, movablePoint + translation);

                Assert.AreEqual(
                    snappedPoint + translation, snappedPointTranslatedInputs,
                    $"Failed with {nameof(fixedPoint)} = {fixedPoint}, {nameof(movablePoint)} = {movablePoint} and {nameof(translation)} = {translation}."
                    );
            }
        }
        /// <summary>
        /// Tests that rotating the input points of <see cref="CoordSnapping.SnapToPerfectLine(IntVector2, IntVector2, int)"/> (by the same angle), rotates the snapped point by the same angle.
        /// </summary>
        [Test]
        [Category("Drawing")]
        public void SnapToPerfectLine_RotatingInputsRotatesOutput()
        {
            Random random = new Random(0);
            IntRect testRegion = new IntRect((-20, -20), (20, 20));
            for (int i = 0; i < 10_000; i++)
            {
                IntVector2 fixedPoint = testRegion.RandomPoint(random);
                IntVector2 movablePoint = testRegion.RandomPoint(random);

                IntVector2 snappedPoint = CoordSnapping.SnapToPerfectLine(fixedPoint, movablePoint);

                foreach (QuadrantalAngle angle in TypeExtensions.GetValues<QuadrantalAngle>())
                {
                    IntVector2 snappedPointRotatedInputs = CoordSnapping.SnapToPerfectLine(fixedPoint.Rotate(angle), movablePoint.Rotate(angle));

                    Assert.AreEqual(
                        snappedPoint.Rotate(angle), snappedPointRotatedInputs,
                        $"Failed with {nameof(fixedPoint)} = {fixedPoint}, {nameof(movablePoint)} = {movablePoint} and {nameof(angle)} = {angle}."
                        );
                }
            }
        }
        /// <summary>
        /// Tests that reflecting the input points of <see cref="CoordSnapping.SnapToPerfectLine(IntVector2, IntVector2, int)"/> (across the same axis), reflects the snapped point across the
        /// same axis.
        /// </summary>
        [Test]
        [Category("Drawing")]
        public void SnapToPerfectLine_ReflectingInputsReflectsOutput()
        {
            Random random = new Random(0);
            IntRect testRegion = new IntRect((-20, -20), (20, 20));
            for (int i = 0; i < 10_000; i++)
            {
                IntVector2 fixedPoint = testRegion.RandomPoint(random);
                IntVector2 movablePoint = testRegion.RandomPoint(random);

                IntVector2 snappedPoint = CoordSnapping.SnapToPerfectLine(fixedPoint, movablePoint);

                foreach (CardinalOrdinalAxis axis in Axes.CardinalAxes)
                {
                    IntVector2 snappedPointReflectedInputs = CoordSnapping.SnapToPerfectLine(fixedPoint.Flip(axis), movablePoint.Flip(axis));

                    Assert.AreEqual(
                        snappedPoint.Flip(axis), snappedPointReflectedInputs,
                        $"Failed with {nameof(fixedPoint)} = {fixedPoint}, {nameof(movablePoint)} = {movablePoint} and {nameof(axis)} = {axis}."
                        );
                }
            }
        }
    }
}

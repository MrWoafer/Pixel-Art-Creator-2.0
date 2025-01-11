using System;

using NUnit.Framework;

using PAC.DataStructures;
using PAC.Drawing;

namespace PAC.Tests
{
    /// <summary>
    /// Tests for <see cref="CoordSnapping"/>.
    /// </summary>
    public class CoordSnappingTests
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
    }
}

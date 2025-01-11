using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PAC.DataStructures;
using PAC.Extensions;
using PAC.Maths;

namespace PAC.Shapes
{
    public class IsometricCuboid : IIsometricShape, IEquatable<IsometricCuboid>
    {
        private IntVector2 baseStart;
        private IntVector2 baseEnd;

        public bool filled { get; set; }
        public bool showBackEdges { get; set; }

        /// <summary>
        /// The height of the cuboid if you imagined it in 3D space. In other words, how much the bottom face / isometric rectangle is shifted up to form the top face / isometric rectangle.
        /// </summary>
        public int height { get; set; }

        public IsometricRectangle bottomRectangle => new IsometricRectangle(baseStart, baseEnd, filled) + MathExtensions.ClampNonPositive(height) * IntVector2.up;
        public IsometricRectangle topRectangle => bottomRectangle + Math.Abs(height) * IntVector2.up;
        private I1DShape[] border
        {
            get
            {
                I1DShape[] border = new I1DShape[showBackEdges ? 6 : 5];
                border[0] = showBackEdges ? bottomRectangle.border : bottomRectangle.lowerBorder;
                border[1] = topRectangle.border;
                border[2] = new Line(bottomRectangle.leftCorner, topRectangle.leftCorner);
                border[3] = new Line(bottomRectangle.rightCorner, topRectangle.rightCorner);
                border[4] = new Line(bottomRectangle.bottomCorner, topRectangle.bottomCorner);

                if (showBackEdges)
                {
                    border[5] = new Line(bottomRectangle.topCorner, topRectangle.topCorner);
                }

                return border;
            }
        }

        public IntRect boundingRect => IntRect.BoundingRect(bottomRectangle.boundingRect, topRectangle.boundingRect);

        public int Count
        {
            get
            {
                if (!filled)
                {
                    return border.Sum(path => path.Count);
                }

                int count = 0;
                Path lowerBorder = bottomRectangle.lowerBorder;
                Path upperBorder = topRectangle.upperBorder;
                foreach (int x in boundingRect.xRange)
                {
                    count += upperBorder.MaxY(x) - lowerBorder.MinY(x) + 1;
                }
                return count;
            }
        }

        public IsometricCuboid(IntVector2 baseStart, IntVector2 baseEnd, int height, bool filled, bool showBackEdges)
        {
            this.baseStart = baseStart;
            this.baseEnd = baseEnd;
            this.height = height;
            this.filled = filled;
            this.showBackEdges = showBackEdges;
        }

        public bool Contains(IntVector2 pixel)
        {
            if (filled)
            {
                return boundingRect.ContainsX(pixel.x) && bottomRectangle.lowerBorder.MinY(pixel.x) <= pixel.y && pixel.y <= topRectangle.upperBorder.MaxY(pixel.x);
            }
            else
            {
                foreach (I1DShape path in border)
                {
                    if (path.Contains(pixel))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Translates the isometric cuboid by the given vector.
        /// </summary>
        public static IsometricCuboid operator +(IsometricCuboid isoRectangle, IntVector2 translation) => isoRectangle.Translate(translation);
        /// <summary>
        /// Translates the isometric cuboid by the given vector.
        /// </summary>
        public static IsometricCuboid operator +(IntVector2 translation, IsometricCuboid isoRectangle) => isoRectangle + translation;
        /// <summary>
        /// Translates the isometric cuboid by the given vector.
        /// </summary>
        public static IsometricCuboid operator -(IsometricCuboid isoRectangle, IntVector2 translation) => isoRectangle + -translation;

        IIsometricShape IIsometricShape.Translate(IntVector2 translation) => Translate(translation);
        /// <summary>
        /// Translates the isometric cuboid by the given vector.
        /// </summary>
        public IsometricCuboid Translate(IntVector2 translation) => new IsometricCuboid(baseStart + translation, baseEnd + translation, height, filled, showBackEdges);

        IIsometricShape IIsometricShape.Flip(FlipAxis axis) => Flip(axis);
        /// <summary>
        /// Reflects the isometric cuboid across the given axis.
        /// </summary>
        public IsometricCuboid Flip(FlipAxis axis) => axis switch
        {
            FlipAxis.None => DeepCopy(),
            FlipAxis.Vertical => new IsometricCuboid(baseStart.Flip(axis), baseEnd.Flip(axis), height, filled, showBackEdges),
            FlipAxis.Horizontal | FlipAxis._45Degrees | FlipAxis.Minus45Degrees
            => throw new ArgumentException($"{nameof(Flip)}() is undefined for {nameof(IsometricCuboid)} across the {axis} axis.", nameof(axis)),
            _ => throw new NotImplementedException($"Unknown / unimplemented FlipAxis: {axis}")
        };

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator()
        {
            if (filled)
            {
                Path lowerBorder = bottomRectangle.lowerBorder;
                Path upperBorder = topRectangle.upperBorder;
                foreach (int x in boundingRect.xRange)
                {
                    foreach (int y in IntRange.InclIncl(lowerBorder.MinY(x), upperBorder.MaxY(x)))
                    {
                        yield return new IntVector2(x, y);
                    }
                }
                yield break;
            }

            foreach (IntVector2 pixel in border.Flatten())
            {
                yield return pixel;
            }
        }

        public static bool operator ==(IsometricCuboid a, IsometricCuboid b)
            => a.bottomRectangle == b.bottomRectangle && a.height == b.height && a.filled == b.filled && a.showBackEdges == b.showBackEdges;
        public static bool operator !=(IsometricCuboid a, IsometricCuboid b) => !(a == b);
        public bool Equals(IsometricCuboid other) => this == other;
        public override bool Equals(object obj) => obj is IsometricCuboid other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(bottomRectangle, height, filled, showBackEdges);

        public override string ToString()
            => $"IsometricCuboid({baseStart}, {baseEnd}, {height}, {(filled ? "filled" : "unfilled")}, {(showBackEdges ? "show back edges" : "don't show back edges")})";

        IIsometricShape IIsometricShape.DeepCopy() => DeepCopy();
        public IsometricCuboid DeepCopy() => new IsometricCuboid(baseStart, baseEnd, height, filled, showBackEdges);
    }
}
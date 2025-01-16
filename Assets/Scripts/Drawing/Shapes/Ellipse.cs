using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PAC.DataStructures;
using PAC.Shapes.Interfaces;

using UnityEngine;

namespace PAC.Shapes
{
    public class Ellipse : I2DShape<Ellipse>, IDeepCopyableShape<Ellipse>, IEquatable<Ellipse>
    {
        // NOTE: For this shape, we work in a coordinate system where integer coordinates refer to the CENTRE of a pixel - e.g. the centre of pixel (0, 0) is (0, 0), not (0.5, 0.5).

        public bool filled { get; set; }

        // These values are calculated and cached when you set the size of the ellipse
        private float xRadius;
        private float yRadius;
        private Vector2 centre;

        /// <summary>True if the ellipse is a circle.</summary>
        public bool isCircle => boundingRect.width == boundingRect.height;

        private IntRect _boundingRect;
        public IntRect boundingRect
        {
            get => _boundingRect;
            set
            {
                _boundingRect = value;

                xRadius = boundingRect.width / 2f;
                yRadius = boundingRect.height / 2f;
                centre = ((Vector2)boundingRect.bottomLeft + boundingRect.topRight) / 2f;
            }
        }

        public int Count => this.Count();

        public Ellipse(IntVector2 corner, IntVector2 oppositeCorner, bool filled) : this(new IntRect(corner, oppositeCorner), filled) { }
        public Ellipse(IntRect boundingRect, bool filled)
        {
            this.boundingRect = boundingRect;
            this.filled = filled;
        }

        /// <summary>
        /// Determines whether (x, y) is inside the filled ellipse (including the border).
        /// </summary>
        private bool IsInside(int x, int y) => IsInside(new IntVector2(x, y));
        /// <summary>
        /// Determines whether the pixel is inside the filled ellipse (including the border).
        /// </summary>
        private bool IsInside(IntVector2 pixel)
        {
            // Manually override 1xn and 2xn case (as otherwise the algorithm doesn't include the top/bottom row in the 2xn case - the 1xn case is just because we might as well include it)
            if (boundingRect.width <= 2)
            {
                return boundingRect.Contains(pixel);
            }
            // Manually override nx1 and nx2 case (as otherwise the algorithm doesn't include the left/right column in the 2xn case - the 1xn case is just because we might as well include it)
            if (boundingRect.height <= 2)
            {
                return boundingRect.Contains(pixel);
            }

            // Manually override 3x3 case (as otherwise the algorithm gives a 3x3 square instead of the more aesthetic 'plus sign')
            if (boundingRect.width == 3 && boundingRect.height == 3)
            {
                IntVector2 centre = boundingRect.bottomLeft + IntVector2.one;
                // This just gives the 'plus sign' shape we want
                return IntVector2.L1Distance(pixel, centre) <= 1;
            }

            return (pixel.x - centre.x) * (pixel.x - centre.x) / (xRadius * xRadius) + (pixel.y - centre.y) * (pixel.y - centre.y) / (yRadius * yRadius) <= 1f;
        }

        public bool Contains(IntVector2 pixel)
        {
            // Filled
            if (filled)
            {
                return IsInside(pixel);
            }

            // Unfilled

            if (!IsInside(pixel))
            {
                return false;
            }
            if (!IsInside(pixel + IntVector2.up) || !IsInside(pixel + IntVector2.down) || !IsInside(pixel + IntVector2.left) || !IsInside(pixel + IntVector2.right))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Translates the ellipse by the given vector.
        /// </summary>
        public static Ellipse operator +(Ellipse ellipse, IntVector2 translation) => ellipse.Translate(translation);
        /// <summary>
        /// Translates the ellipse by the given vector.
        /// </summary>
        public static Ellipse operator +(IntVector2 translation, Ellipse ellipse) => ellipse + translation;
        /// <summary>
        /// Translates the ellipse by the given vector.
        /// </summary>
        public static Ellipse operator -(Ellipse ellipse, IntVector2 translation) => ellipse + -translation;
        /// <summary>
        /// Reflects the ellipse through the origin.
        /// </summary>
        public static Ellipse operator -(Ellipse ellipse) => new Ellipse(-ellipse.boundingRect, ellipse.filled);

        /// <summary>
        /// Translates the ellipse by the given vector.
        /// </summary>
        public Ellipse Translate(IntVector2 translation) => new Ellipse(boundingRect + translation, filled);

        /// <summary>
        /// Reflects the ellipse across the given axis.
        /// </summary>
        public Ellipse Flip(FlipAxis axis) => new Ellipse(boundingRect.Flip(axis), filled);

        /// <summary>
        /// Rotates the ellipse by the given angle.
        /// </summary>
        public Ellipse Rotate(RotationAngle angle) => new Ellipse(boundingRect.Rotate(angle), filled);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator()
        {
            // Manually define horizontal/vertical lines to avoid repeating pixels
            if (boundingRect.width == 1)
            {
                for (int y = boundingRect.bottomLeft.y; y <= boundingRect.topRight.y; y++)
                {
                    yield return new IntVector2(boundingRect.bottomLeft.x, y);
                }
                yield break;
            }
            if (boundingRect.height == 1)
            {
                for (int x = boundingRect.bottomLeft.x; x <= boundingRect.topRight.x; x++)
                {
                    yield return new IntVector2(x, boundingRect.bottomLeft.y);
                }
                yield break;
            }

            // Filled
            if (filled)
            {
                for (int y = boundingRect.bottomLeft.y; y <= boundingRect.topRight.y; y++)
                {
                    int x = Mathf.FloorToInt(centre.x);
                    while (IsInside(x, y))
                    {
                        yield return new IntVector2(x, y);
                        x--;
                    }

                    x = Mathf.FloorToInt(centre.x) + 1;
                    while (IsInside(x, y))
                    {
                        yield return new IntVector2(x, y);
                        x++;
                    }
                }
                yield break;
            }

            // Unfilled

            IntVector2 primaryDirection = IntVector2.right;
            IntVector2 secondaryDirection = new IntVector2(1, -1);
            IntVector2 tertiaryDirection = IntVector2.down;
            IntVector2 start = new IntVector2(boundingRect.bottomLeft.x + boundingRect.width / 2, boundingRect.topRight.y);
            IntVector2 pixel = start;

            int iterations = 0;
            do
            {
                iterations++;

                if (IsInside(pixel + primaryDirection))
                {
                    yield return pixel;
                    pixel += primaryDirection;
                }
                else if (IsInside(pixel + secondaryDirection))
                {
                    yield return pixel;
                    pixel += secondaryDirection;
                }
                else if (IsInside(pixel + tertiaryDirection))
                {
                    yield return pixel;
                    pixel += tertiaryDirection;
                }
                else
                {
                    primaryDirection = primaryDirection.Rotate(RotationAngle._90);
                    secondaryDirection = secondaryDirection.Rotate(RotationAngle._90);
                    tertiaryDirection = tertiaryDirection.Rotate(RotationAngle._90);
                }
            }
            while (pixel != start && iterations < 10_000);
        }

        public static bool operator ==(Ellipse a, Ellipse b) => a.boundingRect == b.boundingRect && a.filled == b.filled;
        public static bool operator !=(Ellipse a, Ellipse b) => !(a == b);
        public bool Equals(Ellipse other) => this == other;
        public override bool Equals(object obj) => obj is Ellipse other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(boundingRect, filled);

        public override string ToString() => "Ellipse(" + boundingRect.bottomLeft + ", " + boundingRect.topRight + ", " + (filled ? "filled" : "unfilled") + ")";

        public Ellipse DeepCopy() => new Ellipse(boundingRect, filled);
    }
}
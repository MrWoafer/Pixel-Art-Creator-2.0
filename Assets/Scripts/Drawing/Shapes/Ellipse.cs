using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PAC.DataStructures;

using UnityEngine;

namespace PAC.Drawing
{
    public static partial class Shapes
    {
        public class Ellipse : I2DShape, IEquatable<Ellipse>
        {
            // NOTE: For this shape, we work in a coordinate system where integer coordinates refer to the CENTRE of a pixel - e.g. the centre of pixel (0, 0) is (0, 0), not (0.5, 0.5).

            public IntVector2 bottomLeft
            {
                get => boundingRect.bottomLeft;
                set => SetValues(value, topRight);
            }
            public IntVector2 topRight
            {
                get => boundingRect.topRight;
                set => SetValues(value, bottomLeft);
            }
            public IntVector2 bottomRight
            {
                get => boundingRect.bottomRight;
                set => SetValues(value, topLeft);
            }
            public IntVector2 topLeft
            {
                get => boundingRect.topLeft;
                set => SetValues(value, bottomRight);
            }

            public bool filled { get; set; }

            // These values are calculated and cached when you set the size of the ellipse
            private float xRadius;
            private float yRadius;
            private Vector2 centre;

            /// <summary>True if the ellipse is a circle.</summary>
            public bool isCircle => boundingRect.width == boundingRect.height;

            public IntRect boundingRect { get; private set; }

            public int Count => ((IEnumerable<IntVector2>)this).Count();

            public Ellipse(IntVector2 corner, IntVector2 oppositeCorner, bool filled)
            {
                boundingRect = new IntRect(IntVector2.zero, IntVector2.zero);
                this.filled = filled;

                xRadius = 0f;
                yRadius = 0f;
                centre = Vector2.zero;

                SetValues(corner, oppositeCorner);
            }

            private void SetValues(IntVector2 corner, IntVector2 oppositeCorner)
            {
                boundingRect = new IntRect(corner, oppositeCorner);

                xRadius = boundingRect.width / 2f;
                yRadius = boundingRect.height / 2f;
                centre = ((Vector2)bottomLeft + topRight) / 2f;
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
                    IntVector2 centre = bottomLeft + IntVector2.one;
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
            public static Ellipse operator -(Ellipse ellipse, IntVector2 translation) => ellipse + (-translation);
            /// <summary>
            /// Reflects the ellipse through the origin.
            /// </summary>
            public static Ellipse operator -(Ellipse ellipse) => new Ellipse(-ellipse.bottomLeft, -ellipse.topRight, ellipse.filled);

            I2DShape I2DShape.Translate(IntVector2 translation) => Translate(translation);
            /// <summary>
            /// Translates the ellipse by the given vector.
            /// </summary>
            public Ellipse Translate(IntVector2 translation) => new Ellipse(bottomLeft + translation, topRight + translation, filled);

            I2DShape I2DShape.Flip(FlipAxis axis) => Flip(axis);
            /// <summary>
            /// Reflects the ellipse across the given axis.
            /// </summary>
            public Ellipse Flip(FlipAxis axis) => new Ellipse(bottomLeft.Flip(axis), topRight.Flip(axis), filled);

            I2DShape I2DShape.Rotate(RotationAngle angle) => Rotate(angle);
            /// <summary>
            /// Rotates the ellipse by the given angle.
            /// </summary>
            public Ellipse Rotate(RotationAngle angle) => new Ellipse(bottomLeft.Rotate(angle), topRight.Rotate(angle), filled);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<IntVector2> GetEnumerator()
            {
                // Manually define horizontal/vertical lines to avoid repeating pixels
                if (boundingRect.width == 1)
                {
                    for (int y = bottomLeft.y; y <= topRight.y; y++)
                    {
                        yield return new IntVector2(bottomLeft.x, y);
                    }
                    yield break;
                }
                if (boundingRect.height == 1)
                {
                    for (int x = bottomLeft.x; x <= topRight.x; x++)
                    {
                        yield return new IntVector2(x, bottomLeft.y);
                    }
                    yield break;
                }

                // Filled
                if (filled)
                {
                    for (int y = bottomLeft.y; y <= topRight.y; y++)
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
                IntVector2 start = new IntVector2(bottomLeft.x + boundingRect.width / 2, topRight.y);
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

            public override int GetHashCode() => HashCode.Combine(bottomLeft, topRight, filled);

            public override string ToString() => "Ellipse(" + bottomLeft + ", " + topRight + ", " + (filled ? "filled" : "unfilled") + ")";

            I2DShape I2DShape.DeepCopy() => DeepCopy();
            public Ellipse DeepCopy() => new Ellipse(bottomLeft, topRight, filled);
        }
    }
}
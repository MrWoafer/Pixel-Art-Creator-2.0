using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PAC.DataStructures;
using PAC.Extensions;
using UnityEngine;

namespace PAC.Drawing
{
    /// <summary>
    /// A class to define how different shapes are drawn.
    /// </summary>
    public static class Shapes
    {
        public interface IShape : IReadOnlyCollection<IntVector2>
        {
            /// <summary>
            /// The smallest IntRect containing the whole shape.
            /// </summary>
            public IntRect boundingRect { get; }

            /// <summary>
            /// Returns whether the pixel is in the shape.
            /// </summary>
            public bool Contains(IntVector2 pixel);
        }

        /// <summary>
        /// A pixel-perfect line between two points, ordered from start to end.
        /// </summary>
        public struct Line : IShape
        {
            // NOTE: For this shape, we work in a coordinate system where integer coordinates refer to the CENTRE of a pixel - e.g. the centre of pixel (0, 0) is (0, 0), not (0.5, 0.5).

            private IntVector2 _start;
            public IntVector2 start
            {
                get => _start;
                set => SetValues(value, end);
            }

            private IntVector2 _end;
            public IntVector2 end
            {
                get => _end;
                set => SetValues(start, value);
            }

            // These values are calculated and cached when you set the start/end of the line
            /// <summary>
            /// The start point of the imaginary non-pixel-perfect line. We adjust these to the outside corners of the start/end pixels so that the line will always use blocks of
            /// equal size when possible.
            /// </summary>
            private Vector2 imaginaryStart;
            /// <summary>
            /// The start point of the imaginary non-pixel-perfect line. We adjust these to the outside corners of the start/end pixels so that the line will always use blocks of
            /// equal size when possible.
            /// </summary>
            private Vector2 imaginaryEnd;
            /// <summary>
            /// The gradient of the imaginary non-pixel-perfect line.
            /// </summary>
            private float imaginaryGradient;
            /// <summary>
            /// Whether the line is more horizontal than vertical. If the line is at +/- 45 degrees then it is true.
            /// </summary>
            private bool isMoreHorizontal;

            public Line reverse => new Line(end, start);

            /// <summary>
            /// Whether this line is divided into blocks of a constant size. E.g. a 12x4 line is perfect as it is drawn as 4 horizontal blocks of 3 pixels.
            /// </summary>
            public bool isPerfect => isMoreHorizontal ? (end.x - start.x + 1) % (end.y - start.y + 1) == 0 : (end.y - start.y + 1) % (end.x - start.x + 1) == 0;

            public IntRect boundingRect => new IntRect(start, end);

            public int Count => IntVector2.SupDistance(start, end) + 1;

            public Line(IntVector2 start, IntVector2 end)
            {
                // Initialise fields before we actually define them in SetValues()
                _start = start;
                _end = end;
                imaginaryStart = start;
                imaginaryEnd = end;
                imaginaryGradient = 0f;
                isMoreHorizontal = false;

                SetValues(start, end);
            }

            private void SetValues(IntVector2 start, IntVector2 end)
            {
                _start = start;
                _end = end;

                if (start == end)
                {
                    imaginaryStart = start;
                    imaginaryEnd = end;
                }
                if (start.x == end.x)
                {
                    // end is directly above start
                    if (start.y < end.y)
                    {
                        imaginaryStart = start + 0.5f * Vector2.down;
                        imaginaryEnd = end + 0.5f * Vector2.up;
                    }
                    // end is directly below start
                    else
                    {
                        imaginaryStart = start + 0.5f * Vector2.up;
                        imaginaryEnd = end + 0.5f * Vector2.down;
                    }
                }
                else if (start.y == end.y)
                {
                    // end is directly to the right of start
                    if (start.x < end.x)
                    {
                        imaginaryStart = start + 0.5f * Vector2.left;
                        imaginaryEnd = end + 0.5f * Vector2.right;
                    }
                    // end is directly to the left of start
                    else
                    {
                        imaginaryStart = start + 0.5f * Vector2.right;
                        imaginaryEnd = end + 0.5f * Vector2.left;
                    }
                }
                if (start.x < end.x)
                {
                    // end is to top-right of start
                    if (start.y < end.y)
                    {
                        imaginaryStart = start - 0.5f * Vector2.one;
                        imaginaryEnd = end + 0.5f * Vector2.one;
                    }
                    // end is to bottom-right of start
                    else
                    {
                        imaginaryStart = start + 0.5f * new Vector2(-1, 1);
                        imaginaryEnd = end + 0.5f * new Vector2(1, -1);
                    }
                }
                else
                {
                    // end is to top-left of start
                    if (start.y < end.y)
                    {
                        imaginaryStart = start + 0.5f * new Vector2(1, -1);
                        imaginaryEnd = end + 0.5f * new Vector2(-1, 1);
                    }
                    // end is to bottom-left of start
                    else
                    {
                        imaginaryStart = start + 0.5f * Vector2.one;
                        imaginaryEnd = end - 0.5f * Vector2.one;
                    }
                }

                // True iff 1 >= gradient >= -1, i.e. the line is more (or equally) horizontal than vertical
                isMoreHorizontal = Math.Abs(end.y - start.y) <= Math.Abs(end.x - start.x);

                if (imaginaryStart.x == imaginaryEnd.x)
                {
                    if (imaginaryStart.y == imaginaryEnd.y)
                    {
                        imaginaryGradient = float.NaN;
                    }
                    else
                    {
                        imaginaryGradient = float.PositiveInfinity;
                    }
                }
                else if (isMoreHorizontal)
                {
                    imaginaryGradient = (imaginaryEnd.y - imaginaryStart.y) / (imaginaryEnd.x - imaginaryStart.x);
                }
                else
                {
                    imaginaryGradient = (imaginaryEnd.x - imaginaryStart.x) / (imaginaryEnd.y - imaginaryStart.y);
                }
            }

            /// <summary>
            /// Returns whether the pixel is in the line.
            /// </summary>
            public bool Contains(IntVector2 pixel)
            {
                int index = isMoreHorizontal ? (pixel.x - start.x) * Math.Sign(end.x - start.x) : (pixel.y - start.y) * Math.Sign(end.y - start.y);
                if (index < 0 || index >= Count)
                {
                    return false;
                }
                return this[index] == pixel;
            }

            public IntVector2 this[int index]
            {
                get
                {
                    if (index < 0)
                    {
                        throw new IndexOutOfRangeException("Index cannot be negative. Index: " + index);
                    }
                    if (index >= Count)
                    {
                        throw new IndexOutOfRangeException("Index cannot be >= Count. Index: " + index + ". Count: " + Count);
                    }

                    if (index == 0)
                    {
                        return start;
                    }
                    if (index == Count - 1)
                    {
                        return end;
                    }

                    // Index is in the segment from start to end (exclusive).

                    if (start.x == end.x)
                    {
                        // Single point
                        if (start.y == end.y)
                        {
                            return start;
                        }

                        // Vertical line
                        return new IntVector2(start.x, start.y + index * Math.Sign(end.y - start.y));
                    }
                    // Horizontal line
                    if (start.y == end.y)
                    {
                        return new IntVector2(start.x + index * Math.Sign(end.x - start.x), start.y);
                    }

                    if (isMoreHorizontal)
                    {
                        int x = start.x + index * Math.Sign(end.x - start.x);

                        // Line equation is:
                        //      gradient = (y - lineStart.y) / (x - lineStart.x)
                        // We plug in x + 0.5 (the + 0.5 if because we use the centre of the pixel)
                        float y = (x - imaginaryStart.x) * imaginaryGradient + imaginaryStart.y;

                        // Deal with edge case of y being exactly on the border of two pixels
                        // We always pull the y so it's closer to the endpoint x is closest to (e.g. if x is closer to start.x than end.x then we round y up/down to whichever is closer to start.y)
                        if ((x - imaginaryStart.x) * (imaginaryEnd.y - imaginaryStart.y) % (imaginaryEnd.x - imaginaryStart.x) == 0)
                        {
                            // If we're in the first half of the line
                            if (2 * Math.Abs(x - start.x) <= Math.Abs(end.x - start.x))
                            {
                                return new IntVector2(x, Mathf.FloorToInt(y) + (start.y < end.y ? 0 : 1));
                            }
                            else
                            {
                                return new IntVector2(x, Mathf.FloorToInt(y) + (start.y < end.y ? 1 : 0));
                            }
                        }
                        else
                        {
                            return new IntVector2(x, Mathf.RoundToInt(y));
                        }
                    }
                    else
                    {
                        int y = start.y + index * Math.Sign(end.y - start.y);

                        // Line equation is:
                        //      gradient = (x - lineStart.x) / (y - lineStart.y)
                        // We plug in y + 0.5 (the + 0.5 if because we use the centre of the pixel)
                        float x = (y - imaginaryStart.y) * imaginaryGradient + imaginaryStart.x;

                        // Deal with edge case of x being exactly on the border of two pixels
                        // We always pull the x so it's closer to the endpoint y is closest to (e.g. if y is closer to start.y than end.y then we round x up/down to whichever is closer to start.x)
                        if ((y - imaginaryStart.y) * (imaginaryEnd.x - imaginaryStart.x) % (imaginaryEnd.y - imaginaryStart.y) == 0)
                        {
                            // If we're in the first half of the line
                            if (2 * Math.Abs(y - start.y) <= Math.Abs(end.y - start.y))
                            {
                                return new IntVector2(Mathf.FloorToInt(x) + (start.x < end.x ? 0 : 1), y);
                            }
                            else
                            {
                                return new IntVector2(Mathf.FloorToInt(x) + (start.x < end.x ? 1 : 0), y);
                            }
                        }
                        else
                        {
                            return new IntVector2(Mathf.RoundToInt(x), y);
                        }
                    }
                }
            }
            public IEnumerable<IntVector2> this[Range range]
            {
                get
                {
                    foreach (int i in range.AsIEnumerable(Count))
                    {
                        yield return this[i];
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<IntVector2> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return this[i];
                }
            }

            public static bool operator !=(Line a, Line b) => !(a == b);
            public static bool operator ==(Line a, Line b)
            {
                return a.start == b.start && a.end == b.end;
            }
            public override bool Equals(object obj)
            {
                if (obj == null || !GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    return this == (Line)obj;
                }
            }

            public override int GetHashCode() => HashCode.Combine(start, end);

            public override string ToString() => "Line(" + start + ", " + end + ")";
        }

        /// <summary>
        /// <para>
        /// Represents a sequence of Lines.
        /// The lines must connect, meaning the end of each line must be equal to or adjacent to (including diagonally) the start of the next line.
        /// </para>
        /// <para>
        /// When iterating through the Path, if the end of a line equals the start of the next line, that pixel with not be double-counted. Other than in that case, pixels can be double-counted.
        /// </para>
        /// </summary>
        public class Path : IShape
        {
            private List<Line> _lines = new List<Line>();
            public ReadOnlyCollection<Line> lines => _lines.AsReadOnly();

            /// <summary>
            /// Whether the end of the last line is adjacent to (including diagonally) the start of the first line.
            /// </summary>
            public bool isLoop => IntVector2.SupDistance(_lines[0].start, _lines[^1].end) <= 1;

            /// <summary>
            /// Whether the path crosses itself. Does not include the end of one line being equal to the start of the next.
            /// </summary>
            public bool selfIntersects
            {
                get
                {
                    HashSet<IntVector2> visited = new HashSet<IntVector2>();
                    foreach (IntVector2 point in this)
                    {
                        if (visited.Contains(point))
                        {
                            return true;
                        }
                        visited.Add(point);
                    }
                    return false;
                }
            }

            public IntRect boundingRect => IntRect.BoundingRect(from line in _lines select line.boundingRect);

            public int Count
            {
                get
                {
                    // First line
                    int count = _lines[0].Count;
                    if (_lines.Count == 1)
                    {
                        return count;
                    }

                    // Middle _lines (not first or last)
                    for (int i = 1; i < _lines.Count - 1; i++)
                    {
                        if (_lines[i - 1].end == _lines[i].start)
                        {
                            count += _lines[i].Count - 1;
                        }
                        else
                        {
                            count += _lines[i].Count;
                        }
                    }

                    // Last line
                    count += _lines[^1].Count;
                    if (_lines[^2].end == _lines[^1].start)
                    {
                        count--;
                    }
                    if (_lines[^1].end == _lines[0].start)
                    {
                        count--;
                    }

                    return count;
                }
            }

            // To prevent creation of paths from 0 points/lines. (Those constructors have checks anyway but this stops the 'ambiguous call' error those give when trying to use an empty constructor)
            private Path() { }
            /// <summary>
            /// Creates a path through the given points.
            /// </summary>
            public Path(params IntVector2[] points)
            {
                if (points.Length == 0)
                {
                    throw new ArgumentException("Cannot create a path from 0 points.", "points");
                }

                if (points.Length == 1)
                {
                    _lines.Add(new Line(points[0], points[0]));
                }

                for (int i = 0; i < points.Length - 1; i++)
                {
                    _lines.Add(new Line(points[i], points[i + 1]));
                }
            }
            /// <summary>
            /// The lines must connect, meaning the end of each line must be equal to or adjacent to (including diagonally) the start of the next line.
            /// </summary>
            public Path(params Line[] lines)
            {
                if (lines.Length == 0)
                {
                    throw new ArgumentException("Cannot create a path from 0 lines.", "lines");
                }

                this._lines.Add(lines[0]);
                for (int i = 1; i < lines.Length; i++)
                {
                    if (IntVector2.SupDistance(lines[i - 1].end, lines[i].start) > 1)
                    {
                        throw new ArgumentException("Lines " + (i - 1) + " and " + i + " do not connect.", "lines");
                    }
                    this._lines.Add(lines[i]);
                }
            }

            public bool Contains(IntVector2 pixel)
            {
                foreach (Line line in _lines)
                {
                    if (line.Contains(pixel))
                    {
                        return true;
                    }
                }
                return false;
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
            public IEnumerator<IntVector2> GetEnumerator()
            {
                // First line
                foreach (IntVector2 pixel in _lines[0])
                {
                    yield return pixel;
                }
                if (_lines.Count == 1)
                {
                    yield break;
                }

                // Middle _lines (not first or last)
                for (int i = 1; i < _lines.Count - 1; i++)
                {
                    int start = 0;
                    if (_lines[i - 1].end == _lines[i].start)
                    {
                        start = 1;
                    }
                    foreach (IntVector2 pixel in _lines[i][start..])
                    {
                        yield return pixel;
                    }
                }

                // Last line
                {
                    int start = 0;
                    int end = _lines[^1].Count;
                    if (_lines[^2].end == _lines[^1].start)
                    {
                        start = 1;
                    }
                    if (_lines[^1].end == _lines[0].start)
                    {
                        end = _lines[^1].Count - 1;
                    }
                    foreach (IntVector2 pixel in _lines[^1][start..end])
                    {
                        yield return pixel;
                    }
                }
            }

            public static bool operator !=(Path a, Path b) => !(a == b);
            public static bool operator ==(Path a, Path b)
            {
                if (a._lines.Count != b._lines.Count)
                {
                    return false;
                }

                for (int i = 0; i < a._lines.Count; i++)
                {
                    if (a._lines[i] != b._lines[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            public override bool Equals(object obj)
            {
                if (obj == null || !GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    return this == (Path)obj;
                }
            }

            public override int GetHashCode() => _lines.GetHashCode();

            public override string ToString() => "Path(" + string.Join(", ", _lines) + ")";
        }

        public struct Rectangle : IShape
        {
            public IntVector2 bottomLeft
            {
                get => boundingRect.bottomLeft;
                set => boundingRect = new IntRect(value, topRight);
            }
            public IntVector2 topRight
            {
                get => boundingRect.topRight;
                set => boundingRect = new IntRect(value, bottomLeft);
            }
            public IntVector2 bottomRight
            {
                get => boundingRect.bottomRight;
                set => boundingRect = new IntRect(value, topLeft);
            }
            public IntVector2 topLeft
            {
                get => boundingRect.topLeft;
                set => boundingRect = new IntRect(value, bottomRight);
            }

            public bool filled { get; set; }

            public int width => boundingRect.width;
            public int height => boundingRect.height;

            /// <summary>True if the rect is a square.</summary>
            public bool isSquare => width == height;

            public IntRect boundingRect { get; private set; }

            public int Count
            {
                get
                {
                    // Filled
                    if (filled)
                    {
                        return width * height;
                    }
                    // Unfilled
                    if (width == 1)
                    {
                        return height;
                    }
                    if (height == 1)
                    {
                        return width;
                    }
                    return 2 * (width + height) - 4;
                }
            }

            public Rectangle(IntVector2 corner, IntVector2 oppositeCorner, bool filled)
            {
                boundingRect = new IntRect(corner, oppositeCorner);
                this.filled = filled;
            }

            public bool Contains(IntVector2 pixel)
            {
                if (filled)
                {
                    return boundingRect.Contains(pixel);
                }
                return boundingRect.Contains(pixel) && !(pixel > bottomLeft && pixel < topRight);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<IntVector2> GetEnumerator()
            {
                // Filled - start with the bottom row, read left to right, then the next row, etc.
                if (filled)
                {
                    for (int y = bottomLeft.y; y <= topRight.y; y++)
                    {
                        for (int x = bottomLeft.x; x <= topRight.x; x++)
                        {
                            yield return new IntVector2(x, y);
                        }
                    }
                    yield break;
                }

                // Unfilled - go clockwise, starting at bottom-left

                // Up the left side
                for (int y = bottomLeft.y; y <= topRight.y; y++)
                {
                    yield return new IntVector2(bottomLeft.x, y);
                }

                // Rectangle has width 1
                if (bottomLeft.x == topRight.x)
                {
                    yield break;
                }

                // Along the top side (left to right)
                for (int x = bottomLeft.x + 1; x <= topRight.x; x++)
                {
                    yield return new IntVector2(x, topRight.y);
                }

                // Rectangle has height 1
                if (bottomLeft.y == topRight.y)
                {
                    yield break;
                }

                // Down the right side
                for (int y = topRight.y - 1; y >= bottomLeft.y; y--)
                {
                    yield return new IntVector2(topRight.x, y);
                }

                // Along the bottom side (right to left)
                for (int x = topRight.x - 1; x >= bottomLeft.x + 1; x--)
                {
                    yield return new IntVector2(x, bottomLeft.y);
                }
            }

            public static bool operator !=(Rectangle a, Rectangle b) => !(a == b);
            public static bool operator ==(Rectangle a, Rectangle b)
            {
                return a.boundingRect == b.boundingRect && a.filled == b.filled;
            }
            public override bool Equals(object obj)
            {
                if (obj == null || !GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    return this == (Rectangle)obj;
                }
            }

            public override int GetHashCode() => HashCode.Combine(bottomLeft, topRight, filled);

            public override string ToString() => "Rectangle(" + bottomLeft + ", " + topRight + ", " + (filled ? "filled" : "unfilled") + ")";
        }

        public struct Ellipse : IShape
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
            public int width { get; private set; }
            public int height { get; private set; }
            private float xRadius;
            private float yRadius;
            private Vector2 centre;

            /// <summary>True if the ellipse is a circle.</summary>
            public bool isCircle => width == height;

            public IntRect boundingRect { get; private set; }

            public int Count => ((IEnumerable<IntVector2>)this).Count();

            public Ellipse(IntVector2 corner, IntVector2 oppositeCorner, bool filled)
            {
                boundingRect = new IntRect(IntVector2.zero, IntVector2.zero);
                this.filled = filled;

                width = 0;
                height = 0;

                xRadius = 0f;
                yRadius = 0f;
                centre = Vector2.zero;

                SetValues(corner, oppositeCorner);
            }

            private void SetValues(IntVector2 corner, IntVector2 oppositeCorner)
            {
                boundingRect = new IntRect(corner, oppositeCorner);

                width = boundingRect.width;
                height = boundingRect.height;

                xRadius = width / 2f;
                yRadius = height / 2f;
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
                if (width <= 2)
                {
                    return boundingRect.Contains(pixel);
                }
                // Manually override nx1 and nx2 case (as otherwise the algorithm doesn't include the left/right column in the 2xn case - the 1xn case is just because we might as well include it)
                if (height <= 2)
                {
                    return boundingRect.Contains(pixel);
                }

                // Manually override 3x3 case (as otherwise the algorithm gives a 3x3 square instead of the more aesthetic 'plus sign')
                if (width == 3 && height == 3)
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

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<IntVector2> GetEnumerator()
            {
                // Manually define horizontal/vertical lines to avoid repeating pixels
                if (width == 1)
                {
                    for (int y = bottomLeft.y; y <= topRight.y; y++)
                    {
                        yield return new IntVector2(bottomLeft.x, y);
                    }
                    yield break;
                }
                if (height == 1)
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
                IntVector2 start = new IntVector2(bottomLeft.x + width / 2, topRight.y);
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

            public static bool operator !=(Ellipse a, Ellipse b) => !(a == b);
            public static bool operator ==(Ellipse a, Ellipse b)
            {
                return a.boundingRect == b.boundingRect && a.filled == b.filled;
            }
            public override bool Equals(object obj)
            {
                if (obj == null || !GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    return this == (Ellipse)obj;
                }
            }

            public override int GetHashCode() => HashCode.Combine(bottomLeft, topRight, filled);

            public override string ToString() => "Ellipse(" + bottomLeft + ", " + topRight + ", " + (filled ? "filled" : "unfilled") + ")";
        }

        public struct RightTriangle : IShape
        {
            public enum RightAngleLocation
            {
                Bottom = -1,
                Top = 1,
                Left = -2,
                Right = 2
            }

            private IntVector2 _bottomCorner;
            /// <summary>
            /// The lower of the two corners that don't contain the right angle. This should always be distinct from topCorner, unless the triangle is a single point.
            /// </summary>
            public IntVector2 bottomCorner
            {
                get => _bottomCorner;
                set
                {
                    if (value.y <= _topCorner.y)
                    {
                        _bottomCorner = value;
                    }
                    else
                    {
                        _bottomCorner = _topCorner;
                        _topCorner = value;
                    }
                }
            }
            private IntVector2 _topCorner;
            /// <summary>
            /// The higher of the two corners that don't contain the right angle. This should always be distinct from bottomCorner, unless the triangle is a single point.
            /// </summary>
            public IntVector2 topCorner
            {
                get => _topCorner;
                set
                {
                    if (value.y >= _bottomCorner.y)
                    {
                        _topCorner = value;
                    }
                    else
                    {
                        _topCorner = _bottomCorner;
                        _bottomCorner = value;
                    }
                }
            }
            /// <summary>
            /// The left-most of the two corners that don't contain the right angle. This should always be distinct from rightCorner, unless the triangle is a single point.
            /// </summary>
            public IntVector2 leftCorner
            {
                get => _bottomCorner.x <= _topCorner.x ? _bottomCorner : _topCorner;
                set
                {
                    if (leftCorner == _bottomCorner)
                    {
                        bottomCorner = value;
                    }
                    else
                    {
                        topCorner = value;
                    }
                }
            }
            /// <summary>
            /// The right-most of the two corners that don't contain the right angle. This should always be distinct from leftCorner, unless the triangle is a single point.
            /// </summary>
            public IntVector2 rightCorner
            {
                // We do > here and <= in leftCorner to ensure that if the triangle's corners have the same x coord then leftCorner and rightCorner are still different corners.
                get => _bottomCorner.x > _topCorner.x ? _bottomCorner : _topCorner;
                set
                {
                    if (rightCorner == _bottomCorner)
                    {
                        bottomCorner = value;
                    }
                    else
                    {
                        topCorner = value;
                    }
                }
            }
            /// <summary>
            /// The corner that contains the right angle.
            /// </summary>
            public IntVector2 rightAngleCorner
            {
                get
                {
                    switch (rightAngleLocation)
                    {
                        case RightAngleLocation.Bottom: return new IntVector2(topCorner.x, bottomCorner.y);
                        case RightAngleLocation.Top: return new IntVector2(bottomCorner.x, topCorner.y);
                        case RightAngleLocation.Left: return new IntVector2(leftCorner.x, rightCorner.y);
                        case RightAngleLocation.Right: return new IntVector2(rightCorner.x, leftCorner.y);
                        default: throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation);
                    }
                }
            }

            public RightAngleLocation rightAngleLocation { get; set; }

            public bool filled { get; set; }

            public int width => boundingRect.width;
            public int height => boundingRect.height;

            /// <summary>True if the triangle is an isosceles triangle.</summary>
            public bool isIsosceles => width == height;

            public IntRect boundingRect => new IntRect(bottomCorner, topCorner);

            public int Count
            {
                get
                {
                    if (width == 1)
                    {
                        return height;
                    }
                    if (height == 1)
                    {
                        return width;
                    }

                    if (!filled)
                    {
                        return GetPath().Count;
                    }

                    if (width >= height)
                    {
                        // The vertical side of the triangle won't be counted in the loop
                        int count = height;
                        // Go along the hypotenuse
                        foreach (IntVector2 pixel in GetPath().lines[0])
                        {
                            count += Math.Abs(pixel.y - rightAngleCorner.y) + 1;
                        }
                        return count;
                    }
                    else
                    {
                        // The horizontal side of the triangle won't be counted in the loop
                        int count = width;
                        // Go along the hypotenuse
                        foreach (IntVector2 pixel in GetPath().lines[0])
                        {
                            count += Math.Abs(pixel.x - rightAngleCorner.x) + 1;
                        }
                        return count;
                    }
                }
            }

            public RightTriangle(IntVector2 corner, IntVector2 oppositeCorner, RightAngleLocation rightAngleLocation, bool filled)
            {
                if (corner.y <= oppositeCorner.y)
                {
                    _bottomCorner = corner;
                    _topCorner = oppositeCorner;
                }
                else
                {
                    _bottomCorner = oppositeCorner;
                    _topCorner = corner;
                }

                this.rightAngleLocation = rightAngleLocation;
                this.filled = filled;
            }

            public bool Contains(IntVector2 pixel)
            {
                if (!filled)
                {
                    return GetPath().Contains(pixel);
                }

                // Non-zero winding number algorithm
                if (width <= 1 || height <= 1)
                {
                    return boundingRect.Contains(pixel);
                }

                Path path = GetPath();
                int previousY = path.First().y + (path.lines[0].start.y < path.lines[0].end.y ? -1 : 1);
                int windingNumber = 0;
                foreach (IntVector2 borderPixel in path)
                {
                    if (borderPixel == pixel)
                    {
                        return true;
                    }
                    if (borderPixel.y != bottomCorner.y && borderPixel.y != topCorner.y)
                    {
                        if (borderPixel.y == pixel.y && borderPixel.x > pixel.x)
                        {
                            windingNumber += borderPixel.y - previousY;
                        }
                    }
                    previousY = borderPixel.y;
                }
                return windingNumber != 0;
            }

            private Path GetPath()
            {
                // Single points / vertical lines / horizontal lines
                if (bottomCorner.x == topCorner.x || bottomCorner.y == topCorner.y)
                {
                    return new Path (new Line(bottomCorner, topCorner));
                }

                IntVector2 startCorner = bottomCorner;
                IntVector2 endCorner = topCorner;
                IntVector2 startAdjusted;
                IntVector2 endAdjusted;

                if (rightAngleLocation == RightAngleLocation.Bottom)
                {
                    startAdjusted = bottomCorner + IntVector2.up;
                    endAdjusted = topCorner + (startCorner == leftCorner ? IntVector2.left : IntVector2.right);
                }
                else if (rightAngleLocation == RightAngleLocation.Top)
                {
                    startAdjusted = bottomCorner + (endCorner == leftCorner ? IntVector2.left : IntVector2.right);
                    endAdjusted = topCorner + IntVector2.down;
                }
                else if (rightAngleLocation == RightAngleLocation.Left)
                {
                    startAdjusted = bottomCorner + (startCorner == leftCorner ? IntVector2.right : IntVector2.up);
                    endAdjusted = topCorner + (startCorner == leftCorner ? IntVector2.down : IntVector2.right);
                }
                else if (rightAngleLocation == RightAngleLocation.Right)
                {
                    startAdjusted = bottomCorner + (startCorner == leftCorner ? IntVector2.up : IntVector2.left);
                    endAdjusted = topCorner + (startCorner == leftCorner ? IntVector2.left : IntVector2.down);
                }
                else
                {
                    throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation);
                }

                // This is to ensure reflective invariance
                // If width >= height, we draw the hypotenuse starting from the corner with the same y coord as the right angle corner
                // If width < height, we draw the hypotenuse starting from the corner with the same x coord as the right angle corner
                if ((width < height) != (startCorner.y != rightAngleCorner.y))
                {
                    IntVector2 temp;

                    temp = startCorner;
                    startCorner = endCorner;
                    endCorner = temp;

                    temp = startAdjusted;
                    startAdjusted = endAdjusted;
                    endAdjusted = temp;
                }

                // The line order is start corner -> end corner -> right angle corner -> start corner

                return new Path(
                        new Line(startAdjusted, endAdjusted),
                        new Line(endCorner, rightAngleCorner),
                        new Line(rightAngleCorner, startCorner)
                    );
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<IntVector2> GetEnumerator()
            {
                Path path = GetPath();
                foreach (IntVector2 pixel in path)
                {
                    yield return pixel;
                }

                if (!filled)
                {
                    yield break;
                }

                // either up, left or right
                IntVector2 directionToTopCorner = IntVector2.Simplify(topCorner - rightAngleCorner);
                // either down, left or right
                IntVector2 directionToBottomCorner = IntVector2.Simplify(bottomCorner - rightAngleCorner);
                int iterations = 0;

                for(IntVector2 rowStart = rightAngleCorner + directionToTopCorner + directionToBottomCorner; !path.Contains(rowStart) && iterations < 10_000; rowStart += directionToTopCorner)
                {
                    for (IntVector2 pixel = rowStart; !path.Contains(pixel) && iterations < 10_000; pixel += directionToBottomCorner)
                    {
                        iterations++;
                        yield return pixel;
                    }
                }
            }

            public static bool operator !=(RightTriangle a, RightTriangle b) => !(a == b);
            public static bool operator ==(RightTriangle a, RightTriangle b)
            {
                return a.bottomCorner == b.bottomCorner && a.topCorner == b.topCorner && a.rightAngleCorner == b.rightAngleCorner && a.filled == b.filled;
            }
            public override bool Equals(object obj)
            {
                if (obj == null || !GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    return this == (RightTriangle)obj;
                }
            }

            public override int GetHashCode() => HashCode.Combine(bottomCorner, topCorner, rightAngleLocation, filled);

            public override string ToString() => "RightTriangle(" + bottomCorner + ", " + topCorner + ", " + rightAngleLocation + ", " + (filled ? "filled" : "unfilled") + ")";
        }

        public static Texture2D LineTex(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour)
        {
            Texture2D tex = Tex2DSprite.BlankTexture(texWidth, texHeight);

            foreach (IntVector2 pixel in new Line(start, end))
            {
                tex.SetPixel(pixel, colour);
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D RectangleTex(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour, bool filled)
        {
            Texture2D tex = Tex2DSprite.BlankTexture(texWidth, texHeight);

            foreach (IntVector2 pixel in new Rectangle(start, end, filled))
            {
                tex.SetPixel(pixel, colour);
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D Square(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour, bool filled, bool stayWithinImageBounds)
        {
            return RectangleTex(texWidth, texHeight, start, SnapEndCoordToSquare(texWidth, texHeight, start, end, stayWithinImageBounds), colour, filled);
        }

        /// <summary>
        /// Snaps the end coord so that the rect it forms with the start coord is a square.
        /// </summary>
        public static IntVector2 SnapEndCoordToSquare(IntVector2 start, IntVector2 end)
        {
            int sideLength = Mathf.Max(Mathf.Abs(end.x - start.x), Mathf.Abs(end.y - start.y));

            return start + new IntVector2(sideLength * (int)Mathf.Sign(end.x - start.x), sideLength * (int)Mathf.Sign(end.y - start.y));
        }
        /// <summary>
        /// Snaps the end coord so that the rect it forms with the start coord is a square.
        /// </summary>
        public static IntVector2 SnapEndCoordToSquare(int texWidth, int texHeight, IntVector2 start, IntVector2 end, bool stayWithinImageBounds)
        {
            int width = Mathf.Abs(end.x - start.x);
            int height = Mathf.Abs(end.y - start.y);

            int sideLength = Mathf.Max(width, height);

            do
            {
                end = start + new IntVector2(sideLength * (int)Mathf.Sign(end.x - start.x), sideLength * (int)Mathf.Sign(end.y - start.y));
                sideLength--;
            }
            while (stayWithinImageBounds && !new IntRect(IntVector2.zero, new IntVector2(texWidth - 1, texHeight - 1)).Contains(end) && sideLength >= 0);

            return end;
        }

        public static Texture2D EllipseTex(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour, bool filled)
        {
            Texture2D tex = Tex2DSprite.BlankTexture(texWidth, texHeight);

            foreach (IntVector2 pixel in new Ellipse(start, end, filled))
            {
                tex.SetPixel(pixel, colour);
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D Circle(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour, bool filled, bool stayWithinImageBounds)
        {
            return EllipseTex(texWidth, texHeight, start, SnapEndCoordToSquare(texWidth, texHeight, start, end, stayWithinImageBounds), colour, filled);
        }

        public static Texture2D RightTriangleTex(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour, bool rightAngleOnBottom, bool filled)
        {
            Texture2D tex = Tex2DSprite.BlankTexture(texWidth, texHeight);

            foreach (IntVector2 pixel in new RightTriangle(start, end, rightAngleOnBottom ? RightTriangle.RightAngleLocation.Bottom : RightTriangle.RightAngleLocation.Top, filled))
            {
                tex.SetPixel(pixel, colour);
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D Diamond(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour, bool filled)
        {
            if (!new IntRect(start, end).isSquare)
            {
                throw new System.NotImplementedException("Not yet implemented for non-square diamond dimensions.");
            }
            if (Mathf.Abs(start.x - end.x) % 2 == 1)
            {
                throw new System.NotImplementedException("Not yet implemented for diamonds of even width/height.");
            }

            IntVector2 bottomLeft = new IntVector2(Mathf.Min(start.x, end.x), Mathf.Min(start.y, end.y));
            IntVector2 topRight = new IntVector2(Mathf.Max(start.x, end.x), Mathf.Max(start.y, end.y));
            IntVector2 centre = (bottomLeft + topRight) / 2;
            int radius = Mathf.Abs(start.x - end.x) / 2;

            Texture2D tex = Tex2DSprite.BlankTexture(texWidth, texHeight);

            if (filled)
            {
                for (int x = bottomLeft.x; x <= topRight.x; x++)
                {
                    for (int y = bottomLeft.y; y <= topRight.y; y++)
                    {
                        int offX = x - centre.x;
                        int offY = y - centre.y;
                        if (Mathf.Abs(offX + offY) <= radius && Mathf.Abs(offX - offY) <= radius)
                        {
                            tex.SetPixel(x, y, colour);
                        }
                    }
                }
            }
            else
            {
                throw new System.NotImplementedException("Not yet implemented for unfilled diamonds.");
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D IsoRectangle(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour, bool filled)
        {
            Texture2D tex = IsoRectangleOnExistingTex(Tex2DSprite.BlankTexture(texWidth, texHeight), texWidth, texHeight, start, end, colour, filled, true);
            tex.Apply();
            return tex;
        }
        /// <summary>
        /// Draws the isometric rectangle on the given texture.
        /// </summary>
        private static Texture2D IsoRectangleOnExistingTex(Texture2D texture, int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour, bool filled, bool drawTopLines)
        {
            return IsoRectangleOnExistingTexReturnCorners(texture, texWidth, texHeight, start, end, colour, filled, drawTopLines).Item1;
        }
        /// <summary>
        /// Draws the isometric rectangle on the given texture and returns the corners in the given order: left, top, right, bottom.
        /// </summary>
        private static Tuple<Texture2D, IntVector2, IntVector2, IntVector2, IntVector2> IsoRectangleOnExistingTexReturnCorners(Texture2D texture, int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour, bool filled, bool drawTopLines)
        {
            if (start == end)
            {
                texture.SetPixel(start, colour);
                texture.Apply();
                return new Tuple<Texture2D, IntVector2, IntVector2, IntVector2, IntVector2>(texture, start, start, start, start);
            }

            IntRect rect = new IntRect(IntVector2.zero, new IntVector2(texWidth - 1, texHeight - 1));

            /// Relabel start/end corners so that start is the left corner and end is the right corner.
            if (start.x > end.x)
            {
                IntVector2 temp = start;
                start = end;
                end = temp;
            }

            /// If start is the left corner and end is the right (instead of top/bottom)
            if (2 * Mathf.Abs(end.y - start.y) <= Mathf.Abs(end.x - start.x))
            {
                Vector2 lineStart = new Vector2(start.x, start.y + 1f);
                Vector2 lineEnd = new Vector2(end.x + 1f, end.y + 1f);
                bool startOf2PixelBlockStartValueStart = true;
                bool startOf2PixelBlockStartValueEnd = true;

                int cornerType = (end.x - start.x + 1 + (end.y - start.y) * 2) % 4;
                if (cornerType == 0 || cornerType == 3)
                {
                    lineStart += new Vector2(1f, -1f);
                    lineEnd += new Vector2(-1f, -1f);

                    startOf2PixelBlockStartValueStart = false;
                    startOf2PixelBlockStartValueEnd = false;
                }
                else if (cornerType == 1)
                {
                    lineEnd += new Vector2(-1f, -1f);

                    startOf2PixelBlockStartValueEnd = false;
                }

                float meetingX = (lineStart.x + lineEnd.x) / 2f + lineStart.y - lineEnd.y;
                float meetingY = (meetingX - lineEnd.x) / 2f + lineEnd.y;
                IntVector2 lowerMeetingPoint = new IntVector2(Mathf.FloorToInt(meetingX), Mathf.FloorToInt(meetingY));
                IntVector2 upperMeetingPoint = end - (lowerMeetingPoint - start);

                int x = start.x;
                int y = start.y;
                bool startOf2PixelBlock = startOf2PixelBlockStartValueStart;

                while (x <= lowerMeetingPoint.x && y >= lowerMeetingPoint.y)
                {
                    if (rect.Contains(x, y))
                    {
                        texture.SetPixel(x, y, colour);
                    }

                    startOf2PixelBlock = !startOf2PixelBlock;
                    x++;

                    if (startOf2PixelBlock)
                    {
                        y--;
                    }
                }

                x = end.x;
                y = end.y;
                startOf2PixelBlock = startOf2PixelBlockStartValueEnd;

                while (x >= lowerMeetingPoint.x && y >= lowerMeetingPoint.y)
                {
                    if (rect.Contains(x, y))
                    {
                        texture.SetPixel(x, y, colour);
                    }

                    startOf2PixelBlock = !startOf2PixelBlock;

                    x--;

                    if (startOf2PixelBlock)
                    {
                        y--;
                    }
                }

                if (drawTopLines)
                {
                    x = start.x;
                    y = start.y;
                    startOf2PixelBlock = startOf2PixelBlockStartValueStart;

                    while (x <= upperMeetingPoint.x && y <= upperMeetingPoint.y)
                    {
                        if (rect.Contains(x, y))
                        {
                            texture.SetPixel(x, y, colour);
                        }

                        x++;
                        startOf2PixelBlock = !startOf2PixelBlock;

                        if (startOf2PixelBlock)
                        {
                            y++;
                        }
                    }

                    x = end.x;
                    y = end.y;
                    startOf2PixelBlock = startOf2PixelBlockStartValueEnd;

                    while (x >= upperMeetingPoint.x && y <= upperMeetingPoint.y)
                    {
                        if (rect.Contains(x, y))
                        {
                            texture.SetPixel(x, y, colour);
                        }

                        x--;
                        startOf2PixelBlock = !startOf2PixelBlock;

                        if (startOf2PixelBlock)
                        {
                            y++;
                        }
                    }
                }

                if (filled && Mathf.Abs(end.y - start.y) > 1)
                {
                    texture = Tex2DSprite.Fill(texture, lowerMeetingPoint + IntVector2.up, colour);
                }

                return new Tuple<Texture2D, IntVector2, IntVector2, IntVector2, IntVector2>(texture, start, upperMeetingPoint, end, lowerMeetingPoint);
            }
            else
            {
                /// Relabel start/end corners so that start is the bottom corner and end is the top corner.
                if (start.y > end.y)
                {
                    IntVector2 temp = start;
                    start = end;
                    end = temp;
                }

                Vector2 lineStart = new Vector2(start.x + 1f, start.y + 0.5f);
                Vector2 lineEnd = new Vector2(end.x, end.y + 0.5f);
                bool startOf2PixelBlockStartValueStartLeft = false;
                bool startOf2PixelBlockStartValueStartRight = true;
                bool startOf2PixelBlockStartValueEndLeft = true;
                bool startOf2PixelBlockStartValueEndRight = false;

                int cornerType = (end.x - start.x + 1 + (end.y - start.y) * 2) % 4;
                if (cornerType == 1 || cornerType == 3)
                {
                    lineEnd += new Vector2(1f, 0f);

                    startOf2PixelBlockStartValueEndLeft = false;
                    startOf2PixelBlockStartValueEndRight = true;
                }

                float meetingX = (lineStart.x + lineEnd.x) / 2f + lineStart.y - lineEnd.y;
                float meetingY = (meetingX - lineEnd.x) / 2f + lineEnd.y;
                IntVector2 leftMeetingPoint = new IntVector2(Mathf.FloorToInt(meetingX), Mathf.FloorToInt(meetingY));
                IntVector2 rightMeetingPoint = end + (start - leftMeetingPoint);

                if (cornerType == 0)
                {
                    leftMeetingPoint += IntVector2.right;
                    rightMeetingPoint += IntVector2.left;
                }
                else if (cornerType == 1)
                {
                    rightMeetingPoint += IntVector2.right;
                }
                else if (cornerType == 3)
                {
                    leftMeetingPoint += IntVector2.right;
                }

                int x = start.x;
                int y = start.y;

                while (x >= leftMeetingPoint.x && y <= leftMeetingPoint.y)
                {
                    if (rect.Contains(x, y))
                    {
                        texture.SetPixel(x, y, colour);
                    }

                    startOf2PixelBlockStartValueStartLeft = !startOf2PixelBlockStartValueStartLeft;
                    x--;

                    if (startOf2PixelBlockStartValueStartLeft)
                    {
                        y++;
                    }
                }

                x = start.x;
                y = start.y;

                while (x <= rightMeetingPoint.x && y <= rightMeetingPoint.y)
                {
                    if (rect.Contains(x, y))
                    {
                        texture.SetPixel(x, y, colour);
                    }

                    x++;
                    startOf2PixelBlockStartValueStartRight = !startOf2PixelBlockStartValueStartRight;

                    if (startOf2PixelBlockStartValueStartRight)
                    {
                        y++;
                    }
                }

                if (drawTopLines)
                {
                    x = end.x;
                    y = end.y;

                    while (x >= leftMeetingPoint.x && y >= leftMeetingPoint.y)
                    {
                        if (rect.Contains(x, y))
                        {
                            texture.SetPixel(x, y, colour);
                        }

                        startOf2PixelBlockStartValueEndLeft = !startOf2PixelBlockStartValueEndLeft;

                        x--;

                        if (startOf2PixelBlockStartValueEndLeft)
                        {
                            y--;
                        }
                    }

                    x = end.x;
                    y = end.y;

                    while (x <= rightMeetingPoint.x && y >= rightMeetingPoint.y)
                    {
                        if (rect.Contains(x, y))
                        {
                            texture.SetPixel(x, y, colour);
                        }

                        x++;
                        startOf2PixelBlockStartValueEndRight = !startOf2PixelBlockStartValueEndRight;

                        if (startOf2PixelBlockStartValueEndRight)
                        {
                            y--;
                        }
                    }
                }

                if (filled && Mathf.Abs(end.x - start.x) > 1)
                {
                    texture = Tex2DSprite.Fill(texture, leftMeetingPoint + IntVector2.right, colour);
                }

                return new Tuple<Texture2D, IntVector2, IntVector2, IntVector2, IntVector2>(texture, leftMeetingPoint, end, rightMeetingPoint, start);
            }
        }

        public static Texture2D IsoBox(int texWidth, int texHeight, IntVector2 baseStart, IntVector2 baseEnd, IntVector2 heightEnd, Color colour, bool filled)
        {
            Texture2D tex = Tex2DSprite.BlankTexture(texWidth, texHeight);
            IntRect rect = new IntRect(IntVector2.zero, new IntVector2(texWidth - 1, texHeight - 1));

            IntVector2 offset = new IntVector2(0, heightEnd.y - baseEnd.y);
            Tuple<Texture2D, IntVector2, IntVector2, IntVector2, IntVector2> texAndCorners = IsoRectangleOnExistingTexReturnCorners(tex, texWidth, texHeight, baseStart, baseEnd, colour, false, !(filled && offset.y > 0f));
            tex = texAndCorners.Item1;

            IntVector2 left = texAndCorners.Item2;
            IntVector2 top = texAndCorners.Item3;
            IntVector2 right = texAndCorners.Item4;
            IntVector2 bottom = texAndCorners.Item5;

            tex = IsoRectangleOnExistingTex(tex, texWidth, texHeight, baseStart + offset, baseEnd + offset, colour, false, !(filled && offset.y < 0f));

            if (!filled)
            {
                foreach (IntVector2 corner in new IntVector2[] { left, top, right, bottom })
                {
                    for (int y = corner.y; y != corner.y + offset.y; y += MathF.Sign(offset.y))
                    {
                        if (rect.Contains(corner.x, y))
                        {
                            tex.SetPixel(corner.x, y, colour);
                        }
                    }
                }
            }
            else
            {
                foreach (IntVector2 corner in new IntVector2[] { left, right, bottom })
                {
                    for (int y = corner.y; y != corner.y + offset.y; y += MathF.Sign(offset.y))
                    {
                        if (rect.Contains(corner.x, y))
                        {
                            tex.SetPixel(corner.x, y, colour);
                        }
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D Gradient(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color startColour, Color endColour, GradientMode gradientMode)
        {
            switch (gradientMode)
            {
                case GradientMode.Linear: return GradientLinear(texWidth, texHeight, start, end, startColour, endColour);
                case GradientMode.Radial: return GradientRadial(texWidth, texHeight, start, end, startColour, endColour);
                default: throw new System.Exception("Unknown / unimplemented gradient mode: " + gradientMode);
            }
        }

        public static Texture2D GradientLinear(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color startColour, Color endColour)
        {
            Texture2D tex = Tex2DSprite.BlankTexture(texWidth, texHeight);

            if (start == end)
            {
                for (int x = 0; x < texWidth; x++)
                {
                    for (int y = 0; y < texHeight; y++)
                    {
                        tex.SetPixel(x, y, startColour);
                    }
                }

                tex.Apply();
                return tex;
            }

            Vector2 dir = end - start;
            float distance = dir.magnitude;

            for (int x = 0; x < texWidth; x++)
            {
                for (int y = 0; y < texHeight; y++)
                {
                    if (x == start.x && y == start.y)
                    {
                        tex.SetPixel(x, y, startColour);
                    }
                    else
                    {
                        Vector2 point = new Vector2(x, y);
                        float perpendicularDistance = Vector2.Distance(start, point) * Vector2.Dot(point - start, dir) /
                                                      (point - start).magnitude / dir.magnitude;

                        float scalar = Mathf.Clamp01(perpendicularDistance / distance);

                        tex.SetPixel(x, y, Color.Lerp(startColour, endColour, scalar));
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D GradientRadial(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color startColour, Color endColour)
        {
            Texture2D tex = Tex2DSprite.BlankTexture(texWidth, texHeight);

            if (start == end)
            {
                for (int x = 0; x < texWidth; x++)
                {
                    for (int y = 0; y < texHeight; y++)
                    {
                        tex.SetPixel(x, y, endColour);
                    }
                }

                tex.Apply();
                return tex;
            }

            float distance = (end - start).magnitude;

            for (int x = 0; x < texWidth; x++)
            {
                for (int y = 0; y < texHeight; y++)
                {
                    float distanceToPoint = Vector2.Distance(start, new Vector2(x, y));

                    float scalar = Mathf.Clamp01(distanceToPoint / distance);

                    tex.SetPixel(x, y, Color.Lerp(startColour, endColour, scalar));
                }
            }

            tex.Apply();
            return tex;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using PAC;
using PAC.DataStructures;
using PAC.Drawing;
using PAC.Exceptions;
using PAC.Extensions;
using PAC.Maths;
using PAC.Shapes.Interfaces;

namespace PAC.Shapes
{
    /// <summary>
    /// <para>
    /// Represents a sequence of Lines.
    /// The lines must connect, meaning the end of each line must be equal to or adjacent to (including diagonally) the start of the next line.
    /// </para>
    /// <para>
    /// When iterating through the Path, pixels will never appear twice in succession. E.g. if the end of a line equals the start of the next line, that pixel with not be double-counted.
    /// Additionally, the last pixel of the path will not be counted if it's the same as first pixel of the path. Other than those cases, pixels can be double-counted.
    /// </para>
    /// </summary>
    public class Path : I1DShape<Path>, IDeepCopyableShape<Path>, IEquatable<Path>
    {
        /// <summary>
        /// The lines in this list should not be referenced by anything other than this object, as editing them can potentially causing us to lose the property that the lines connect. For this
        /// reason, we deep-copy lines in the constructor.
        /// </summary>
        private List<Line> _lines = new List<Line>();
        public ReadOnlyCollection<Line> lines => _lines.AsReadOnly();

        public IntVector2 start => _lines[0].start;
        public IntVector2 end => _lines[^1].end;

        /// <summary>
        /// Whether the end of the last line is adjacent to (including diagonally) the start of the first line.
        /// </summary>
        public bool isLoop => IntVector2.SupDistance(start, end) <= 1;

        /// <summary>
        /// <para>
        /// Returns a new path going through the lines in reverse order and with their starts / ends swapped.
        /// </para>
        /// <para>
        /// Note: this is not guaranteed to give the same shape - the centre pixel of a line can change when swapping its start / end.
        /// </para>
        /// </summary>
        public Path reverse
        {
            get
            {
                List<Line> reversedLines = new List<Line>(_lines.Count);
                for (int i = _lines.Count - 1; i >= 0; i--)
                {
                    reversedLines.Add(_lines[i].reverse);
                }
                return new Path(reversedLines);
            }
        }

        /// <summary>
        /// True iff the path never changes pixel.
        /// </summary>
        public bool isPoint => boundingRect.Count == 1;
        /// <summary>
        /// True iff the path never changes x coord.
        /// </summary>
        public bool isVertical => boundingRect.width == 1;
        /// <summary>
        /// True iff the path never changes y coord.
        /// </summary>
        public bool isHorizontal => boundingRect.height == 1;

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
        /// <summary>
        /// Whether the path defines a simple polygon (one that doesn't self-intersect - note however that lines are allowed to go back over).
        /// </summary>
        public bool isSimplePolygon
        {
            get
            {
                /// <summary>
                /// Returns whether the line (in1, meetingPoint, out1) crosses the line (in2, meetingPoint, out2).
                /// </summary>
                static bool IsCrossingPoint(IntVector2 meetingPoint, IntVector2 in1, IntVector2 out1, IntVector2 in2, IntVector2 out2)
                {
                    foreach (IntVector2 point in new IntVector2[] { in1, out1, in2, out2 })
                    {
                        if (IntVector2.SupDistance(point, meetingPoint) > 1)
                        {
                            throw new ArgumentException("The points do not form connected lines.");
                        }
                    }

                    // If any of the points are equal, the lines do not cross
                    if (new IntVector2[] { in1, out1, in2, out2 }.Distinct().Count() < 4)
                    {
                        return false;
                    }

                    // Go round the border of the 3x3 square centred on meeting point. The lines cross if and only if we see in2 or out2 (but not both) strictly between in1 and out1 or
                    // vice versa.
                    // E.g.
                    //
                    // Lines cross:
                    //      1  _  2
                    //      _     1
                    //      _  2  _
                    //
                    // Lines don't cross:
                    //      1  _  _
                    //      _     1
                    //      _  2  2
                    //

                    // How many of in1 / out1 we've seen
                    int count1 = 0;
                    // How many of in2 / out2 we've seen
                    int count2 = 0;
                    // Whether the last of in1 / out1 / in2 / out2 we saw was on line 1 or line 2
                    int lastSeen = 0;
                    foreach (IntVector2 point in new Rectangle(meetingPoint + IntVector2.downLeft, meetingPoint + IntVector2.upRight, false))
                    {
                        if (point == in1 || point == out1)
                        {
                            count1++;
                            if (count1 == 2)
                            {
                                return lastSeen == 2;
                            }
                            lastSeen = 1;
                        }
                        if (point == in2 || point == out2)
                        {
                            count2++;
                            if (count2 == 2)
                            {
                                return lastSeen == 1;
                            }
                            lastSeen = 2;
                        }
                    }

                    throw new UnreachableException();
                }

                if (!isLoop)
                {
                    return false;
                }

                IntVector2[] points = this.ToArray();
                HashSet<IntVector2> visited = new HashSet<IntVector2>();
                foreach ((IntVector2 pixel, int index) in points.Enumerate())
                {
                    if (visited.Contains(pixel))
                    {
                        int indexOfPreviousVisit = (index - 1).Mod(points.Length);
                        while (points[indexOfPreviousVisit] != pixel)
                        {
                            indexOfPreviousVisit = (indexOfPreviousVisit - 1).Mod(points.Length);
                        }

                        if (IsCrossingPoint(pixel,
                            points[(index - 1).Mod(points.Length)], points[(index + 1).Mod(points.Length)],
                            points[(indexOfPreviousVisit - 1).Mod(points.Length)], points[(indexOfPreviousVisit + 1).Mod(points.Length)]
                            )
                        )
                        {
                            return false;
                        }
                    }
                    visited.Add(points[index]);
                }
                return true;
            }
        }

        public IntRect boundingRect => IntRect.BoundingRect(_lines.Select(l => l.boundingRect));

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

                IntVector2 previousPixel = _lines[0].end;

                // Middle lines (not first or last)
                for (int i = 1; i < _lines.Count - 1; i++)
                {
                    if (_lines[i].start == previousPixel)
                    {
                        count += _lines[i].Count - 1;
                    }
                    else
                    {
                        count += _lines[i].Count;
                    }
                    previousPixel = _lines[i].end;
                }

                // Last line
                int start = 0;
                int end = _lines[^1].Count;
                if (_lines[^1].start == previousPixel)
                {
                    start++;
                }
                if (_lines[^1].end == _lines[0].start)
                {
                    end--;
                }

                if (end >= start)
                {
                    count += end - start;
                }

                return count;
            }
        }

        // To prevent creation of paths from 0 points/lines. (Those constructors have checks anyway but this stops the 'ambiguous call' error those give when trying to use an empty constructor)
        private Path() { }
        /// <summary>
        /// Creates a path through the given points.
        /// </summary>
        public Path(params IntVector2[] points) : this((IEnumerable<IntVector2>)points) { }
        /// <summary>
        /// Creates a path through the given points.
        /// </summary>
        public Path(IEnumerable<IntVector2> points)
        {
            if (points.None())
            {
                throw new ArgumentException("Cannot create a path from 0 points.", "points");
            }

            if (points.CountExactly(1))
            {
                _lines.Add(new Line(points.First(), points.First()));
            }
            else
            {
                foreach ((IntVector2 point, IntVector2 nextPoint) in points.ZipCurrentAndNext())
                {
                    _lines.Add(new Line(point, nextPoint));
                }
            }
        }
        /// <summary>
        /// <para>
        /// The lines must connect, meaning the end of each line must be equal to or adjacent to (including diagonally) the start of the next line.
        /// </para>
        /// <para>
        /// Creates deep copies of the lines.
        /// </para>
        /// </summary>
        public Path(params Line[] lines) : this((IEnumerable<Line>)lines) { }
        /// <summary>
        /// <para>
        /// The lines must connect, meaning the end of each line must be equal to or adjacent to (including diagonally) the start of the next line.
        /// </para>
        /// <para>
        /// Creates deep copies of the lines.
        /// </para>
        /// </summary>
        public Path(IEnumerable<Line> lines)
        {
            if (lines.None())
            {
                throw new ArgumentException("Cannot create a path from 0 lines.", "lines");
            }

            if (lines.CountExactly(1))
            {
                _lines.Add(lines.First().DeepCopy());
            }
            else
            {
                _lines.Add(lines.First().DeepCopy());
                foreach ((Line line, Line nextLine) in lines.ZipCurrentAndNext())
                {
                    if (IntVector2.SupDistance(line.end, nextLine.start) > 1)
                    {
                        throw new ArgumentException(line + " and " + nextLine + " do not connect.", "lines");
                    }
                    _lines.Add(nextLine.DeepCopy());
                }
            }
        }

        /// <summary>
        /// Treats the <see cref="Line"/> as a <see cref="Path"/> with one line segment.
        /// </summary>
        public static implicit operator Path(Line line) => new Path(line);

        /// <summary>
        /// <para>
        /// Puts the paths one after the other to create a new path. The paths must connect.
        /// </para>
        /// <para>
        /// Creates deep copies of the paths.
        /// </para>
        /// </summary>
        public static Path Concat(params Path[] paths) => Concat((IEnumerable<Path>)paths);
        /// <summary>
        /// <para>
        /// Puts the paths one after the other to create a new path. The paths must connect.
        /// </para>
        /// <para>
        /// Creates deep copies of the paths.
        /// </para>
        /// </summary>
        public static Path Concat(IEnumerable<Path> paths)
        {
            if (paths.None())
            {
                throw new ArgumentException("Cannot concat an empty collection of paths.", "paths");
            }

            if (paths.CountExactly(1))
            {
                return paths.First().DeepCopy();
            }
            else
            {
                List<Line> lines = new List<Line>();
                lines.AddRange(paths.First()._lines);
                foreach ((Path path, Path nextPath) in paths.ZipCurrentAndNext())
                {
                    if (IntVector2.SupDistance(path.end, nextPath.start) > 1)
                    {
                        throw new ArgumentException(path + " and " + nextPath + " do not connect.", "paths");
                    }
                    lines.AddRange(nextPath._lines);
                }
                return new Path(lines);
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

        /// <summary>
        /// Returns the minimum x coord of the pixels on the path that have the given y coord.
        /// </summary>
        public int MinX(int y)
        {
            if (!boundingRect.ContainsY(y))
            {
                throw new ArgumentOutOfRangeException("y must be within the y range of the path. y: " + y + "; path y range: [" + boundingRect.bottomLeft.y + ", " + boundingRect.topRight.y + "]");
            }
            return _lines.Where(l => l.boundingRect.ContainsY(y)).Min(l => l.MinX(y));
        }
        /// <summary>
        /// Returns the maximum x coord of the pixels on the path that have the given y coord.
        /// </summary>
        public int MaxX(int y)
        {
            if (!boundingRect.ContainsY(y))
            {
                throw new ArgumentOutOfRangeException("y must be within the y range of the path. y: " + y + "; path y range: [" + boundingRect.bottomLeft.y + ", " + boundingRect.topRight.y + "]");
            }
            return _lines.Where(l => l.boundingRect.ContainsY(y)).Max(l => l.MaxX(y));
        }
        /// <summary>
        /// Returns the minimum y coord of the pixels on the path that have the given x coord.
        /// </summary>
        public int MinY(int x)
        {
            if (!boundingRect.ContainsX(x))
            {
                throw new ArgumentOutOfRangeException("x must be within the x range of the path. x: " + x + "; path x range: [" + boundingRect.bottomLeft.x + ", " + boundingRect.topRight.x + "]");
            }
            return _lines.Where(l => l.boundingRect.ContainsX(x)).Min(l => l.MinY(x));
        }
        /// <summary>
        /// Returns the maximum y coord of the pixels on the path that have the given x coord.
        /// </summary>
        public int MaxY(int x)
        {
            if (!boundingRect.ContainsX(x))
            {
                throw new ArgumentOutOfRangeException("x must be within the x range of the path. x: " + x + "; path x range: [" + boundingRect.bottomLeft.x + ", " + boundingRect.topRight.x + "]");
            }
            return _lines.Where(l => l.boundingRect.ContainsX(x)).Max(l => l.MaxY(x));
        }

        /// <summary>
        /// Returns the number of pixels on the path that have the given x coord. A pixel that appears n times in the enumerator will be counted n times.
        /// </summary>
        public int CountOnX(int x)
        {
            // First line
            int count = _lines[0].CountOnX(x);
            if (_lines.Count == 1)
            {
                return count;
            }

            IntVector2 previousPixel = _lines[0].end;

            // Middle lines (not first or last)
            for (int i = 1; i < _lines.Count - 1; i++)
            {
                count += _lines[i].CountOnX(x);
                if (_lines[i].start == previousPixel && x == previousPixel.x)
                {
                    count--;
                }
                previousPixel = _lines[i].end;
            }

            // Last line
            int toAdd = _lines[^1].CountOnX(x);
            if (_lines[^1].start == previousPixel && x == previousPixel.x)
            {
                toAdd--;
            }
            if (_lines[^1].end == _lines[0].start && x == _lines[0].start.x)
            {
                toAdd--;
            }
            count += toAdd.ClampNonNegative();

            return count;
        }
        /// <summary>
        /// Returns the number of pixels on the path that have the given y coord. A pixel that appears n times in the enumerator will be counted n times.
        /// </summary>
        public int CountOnY(int y)
        {
            // First line
            int count = _lines[0].CountOnY(y);
            if (_lines.Count == 1)
            {
                return count;
            }

            IntVector2 previousPixel = _lines[0].end;

            // Middle lines (not first or last)
            for (int i = 1; i < _lines.Count - 1; i++)
            {
                count += _lines[i].CountOnY(y);
                if (_lines[i].start == previousPixel && y == previousPixel.y)
                {
                    count--;
                }
                previousPixel = _lines[i].end;
            }

            // Last line
            int toAdd = _lines[^1].CountOnY(y);
            if (_lines[^1].start == previousPixel && y == previousPixel.y)
            {
                toAdd--;
            }
            if (_lines[^1].end == _lines[0].start && y == _lines[0].start.y)
            {
                toAdd--;
            }
            count += toAdd.ClampNonNegative();

            return count;
        }

        /// <summary>
        /// Translates the path by the given vector.
        /// </summary>
        public static Path operator +(Path path, IntVector2 translation) => path.Translate(translation);
        /// <summary>
        /// Translates the path by the given vector.
        /// </summary>
        public static Path operator +(IntVector2 translation, Path path) => path + translation;
        /// <summary>
        /// Translates the path by the given vector.
        /// </summary>
        public static Path operator -(Path path, IntVector2 translation) => path + -translation;
        /// <summary>
        /// Reflects the path through the origin.
        /// </summary>
        public static Path operator -(Path path) => path.Rotate(RotationAngle._180);

        /// <summary>
        /// Translates the path by the given vector.
        /// </summary>
        public Path Translate(IntVector2 translation) => new Path(_lines.Select(l => l.Translate(translation)));

        /// <summary>
        /// Reflects the path across the given axis.
        /// </summary>
        public Path Flip(FlipAxis axis) => new Path(_lines.Select(l => l.Flip(axis)));

        /// <summary>
        /// Rotates the path by the given angle.
        /// </summary>
        public Path Rotate(RotationAngle angle) => new Path(_lines.Select(l => l.Rotate(angle)));

        private enum WindingNumberCornerType
        {
            /// <summary>
            /// At this point, the path is going downward (or horizontal between two downward sections).
            /// </summary>
            Downward = -1,
            /// <summary>
            /// On both sides of this point, the path goes down before it goes up; or on both sides of this point, the path goes up before it goes down.
            /// </summary>
            LocalYExtremum = 0,
            /// <summary>
            /// At this point, the path is going upward (or horizontal between two upward sections).
            /// </summary>
            Upward = 1
        }
        /// <summary>
        /// <para>
        /// Computes how many times the path goes round the given point. Each anticlockwise rotation adds +1; each clockwise rotation adds -1.
        /// </para>
        /// <para>
        /// Only defined for paths that are loops and for points not on the path.
        /// </para>
        /// </summary>
        public int WindingNumber(IntVector2 pixel)
        {
            if (!isLoop)
            {
                throw new ArgumentException("Winding number is only defined for paths that are loops.");
            }
            if (Contains(pixel))
            {
                throw new ArgumentException("Winding number is undefined for points on the path.");
            }

            if (isVertical || isHorizontal)
            {
                return 0;
            }

            int windingNumber = 0;
            for (int i = 0; i < _lines.Count; i++)
            {
                Line line = _lines[i];

                // Check if start of line is a local y extremum

                // This starting value doesn't mean anything as it should always get overwritten in the loop
                WindingNumberCornerType startCornerType = WindingNumberCornerType.Downward;
                for (int index = (i - 1).Mod(_lines.Count); index != i; index = (index - 1).Mod(_lines.Count))
                {
                    if (_lines[index].end.y != line.start.y)
                    {
                        if (Math.Sign(line.start.y - _lines[index].end.y) == Math.Sign(line.start.y - line.end.y))
                        {
                            startCornerType = WindingNumberCornerType.LocalYExtremum;
                        }
                        else
                        {
                            // Will be Upward or Downward
                            startCornerType = (WindingNumberCornerType)Math.Sign(line.start.y - _lines[index].end.y);
                        }
                        break;
                    }
                    else if (!_lines[index].isHorizontal)
                    {
                        if (Math.Sign(_lines[index].end.y - _lines[index].start.y) != Math.Sign(line.end.y - line.start.y))
                        {
                            startCornerType = WindingNumberCornerType.LocalYExtremum;
                        }
                        else
                        {
                            // Will be Upward or Downward
                            startCornerType = (WindingNumberCornerType)Math.Sign(_lines[index].end.y - _lines[index].start.y);
                        }
                        break;
                    }
                }

                // Check if end of line is a local y extremum

                // This starting value doesn't mean anything as it should should always get overwritten in the loop
                WindingNumberCornerType endCornerType = WindingNumberCornerType.Downward;
                for (int index = (i + 1) % _lines.Count; index != i; index = (index + 1) % _lines.Count)
                {
                    if (_lines[index].start.y != line.end.y)
                    {
                        if (Math.Sign(line.end.y - _lines[index].start.y) == Math.Sign(line.end.y - line.start.y))
                        {
                            endCornerType = WindingNumberCornerType.LocalYExtremum;
                        }
                        else
                        {
                            // Will be Upward or Downward
                            endCornerType = (WindingNumberCornerType)Math.Sign(_lines[index].start.y - line.end.y);
                        }
                        break;
                    }
                    else if (!_lines[index].isHorizontal)
                    {
                        if (Math.Sign(_lines[index].end.y - _lines[index].start.y) != Math.Sign(line.end.y - line.start.y))
                        {
                            endCornerType = WindingNumberCornerType.LocalYExtremum;
                        }
                        else
                        {
                            // Will be Upward or Downward
                            endCornerType = (WindingNumberCornerType)Math.Sign(_lines[index].end.y - _lines[index].start.y);
                        }
                        break;
                    }
                }

                // The loops above don't identify local y extrema for horizontal lines. They do calculate whether the next change in y is up or down. Using this we can deduce whether it's
                // a local y extremum or not.
                if (line.isHorizontal && startCornerType != endCornerType)
                {
                    startCornerType = WindingNumberCornerType.LocalYExtremum;
                    endCornerType = WindingNumberCornerType.LocalYExtremum;
                }

                // Decide whether to count the start/end y coord of the line in the raycast

                Line nextLine = _lines[(i + 1) % _lines.Count];
                bool countStartY = startCornerType != WindingNumberCornerType.LocalYExtremum;
                bool countEndY = endCornerType != WindingNumberCornerType.LocalYExtremum && line.end.y != nextLine.start.y;

                if (line.PointIsToLeft(pixel))
                {
                    if (pixel.y == line.end.y)
                    {
                        if (countEndY)
                        {
                            windingNumber += (int)endCornerType;
                        }
                    }
                    else if (pixel.y == line.start.y)
                    {
                        if (countStartY)
                        {
                            windingNumber += (int)startCornerType;
                        }
                    }
                    else
                    {
                        windingNumber += Math.Sign(line.end.y - line.start.y);
                    }
                }
            }

            return windingNumber;
        }

        /// <summary>
        /// Iterates through the points in the path, but also returns the index of the line that pixel came from. (If a pixel appears multiple times in the path, the line index of the i-th
        /// occurrence will be the index of the i-th line it appears in. In other words, the line index is non-decreasing and can only increment in steps of 1.)
        /// </summary>
        private IEnumerable<(IntVector2 pixel, int lineIndex)> EnumerateWithLineIndex()
        {
            // First line
            foreach (IntVector2 pixel in _lines[0])
            {
                yield return (pixel, 0);
            }
            if (_lines.Count == 1)
            {
                yield break;
            }

            IntVector2 previousPixel = _lines[0].end;

            // Middle lines (not first or last)
            for (int i = 1; i < _lines.Count - 1; i++)
            {
                int start = 0;
                if (_lines[i].start == previousPixel)
                {
                    start = 1;
                }
                foreach (IntVector2 pixel in _lines[i][start..])
                {
                    yield return (pixel, i);
                }
                previousPixel = _lines[i].end;
            }

            // Last line
            {
                int start = 0;
                int end = _lines[^1].Count;
                if (_lines[^1].start == previousPixel)
                {
                    start++;
                }
                if (_lines[^1].end == _lines[0].start)
                {
                    end--;
                }

                if (end < start)
                {
                    yield break;
                }

                foreach (IntVector2 pixel in _lines[^1][start..end])
                {
                    yield return (pixel, _lines.Count - 1);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator() => EnumerateWithLineIndex().Select(x => x.pixel).GetEnumerator();

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
        public static bool operator !=(Path a, Path b) => !(a == b);
        public bool Equals(Path other) => this == other;
        public override bool Equals(object obj) => obj is Path other && Equals(other);

        public override int GetHashCode() => _lines.GetHashCode();

        public override string ToString() => "Path(" + string.Join(", ", _lines) + ")";

        public Path DeepCopy()
        {
            List<Line> copiedLines = new List<Line>(_lines.Count);
            foreach (Line line in _lines)
            {
                copiedLines.Add(line.DeepCopy());
            }
            return new Path(copiedLines);
        }
    }
}
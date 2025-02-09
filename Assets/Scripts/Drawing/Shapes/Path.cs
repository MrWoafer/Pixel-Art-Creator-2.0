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

        public IntRect boundingRect => IntRect.BoundingRect(_lines.Select(l => l.boundingRect));

        public int Count => Count_Impl(line => line.Count(), point => true);
        /// <summary>
        /// An abstraction used to implement <see cref="Count"/>, <see cref="CountOnX(int)"/> and <see cref="CountOnY(int)"/>.
        /// </summary>
        /// <remarks>
        /// Adds together the value of <paramref name="countingFunction"/> for each line in the <see cref="Path"/>. This may double-count points that are the end of one line and the start of
        /// the next, so for each of these points it uses <paramref name="hasDoubledCountedJoiningPoint"/> to determine whether to correct it for each such point.
        /// </remarks>
        private int Count_Impl(Func<Line, int> countingFunction, Func<IntVector2, bool> hasDoubledCountedJoiningPoint)
        {
            // First line
            int count = countingFunction(_lines[0]);
            if (_lines.Count == 1)
            {
                return count;
            }

            // Middle lines (not first or last)
            for (int i = 1; i < _lines.Count - 1; i++)
            {
                count += countingFunction(_lines[i]);
                if (_lines[i].start == lines[i - 1].end && hasDoubledCountedJoiningPoint(lines[i - 1].end))
                {
                    count--;
                }
            }

            // Last line (if there's more than one line)
            int toAdd = countingFunction(_lines[^1]);
            if (_lines[^1].start == lines[^2].end && hasDoubledCountedJoiningPoint(lines[^2].end))
            {
                toAdd--;
            }
            if (_lines[^1].end == _lines[0].start && hasDoubledCountedJoiningPoint(_lines[0].start))
            {
                toAdd--;
            }
            count += toAdd.ClampNonNegative();

            return count;
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

            if (points.CountIsExactly(1))
            {
                _lines.Add(new Line(points.First()));
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

            if (lines.CountIsExactly(1))
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

            if (paths.CountIsExactly(1))
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

        public bool Contains(IntVector2 point) => _lines.Any(line => line.Contains(point));

        /// <summary>
        /// Returns the minimum x coord of the pixels on the path that have the given y coord.
        /// </summary>
        public int MinX(int y)
        {
            if (!boundingRect.ContainsY(y))
            {
                throw new ArgumentOutOfRangeException(nameof(y), $"{nameof(y)} must be within the y range of the path. {nameof(y)}: {y}; path y range: {boundingRect.yRange}.");
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
                throw new ArgumentOutOfRangeException(nameof(y), $"{nameof(y)} must be within the y range of the path. {nameof(y)}: {y}; path y range: {boundingRect.yRange}.");
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
                throw new ArgumentOutOfRangeException(nameof(x), $"{nameof(x)} must be within the x range of the path. {nameof(x)}: {x}; path x range: {boundingRect.xRange}.");
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
                throw new ArgumentOutOfRangeException(nameof(x), $"{nameof(x)} must be within the x range of the path. {nameof(x)}: {x}; path x range: {boundingRect.xRange}.");
            }
            return _lines.Where(l => l.boundingRect.ContainsX(x)).Max(l => l.MaxY(x));
        }

        /// <summary>
        /// Returns the number of pixels on the path that have the given x coord. A pixel that appears n times in the enumerator will be counted n times.
        /// </summary>
        public int CountOnX(int x) => Count_Impl(line => line.CountOnX(x), point => point.x == x);
        /// <summary>
        /// Returns the number of pixels on the path that have the given y coord. A pixel that appears n times in the enumerator will be counted n times.
        /// </summary>
        public int CountOnY(int y) => Count_Impl(line => line.CountOnY(y), point => point.y == y);

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

        public static bool operator ==(Path a, Path b) => a._lines.SequenceEqual(b._lines);
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using PAC.DataStructures;
using PAC.Extensions;
using PAC.Maths;
using PAC.Geometry.Shapes.Interfaces;
using PAC.Geometry.Axes;

namespace PAC.Geometry.Shapes
{
    /// <summary>
    /// A sequence of connected <see cref="Line"/>s.
    /// <example>
    /// For example,
    /// <code>
    ///        end of line 1
    ///       start of line 2    # &lt; end of line 3
    ///              v           #
    ///              # #         #
    ///            #     # #     # &lt; start of line 3
    ///          #           # #
    ///        #               ^
    ///        ^         end of line 2
    /// start of line 1
    /// </code>
    /// </example>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="Line"/>s must connect, meaning the end of each <see cref="Line"/> must be equal to or adjacent (including diagonally) to the start of the next <see cref="Line"/>. This does
    /// not have to hold for the last <see cref="Line"/>.
    /// </para>
    /// <para>
    /// <b>Enumerator:</b>
    /// </para>
    /// <para>
    /// Points can be double-counted, but the purpose of <see cref="Path"/> is that they will never appear twice in succession. This allows iterating over the sequence of <see cref="Line"/>s
    /// without double-counting joining points between one <see cref="Line"/> and the next. Additionally, if the last point of the <see cref="Path"/> equals the first point, then the last point
    /// will not be counted, allowing iterating over a sequence of <see cref="Line"/>s that create a loop without double-counting the starting point.
    /// </para>
    /// </remarks>
    public class Path : I1DShape<Path>, IDeepCopyableShape<Path>, IEquatable<Path>
    {
        /// <summary>
        /// The non-readonly backing for <see cref="lines"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="Line"/>s in this list should not be referenced by anything other than this object, as editing them can potentially cause us to lose the property that the
        /// <see cref="Line"/>s connect. For this reason, we deep-copy <see cref="Line"/>s in the constructors.
        /// </remarks>
        private List<Line> _lines = new List<Line>();
        /// <summary>
        /// The sequence of <see cref="Line"/>s that make up the <see cref="Path"/>.
        /// </summary>
        /// <remarks>
        /// Do not edit these <see cref="Line"/>s except through methods provided in <see cref="Path"/>, as editing them can potentially cause us to lose the property that the <see cref="Line"/>s
        /// connect.
        /// </remarks>
        public ReadOnlyCollection<Line> lines => _lines.AsReadOnly();

        /// <summary>
        /// The first point in the <see cref="Path"/>.
        /// </summary>
        /// <seealso cref="end"/>
        /// <remarks>
        /// This will be the first point in the enumerator.
        /// </remarks>
        public IntVector2 start => _lines[0].start;
        /// <summary>
        /// The last point in the <see cref="Path"/>.
        /// </summary>
        /// <remarks>
        /// This may not be the last point in the enumerator: if it's equal to <see cref="start"/>, and the enumerator has length > 1, it won't be counted at the end. Other than in that case,
        /// it will be the last point in the enumerator.
        /// </remarks>
        /// <seealso cref="start"/>
        public IntVector2 end => _lines[^1].end;

        /// <summary>
        /// Whether the <see cref="end"/> of the <see cref="Path"/> is adjacent (including diagonally) to the <see cref="start"/>.
        /// </summary>
        public bool isLoop => IntVector2.SupDistance(start, end) <= 1;

        /// <summary>
        /// Returns a new <see cref="Path"/> going through the <see cref="Line"/>s in reverse order and with their starts / ends swapped.
        /// </summary>
        /// <remarks>
        /// This is not guaranteed to give the same geometry: the midpoint of a <see cref="Line"/> can move when swapping its start / end.
        /// </remarks>
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
        /// True iff the <see cref="Path"/> never changes coordinate.
        /// </summary>
        /// <seealso cref="isVertical"/>
        /// <seealso cref="isHorizontal"/>
        public bool isPoint => boundingRect.Count == 1;
        /// <summary>
        /// True iff the <see cref="Path"/> never changes x coord.
        /// </summary>
        /// <seealso cref="isPoint"/>
        /// <seealso cref="isHorizontal"/>
        public bool isVertical => boundingRect.width == 1;
        /// <summary>
        /// True iff the <see cref="Path"/> never changes y coord.
        /// </summary>
        /// <seealso cref="isPoint"/>
        /// <seealso cref="isVertical"/>
        public bool isHorizontal => boundingRect.height == 1;

        /// <summary>
        /// Whether the enumerator repeats any points.
        /// </summary>
        public bool selfIntersects => !this.AreAllDistinct();

        public IntRect boundingRect => IntRect.BoundingRect(_lines.Select(l => l.boundingRect));

        public int Count => Count_Impl(line => line.Count(), point => true);
        /// <summary>
        /// An abstraction used to implement <see cref="Count"/>, <see cref="CountOnX(int)"/> and <see cref="CountOnY(int)"/>.
        /// </summary>
        /// <remarks>
        /// Adds together the value of <paramref name="countingFunction"/> for each <see cref="Line"/> in the <see cref="Path"/>. This may double-count points that are the end of one line and the
        /// start of the next, so for each of these points it uses <paramref name="hasDoubledCountedJoiningPoint"/> to determine whether to correct it for each such point.
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
            count += toAdd.ClampNonNegative(); // clamped to deal with the case that both ifs above uncount the same point, so we don't double-uncount it

            return count;
        }

        // To prevent creation of paths from 0 points/lines. (Those constructors have checks anyway but this stops the 'ambiguous call' error those give when trying to use an empty constructor)
        private Path() { }
        /// <summary>
        /// Creates a <see cref="Path"/> by connecting the sequence of points with <see cref="Line"/>s.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="points"/> is empty.</exception>
        public Path(params IntVector2[] points) : this((IEnumerable<IntVector2>)points) { }
        /// <summary>
        /// Creates a <see cref="Path"/> by connecting the sequence of points with <see cref="Line"/>s.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="points"/> is empty.</exception>
        public Path(IEnumerable<IntVector2> points)
        {
            if (points.None())
            {
                throw new ArgumentException($"Cannot create a {nameof(Path)} from 0 points.", nameof(points));
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
        /// Creates a <see cref="Path"/> with the given sequence of <see cref="Line"/>s.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="Line"/>s must connect, meaning the end of each <see cref="Line"/> must be equal to or adjacent to (including diagonally) the start of the next <see cref="Line"/>.
        /// </para>
        /// <para>
        /// Creates deep copies of the <see cref="Line"/>s.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="lines"/> is empty or the <see cref="Line"/>s it contains do not connect.</exception>
        public Path(params Line[] lines) : this((IEnumerable<Line>)lines) { }
        /// <summary>
        /// Creates a <see cref="Path"/> with the given sequence of <see cref="Line"/>s.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="Line"/>s must connect, meaning the end of each <see cref="Line"/> must be equal to or adjacent to (including diagonally) the start of the next <see cref="Line"/>.
        /// </para>
        /// <para>
        /// Creates deep copies of the <see cref="Line"/>s.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="lines"/> is empty or the <see cref="Line"/>s it contains do not connect.</exception>
        public Path(IEnumerable<Line> lines)
        {
            if (lines.None())
            {
                throw new ArgumentException($"Cannot create a {nameof(Path)} from 0 {nameof(Line)}s.", nameof(lines));
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
                        throw new ArgumentException($"{line} and {nextLine} do not connect.", nameof(lines));
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
        /// Puts the <see cref="Path"/>s one after the other to create a new <see cref="Path"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="Path"/>s must connect, meaning the end of each <see cref="Path"/> must be equal to or adjacent to (including diagonally) the start of the next <see cref="Path"/>.
        /// </para>
        /// <para>
        /// Creates deep copies of the <see cref="Path"/>s.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="paths"/> is empty or the <see cref="Path"/>s it contains do not connect.</exception>
        public static Path Concat(params Path[] paths) => Concat((IEnumerable<Path>)paths);
        /// <summary>
        /// Puts the <see cref="Path"/>s one after the other to create a new <see cref="Path"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="Path"/>s must connect, meaning the end of each <see cref="Path"/> must be equal to or adjacent to (including diagonally) the start of the next <see cref="Path"/>.
        /// </para>
        /// <para>
        /// Creates deep copies of the <see cref="Path"/>s.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="paths"/> is empty or the <see cref="Path"/>s it contains do not connect.</exception>
        public static Path Concat(IEnumerable<Path> paths)
        {
            if (paths.None())
            {
                throw new ArgumentException($"Cannot concat an empty collection of {nameof(Path)}s.", nameof(paths));
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
                        throw new ArgumentException($"{path} and {nextPath} do not connect.", nameof(paths));
                    }
                    lines.AddRange(nextPath._lines);
                }
                return new Path(lines);
            }
        }

        public bool Contains(IntVector2 point) => _lines.Any(line => line.Contains(point));

        /// <summary>
        /// Returns the minimum x coord of the points on the <see cref="Path"/> that have the given y coord.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="y"/> is not in the range of y coords spanned by the <see cref="Path"/>.</exception>
        public int MinX(int y)
        {
            if (!boundingRect.ContainsY(y))
            {
                throw new ArgumentOutOfRangeException(nameof(y), $"{nameof(y)} must be within the y range of the {nameof(Path)}. {nameof(y)}: {y}; {nameof(Path)} y range: {boundingRect.yRange}.");
            }
            return _lines.Where(l => l.boundingRect.ContainsY(y)).Min(l => l.MinX(y));
        }
        /// <summary>
        /// Returns the maximum x coord of the points on the <see cref="Path"/> that have the given y coord.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="y"/> is not in the range of y coords spanned by the <see cref="Path"/>.</exception>
        public int MaxX(int y)
        {
            if (!boundingRect.ContainsY(y))
            {
                throw new ArgumentOutOfRangeException(nameof(y), $"{nameof(y)} must be within the y range of the {nameof(Path)}. {nameof(y)}: {y}; {nameof(Path)} y range: {boundingRect.yRange}.");
            }
            return _lines.Where(l => l.boundingRect.ContainsY(y)).Max(l => l.MaxX(y));
        }
        /// <summary>
        /// Returns the minimum y coord of the points on the <see cref="Path"/> that have the given x coord.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="x"/> is not in the range of x coords spanned by the <see cref="Path"/>.</exception>
        public int MinY(int x)
        {
            if (!boundingRect.ContainsX(x))
            {
                throw new ArgumentOutOfRangeException(nameof(x), $"{nameof(x)} must be within the x range of the {nameof(Path)}. {nameof(x)}: {x}; {nameof(Path)} x range: {boundingRect.xRange}.");
            }
            return _lines.Where(l => l.boundingRect.ContainsX(x)).Min(l => l.MinY(x));
        }
        /// <summary>
        /// Returns the maximum y coord of the points on the <see cref="Path"/> that have the given x coord.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="x"/> is not in the range of x coords spanned by the <see cref="Path"/>.</exception>
        public int MaxY(int x)
        {
            if (!boundingRect.ContainsX(x))
            {
                throw new ArgumentOutOfRangeException(nameof(x), $"{nameof(x)} must be within the x range of the {nameof(Path)}. {nameof(x)}: {x}; {nameof(Path)} x range: {boundingRect.xRange}.");
            }
            return _lines.Where(l => l.boundingRect.ContainsX(x)).Max(l => l.MaxY(x));
        }

        /// <summary>
        /// Returns the number of points on the <see cref="Path"/> that have the given x coord.
        /// </summary>
        /// <remarks>
        /// A point that appears n times in the enumerator will be counted n times.
        /// </remarks>
        public int CountOnX(int x) => Count_Impl(line => line.CountOnX(x), point => point.x == x);
        /// <summary>
        /// Returns the number of points on the <see cref="Path"/> that have the given y coord.
        /// </summary>
        /// <remarks>
        /// A point that appears n times in the enumerator will be counted n times.
        /// </remarks>
        public int CountOnY(int y) => Count_Impl(line => line.CountOnY(y), point => point.y == y);

        /// <summary>
        /// Returns a deep copy of the <see cref="Path"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static Path operator +(Path path, IntVector2 translation) => path.Translate(translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="Path"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static Path operator +(IntVector2 translation, Path path) => path + translation;
        /// <summary>
        /// Returns a deep copy of the <see cref="Path"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static Path operator -(Path path, IntVector2 translation) => path + (-translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="Path"/> rotated 180 degrees about the origin (equivalently, reflected through the origin).
        /// </summary>
        /// <seealso cref="Rotate(QuadrantalAngle)"/>
        /// <seealso cref="Reflect(CardinalOrdinalAxis)"/>
        public static Path operator -(Path path) => path.Rotate(QuadrantalAngle._180);

        public Path Translate(IntVector2 translation) => new Path(_lines.Select(l => l.Translate(translation)));
        public Path Reflect(CardinalOrdinalAxis axis) => new Path(_lines.Select(l => l.Reflect(axis)));
        public Path Rotate(QuadrantalAngle angle) => new Path(_lines.Select(l => l.Rotate(angle)));

        /// <summary>
        /// Iterates through the points in the <see cref="Path"/>, but also returns the index (in <see cref="_lines"/>) of the <see cref="Line"/> that point came from.
        /// </summary>
        /// <remarks>
        /// If a point appears multiple times in the <see cref="Path"/>, the line index of the i-th occurrence will be the index of the i-th <see cref="Line"/> it appears in. In other words, the
        /// line index is non-decreasing and a point will never appear with the same line index twice.
        /// </remarks>
        private IEnumerable<(IntVector2 point, int lineIndex)> EnumerateWithLineIndex()
        {
            // First line
            foreach (IntVector2 point in _lines[0])
            {
                yield return (point, 0);
            }
            if (_lines.Count == 1)
            {
                yield break;
            }

            IntVector2 previousPoint = _lines[0].end;

            // Middle lines (not first or last)
            for (int i = 1; i < _lines.Count - 1; i++)
            {
                int start = 0;
                if (_lines[i].start == previousPoint)
                {
                    start = 1;
                }
                foreach (IntVector2 point in _lines[i][start..])
                {
                    yield return (point, i);
                }
                previousPoint = _lines[i].end;
            }

            // Last line (if there's more than one line)
            {
                int start = 0;
                int end = _lines[^1].Count;
                if (_lines[^1].start == previousPoint)
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

                foreach (IntVector2 point in _lines[^1][start..end])
                {
                    yield return (point, _lines.Count - 1);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator() => EnumerateWithLineIndex().Select(x => x.point).GetEnumerator();

        /// <summary>
        /// Whether the two <see cref="Path"/>s have the same sequence of <see cref="Line"/>s in <see cref="lines"/>.
        /// </summary>
        public static bool operator ==(Path a, Path b) => a._lines.SequenceEqual(b._lines);
        /// <summary>
        /// See <see cref="operator ==(Path, Path)"/>.
        /// </summary>
        public static bool operator !=(Path a, Path b) => !(a == b);
        /// <summary>
        /// See <see cref="operator ==(Path, Path)"/>.
        /// </summary>
        public bool Equals(Path other) => this == other;
        /// <summary>
        /// See <see cref="Equals(Path)"/>.
        /// </summary>
        public override bool Equals(object obj) => obj is Path other && Equals(other);

        public override int GetHashCode() => _lines.GetHashCode();

        public override string ToString() => $"{nameof(Path)}({string.Join(", ", _lines)})";

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
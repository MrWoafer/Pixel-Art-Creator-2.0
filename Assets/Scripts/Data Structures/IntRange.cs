using PAC.Extensions.System.Collections;
using PAC.Interfaces;

using System;
using System.Collections;
using System.Collections.Generic;

namespace PAC.DataStructures
{
    /// <summary>
    /// A set of consecutive integers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The enumerator iterates throught the elements from <see cref="startElement"/> to <see cref="endElement"/>.
    /// </para>
    /// <para>
    /// If <see cref="startBoundary"/> and <see cref="endBoundary"/> are equal but their being inclusive / exclusive do not match, we define the range to be empty. This definition is useful in
    /// situations such as
    /// <code language="csharp">
    /// foreach (int index in IntRange.InclExcl(0, array.Length))
    /// {
    ///     Console.WriteLine($"Element {index}: {array[index]}");
    /// }
    /// </code>
    /// so that it's equivalent to
    /// <code language="csharp">
    /// for (int index = 0; index &lt; array.Length; index++)
    /// {
    ///     Console.WriteLine($"Element {index}: {array[index]}");
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    public readonly struct IntRange : IReadOnlyList<int>, IReadOnlyContains<int>, IEquatable<IntRange>, ISetComparable<IntRange>
    {
        #region Fields
        /// <summary>
        /// The start of the range. Can be inclusive or exclusive.
        /// </summary>
        /// <remarks>
        /// Will be different from <see cref="startElement"/> if this boundary is exclusive.
        /// </remarks>
        /// <seealso cref="startBoundaryInclusive"/>
        /// <seealso cref="endBoundary"/>
        public readonly int startBoundary;
        /// <summary>
        /// The end of the range. Can be inclusive or exclusive.
        /// </summary>
        /// <remarks>
        /// Will be different from <see cref="endElement"/> if this boundary is exclusive.
        /// </remarks>
        /// <seealso cref="endBoundaryInclusive"/>
        /// <seealso cref="startBoundary"/>
        public readonly int endBoundary;
        /// <summary>
        /// Whether <see cref="startBoundary"/> is inclusive or exclusive.
        /// </summary>
        /// <seealso cref="endBoundaryInclusive"/>
        public readonly bool startBoundaryInclusive;
        /// <summary>
        /// Whether <see cref="endBoundary"/> is inclusive or exclusive.
        /// </summary>
        /// <seealso cref="startBoundaryInclusive"/>
        public readonly bool endBoundaryInclusive;
        #endregion

        #region Properties
        /// <summary>
        /// The first element when iterating through the range (from start to end).
        /// </summary>
        /// <remarks>
        /// Will be different from <see cref="startBoundary"/> if that boundary is exclusive.
        /// </remarks>
        /// <exception cref="UndefinedOperationException">The range is empty.</exception>
        /// <seealso cref="endElement"/>
        public int startElement => (isEmpty, startBoundaryInclusive) switch
        {
            (false, false) => (startBoundary < endBoundary) ? startBoundary + 1 : startBoundary - 1,
            (false, true) => startBoundary,
            (true, _) => throw new InvalidOperationException("An empty range has no start element.")
        };
        /// <summary>
        /// The last element when iterating through the range (from start to end).
        /// </summary>
        /// <remarks>
        /// Will be different from <see cref="endBoundary"/> if that boundary is exclusive.
        /// </remarks>
        /// <exception cref="UndefinedOperationException">The range is empty.</exception>
        /// <seealso cref="startElement"/>
        public int endElement => (isEmpty, endBoundaryInclusive) switch
        {
            (false, false) => (startBoundary < endBoundary) ? endBoundary - 1 : endBoundary + 1,
            (false, true) => endBoundary,
            (true, _) => throw new InvalidOperationException("An empty range has no start element.")
        };

        /// <summary>
        /// The lower of <see cref="startBoundary"/> and <see cref="endBoundary"/>.
        /// </summary>
        /// <remarks>
        /// Will be different from <see cref="minElement"/> if this boundary is exclusive.
        /// </remarks>
        /// <seealso cref="minBoundaryInclusive"/>
        /// <seealso cref="maxBoundary"/>
        public int minBoundary => Math.Min(startBoundary, endBoundary);
        /// <summary>
        /// The higher of <see cref="startBoundary"/> and <see cref="endBoundary"/>.
        /// </summary>
        /// <remarks>
        /// Will be different from <see cref="maxElement"/> if this boundary is exclusive.
        /// </remarks>
        /// <seealso cref="maxBoundaryInclusive"/>
        /// <seealso cref="minBoundary"/>
        public int maxBoundary => Math.Max(startBoundary, endBoundary);
        /// <summary>
        /// Whether <see cref="minBoundary"/> is inclusive or exclusive.
        /// </summary>
        /// <remarks>
        /// If <see cref="startBoundary"/> and <see cref="endBoundary"/> are equal but their being inclusive / exclusive do not match, we define this to be exclusive. See <see cref="IntRange"/> for
        /// more details.
        /// </remarks>
        /// <seealso cref="maxBoundaryInclusive"/>
        public bool minBoundaryInclusive => (endBoundary - startBoundary) switch
        {
            > 0 => startBoundaryInclusive,
            0 => startBoundaryInclusive && endBoundaryInclusive,
            < 0 => endBoundaryInclusive,
        };
        /// <summary>
        /// Whether <see cref="maxBoundary"/> is inclusive or exclusive.
        /// </summary>
        /// <remarks>
        /// If <see cref="startBoundary"/> and <see cref="endBoundary"/> are equal but their being inclusive / exclusive do not match, we define this to be exclusive. See <see cref="IntRange"/> for
        /// more details.
        /// </remarks>
        /// <seealso cref="minBoundaryInclusive"/>
        public bool maxBoundaryInclusive => (endBoundary - startBoundary) switch
        {
            > 0 => endBoundaryInclusive,
            0 => startBoundaryInclusive && endBoundaryInclusive,
            < 0 => startBoundaryInclusive,
        };

        /// <summary>
        /// The lowest element in the range.
        /// </summary>
        /// <remarks>
        /// Will be different from <see cref="minBoundary"/> if that boundary is exclusive.
        /// </remarks>
        /// <exception cref="UndefinedOperationException">The range is empty.</exception>
        /// <seealso cref="maxElement"/>
        public int minElement => isEmpty switch
        {
            false => minBoundaryInclusive ? minBoundary : (minBoundary + 1),
            true => throw new InvalidOperationException("An empty range has no minimum element.")
        };
        /// <summary>
        /// The highest element in the range.
        /// </summary>
        /// <remarks>
        /// Will be different from <see cref="maxBoundary"/> if that boundary is exclusive.
        /// </remarks>
        /// <exception cref="UndefinedOperationException">The range is empty.</exception>
        /// <seealso cref="minElement"/>
        public int maxElement => isEmpty switch
        {
            false => maxBoundaryInclusive ? maxBoundary : (maxBoundary - 1),
            true => throw new InvalidOperationException("An empty range has no maximum element.")
        };

        /// <summary>
        /// The range with <see cref="startBoundary"/> and <see cref="endBoundary"/> swapped, and <see cref="startBoundaryInclusive"/> and <see cref="endBoundaryInclusive"/> swapped.
        /// </summary>
        public IntRange reverse => new IntRange(endBoundary, startBoundary, endBoundaryInclusive, startBoundaryInclusive);
        /// <summary>
        /// A range with the same elements, in increasing order.
        /// </summary>
        /// <returns>
        /// Either the range unchanged or <see cref="reverse"/>.
        /// </returns>
        /// <seealso cref="asDecreasing"/>
        public IntRange asIncreasing => startBoundary <= endBoundary ? this : reverse;
        /// <summary>
        /// A range with the same elements, in decreasing order.
        /// </summary>
        /// <returns>
        /// Either the range unchanged or <see cref="reverse"/>.
        /// </returns>
        /// <seealso cref="asIncreasing"/>
        public IntRange asDecreasing => endBoundary <= startBoundary ? this : reverse;

        /// <summary>
        /// A range with the same elements, in the same order, but expressed with <see cref="startBoundary"/> exclusive and <see cref="endBoundary"/> exclusive.
        /// </summary>
        /// <seealso cref="asExclIncl"/>
        /// <seealso cref="asInclExcl"/>
        /// <seealso cref="asInclIncl"/>
        public IntRange asExclExcl
        {
            get
            {
                if (isExclExcl)
                {
                    return this;
                }
                else if (isEmpty)
                {
                    // The calling range is incl excl or excl incl and has the start and end boundaries equal
                    return ExclExcl(startBoundary, startBoundary);
                }
                else
                {
                    return (endElement >= startElement) ? ExclExcl(startElement - 1, endElement + 1) : ExclExcl(startElement + 1, endElement - 1);
                }
            }
        }
        /// <summary>
        /// A range with the same elements, in the same order, but expressed with <see cref="startBoundary"/> exclusive and <see cref="endBoundary"/> inclusive.
        /// </summary>
        /// <seealso cref="asExclExcl"/>
        /// <seealso cref="asInclExcl"/>
        /// <seealso cref="asInclIncl"/>
        public IntRange asExclIncl
        {
            get
            {
                if (isExclIncl)
                {
                    return this;
                }
                else if (!isEmpty)
                {
                    return ExclIncl((endElement >= startElement) ? (startElement - 1) : (startElement + 1), endElement);
                }
                else if (isInclExcl)
                {
                    // The calling range has the start and end boundaries equal
                    return reverse;
                }
                else
                {
                    // The calling range is excl excl
                    return ExclIncl(startBoundary, startBoundary);
                }
            }
        }
        /// <summary>
        /// A range with the same elements, in the same order, but expressed with <see cref="startBoundary"/> inclusive and <see cref="endBoundary"/> exclusive.
        /// </summary>
        /// <seealso cref="asExclExcl"/>
        /// <seealso cref="asExclIncl"/>
        /// <seealso cref="asInclIncl"/>
        public IntRange asInclExcl
        {
            get
            {
                if (isInclExcl)
                {
                    return this;
                }
                else if (!isEmpty)
                {
                    return InclExcl(startElement, (endElement >= startElement) ? (endElement + 1) : (endElement - 1));
                }
                else if (isExclIncl)
                {
                    // The calling range has the start and end boundaries equal
                    return reverse;
                }
                else
                {
                    // The calling range is excl excl
                    return InclExcl(startBoundary, startBoundary);
                }
            }
        }
        /// <summary>
        /// A range with the same elements, in the same order, but expressed with <see cref="startBoundary"/> inclusive and <see cref="endBoundary"/> inclusive.
        /// </summary>
        /// <exception cref="InvalidOperationException">The range is empty.</exception>
        /// <seealso cref="asExclExcl"/>
        /// <seealso cref="asExclIncl"/>
        /// <seealso cref="asInclExcl"/>
        public IntRange asInclIncl => isEmpty ? throw new InvalidOperationException("Cannot express an empty range as a range with both boundaries inclusive.") : InclIncl(startElement, endElement);

        /// <summary>
        /// Whether the range has no elements.
        /// </summary>
        public bool isEmpty => (endBoundary - startBoundary) switch
        {
            0 => !startBoundaryInclusive || !endBoundaryInclusive,
            1 or -1 => !startBoundaryInclusive && !endBoundaryInclusive,
            _ => false
        };

        /// <summary>
        /// Whether <see cref="startBoundary"/> is exclusive and <see cref="endBoundary"/> is exclusive.
        /// </summary>
        /// <remarks>
        /// Equivalent to <c>!<see cref="startBoundaryInclusive"/> &amp;&amp; !<see cref="endBoundaryInclusive"/></c>.
        /// </remarks>
        /// <seealso cref="isExclIncl"/>
        /// <seealso cref="isInclExcl"/>
        /// <seealso cref="isInclIncl"/>
        public bool isExclExcl => !startBoundaryInclusive && !endBoundaryInclusive;
        /// <summary>
        /// Whether <see cref="startBoundary"/> is exclusive and <see cref="endBoundary"/> is inclusive.
        /// </summary>
        /// <remarks>
        /// Equivalent to <c>!<see cref="startBoundaryInclusive"/> &amp;&amp; <see cref="endBoundaryInclusive"/></c>.
        /// </remarks>
        /// <seealso cref="isExclExcl"/>
        /// <seealso cref="isInclExcl"/>
        /// <seealso cref="isInclIncl"/>
        public bool isExclIncl => !startBoundaryInclusive && endBoundaryInclusive;
        /// <summary>
        /// Whether <see cref="startBoundary"/> is inclusive and <see cref="endBoundary"/> is exclusive.
        /// </summary>
        /// <remarks>
        /// Equivalent to <c><see cref="startBoundaryInclusive"/> &amp;&amp; !<see cref="endBoundaryInclusive"/></c>.
        /// </remarks>
        /// <seealso cref="isExclExcl"/>
        /// <seealso cref="isExclIncl"/>
        /// <seealso cref="isInclIncl"/>
        public bool isInclExcl => startBoundaryInclusive && !endBoundaryInclusive;
        /// <summary>
        /// Whether <see cref="startBoundary"/> is inclusive and <see cref="endBoundary"/> is inclusive.
        /// </summary>
        /// <remarks>
        /// Equivalent to <c><see cref="startBoundaryInclusive"/> &amp;&amp; <see cref="endBoundaryInclusive"/></c>.
        /// </remarks>
        /// <seealso cref="isExclExcl"/>
        /// <seealso cref="isExclIncl"/>
        /// <seealso cref="isInclExcl"/>
        public bool isInclIncl => startBoundaryInclusive && endBoundaryInclusive;

        /// <summary>
        /// The number of elements in the range.
        /// </summary>
        /// <remarks>
        /// May throw an <see cref="OverflowException"/>. For example, the <see cref="Count"/> of a range from <see cref="int.MinValue"/> to <see cref="int.MaxValue"/> cannot be expressed as an
        /// <see cref="int"/>. Using <see cref="LongCount"/> instead will never encounter this issue.
        /// </remarks>
        /// <exception cref="OverflowException">The number of elements is too large to represent as an <see cref="int"/>.</exception>
        public int Count => isEmpty ? 0 : checked(Math.Abs(endElement - startElement) + 1);

        /// <summary>
        /// The same as <see cref="Count"/> but computed as a <see cref="long"/>.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="Count"/>, this will never throw an <see cref="OverflowException"/>. For example, the largest range, <see cref="int.MinValue"/> to <see cref="int.MaxValue"/>, cannot be
        /// expressed as an <see cref="int"/> but can be expressed as a <see cref="long"/>.
        /// </remarks>
        public long LongCount => isEmpty ? 0 : (Math.Abs((long)endElement - (long)startElement) + 1);
        #endregion

        #region Predefined Instances
        /// <summary>
        /// The range from <see cref="int.MinValue"/> (inclusive) to <see cref="int.MaxValue"/> (inclusive).
        /// </summary>
        public static readonly IntRange All = new IntRange(int.MinValue, int.MaxValue, true, true);
        /// <summary>
        /// The range from 0 (exclusive) to 0 (exclusive), which contains no elements.
        /// </summary>
        public static readonly IntRange Empty = new IntRange(0, 0, false, false);
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new range.
        /// </summary>
        /// <remarks>
        /// If <paramref name="startBoundary"/> and <paramref name="endBoundary"/> are equal but their being inclusive / exclusive do not match, we define the range to be empty. See
        /// <see cref="IntRange"/> for more details.
        /// </remarks>
        /// <param name="startBoundary">The start of the range. Can be inclusive or exclusive.</param>
        /// <param name="endBoundary">The end of the range. Can be inclusive or exclusive.</param>
        /// <param name="startBoundaryInclusive"> Whether the start boundary is inclusive or exclusive.</param>
        /// <param name="endBoundaryInclusive">Whether the end boundary is inclusive or exclusive.</param>
        public IntRange(int startBoundary, int endBoundary, bool startBoundaryInclusive, bool endBoundaryInclusive)
        {
            this.startBoundary = startBoundary;
            this.endBoundary = endBoundary;
            this.startBoundaryInclusive = startBoundaryInclusive;
            this.endBoundaryInclusive = endBoundaryInclusive;
        }

        /// <summary>
        /// Creates a range from <paramref name="x"/> (inclusive) to <paramref name="x"/> (inclusive).
        /// </summary>
        public static IntRange Singleton(int x) => new IntRange(x, x, true, true);

        /// <summary>
        /// Creates a range from <paramref name="x"/> (inclusive) to <see cref="int.MinValue"/> (inclusive).
        /// </summary>
        /// <seealso cref="LessThan(int)"/>
        /// <seealso cref="AtLeast(int)"/>
        /// <seealso cref="MoreThan(int)"/>
        public static IntRange AtMost(int x) => new IntRange(x, int.MinValue, true, true);
        /// <summary>
        /// Creates a range from <paramref name="x"/> (exclusive) to <see cref="int.MinValue"/> (inclusive).
        /// </summary>
        /// <seealso cref="AtMost(int)"/>
        /// <seealso cref="AtLeast(int)"/>
        /// <seealso cref="MoreThan(int)"/>
        public static IntRange LessThan(int x) => new IntRange(x, int.MinValue, false, true);
        /// <summary>
        /// Creates a range from <paramref name="x"/> (inclusive) to <see cref="int.MaxValue"/> (inclusive).
        /// </summary>
        /// <seealso cref="AtMost(int)"/>
        /// <seealso cref="LessThan(int)"/>
        /// <seealso cref="MoreThan(int)"/>
        public static IntRange AtLeast(int x) => new IntRange(x, int.MaxValue, true, true);
        /// <summary>
        /// Creates a range from <paramref name="x"/> (exclusive) to <see cref="int.MaxValue"/> (inclusive).
        /// </summary>
        /// <seealso cref="AtMost(int)"/>
        /// <seealso cref="LessThan(int)"/>
        /// <seealso cref="AtLeast(int)"/>
        public static IntRange MoreThan(int x) => new IntRange(x, int.MaxValue, false, true);

        /// <summary>
        /// Creates a range from <paramref name="startBoundary"/> (exclusive) to <paramref name="endBoundary"/> (exclusive).
        /// </summary>
        /// <seealso cref="ExclIncl(int, int)"/>
        /// <seealso cref="InclExcl(int, int)"/>
        /// <seealso cref="InclIncl(int, int)"/>
        public static IntRange ExclExcl(int startBoundary, int endBoundary) => new IntRange(startBoundary, endBoundary, false, false);
        /// <summary>
        /// Creates a range from <paramref name="startBoundary"/> (exclusive) to <paramref name="endBoundary"/> (inclusive).
        /// </summary>
        /// <remarks>
        /// If <paramref name="startBoundary"/> and <paramref name="endBoundary"/> are equal, we define the range to be empty. See <see cref="IntRange"/> for more details.
        /// </remarks>
        /// <seealso cref="ExclExcl(int, int)"/>
        /// <seealso cref="InclExcl(int, int)"/>
        /// <seealso cref="InclIncl(int, int)"/>
        public static IntRange ExclIncl(int startBoundary, int endBoundary) => new IntRange(startBoundary, endBoundary, false, true);
        /// <summary>
        /// Creates a range from <paramref name="startBoundary"/> (inclusive) to <paramref name="endBoundary"/> (exclusive).
        /// </summary>
        /// <remarks>
        /// If <paramref name="startBoundary"/> and <paramref name="endBoundary"/> are equal, we define the range to be empty. See <see cref="IntRange"/> for more details.
        /// </remarks>
        /// <seealso cref="ExclExcl(int, int)"/>
        /// <seealso cref="ExclIncl(int, int)"/>
        /// <seealso cref="InclIncl(int, int)"/>
        public static IntRange InclExcl(int startBoundary, int endBoundary) => new IntRange(startBoundary, endBoundary, true, false);
        /// <summary>
        /// Creates a range from <paramref name="startBoundary"/> (inclusive) to <paramref name="endBoundary"/> (inclusive).
        /// </summary>
        /// <seealso cref="ExclExcl(int, int)"/>
        /// <seealso cref="ExclIncl(int, int)"/>
        /// <seealso cref="InclExcl(int, int)"/>
        public static IntRange InclIncl(int startBoundary, int endBoundary) => new IntRange(startBoundary, endBoundary, true, true);
        #endregion

        #region Comparison
        /// <summary>
        /// Whether the two ranges have the same start boundary and end boundary and the same inclusivity at each boundary - i.e. compares the <see langword="struct"/>s' fields.
        /// </summary>
        /// <remarks>
        /// Note that having the same elements does not necessarily mean they are ==, due to the boundaries being inclusive or exclusive.
        /// </remarks>
        /// <seealso cref="Equals(IntRange)"/>
        /// <seealso cref="SequenceEquals(IntRange)"/>
        /// <seealso cref="SetEquals(IntRange)"/>
        public static bool operator ==(IntRange a, IntRange b) =>
            a.startBoundary == b.startBoundary
            && a.endBoundary == b.endBoundary
            && a.startBoundaryInclusive == b.startBoundaryInclusive
            && a.endBoundaryInclusive == b.endBoundaryInclusive;
        /// <summary>
        /// Whether the two ranges have the same start boundary and end boundary and the same inclusivity at each boundary - i.e. compares the <see langword="struct"/>s' fields.
        /// </summary>
        /// <remarks>
        /// Note that being != does not necessarily mean they have different elements, due to the boundaries being inclusive or exclusive.
        /// </remarks>
        /// <seealso cref="Equals(IntRange)"/>
        /// <seealso cref="SequenceEquals(IntRange)"/>
        /// <seealso cref="SetEquals(IntRange)"/>
        public static bool operator !=(IntRange a, IntRange b) => !(a == b);
        /// <summary>
        /// The same as <see cref="operator ==(IntRange, IntRange)"/>.
        /// </summary>
        /// <seealso cref="SequenceEquals(IntRange)"/>
        /// <seealso cref="SetEquals(IntRange)"/>
        public bool Equals(IntRange other) => this == other;
        /// <summary>
        /// The same as <see cref="Equals(IntRange)"/>.
        /// </summary>
        /// <seealso cref="SequenceEquals(IntRange)"/>
        /// <seealso cref="SetEquals(IntRange)"/>
        public override bool Equals(object obj) => obj is IntRange other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(startBoundary, endBoundary, startBoundaryInclusive, endBoundaryInclusive);

        /// <summary>
        /// Whether the two ranges have exactly the same elements in the same order.
        /// </summary>
        /// <remarks>
        /// Note that ranges with different boundaries can give the same sequence (and therefore be considered sequence-equal), due to the boundaries being inclusive or exclusive.
        /// </remarks>
        /// <seealso cref="Equals(IntRange)"/>
        /// <seealso cref="SetEquals(IntRange)"/>
        public bool SequenceEquals(IntRange other) => SequenceEqual(this, other);
        /// <summary>
        /// Whether the two ranges have exactly the same elements in the same order.
        /// </summary>
        /// <remarks>
        /// Note that ranges with different boundaries can give the same sequence (and therefore be considered sequence-equal), due to the boundaries being inclusive or exclusive.
        /// </remarks>
        /// <seealso cref="Equals(IntRange)"/>
        /// <seealso cref="SetEquals(IntRange)"/>
        public static bool SequenceEqual(IntRange a, IntRange b) => (a.isEmpty, b.isEmpty) switch
        {
            (false, false) => a.startElement == b.startElement && a.endElement == b.endElement,
            (false, true) or (true, false) => false,
            (true, true) => true,
        };

        public bool IsSubsequenceOf(IntRange other) => isEmpty || (!other.isEmpty && other.minElement <= minElement && maxElement <= other.maxElement);
        public bool IsProperSubsequenceOf(IntRange other) => IsSubsequenceOf(other) && !SequenceEqual(this, other);

        public bool IsSupersequenceOf(IntRange other) => other.IsSubsequenceOf(this);
        public bool IsProperSupersequenceOf(IntRange other) => IsSupersequenceOf(other) && !SequenceEqual(this, other);

        /// <summary>
        /// Whether the two ranges have exactly the same elements, ignoring order.
        /// </summary>
        /// <remarks>
        /// Note that ranges with different boundaries can have the same elements (and therefore be considered set-equal), due to the boundaries being inclusive or exclusive.
        /// </remarks>
        /// <seealso cref="Equals(IntRange)"/>
        /// <seealso cref="SequenceEquals(IntRange)"/>
        public bool SetEquals(IntRange other) => SetEqual(this, other);
        /// <summary>
        /// Whether the two ranges have exactly the same elements, ignoring order.
        /// </summary>
        /// <remarks>
        /// Note that ranges with different boundaries can have the same elements (and therefore be considered set-equal), due to the boundaries being inclusive or exclusive.
        /// </remarks>
        /// <seealso cref="Equals(IntRange)"/>
        /// <seealso cref="SequenceEquals(IntRange)"/>
        public static bool SetEqual(IntRange a, IntRange b) => (a.isEmpty, b.isEmpty) switch
        {
            (false, false) => a.minElement == b.minElement && a.maxElement == b.maxElement,
            (false, true) or (true, false) => false,
            (true, true) => true,
        };

        public bool IsSubsetOf(IntRange other) => isEmpty || (!other.isEmpty && other.minElement <= minElement && maxElement <= other.maxElement);
        public bool IsSupersetOf(IntRange other) => other.IsSubsetOf(this);

        /// <summary>
        /// Whether the given integer is in the range.
        /// </summary>
        public bool Contains(int x) => !isEmpty && minElement <= x && x <= maxElement;

        /// <summary>
        /// Whether the two ranges have any elements in common.
        /// </summary>
        /// <seealso cref="Intersect(IntRange, IntRange)"/>
        public bool Intersects(IntRange other) => Intersect(this, other);
        /// <summary>
        /// Whether the two ranges have any elements in common.
        /// </summary>
        /// <seealso cref="Intersects(IntRange)"/>
        public static bool Intersect(IntRange a, IntRange b)
        {
            if (a.isEmpty || b.isEmpty)
            {
                return false;
            }
            if (a.maxElement <= b.maxElement)
            {
                return b.minElement <= a.maxElement;
            }
            else
            {
                return a.minElement <= b.maxElement;
            }
        }
        #endregion

        #region Operations
        /// <summary>
        /// Translates the range by the given integer.
        /// </summary>
        /// <exception cref="OverflowException">If any translated elements cannot be expressed as an <see cref="int"/>.</exception>
        public static IntRange operator +(IntRange range, int integer) =>
            checked(new IntRange(range.startBoundary + integer, range.endBoundary + integer, range.startBoundaryInclusive, range.endBoundaryInclusive));
        /// <summary>
        /// Translates the range by the given integer.
        /// </summary>
        /// <exception cref="OverflowException">If any translated elements cannot be expressed as an <see cref="int"/>.</exception>
        public static IntRange operator +(int integer, IntRange range) => range + integer;
        /// <summary>
        /// Translates the range by the given integer.
        /// </summary>
        /// <exception cref="OverflowException">If any translated elements cannot be expressed as an <see cref="int"/>.</exception>
        public static IntRange operator -(IntRange range, int integer) => range + (-integer);
        /// <summary>
        /// Subtracts each element of the range from the given integer.
        /// </summary>
        /// <exception cref="OverflowException">If any translated elements cannot be expressed as an <see cref="int"/>.</exception>
        public static IntRange operator -(int integer, IntRange range) => integer + (-range);
        /// <summary>
        /// Negates each element of the range.
        /// </summary>
        /// <exception cref="OverflowException">If any negated elements cannot be expressed as an <see cref="int"/>. This only occurs if the range contains <see cref="int.MinValue"/>.</exception>
        public static IntRange operator -(IntRange range) => checked(new IntRange(-range.startBoundary, -range.endBoundary, range.startBoundaryInclusive, range.endBoundaryInclusive));

        /// <summary>
        /// Adds <paramref name="startBoundaryOffset"/> to <see cref="startBoundary"/> and adds <paramref name="endBoundaryOffset"/> to <see cref="endBoundary"/>.
        /// </summary>
        /// <remarks>
        /// Preserves <see cref="startBoundaryInclusive"/> and <see cref="endBoundaryInclusive"/>.
        /// </remarks>
        public IntRange Extend(int startBoundaryOffset, int endBoundaryOffset)
            => new IntRange(startBoundary + startBoundaryOffset, endBoundary + endBoundaryOffset, startBoundaryInclusive, endBoundaryInclusive);

        /// <summary>
        /// Returns the elements the two ranges have in common.
        /// </summary>
        /// <returns>
        /// The intersection of the two ranges, expressed with both boundaries inclusive, unless the intersection is empty, in which case it will be expressed with both boundaries exclusive.
        /// </returns>
        /// <seealso cref="Intersection(IntRange, IntRange)"/>
        public IntRange Intersection(IntRange other) => Intersection(this, other);
        /// <summary>
        /// Returns the elements the two ranges have in common.
        /// </summary>
        /// <returns>
        /// The intersection of the two ranges. It will be expressed with both boundaries inclusive, unless the intersection is empty, in which case it will be expressed with both boundaries
        /// exclusive.
        /// </returns>
        /// <seealso cref="Intersection(IntRange)"/>
        public static IntRange Intersection(IntRange a, IntRange b) => Intersect(a, b) ? InclIncl(Math.Max(a.minElement, b.minElement), Math.Min(a.maxElement, b.maxElement)) : Empty;

        /// <summary>
        /// Clamps the given integer to be within the <see cref="IntRange"/>.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item><paramref name="n"/> if <see cref="minElement"/> &lt;= <paramref name="n"/> &lt;= <see cref="maxElement"/></item>
        /// <item><see cref="minElement"/> if <paramref name="n"/> &lt; <see cref="minElement"/></item>
        /// <item><see cref="maxElement"/> if <see cref="maxElement"/> &lt; <paramref name="n"/></item>
        /// </list>
        /// </returns>
        /// <param name="n">The value to clamp.</param>
        /// <exception cref="InvalidOperationException">The range is empty.</exception>
        /// <seealso cref="Math.Clamp(int, int, int)"/>
        public int Clamp(int n) => isEmpty ? throw new InvalidOperationException("The range is empty.") : Math.Clamp(n, minElement, maxElement);
        #endregion

        #region Enumerator
        /// <summary>
        /// Indexes the elements of the range from <see cref="startElement"/> to <see cref="endElement"/>.
        /// </summary>
        /// <remarks>
        /// Some ranges, such as a range from <see cref="int.MinValue"/> to <see cref="int.MaxValue"/>, are too long for every element to be indexed by an <see cref="int"/>. Using
        /// <see cref="this[long]"/> instead will never have this issue.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the range.</exception>
        public int this[int index] => this[(long)index];
        /// <summary>
        /// Indexes the elements of the range from <see cref="startElement"/> to <see cref="endElement"/>.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="this[int]"/>, every element in a range can be indexed with a <see cref="long"/>. For example, the range from <see cref="int.MinValue"/> to <see cref="int.MaxValue"/>
        /// is too long for every element to be indexed by an <see cref="int"/>, but every element can be indexed by a <see cref="long"/>.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the range.</exception>
        public int this[long index]
        {
            get
            {
                int value;
                try
                {
                    value = isEmpty switch
                    {
                        false => checked((int)((startBoundary < endBoundary) ? (startElement + index) : (startElement - index))),
                        true => throw new ArgumentOutOfRangeException(nameof(index), "The range is empty.")
                    };
                }
                catch (OverflowException)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"The index is out of range of the number of elements. Range: {this}. Count: {LongCount}. Index: {index}.");
                }

                return Contains(value) switch
                {
                    false => throw new ArgumentOutOfRangeException(nameof(index), $"The index is out of range of the number of elements. Range: {this}. Count: {LongCount}. Index: {index}."),
                    true => value
                };
            }
        }

        /// <summary>
        /// Iterates throught the elements from <see cref="startElement"/> to <see cref="endElement"/>.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        /// <summary>
        /// Iterates throught the elements from <see cref="startElement"/> to <see cref="endElement"/>.
        /// </summary>
        public IEnumerator<int> GetEnumerator()
        {
            if (isEmpty)
            {
                yield break;
            }

            int startElement = this.startElement;
            int endElement = this.endElement;
            if (startElement < endElement)
            {
                for (int i = startElement; i <= endElement; i++)
                {
                    yield return i;
                }
            }
            else
            {
                for (int i = startElement; i >= endElement; i--)
                {
                    yield return i;
                }
            }
        }
        #endregion

        /// <summary>
        /// <para>
        /// Represents the range as a string using the mathematical notation for intervals - i.e. square brackets denote an inclusive boundary and round brackets denote an exclusive boundary.
        /// </para>
        /// <para>
        /// <example>
        /// For example, the range from -1 (inclusive) to 5 (exclusive) will be represented as <c>"[-1, 5)"</c>.
        /// </example>
        /// </para>
        /// </summary>
        public override string ToString() => $"{(startBoundaryInclusive ? "[" : "(")}{startBoundary}, {endBoundary}{(endBoundaryInclusive ? "]" : ")")}";
    }
}

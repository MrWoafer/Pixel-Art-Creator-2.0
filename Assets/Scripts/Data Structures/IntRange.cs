using PAC.Exceptions;
using PAC.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PAC.DataStructures
{
    /// <summary>
    /// <para>
    /// A set of consecutive integers.
    /// </para>
    /// <para>
    /// If the start boundary and end boundary are equal but their being inclusive / exclusive do not match, we define the range to be empty.
    /// </para>
    /// <para>
    /// The enumerator iterates throught the elements from start to end.
    /// </para>
    /// </summary>
    public readonly struct IntRange : IReadOnlyList<int>, IEquatable<IntRange>
    {
        #region Fields
        /// <summary>
        /// <para>
        /// The start of the range. Can be inclusive or exclusive.
        /// </para>
        /// <para>
        /// Will be different from startElement if this boundary is exclusive.
        /// </para>
        /// </summary>
        public readonly int startBoundary;
        /// <summary>
        /// <para>
        /// The end of the range. Can be inclusive or exclusive.
        /// </para>
        /// <para>
        /// Will be different from endElement if this boundary is exclusive.
        /// </para>
        /// </summary>
        public readonly int endBoundary;
        /// <summary>
        /// <para>
        /// Whether the start boundary is inclusive or exclusive.
        /// </para>
        /// <para>
        /// If the start boundary and end boundary are equal but their being inclusive / exclusive do not match, we define the range to be empty.
        /// </para>
        /// </summary>
        public readonly bool startBoundaryInclusive;
        /// <summary>
        /// <para>
        /// Whether the end boundary is inclusive or exclusive.
        /// </para>
        /// <para>
        /// If the start boundary and end boundary are equal but their being inclusive / exclusive do not match, we define the range to be empty.
        /// </para>
        /// </summary>
        public readonly bool endBoundaryInclusive;
        #endregion

        #region Properties
        /// <summary>
        /// <para>
        /// The first element when iterating through the range (from start to end).
        /// </para>
        /// <para>
        /// Will be different from startBoundary if that boundary is exclusive.
        /// </para>
        /// </summary>
        public int startElement => (isEmpty, startBoundaryInclusive) switch
        {
            (false, false) => (startBoundary < endBoundary) ? startBoundary + 1 : startBoundary - 1,
            (false, true) => startBoundary,
            (true, _) => throw new UndefinedOperationException("An empty range has no start element.")
        };
        /// <summary>
        /// <para>
        /// The last element when iterating through the range (from start to end).
        /// </para>
        /// <para>
        /// Will be different from endBoundary if that boundary is exclusive.
        /// </para>
        /// </summary>
        public int endElement => (isEmpty, endBoundaryInclusive) switch
        {
            (false, false) => (startBoundary < endBoundary) ? endBoundary - 1 : endBoundary + 1,
            (false, true) => endBoundary,
            (true, _) => throw new UndefinedOperationException("An empty range has no start element.")
        };

        /// <summary>
        /// <para>
        /// The lower of startBoundary and endBoundary.
        /// </para>
        /// <para>
        /// Will be different from minElement if this boundary is exclusive.
        /// </para>
        /// </summary>
        public int minBoundary => Math.Min(startBoundary, endBoundary);
        /// <summary>
        /// <para>
        /// The larger of startBoundary and endBoundary.
        /// </para>
        /// <para>
        /// Will be different from maxElement if this boundary is exclusive.
        /// </para>
        /// </summary>
        public int maxBoundary => Math.Max(startBoundary, endBoundary);
        /// <summary>
        /// <para>
        /// Whether minBoundary is inclusive or exclusive.
        /// </para>
        /// <para>
        /// If the min boundary and max boundary are equal but their being inclusive / exclusive do not match, we define the range to be empty.
        /// </para>
        /// </summary>
        public bool minBoundaryInclusive => (endBoundary - startBoundary) switch
        {
            > 0 => startBoundaryInclusive,
            0 => startBoundaryInclusive && endBoundaryInclusive,
            < 0 => endBoundaryInclusive,
        };
        /// <summary>
        /// <para>
        /// Whether maxBoundary is inclusive or exclusive.
        /// </para>
        /// <para>
        /// If the min boundary and max boundary are equal but their being inclusive / exclusive do not match, we define the range to be empty.
        /// </para>
        /// </summary>
        public bool maxBoundaryInclusive => (endBoundary - startBoundary) switch
        {
            > 0 => endBoundaryInclusive,
            0 => startBoundaryInclusive && endBoundaryInclusive,
            < 0 => startBoundaryInclusive,
        };

        /// <summary>
        /// <para>
        /// The lowest element in the range.
        /// </para>
        /// <para>
        /// Will be different from minBoundary if that boundary is exclusive.
        /// </para>
        /// </summary>
        public int minElement => isEmpty switch
        {
            false => minBoundaryInclusive ? minBoundary : (minBoundary + 1),
            true => throw new UndefinedOperationException("An empty range has no minimum element.")
        };
        /// <summary>
        /// <para>
        /// The largest element in the range.
        /// </para>
        /// <para>
        /// Will be different from maxBoundary if that boundary is exclusive.
        /// </para>
        /// </summary>
        public int maxElement => isEmpty switch
        {
            false => maxBoundaryInclusive ? maxBoundary : (maxBoundary - 1),
            true => throw new UndefinedOperationException("An empty range has no maximum element.")
        };

        /// <summary>
        /// A range with the start boundary and end boundary swapped (and startBoundaryInclusive and endBoundaryInclusive swapped).
        /// </summary>
        public IntRange reverse => new IntRange(endBoundary, startBoundary, endBoundaryInclusive, startBoundaryInclusive);
        /// <summary>
        /// Returns a range with the same elements, in increasing order. Will either return the range unchanged or <see cref="reverse"/>.
        /// </summary>
        public IntRange asIncreasing => startBoundary <= endBoundary ? this : reverse;
        /// <summary>
        /// Returns a range with the same elements, in decreasing order. Will either return the range unchanged or <see cref="reverse"/>.
        /// </summary>
        public IntRange asDecreasing => endBoundary <= startBoundary ? this : reverse;

        /// <summary>
        /// Returns a range with the same elements, in the same order, but expressed with the start boundary exclusive and the end boundary exclusive.
        /// </summary>
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
        /// Returns a range with the same elements, in the same order, but expressed with the start boundary exclusive and the end boundary inclusive.
        /// </summary>
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
        /// Returns a range with the same elements, in the same order, but expressed with the start boundary inclusive and the end boundary exclusive.
        /// </summary>
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
        /// Returns a range with the same elements, in the same order, but expressed with the start boundary inclusive and the end boundary inclusive.
        /// </summary>
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
        /// Whether the start boundary is exclusive and the end boundary is exclusive.
        /// </summary>
        public bool isExclExcl => !startBoundaryInclusive && !endBoundaryInclusive;
        /// <summary>
        /// Whether the start boundary is exclusive and the end boundary is inclusive.
        /// </summary>
        public bool isExclIncl => !startBoundaryInclusive && endBoundaryInclusive;
        /// <summary>
        /// Whether the start boundary is inclusive and the end boundary is exclusive.
        /// </summary>
        public bool isInclExcl => startBoundaryInclusive && !endBoundaryInclusive;
        /// <summary>
        /// Whether the start boundary is inclusive and the end boundary is inclusive.
        /// </summary>
        public bool isInclIncl => startBoundaryInclusive && endBoundaryInclusive;

        /// <summary>
        /// May throw overflow exceptions. E.g. the Count of a range from int.MinValue to int.MaxValue cannot be expressed as an int. Use CountLong to avoid overflow issues.
        /// </summary>
        public int Count => isEmpty ? 0 : checked(Math.Abs(endElement - startElement) + 1);

        /// <summary>
        /// The same as Count but computed as a long, so will never have overflow issues. E.g. the largest range, int.MinValue to int.MaxValue, cannot be expressed as an int but can be expressed as
        /// a long.
        /// </summary>
        public long CountLong => isEmpty ? 0 : checked(Math.Abs((long)endElement - (long)startElement) + 1);
        #endregion

        #region Predefined Instances
        /// <summary>
        /// The range from int.MinValue (inclusive) to int.MaxValue (inclusive).
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
        /// <param name="startBoundary">The start of the range. Can be inclusive or exclusive.</param>
        /// <param name="endBoundary">The end of the range. Can be inclusive or exclusive.</param>
        /// <param name="startBoundaryInclusive">
        /// <para>
        /// Whether the start boundary is inclusive or exclusive.
        /// </para>
        /// <para>
        /// If the start boundary and end boundary are equal but their being inclusive / exclusive do not match, we define the range to be empty.
        /// </para>
        /// </param>
        /// <param name="endBoundaryInclusive">
        /// <para>
        /// Whether the end boundary is inclusive or exclusive.
        /// </para>
        /// <para>
        /// If the start boundary and end boundary are equal but their being inclusive / exclusive do not match, we define the range to be empty.
        /// </para>
        /// </param>
        public IntRange(int startBoundary, int endBoundary, bool startBoundaryInclusive, bool endBoundaryInclusive)
        {
            this.startBoundary = startBoundary;
            this.endBoundary = endBoundary;
            this.startBoundaryInclusive = startBoundaryInclusive;
            this.endBoundaryInclusive = endBoundaryInclusive;
        }

        /// <summary>
        /// Creates a range from x (inclusive) to x (inclusive).
        /// </summary>
        public static IntRange Singleton(int x) => new IntRange(x, x, true, true);

        /// <summary>
        /// Creates a range from x (inclusive) to int.MinValue (inclusive).
        /// </summary>
        public static IntRange AtMost(int x) => new IntRange(x, int.MinValue, true, true);
        /// <summary>
        /// Creates a range from x (exclusive) to int.MinValue (inclusive).
        /// </summary>
        public static IntRange LessThan(int x) => new IntRange(x, int.MinValue, false, true);
        /// <summary>
        /// Creates a range from x (inclusive) to int.MaxValue (inclusive).
        /// </summary>
        public static IntRange AtLeast(int x) => new IntRange(x, int.MaxValue, true, true);
        /// <summary>
        /// Creates a range from x (exclusive) to int.MaxValue (inclusive).
        /// </summary>
        public static IntRange MoreThan(int x) => new IntRange(x, int.MaxValue, false, true);

        /// <summary>
        /// Creates a range from startBoundary (exclusive) to endBoundary (exclusive).
        /// </summary>
        public static IntRange ExclExcl(int startBoundary, int endBoundary) => new IntRange(startBoundary, endBoundary, false, false);
        /// <summary>
        /// <para>
        /// Creates a range from startBoundary (exclusive) to endBoundary (inclusive).
        /// </para>
        /// <para>
        /// If the start boundary and end boundary are equal, we define the range to be empty.
        /// </para>
        /// </summary>
        public static IntRange ExclIncl(int startBoundary, int endBoundary) => new IntRange(startBoundary, endBoundary, false, true);
        /// <summary>
        /// <para>
        /// Creates a range from startBoundary (inclusive) to endBoundary (exclusive).
        /// </para>
        /// <para>
        /// If the start boundary and end boundary are equal, we define the range to be empty.
        /// </para>
        /// </summary>
        public static IntRange InclExcl(int startBoundary, int endBoundary) => new IntRange(startBoundary, endBoundary, true, false);
        /// <summary>
        /// Creates a range from startBoundary (inclusive) to endBoundary (inclusive).
        /// </summary>
        public static IntRange InclIncl(int startBoundary, int endBoundary) => new IntRange(startBoundary, endBoundary, true, true);
        #endregion

        #region Casting
        /// <summary>
        /// Creates a range from x (inclusive) to x (inclusive).
        /// </summary>
        public static implicit operator IntRange(int x) => Singleton(x);
        #endregion

        #region Comparison
        /// <summary>
        /// <para>
        /// Whether the two ranges have the same start boundary and end boundary and the same inclusivity at each boundary.
        /// </para>
        /// <para>
        /// Note that having the same elements does not necessarily mean they are equal in this sense, due to the boundaries being inclusive or exclusive.
        /// </para>
        /// </summary>
        public static bool operator ==(IntRange a, IntRange b) =>
            a.startBoundary == b.startBoundary
            && a.endBoundary == b.endBoundary
            && a.startBoundaryInclusive == b.startBoundaryInclusive
            && a.endBoundaryInclusive == b.endBoundaryInclusive;
        /// <summary>
        /// <para>
        /// Whether the two ranges have the same start boundary and end boundary and the same inclusivity at each boundary.
        /// </para>
        /// <para>
        /// Note that being unequal in this sense does not necessarily mean they have different elements, due to the boundaries being inclusive or exclusive.
        /// </para>
        /// </summary>
        public static bool operator !=(IntRange a, IntRange b) => !(a == b);
        /// <summary>
        /// <para>
        /// Whether the two ranges have the same start boundary and end boundary and the same inclusivity at each boundary.
        /// </para>
        /// <para>
        /// Note that having the same elements does not necessarily mean they are equal in this sense, due to the boundaries being inclusive or exclusive.
        /// </para>
        /// </summary>
        public bool Equals(IntRange other) => this == other;
        /// <summary>
        /// <para>
        /// Whether the object is a range and the two ranges have the same start boundary and end boundary and the same inclusivity at each boundary.
        /// </para>
        /// <para>
        /// Note that having the same elements does not necessarily mean they are equal in this sense, due to the boundaries being inclusive or exclusive.
        /// </para>
        /// </summary>
        public override bool Equals(object obj) => obj is IntRange && Equals((IntRange)obj);

        /// <summary>
        /// <para>
        /// Whether the two ranges have exactly the same elements in the same order.
        /// </para>
        /// <para>
        /// Note that ranges with different boundaries can have the same elements (and therefore be considered equal), due to the boundaries being inclusive or exclusive.
        /// </para>
        /// </summary>
        public bool SequenceEquals(IntRange other) => SequenceEqual(this, other);
        /// <summary>
        /// <para>
        /// Whether the two ranges have exactly the same elements in the same order.
        /// </para>
        /// <para>
        /// Note that ranges with different boundaries can have the same elements (and therefore be considered equal), due to the boundaries being inclusive or exclusive.
        /// </para>
        /// </summary>
        public static bool SequenceEqual(IntRange a, IntRange b) => (a.isEmpty, b.isEmpty) switch
        {
            (false, false) => a.startElement == b.startElement && a.endElement == b.endElement,
            (false, true) or (true, false) => false,
            (true, true) => true,
        };

        /// <summary>
        /// <para>
        /// Whether the two ranges have exactly the same elements, ignoring order.
        /// </para>
        /// <para>
        /// Note that ranges with different boundaries can have the same elements (and therefore be considered equal), due to the boundaries being inclusive or exclusive.
        /// </para>
        /// </summary>
        public bool SetEquals(IntRange other) => SetEqual(this, other);
        /// <summary>
        /// <para>
        /// Whether the two ranges have exactly the same elements, ignoring order.
        /// </para>
        /// <para>
        /// Note that ranges with different boundaries can have the same elements (and therefore be considered equal), due to the boundaries being inclusive or exclusive.
        /// </para>
        /// </summary>
        public static bool SetEqual(IntRange a, IntRange b) => (a.isEmpty, b.isEmpty) switch
        {
            (false, false) => a.minElement == b.minElement && a.maxElement == b.maxElement,
            (false, true) or (true, false) => false,
            (true, true) => true,
        };

        public bool IsSubsequenceOf(IntRange other) => isEmpty || (!other.isEmpty && other.minElement <= minElement && maxElement <= other.maxElement);
        public bool IsProperSubsequenceOf(IntRange other) => IsSubsequenceOf(other) && !SequenceEqual(this, other);

        public bool IsSupersequenceOf(IntRange other) => other.IsSubsequenceOf(this);
        public bool IsProperSupersequenceOf(IntRange other) => IsSupersequenceOf(other) && !SequenceEqual(this, other);

        public bool IsSubsetOf(IntRange other) => isEmpty || (!other.isEmpty && other.minElement <= minElement && maxElement <= other.maxElement);
        public bool IsProperSubsetOf(IntRange other) => IsSubsetOf(other) && !SetEqual(this, other);

        public bool IsSupersetOf(IntRange other) => other.IsSubsetOf(this);
        public bool IsProperSupersetOf(IntRange other) => IsSupersetOf(other) && !SetEqual(this, other);

        /// <summary>
        /// Whether the given integer is in the range.
        /// </summary>
        public bool Contains(int x) => !isEmpty && minElement <= x && x <= maxElement;
        /// <summary>
        /// Whether all the given integers are in the range.
        /// </summary>
        public bool ContainsAll(params int[] elements) => ContainsAll((IEnumerable<int>)elements);
        public bool ContainsAll(IEnumerable<int> elements)
        {
            if (elements.IsEmpty())
            {
                throw new ArgumentException("No elements were given.", nameof(elements));
            }

            // C# can't access 'this' inside lambda expressions.
            IntRange range = this;
            return elements.All(x => range.Contains(x));
        }
        /// <summary>
        /// Whether any of the given integers are in the range.
        /// </summary>
        public bool ContainsAny(params int[] elements) => ContainsAny((IEnumerable<int>)elements);
        public bool ContainsAny(IEnumerable<int> elements)
        {
            if (elements.IsEmpty())
            {
                throw new ArgumentException("No elements were given.", nameof(elements));
            }

            // C# can't access 'this' inside lambda expressions.
            IntRange range = this;
            return elements.Any(x => range.Contains(x));
        }
        /// <summary>
        /// Whether none of the given integers are in the range.
        /// </summary>
        public bool ContainsNone(params int[] elements) => ContainsNone((IEnumerable<int>)elements);
        public bool ContainsNone(IEnumerable<int> elements)
        {
            if (elements.IsEmpty())
            {
                throw new ArgumentException("No elements were given.", nameof(elements));
            }

            // C# can't access 'this' inside lambda expressions.
            IntRange range = this;
            return elements.None(x => range.Contains(x));
        }

        /// <summary>
        /// Whether the two ranges have any elements in common.
        /// </summary>
        public bool Intersects(IntRange other) => Intersect(this, other);
        /// <summary>
        /// Whether the two ranges have any elements in common.
        /// </summary>
        public static bool Intersect(IntRange a, IntRange b) => (a.isEmpty || b.isEmpty) ? false : ((a.maxElement <= b.maxElement) switch
        {
            true => b.minElement <= a.maxElement,
            false => a.minElement <= b.maxElement,
        });
        #endregion

        #region Operations
        /// <summary>
        /// Shifts the range by the given integer.
        /// </summary>
        public static IntRange operator +(IntRange range, int integer) =>
            checked(new IntRange(range.startBoundary + integer, range.endBoundary + integer, range.startBoundaryInclusive, range.endBoundaryInclusive));
        /// <summary>
        /// Shifts the range by the given integer.
        /// </summary>
        public static IntRange operator +(int integer, IntRange range) => range + integer;
        /// <summary>
        /// Shifts the range by the given integer.
        /// </summary>
        public static IntRange operator -(IntRange range, int integer) => range + (-integer);
        /// <summary>
        /// Subtracts each element of the range from the given integer.
        /// </summary>
        public static IntRange operator -(int integer, IntRange range) => integer + (-range);
        /// <summary>
        /// Negates each element of the range.
        /// </summary>
        public static IntRange operator -(IntRange range) => checked(new IntRange(-range.startBoundary, -range.endBoundary, range.startBoundaryInclusive, range.endBoundaryInclusive));

        /// <summary>
        /// Returns the Cartesian product of the ranges, as an IntRect. x specifies the range of the x coords. y specifies the range of the y coords.
        /// </summary>
        public static IntRect operator *(IntRange x, IntRange y) => CartesianProduct(x, y);
        /// <summary>
        /// Returns the Cartesian product of the ranges, as an IntRect. This range specifies the range of the x coords. y specifies the range of the y coords.
        /// </summary>
        public IntRect CartesianProduct(IntRange y) => CartesianProduct(this, y);
        /// <summary>
        /// Returns the Cartesian product of the ranges, as an IntRect. x specifies the range of the x coords. y specifies the range of the y coords.
        /// </summary>
        public static IntRect CartesianProduct(IntRange x, IntRange y) => (x.isEmpty, y.isEmpty) switch
        {
            (false, false) => new IntRect(new IntVector2(x.minElement, y.minElement), new IntVector2(x.maxElement, y.maxElement)),
            (false, true) => throw new InvalidOperationException("The y range is empty."),
            (true, false) => throw new InvalidOperationException("The x range is empty."),
            (true, true) => throw new InvalidOperationException("The x range and the y range are both empty.")
        };

        /// <summary>
        /// Returns the elements the two ranges have in common.
        /// </summary>
        public IntRange Intersection(IntRange other) => Intersection(this, other);
        /// <summary>
        /// Returns the elements the two ranges have in common.
        /// </summary>
        public static IntRange Intersection(IntRange a, IntRange b) => Intersect(a, b) ? InclIncl(Math.Max(a.minElement, b.minElement), Math.Min(a.maxElement, b.maxElement)) : Empty;
        #endregion

        #region Enumerator
        /// <summary>
        /// Indexes the elements of the range from start to end.
        /// </summary>
        public int this[int index] => this[(long)index];
        /// <summary>
        /// Indexes the elements of the range from start to end.
        /// </summary>
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
                        true => throw new IndexOutOfRangeException("Range is empty.")
                    };
                }
                catch (OverflowException)
                {
                    throw new IndexOutOfRangeException($"Index is out of range of the number of elements. Range: {this}. Count: {CountLong}. Index: {index}.");
                }

                return Contains(value) switch
                {
                    false => throw new IndexOutOfRangeException($"Index is out of range of the number of elements. Range: {this}. Count: {CountLong}. Index: {index}."),
                    true => value
                };
            }
        }

        /// <summary>
        /// Iterates throught the elements from start to end.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        /// <summary>
        /// Iterates throught the elements from start to end.
        /// </summary>
        public IEnumerator<int> GetEnumerator()
        {
            if (isEmpty)
            {
                yield break;
            }
            else if (startElement < endElement)
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

        public override string ToString() => $"{(startBoundaryInclusive ? "[" : "(")}{startBoundary}, {endBoundary}{(endBoundaryInclusive ? "]" : ")")}";

        public override int GetHashCode() => HashCode.Combine(startBoundary, endBoundary, startBoundaryInclusive, endBoundaryInclusive);
    }
}

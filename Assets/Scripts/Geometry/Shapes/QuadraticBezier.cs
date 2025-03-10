using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PAC.DataStructures;
using PAC.Geometry.Axes;
using PAC.Geometry.Shapes.Interfaces;
using PAC.Maths;

using UnityEngine;

namespace PAC.Geometry.Shapes
{
    public class QuadraticBezier : I1DShape<QuadraticBezier>, IDeepCopyableShape<QuadraticBezier>, IEquatable<QuadraticBezier>
    {
        public IntVector2 start;
        public IntVector2 end;
        public IntVector2 control;

        public QuadraticBezier reverse => new QuadraticBezier(end, control, start);

        public IntRect boundingRect => IntRect.BoundingRect(this);

        public int Count => Enumerable.Count(this);

        private Vector2 Evaluate(float t) => (1f - t).Square() * (Vector2)start + 2f * (1f - t) * t * (Vector2)control + t.Square() * (Vector2)end;

        public QuadraticBezier(IntVector2 start, IntVector2 control, IntVector2 end)
        {
            this.start = start;
            this.end = end;
            this.control = control;
        }

        public bool Contains(IntVector2 item) => Enumerable.Contains(this, item);

        /// <summary>
        /// Returns a deep copy of the <see cref="QuadraticBezier"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static QuadraticBezier operator +(QuadraticBezier bezier, IntVector2 translation) => bezier.Translate(translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="QuadraticBezier"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static QuadraticBezier operator +(IntVector2 translation, QuadraticBezier bezier) => bezier + translation;
        /// <summary>
        /// Returns a deep copy of the <see cref="QuadraticBezier"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static QuadraticBezier operator -(QuadraticBezier bezier, IntVector2 translation) => bezier + (-translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="QuadraticBezier"/> rotated 180 degrees about the origin (equivalently, reflected through the origin).
        /// </summary>
        /// <seealso cref="Rotate(QuadrantalAngle)"/>
        /// <seealso cref="Flip(CardinalOrdinalAxis)"/>
        public static QuadraticBezier operator -(QuadraticBezier bezier) => new QuadraticBezier(-bezier.start, -bezier.control, -bezier.end);

        public QuadraticBezier Translate(IntVector2 translation) => new QuadraticBezier(start + translation, control + translation, end + translation);
        public QuadraticBezier Flip(CardinalOrdinalAxis axis) => new QuadraticBezier(start.Flip(axis), control.Flip(axis), end.Flip(axis));
        public QuadraticBezier Rotate(QuadrantalAngle angle) => new QuadraticBezier(start.Rotate(angle), control.Rotate(angle), end.Rotate(angle));

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator()
        {
            float t = 0f;
            IntVector2 point = start;
            int iterations = 0;
            while (IntVector2.SupDistance(point, end) > 1 && iterations < 10_000)
            {
                iterations++;

                yield return point;

                float newT = (1f + t) / 2f;
                IntVector2 newPoint = IntVector2.RoundToIntVector2(Evaluate(newT));

                if (newPoint == point)
                {
                    break;
                }
                int iterations2 = 0;
                while (IntVector2.SupDistance(newPoint, point) > 1 && iterations2 < 1_000)
                {
                    iterations2++;
                    newT = (newT + t) / 2f;
                    newPoint = IntVector2.RoundToIntVector2(Evaluate(newT));
                }

                t = newT;
                point = newPoint;
            }

            yield return point;
            yield return end;
        }

        /// <summary>
        /// Whether the two <see cref="QuadraticBezier"/>s have the same <see cref="start"/>, <see cref="end"/> and <see cref="control"/>.
        /// </summary>
        public static bool operator ==(QuadraticBezier a, QuadraticBezier b) => a.start == b.start && a.end == b.end && a.control == b.control;
        /// <summary>
        /// See <see cref="operator ==(QuadraticBezier, QuadraticBezier)"/>.
        /// </summary>
        public static bool operator !=(QuadraticBezier a, QuadraticBezier b) => !(a == b);
        /// <summary>
        /// See <see cref="operator ==(QuadraticBezier, QuadraticBezier)"/>.
        /// </summary>
        public bool Equals(QuadraticBezier other) => this == other;
        /// <summary>
        /// See <see cref="Equals(QuadraticBezier)"/>
        /// </summary>
        public override bool Equals(object obj) => obj is QuadraticBezier other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(start, end);

        public override string ToString() => $"{nameof(QuadraticBezier)}({start}, {control}, {end})";

        public QuadraticBezier DeepCopy() => new QuadraticBezier(start, control, end);
    }
}
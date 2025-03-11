using System;
using System.Collections.Generic;

using PAC.DataStructures;
using PAC.Exceptions;

namespace PAC.Geometry
{
    /// <summary>
    /// One of the following 8 directions:
    /// <list type="bullet">
    /// <item>Up</item>
    /// <item>Down</item>
    /// <item>Left</item>
    /// <item>Right</item>
    /// <item>Up-Right</item>
    /// <item>Up-Left</item>
    /// <item>Down-Right</item>
    /// <item>Down-Left</item>
    /// </list>
    /// </summary>
    public readonly struct Direction8 : IEquatable<Direction8>
    {
        /// <summary>
        /// The <see langword="enum"/> that backs <see cref="Direction8"/>.
        /// </summary>
        private enum Direction : byte
        {
            Up,
            UpRight,
            Right,
            DownRight,
            Down,
            DownLeft,
            Left,
            UpLeft
        }
        /// <summary>
        /// The field that backs <see cref="Direction8"/>.
        /// </summary>
        private readonly Direction direction { get; init; }

        /// <summary>
        /// The direction <c>(0, 1)</c>.
        /// </summary>
        public static readonly Direction8 Up = new Direction8 { direction = Direction.Up };
        /// <summary>
        /// The direction <c>(0, -1)</c>.
        /// </summary>
        public static readonly Direction8 Down = new Direction8 { direction = Direction.Down };
        /// <summary>
        /// The direction <c>(-1, 0)</c>.
        /// </summary>
        public static readonly Direction8 Left = new Direction8 { direction = Direction.Left };
        /// <summary>
        /// The direction <c>(1, 0)</c>.
        /// </summary>
        public static readonly Direction8 Right = new Direction8 { direction = Direction.Right };

        /// <summary>
        /// The direction <c>(1, 1)</c>.
        /// </summary>
        public static readonly Direction8 UpRight = new Direction8 { direction = Direction.UpRight };
        /// <summary>
        /// The direction <c>(-1, 1)</c>.
        /// </summary>
        public static readonly Direction8 UpLeft = new Direction8 { direction = Direction.UpLeft };
        /// <summary>
        /// The direction <c>(1, -1)</c>.
        /// </summary>
        public static readonly Direction8 DownRight = new Direction8 { direction = Direction.DownRight };
        /// <summary>
        /// The direction <c>(-1, -1)</c>.
        /// </summary>
        public static readonly Direction8 DownLeft = new Direction8 { direction = Direction.DownLeft };

        /// <summary>
        /// The directions <see cref="Up"/>, <see cref="Down"/>, <see cref="Left"/> and <see cref="Right"/>.
        /// </summary>
        public static readonly IEnumerable<Direction8> UpDownLeftRight = new Direction8[] { Up, Down, Left, Right };
        /// <summary>
        /// The directions <see cref="UpRight"/>, <see cref="UpLeft"/>, <see cref="DownRight"/> and <see cref="DownLeft"/>.
        /// </summary>
        public static readonly IEnumerable<Direction8> Diagonals = new Direction8[] { UpRight, UpLeft, DownRight, DownLeft };
        /// <summary>
        /// All 8 directions.
        /// </summary>
        public static readonly IEnumerable<Direction8> All = new Direction8[] { Up, Down, Left, Right, UpRight, UpLeft, DownRight, DownLeft };

        public void Deconstruct(out int x, out int y) => (x, y) = (IntVector2)this;

        /// <summary>
        /// Converts the <see cref="Direction8"/> into the corresponding <see cref="IntVector2"/> with <see cref="IntVector2.supNorm"/> 1.
        /// </summary>
        /// <param name="direction"></param>
        public static implicit operator IntVector2(Direction8 direction) => direction.direction switch
        {
            Direction.Up => (0, 1),
            Direction.UpRight => (1, 1),
            Direction.Right => (1, 0),
            Direction.DownRight => (1, -1),
            Direction.Down => (0, -1),
            Direction.DownLeft => (-1, -1),
            Direction.Left => (-1, 0),
            Direction.UpLeft => (-1, 1),
            _ => throw new UnreachableException()
        };
        /// <summary>
        /// Converts an <see cref="IntVector2"/> with <see cref="IntVector2.supNorm"/> 1 into the corresponding <see cref="Direction8"/>.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="direction"/> does not have <see cref="IntVector2.supNorm"/> 1.</exception>
        public static explicit operator Direction8(IntVector2 direction) => direction switch
        {
            (0, 1) => Up,
            (1, 1) => UpRight,
            (1, 0) => Right,
            (1, -1) => DownRight,
            (0, -1) => Down,
            (-1, -1) => DownLeft,
            (-1, 0) => Left,
            (-1, 1) => UpLeft,
            _ => throw new ArgumentException($"{direction} is an invalid vector for a direction.", nameof(direction))
        };

        public static bool operator ==(Direction8 a, Direction8 b) => a.direction == b.direction;
        public static bool operator !=(Direction8 a, Direction8 b) => !(a == b);
        public bool Equals(Direction8 other) => this == other;
        public override bool Equals(object obj) => obj is Direction8 other && Equals(other);

        public override int GetHashCode() => direction.GetHashCode();

        /// <summary>
        /// Returns the direction rotated 180 degrees.
        /// </summary>
        public static Direction8 operator -(Direction8 direction) => direction.direction switch
        {
            Direction.Up => Down,
            Direction.UpRight => DownLeft,
            Direction.Right => Left,
            Direction.DownRight => UpLeft,
            Direction.Down => Up,
            Direction.DownLeft => UpRight,
            Direction.Left => Right,
            Direction.UpLeft => DownRight,
            _ => throw new UnreachableException()
        };

        /// <summary>
        /// Returns the direction rotated by the given angle.
        /// </summary>
        public Direction8 Rotate(QuadrantalAngle angle) => angle switch
        {
            QuadrantalAngle._0 => this,
            QuadrantalAngle.Clockwise90 => direction switch
            {
                Direction.Up => Right,
                Direction.UpRight => DownRight,
                Direction.Right => Down,
                Direction.DownRight => DownLeft,
                Direction.Down => Left,
                Direction.DownLeft => UpLeft,
                Direction.Left => Up,
                Direction.UpLeft => UpRight,
                _ => throw new UnreachableException()
            },
            QuadrantalAngle._180 => -this,
            QuadrantalAngle.Anticlockwise90 => direction switch
            {
                Direction.Up => Left,
                Direction.UpRight => UpLeft,
                Direction.Right => Up,
                Direction.DownRight => UpRight,
                Direction.Down => Right,
                Direction.DownLeft => DownRight,
                Direction.Left => Down,
                Direction.UpLeft => DownLeft,
                _ => throw new UnreachableException()
            },
            _ => throw new UnreachableException(),
        };

        public override string ToString() => direction switch
        {
            Direction.Up => "Up",
            Direction.UpRight => "Up-Right",
            Direction.Right => "Right",
            Direction.DownRight => "Down-Right",
            Direction.Down => "Down",
            Direction.DownLeft => "Down-Left",
            Direction.Left => "Left",
            Direction.UpLeft => "Up-Left",
            _ => throw new UnreachableException()
        };
    }
}
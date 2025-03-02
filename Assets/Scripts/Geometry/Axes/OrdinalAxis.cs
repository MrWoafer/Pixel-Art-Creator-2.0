using System.Collections.Generic;

namespace PAC.Geometry.Axes
{
    /// <summary>
    /// <see cref="Diagonal45Axis"/> or <see cref="Minus45Axis"/>.
    /// </summary>
    public abstract record OrdinalAxis : CardinalOrdinalAxis
    {
        private protected OrdinalAxis() { } // don't allow any instances other than the pre-defined ones

        /// <summary>
        /// A 45-degree axis (going from southwest to northeast).
        /// </summary>
        /// <seealso cref="Minus45"/>
        public static readonly Diagonal45Axis Diagonal45 = new Diagonal45Axis();
        /// <summary>
        /// A -45-degree axis (going from northwest to southeast).
        /// </summary>
        /// <seealso cref="Diagonal45"/>
        public static readonly Minus45Axis Minus45 = new Minus45Axis();

        /// <summary>
        /// <see cref="Diagonal45"/> and <see cref="Minus45"/>.
        /// </summary>
        public static readonly new IEnumerable<OrdinalAxis> Axes = new OrdinalAxis[] { Diagonal45, Minus45 };
    }

    /// <summary>
    /// The type of <see cref="OrdinalAxis.Diagonal45"/>.
    /// </summary>
    public sealed record Diagonal45Axis : OrdinalAxis
    {
        internal Diagonal45Axis() { } // don't allow any instances other than the pre-defined one

        public override string ToString() => nameof(Diagonal45Axis);
    }

    /// <summary>
    /// The type of <see cref="OrdinalAxis.Minus45"/>.
    /// </summary>
    public sealed record Minus45Axis : OrdinalAxis
    {
        internal Minus45Axis() { } // don't allow any instances other than the pre-defined one

        public override string ToString() => nameof(Minus45Axis);
    }
}
using System.Collections.Generic;

namespace PAC.Geometry.Axes
{
    /// <summary>
    /// <see cref="HorizontalAxis"/> or <see cref="VerticalAxis"/>.
    /// </summary>
    public abstract record CardinalAxis : CardinalOrdinalAxis
    {
        private protected CardinalAxis() { } // don't allow any instances other than the pre-defined ones

        /// <summary>
        /// A horizontal axis.
        /// </summary>
        /// <seealso cref="Vertical"/>
        public static readonly HorizontalAxis Horizontal = new HorizontalAxis();
        /// <summary>
        /// A vertical axis.
        /// </summary>
        /// <seealso cref="Horizontal"/>
        public static readonly VerticalAxis Vertical = new VerticalAxis();

        /// <summary>
        /// <see cref="Horizontal"/> and <see cref="Vertical"/>.
        /// </summary>
        public static readonly new IEnumerable<CardinalAxis> Axes = new CardinalAxis[] { Horizontal, Vertical };
    }

    /// <summary>
    /// The type of <see cref="CardinalAxis.Horizontal"/>.
    /// </summary>
    public sealed record HorizontalAxis : CardinalAxis
    {
        internal HorizontalAxis() { } // don't allow any instances other than the pre-defined one

        public override string ToString() => nameof(HorizontalAxis);
    }

    /// <summary>
    /// The type of <see cref="CardinalAxis.Vertical"/>.
    /// </summary>
    public sealed record VerticalAxis : CardinalAxis
    {
        internal VerticalAxis() { } // don't allow any instances other than the pre-defined one

        public override string ToString() => nameof(VerticalAxis);
    }
}
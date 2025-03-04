using System.Collections.Generic;

namespace PAC.Geometry.Axes
{
    /// <summary>
    /// Provides axes and groups of axes.
    /// </summary>
    /// <remarks>
    /// Each axis has its own type, so that axes can be abstracted in a type-safe way. For example, shapes can specify exactly which axes they can reflected across, and this is enforced at
    /// compile time via types.
    /// </remarks>
    public static class Axes
    {
        /// <summary>
        /// A horizontal axis.
        /// </summary>
        /// <seealso cref="Vertical"/>
        public static readonly HorizontalAxis Horizontal;
        /// <summary>
        /// A vertical axis.
        /// </summary>
        /// <seealso cref="Horizontal"/>
        public static readonly VerticalAxis Vertical;

        /// <summary>
        /// <see cref="Horizontal"/> and <see cref="Vertical"/>.
        /// </summary>
        public static readonly IEnumerable<CardinalAxis> CardinalAxes;

        /// <summary>
        /// A 45-degree axis (going from southwest to northeast).
        /// </summary>
        /// <seealso cref="Minus45"/>
        public static readonly Diagonal45Axis Diagonal45;
        /// <summary>
        /// A -45-degree axis (going from northwest to southeast).
        /// </summary>
        /// <seealso cref="Diagonal45"/>
        public static readonly Minus45Axis Minus45;

        /// <summary>
        /// <see cref="Diagonal45"/> and <see cref="Minus45"/>.
        /// </summary>
        public static readonly IEnumerable<OrdinalAxis> OrdinalAxes;

        /// <summary>
        /// <see cref="Horizontal"/>, <see cref="Vertical"/>, <see cref="Diagonal45"/> and <see cref="Minus45"/>.
        /// </summary>
        public static readonly IEnumerable<CardinalOrdinalAxis> CardinalOrdinalAxes;

        static Axes()
        {
            Horizontal = new HorizontalAxis();
            Vertical = new VerticalAxis();
            CardinalAxes = new CardinalAxis[] { Horizontal, Vertical };

            Diagonal45 = new Diagonal45Axis();
            Minus45 = new Minus45Axis();
            OrdinalAxes = new OrdinalAxis[] { Diagonal45, Minus45 };

            CardinalOrdinalAxes = new CardinalOrdinalAxis[] { Horizontal, Vertical, Diagonal45, Minus45 };
        }
    }
}
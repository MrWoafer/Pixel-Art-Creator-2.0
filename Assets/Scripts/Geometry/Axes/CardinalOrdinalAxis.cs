using System.Collections.Generic;

namespace PAC.Geometry.Axes
{
    /// <summary>
    /// Either a <see cref="CardinalAxis"/> or <see cref="OrdinalAxis"/>.
    /// </summary>
    public abstract record CardinalOrdinalAxis
    {
        private protected CardinalOrdinalAxis() { } // don't allow any instances other than the pre-defined ones

        /// <summary>
        /// <see cref="CardinalAxis.Horizontal"/>, <see cref="CardinalAxis.Vertical"/>, <see cref="OrdinalAxis.Diagonal45"/> and <see cref="OrdinalAxis.Minus45"/>.
        /// </summary>
        public static readonly IEnumerable<CardinalOrdinalAxis> Axes = new CardinalOrdinalAxis[] { CardinalAxis.Horizontal, CardinalAxis.Vertical, OrdinalAxis.Diagonal45, OrdinalAxis.Minus45 };
    }
}
namespace PAC.Geometry.Axes
{
    /// <summary>
    /// <see cref="Diagonal45Axis"/> or <see cref="Minus45Axis"/>.
    /// </summary>
    public abstract record OrdinalAxis : CardinalOrdinalAxis
    {
        internal OrdinalAxis() { } // don't allow external types to inherit from this
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
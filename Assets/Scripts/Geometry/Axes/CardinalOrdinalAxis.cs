namespace PAC.Geometry.Axes
{
    /// <summary>
    /// Either a <see cref="CardinalAxis"/> or <see cref="OrdinalAxis"/>.
    /// </summary>
    public abstract record CardinalOrdinalAxis
    {
        internal CardinalOrdinalAxis() { } // don't allow external types to inherit from this
    }
}
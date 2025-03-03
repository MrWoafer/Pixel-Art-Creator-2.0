namespace PAC.Geometry.Axes
{
    /// <summary>
    /// Either a <see cref="CardinalAxis"/> or <see cref="OrdinalAxis"/>.
    /// </summary>
    public abstract record CardinalOrdinalAxis
    {
        private protected CardinalOrdinalAxis() { } // don't allow any instances other than the pre-defined ones
    }
}
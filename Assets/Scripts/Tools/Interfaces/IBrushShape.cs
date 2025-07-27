namespace PAC.Tools.Interfaces
{
    /// <summary>
    /// A <see cref="Tool"/> that has a configurable brush shape.
    /// </summary>
    public interface IBrushShape
    {
        public BrushShape brushShape { get; set; }
    }
}
namespace PAC.Tools.Interfaces
{
    /// <summary>
    /// A <see cref="Tool"/> that has a configurable brush shape.
    /// </summary>
    public interface IHasBrushShape
    {
        public BrushShape brushShape { get; set; }
    }
}
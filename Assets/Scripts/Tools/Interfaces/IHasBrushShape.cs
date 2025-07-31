namespace PAC.Tools.Interfaces
{
    /// <summary>
    /// A <see cref="Tool"/> that has a brush shape that can be read.
    /// </summary>
    /// <remarks>
    /// This is readonly in the sense of the <see langword="readonly"/> modifier: it cannot be assigned a new instance, but editing members of the instance is allowed.
    /// If you want the brush shape to be completely immutable, make sure the concrete <see cref="BrushShape"/> sub-type is immutable.
    /// </remarks>
    public interface IHasBrushShape
    {
        public BrushShape brushShape { get; }
    }

    /// <summary>
    /// A <see cref="Tool"/> that has a brush shape that can be set to a new instance.
    /// </summary>
    public interface IHasSettableBrushShape : IHasBrushShape
    {
        public new BrushShape brushShape { get; set; }
        BrushShape IHasBrushShape.brushShape => brushShape;
    }
}
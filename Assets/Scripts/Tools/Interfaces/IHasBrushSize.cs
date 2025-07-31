namespace PAC.Tools.Interfaces
{
    /// <summary>
    /// A <see cref="BrushShape"/> that has a configurable brush size.
    /// </summary>
    public interface IHasBrushSize
    {
        public int brushSize { get; set; }
    }
}
namespace PAC.Shapes.Interfaces
{
    /// <summary>
    /// An <see cref="IShape"/> that encloses a region that can be filled in.
    /// </summary>
    public interface IFillableShape : IShape
    {
        /// <summary>
        /// Whether the shape has its inside filled-in, or whether it's just the border.
        /// </summary>
        public bool filled { get; set; }
    }
}
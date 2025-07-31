using PAC.Tools.Interfaces;

namespace PAC.Tools
{
    /// <summary>
    /// The pixels, given relative to the position of the mouse, that will be affected by the current brush.
    /// </summary>
    public abstract class BrushShape
    {
        public class Circle : BrushShape, IHasBrushSize
        {
            public int brushSize { get; set; }

            /// <param name="brushSize">See <see cref="brushSize"/>.</param>
            public Circle(int brushSize)
            {
                this.brushSize = brushSize;
            }
        }

        public class Square : BrushShape, IHasBrushSize
        {
            public int brushSize { get; set; }

            /// <param name="brushSize">See <see cref="brushSize"/>.</param>
            public Square(int brushSize)
            {
                this.brushSize = brushSize;
            }
        }

        public class Diamond : BrushShape, IHasBrushSize
        {
            public int brushSize {  get; set; }

            /// <param name="brushSize">See <see cref="brushSize"/>.</param>
            public Diamond(int brushSize)
            {
                this.brushSize = brushSize;
            }
        }

        public class Custom : BrushShape
        {
            public Custom() { }
        }
    }
}
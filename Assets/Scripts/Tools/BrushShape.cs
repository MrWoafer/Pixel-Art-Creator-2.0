namespace PAC.Tools
{
    /// <summary>
    /// The pixels, given relative to the position of the mouse, that will be affected by the current brush.
    /// </summary>
    public abstract class BrushShape
    {
        public class Circle : BrushShape
        {
            public Circle() { }
        }

        public class Square : BrushShape
        {
            public Square() { }
        }

        public class Diamond : BrushShape
        {
            public Diamond() { }
        }

        public class Custom : BrushShape
        {
            public Custom() { }
        }
    }
}
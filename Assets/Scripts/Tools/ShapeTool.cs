using PAC.Input;
using PAC.Tools.Interfaces;

namespace PAC.Tools
{
    public class ShapeTool : Tool
    {
        public override string name => "shape";

        public override bool useMovementInterpolation => false;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => true;

        public enum Shape
        {
            Rectangle = 0,
            Ellipse = 1,
            RightTriangle = 2,
            Diamond = 3,
            IsometricHexagon = 4,
        }
        public Shape shape { get; set; }

        /// <param name="shape">See <see cref="shape"/>.</param>
        public ShapeTool(Shape shape)
        {
            this.shape = shape;
        }
    }
}
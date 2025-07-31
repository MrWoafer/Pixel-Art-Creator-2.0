using PAC.Input;
using PAC.Tools.Interfaces;

namespace PAC.Tools
{
    public class SelectionTool : Tool, IHasBrushShape
    {
        public override string name => "selection";

        public override bool useMovementInterpolation => false;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => false;

        public BrushShape brushShape { get; } = new BrushShape.Pixel();

        public enum Mode
        {
            Draw = 0,
            MagicWand = 1,
            Rectangle = 10,
            Ellipse = 11
        }
        public Mode mode { get; set; }

        /// <param name="mode">See <see cref="mode"/>.</param>
        public SelectionTool(Mode mode)
        {
            this.mode = mode;
        }
    }
}
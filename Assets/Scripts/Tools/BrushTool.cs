using PAC.Input;
using PAC.Tools.Interfaces;

namespace PAC.Tools
{
    public class BrushTool : Tool, IBrushSize, IBrushShape
    {
        public override string name => "brush";

        public override bool useMovementInterpolation => true;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => false;

        public int brushSize { get; set; }
        public BrushShape brushShape { get; set; }

        /// <param name="brushSize">See <see cref="brushSize"/>.</param>
        /// <param name="brushShape">See <see cref="brushShape"/>.</param>
        public BrushTool(int brushSize, BrushShape brushShape)
        {
            this.brushSize = brushSize;
            this.brushShape = brushShape;
        }
    }
}
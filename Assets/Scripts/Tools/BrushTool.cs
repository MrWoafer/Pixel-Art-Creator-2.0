using PAC.Input;
using PAC.Tools.Interfaces;

namespace PAC.Tools
{
    public class BrushTool : Tool, IBrushSize
    {
        public override string name => "brush";

        public override bool useMovementInterpolation => true;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => false;

        public int brushSize { get; set; }

        /// <param name="brushSize">See <see cref="brushSize"/>.</param>
        public BrushTool(int brushSize)
        {
            this.brushSize = brushSize;
        }
    }
}
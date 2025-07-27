using PAC.Input;
using PAC.Tools.Interfaces;

namespace PAC.Tools
{
    public class RubberTool : Tool, IBrushSize
    {
        public override string name => "rubber";

        public override bool useMovementInterpolation => true;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => false;

        public int brushSize { get; set; }

        /// <param name="brushSize">See <see cref="brushSize"/>.</param>
        public RubberTool(int brushSize)
        {
            this.brushSize = brushSize;
        }
    }
}
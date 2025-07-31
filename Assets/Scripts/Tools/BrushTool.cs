using PAC.Input;
using PAC.Tools.Interfaces;

namespace PAC.Tools
{
    public class BrushTool : Tool, IHasSettableBrushShape
    {
        public override string name => "brush";

        public override bool useMovementInterpolation => true;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => false;

        public BrushShape brushShape { get; set; }

        /// <param name="brushShape">See <see cref="brushShape"/>.</param>
        public BrushTool(BrushShape brushShape)
        {
            this.brushShape = brushShape;
        }
    }
}
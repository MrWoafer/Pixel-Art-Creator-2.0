using PAC.Input;
using PAC.Tools.Interfaces;

namespace PAC.Tools
{
    public class RubberTool : Tool
    {
        public override string name => "rubber";

        public override bool useMovementInterpolation => true;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => false;

        public RubberTool() { }
    }
}
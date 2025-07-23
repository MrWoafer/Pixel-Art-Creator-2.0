using PAC.Input;
using PAC.Tools.Interfaces;

namespace PAC.Tools
{
    public sealed class NoneTool : Tool
    {
        public override string name => "none";

        public override bool useMovementInterpolation => false;

        public override bool showBrushBorder => false;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => false;

        public NoneTool() { }
    }
}
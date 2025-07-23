using PAC.Input;

namespace PAC.Tools
{
    public class FillTool : Tool
    {
        public override string name => "fill";

        public override bool useMovementInterpolation => false;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => false;

        public FillTool() { }
    }
}
using PAC.Input;

namespace PAC.Tools
{
    public class BrushTool : Tool
    {
        public override string name => "brush";

        public override bool useMovementInterpolation => true;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => false;

        public BrushTool() { }
    }
}
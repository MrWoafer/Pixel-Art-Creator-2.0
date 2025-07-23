using PAC.Input;

namespace PAC.Tools
{
    public class GlobalEyeDropperTool : Tool
    {
        public override string name => "global eye dropper";

        public override bool useMovementInterpolation => false;

        public override bool showBrushBorder => false;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => false;

        public GlobalEyeDropperTool() { }
    }
}
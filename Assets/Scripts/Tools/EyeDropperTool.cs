using PAC.Input;

namespace PAC.Tools
{
    public class EyeDropperTool : Tool
    {
        public override string name => "eye dropper";

        public override bool useMovementInterpolation => false;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => false;

        public EyeDropperTool() { }
    }
}
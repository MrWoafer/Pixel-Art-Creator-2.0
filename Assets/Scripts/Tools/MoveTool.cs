using PAC.Input;

namespace PAC.Tools
{
    public class MoveTool : Tool
    {
        public override string name => "move";

        public override bool useMovementInterpolation => false;

        public override bool showBrushBorder => false;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => false;

        public MoveTool() { }
    }
}
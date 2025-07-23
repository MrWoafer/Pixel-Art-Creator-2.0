using PAC.Input;

namespace PAC.Tools
{
    public class PencilTool : Tool
    {
        public override string name => "pencil";

        public override bool useMovementInterpolation => true;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => false;

        public PencilTool() { }
    }
}
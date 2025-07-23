using PAC.Input;

namespace PAC.Tools
{
    public class ShapeTool : Tool
    {
        public override string name => "shape";

        public override bool useMovementInterpolation => false;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => true;

        public ShapeTool() { }
    }
}
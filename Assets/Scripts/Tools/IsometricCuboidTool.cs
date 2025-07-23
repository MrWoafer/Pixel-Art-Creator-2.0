using PAC.Input;

namespace PAC.Tools
{
    public class IsometricCuboidTool : Tool
    {
        public override string name => "isometric cuboid";

        public override bool useMovementInterpolation => false;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Manual;
        public override bool canBeCancelled => true;

        public IsometricCuboidTool() { }
    }
}
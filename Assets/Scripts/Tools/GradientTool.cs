using PAC.Input;
using PAC.Tools.Interfaces;

namespace PAC.Tools
{
    public class GradientTool : Tool
    {
        public override string name => "gradient";

        public override bool useMovementInterpolation => false;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => true;

        public enum Mode
        {
            Linear = 0,
            Radial = 1,
        }

        public GradientTool() { }
    }
}
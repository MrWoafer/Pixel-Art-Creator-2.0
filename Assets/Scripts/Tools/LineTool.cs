using PAC.Input;
using PAC.Tools.Interfaces;

namespace PAC.Tools
{
    public class LineTool : Tool
    {
        public override string name => "line";

        public override bool useMovementInterpolation => false;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => true;

        public LineTool() { }
    }
}
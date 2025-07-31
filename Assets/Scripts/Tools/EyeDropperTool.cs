using PAC.Input;
using PAC.Tools.Interfaces;

namespace PAC.Tools
{
    public class EyeDropperTool : Tool, IHasBrushShape
    {
        public override string name => "eye dropper";

        public override bool useMovementInterpolation => false;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => false;

        public BrushShape brushShape { get; } = new BrushShape.Pixel();

        public EyeDropperTool() { }
    }
}
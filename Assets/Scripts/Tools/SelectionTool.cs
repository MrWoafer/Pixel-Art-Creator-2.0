using PAC.Input;

namespace PAC.Tools
{
    public class SelectionTool : Tool
    {
        public override string name => "selection";

        public override bool useMovementInterpolation => false;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => false;

        public enum SelectionMode
        {
            Draw = 0,
            MagicWand = 1,
            Rectangle = 10,
            Ellipse = 11
        }

        public SelectionTool() { }
    }
}
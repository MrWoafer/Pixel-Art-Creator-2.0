using PAC.Input;
using PAC.Tools.Interfaces;

namespace PAC.Tools
{
    public class FillTool : Tool
    {
        public override string name => "fill";

        public override bool useMovementInterpolation => false;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => false;

        public bool floodFillDiagonallyAdjacent { get; set; }

        /// <param name="floodFillDiagonallyAdjacent">See <see cref="floodFillDiagonallyAdjacent"/>.</param>
        public FillTool(bool floodFillDiagonallyAdjacent)
        {
            this.floodFillDiagonallyAdjacent = floodFillDiagonallyAdjacent;
        }
    }
}
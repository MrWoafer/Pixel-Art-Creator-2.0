using System;

using PAC.Input;
using PAC.Tools.Interfaces;

namespace PAC.Tools
{
    public class PencilTool : Tool
    {
        public override string name => "pencil";

        public override bool useMovementInterpolation => true;

        public override bool showBrushBorder => true;

        public override MouseTargetDeselectMode finishMode => MouseTargetDeselectMode.Unclick;
        public override bool canBeCancelled => false;

        private float _lineSmoothingTime = 0.2f;
        /// <summary>
        /// The amount of time you have to draw a new pixel in for an old one to be potentially smoothed.
        /// </summary>
        public float lineSmoothingTime
        {
            get => _lineSmoothingTime;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"{lineSmoothingTime} cannot be negative.");
                }

                _lineSmoothingTime = value;
            }
        }

        public PencilTool() { }
    }
}
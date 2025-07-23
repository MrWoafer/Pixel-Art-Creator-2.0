using PAC.Input;

namespace PAC.Tools.Interfaces
{
    /// <summary>
    /// A tool that can be used on an image.
    /// </summary>
    public abstract class Tool
    {
        public abstract string name { get; }

        /// <summary>
        /// When the mouse position jumps between frames:
        /// <list type="bullet">
        /// <item>
        /// <see langword="true"/> - the tool should be applied to each coord the mouse moved through
        /// </item>
        /// <item>
        /// <see langword="false"/> - just applied to the ending coord
        /// </item>
        /// </list>
        /// </summary>
        public abstract bool useMovementInterpolation { get; }

        /// <summary>
        /// Whether the outline of the brush shape should be shown.
        /// </summary>
        public abstract bool showBrushBorder { get; }

        /// <summary>
        /// What action causes a use of the tool to be ended.
        /// </summary>
        public abstract MouseTargetDeselectMode finishMode { get; }
        public abstract bool canBeCancelled { get; }
    }
}

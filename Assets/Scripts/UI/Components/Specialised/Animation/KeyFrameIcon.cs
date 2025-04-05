using UnityEngine;

namespace PAC.Animation
{
    /// <summary>
    /// Stores the data about a keyframe icon on the animation timeline.
    /// </summary>
    public class KeyFrameIcon : MonoBehaviour
    {
        /// <summary>The frame number this keyframe is on.</summary>
        public int frameIndex;
        /// <summary>The layer index this keyframe is on.</summary>
        public int layerIndex;
    }
}

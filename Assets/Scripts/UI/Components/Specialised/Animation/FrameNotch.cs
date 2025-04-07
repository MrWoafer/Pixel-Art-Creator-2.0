using UnityEngine;
using UnityEngine.UI;

namespace PAC.UI.Components.Specialised.Animation
{
    /// <summary>
    /// A class for the frame number markers on the animation timeline.
    /// </summary>
    public class FrameNotch : MonoBehaviour
    {
        private Text numberText;

        public int frameNum
        {
            get
            {
                return int.Parse(numberText.text);
            }
            set
            {
                SetFrameNumber(value);
            }
        }

        private void Awake()
        {
            numberText = transform.Find("Text").GetComponent<Text>();
        }

        public void SetFrameNumber(int num)
        {
            numberText.text = num.ToString();
        }
    }
}

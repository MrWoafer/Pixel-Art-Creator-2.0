using PAC.Colour;
using UnityEngine;
using UnityEngine.Events;

namespace PAC.ColourPicker
{
    /// <summary>
    /// A class for the HSL colour picker.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The picker gets the <see cref="hue"/> as the x coord on the <see cref="HSLHueSaturationBox"/> (in the range <c>[0, 1]</c>), the <see cref="saturation"/> as the y coord on the
    /// <see cref="HSLHueSaturationBox"/> (in the range <c>[0, 1]</c>), and the <see cref="lightness"/> as the y coord on the <see cref="HSLLightnessSlider"/> (in the range <c>[0, 1]</c>).
    /// </para>
    /// <para>
    /// For the display, the shader treats the HSL as being in gamma space. Hence, for the picked colour to match the colour on the display, the picked colour should be treated as in gamma space.
    /// </para>
    /// </remarks>
    public class HSLColourPicker : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float _mouseSensitivity = 0.1f;
        public float mouseSensitivity => _mouseSensitivity;
        [SerializeField]
        private float _slowSensitivityScalar = 0.5f;
        /// <summary>The sensitivity of the mouse when holding the modifier keyboard shortcut to reduce the sensitivity.</summary>
        public float slowSensitivityScalar => _slowSensitivityScalar;

        [Header("Events")]
        [SerializeField]
        /// <summary>Event invoked when the selected colour changes.</summary>
        private UnityEvent onColourChanged = new UnityEvent();

        [Header("References")]
        private HSLHueSaturationBox hueSaturationBox;
        private HSLLightnessSlider lightnessSlider;

        public float hue => hueSaturationBox.hue;
        public float saturation => hueSaturationBox.saturation;
        public float lightness => lightnessSlider.lightness;
        public float alpha { get; private set; } = 1f;

        public Color color => (Color)hsl;
        public HSLA hsl => new HSLA(hue, saturation, lightness, alpha);

        void Awake()
        {
            hueSaturationBox = transform.Find("Hue Saturation Box").GetComponent<HSLHueSaturationBox>();
            lightnessSlider = transform.Find("Lightness Slider").GetComponent<HSLLightnessSlider>();
        }

        public void UpdateColour()
        {
            onColourChanged.Invoke();
            lightnessSlider.SetDisplayHueSaturation(hsl.h, hsl.s);
        }

        public void SetColour(Color colour)
        {
            HSLA newHSL = (HSLA)colour;
            hueSaturationBox.SetHue(newHSL.h);
            hueSaturationBox.SetSaturation(newHSL.s);
            lightnessSlider.SetLightness(newHSL.l);
            alpha = colour.a;

            UpdateColour();
        }

        /// <summary>
        /// Event invoked when the selected colour changes.
        /// </summary>
        public void SubscribeToOnColourChange(UnityAction call)
        {
            onColourChanged.AddListener(call);
        }
    }
}

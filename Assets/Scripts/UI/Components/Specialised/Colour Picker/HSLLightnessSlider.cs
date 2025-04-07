using PAC.Input;

using UnityEngine;

namespace PAC.UI.Components.Specialised.ColourPicker
{
    /// <summary>
    /// A class for the lightness slider of the HSL colour picker.
    /// </summary>
    public class HSLLightnessSlider : MonoBehaviour
    {
        private float _lightness = 0f;
        public float lightness
        {
            get => _lightness;
            private set
            {
                _lightness = value;
            }
        }

        [Header("References")]
        private HSLColourPicker hslColourPicker;
        private Transform cursor;
        private Renderer renderer;

        private InputTarget inputTarget;
        private Mouse mouse;

        private Vector2 previousMousePos;

        private void Awake()
        {
            hslColourPicker = transform.parent.GetComponent<HSLColourPicker>();
            cursor = transform.Find("Cursor");
            renderer = GetComponent<Renderer>();

            inputTarget = GetComponent<InputTarget>();
            mouse = Finder.mouse;
        }

        private void Start()
        {
            inputTarget.mouseTarget.SubscribeToStateChange(OnMouseInput);
        }

        private void Update()
        {
            if (inputTarget.mouseTarget.state == MouseTargetState.Pressed)
            {
                UpdateMousePosition();

                previousMousePos = mouse.worldPos;
            }
        }

        public void SetLightness(float lightness)
        {
            _lightness = lightness;
            UpdateCursor();
        }

        /// <summary>
        /// Sets the hue/saturation to use for displaying the lightness gradient.
        /// </summary>
        public void SetDisplayHueSaturation(float hue, float saturation)
        {
            renderer.material.SetFloat("_Hue", hue);
            renderer.material.SetFloat("_Saturation", saturation);
        }

        /// <summary>
        /// Called when there in mouse input.
        /// </summary>
        private void OnMouseInput()
        {
            if (inputTarget.mouseTarget.state == MouseTargetState.Pressed)
            {
                Vector3 mouseLocalCoords = transform.InverseTransformPoint(mouse.worldPos);

                mouseLocalCoords = new Vector3(0f, Mathf.Clamp(mouseLocalCoords.y, -0.5f, 0.5f), 0f);

                UpdateValuesFromMouse(mouseLocalCoords);

                previousMousePos = mouse.worldPos;
            }
        }

        /// <summary>
        /// Handles moving the mouse cursor and updating lightness based on how the mouse has moved since last frame.
        /// </summary>
        private void UpdateMousePosition()
        {
            float sensitivity = hslColourPicker.mouseSensitivity;
            if (inputTarget.keyboardTarget.IsHeldExactly(KeyCode.LeftControl))
            {
                sensitivity *= hslColourPicker.slowSensitivityScalar;
            }

            Vector3 mouseLocalCoords = cursor.localPosition + transform.InverseTransformVector(mouse.worldPos - previousMousePos) * sensitivity;

            mouseLocalCoords = new Vector3(0f, Mathf.Clamp(mouseLocalCoords.y, -0.5f, 0.5f), 0f);

            UpdateValuesFromMouse(mouseLocalCoords);
        }

        /// <summary>
        /// Sets the lightness based on the mouse position.
        /// </summary>
        private void UpdateValuesFromMouse(Vector2 mouseLocalCoords)
        {
            SetLightness(mouseLocalCoords.y + 0.5f);

            UpdateCursor();

            hslColourPicker.UpdateColour();
        }

        /// <summary>
        /// Move the cursor to the correct position.
        /// </summary>
        private void UpdateCursor()
        {
            cursor.localPosition = new Vector3(0f, lightness - 0.5f, 0f);
        }
    }
}

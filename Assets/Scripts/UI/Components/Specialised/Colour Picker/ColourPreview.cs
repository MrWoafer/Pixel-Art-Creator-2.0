using PAC.UI.Components.General;
using PAC.Shaders;

using UnityEngine;
using UnityEngine.Events;

namespace PAC.UI.Components.Specialised.ColourPicker
{
    /// <summary>
    /// A class to represent colour previews - the boxes on colour pickers that show what colours you have selected.
    /// </summary>
    public class ColourPreview : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private Color _colour = Color.red;
        /// <summary>The colour displayed in the colour preview.</summary>
        public Color colour
        {
            get
            {
                if (_colour.a == 0f)
                {
                    return Config.Colours.transparent;
                }
                return _colour;
            }
            private set
            {
                _colour = value;
            }
        }
        [SerializeField]
        [Min(0f)]
        private float _outlineThickness = 0.1f;
        public float outlineThickness
        {
            get => _outlineThickness;
            private set
            {
                _outlineThickness = value;
            }
        }
        [SerializeField]
        private bool useRainbowOutline = true;

        [Header("Events")]
        [SerializeField]
        /// <summary>Event invoked when colour preview is selected.</summary>
        private UnityEvent onSelect = new UnityEvent();
        [SerializeField]
        /// <summary>Event invoked when colour preview is deselected.</summary>
        private UnityEvent onDeselect = new UnityEvent();
        /// <summary>Event invoked when colour preview is selected/deselected.</summary>
        [SerializeField]
        private UnityEvent onToggle = new UnityEvent();

        private UIToggleButton toggle;
        private UnityEngine.UI.Image toggleBackground;
        private RainbowOutline rainbowOutline;
        private Transform outerOutline;
        private Transform innerOutline;

        private void Awake()
        {
            GetReferences();
        }

        private void Start()
        {
            toggle.SubscribeToLeftClick(OnInput);
            SetColour(colour);
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                UpdateDisplay();
            }
        }

        private void OnValidate()
        {
            GetReferences();
            UpdateDisplay();
        }

        private void GetReferences()
        {
            toggle = GetComponent<UIToggleButton>();
            toggleBackground = toggle.transform.Find("Canvas").Find("Background").GetComponent<UnityEngine.UI.Image>();
            rainbowOutline = transform.Find("Canvas").Find("Outline").Find("Rainbow").GetComponent<RainbowOutline>();
            outerOutline = transform.Find("Canvas").Find("Outline").Find("Outer Outline");
            innerOutline = transform.Find("Canvas").Find("Outline").Find("Inner Outline");
        }

        private void UpdateDisplay()
        {
            GetReferences();
            UpdateSize();
            toggleBackground.color = colour;
        }

        private void UpdateSize()
        {
            rainbowOutline.transform.localScale = new Vector3(1f + outlineThickness * 2f,
                (transform.localScale.y / transform.localScale.x + outlineThickness * 2f) * transform.localScale.x / transform.localScale.y, 1f);

            rainbowOutline.thickness = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y) / 2f;
            rainbowOutline.outlineEnabled = toggle.on && useRainbowOutline;

            outerOutline.GetComponent<RectTransform>().localScale = rainbowOutline.transform.localScale;

            innerOutline.GetComponent<RectTransform>().localScale = new Vector3(1f + outlineThickness,
                (transform.localScale.y / transform.localScale.x + outlineThickness) * transform.localScale.x / transform.localScale.y, 1f);
        }

        public void SetColour(Color colour)
        {
            this.colour = colour;
            toggle.offBackgroundColour = colour;
            toggle.onBackgroundColour = colour;
            toggle.pressedBackgroundColour = colour;
            toggleBackground.color = colour;
        }

        /// <summary>
        /// Called when the colour preview is clicked.
        /// </summary>
        private void OnInput()
        {
            rainbowOutline.outlineEnabled = toggle.on && useRainbowOutline;
            if (toggle.on)
            {
                onSelect.Invoke();
            }
            else
            {
                onDeselect.Invoke();
            }
            onToggle.Invoke();
        }

        /// <summary>
        /// Event invoked when colour preview is selected.
        /// </summary>
        public void SubscribeToOnSelect(UnityAction call)
        {
            onSelect.AddListener(call);
        }
        /// <summary>
        /// Event invoked when colour preview is deselected.
        /// </summary>
        public void SubscribeToOnDeselect(UnityAction call)
        {
            onDeselect.AddListener(call);
        }
        /// <summary>
        /// Event invoked when colour preview is selected or deselected.
        /// </summary>
        public void SubscribeToOnToggle(UnityAction call)
        {
            onToggle.AddListener(call);
        }
    }
}

using PAC.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PAC.UI
{
    [RequireComponent(typeof(InputTarget), typeof(Collider2D))]
    [AddComponentMenu("Custom UI/UI Scale")]
    public class UIScale : MonoBehaviour
    {
        [Header("Value")]
        [SerializeField]
        private float startingValue = 0f;
        [SerializeField]
        private float min = 0f;
        [SerializeField]
        private float max = 1f;
        [SerializeField]
        [Min(0f)]
        private float scaleSpeed = 1f;
        [SerializeField]
        [Min(0f)]
        private float slowScaleSpeed = 0.5f;

        [Header("Size")]
        [Min(0f)]
        public float width = 1f;
        [Min(0f)]
        public float height = 0.5f;

        [Header("Background")]
        public Color backgroundColour = Color.white;
        public Color backgroundHoverColour = Color.white;
        public Color backgroundPressedColour = Color.white;

        [Header("Text")]
        [SerializeField]
        [Min(0)]
        private int _decimalPlaces = 0;
        public int decimalPlaces
        {
            get
            {
                return _decimalPlaces;
            }
        }
        [SerializeField]
        private Color textColour = Color.black;
        [SerializeField]
        private Color textHoverColour = Color.black;
        [SerializeField]
        private Color textPressedColour = Color.black;

        [Header("Behaviour")]
        [SerializeField]
        private UnityEvent onClick = new UnityEvent();
        [SerializeField]
        private UnityEvent onValueChange = new UnityEvent();

        public float value { get; private set; }

        private InputTarget inputTarget;

        private Image background;
        private BoxCollider2D collider;
        private Text textBox;

        private void Awake()
        {
            inputTarget = GetComponent<InputTarget>();

            value = startingValue;

            GetReferences();
        }

        // Start is called before the first frame update
        void Start()
        {
            inputTarget.mouseTarget.SubscribeToStateChange(MouseTargetInput);

            UpdateDisplay();
            Idle();
        }

        private void Update()
        {
            if (inputTarget.mouseTarget.state == MouseTargetState.Pressed)
            {
                float speed = scaleSpeed;
                if (inputTarget.keyboardTarget.IsHeldExactly(KeyCode.LeftControl) || inputTarget.keyboardTarget.IsHeldExactly(KeyCode.RightControl))
                {
                    speed = slowScaleSpeed;
                }

                SetValue(value + UnityEngine.Input.GetAxis("Mouse Y") * speed);
            }
        }

        private void GetReferences()
        {
            background = transform.Find("Canvas").Find("Background").GetComponent<Image>();
            collider = GetComponent<BoxCollider2D>();
            textBox = transform.Find("Canvas").Find("Text").GetComponent<Text>();
        }

        private void OnValidate()
        {
            GetReferences();

            max = Mathf.Max(max, min);
            startingValue = Mathf.Clamp(startingValue, min, max);
            if (!Application.isPlaying)
            {
                value = startingValue;
            }

            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            UpdateSize();
            UpdateText();
            Idle();
        }

        private void UpdateText()
        {
            textBox.text = value.ToString("n" + decimalPlaces);
        }

        private void UpdateSize()
        {
            background.transform.localScale = new Vector3(width, height, 1f);
            collider.size = new Vector2(width, height);
        }

        private void Idle()
        {
            background.color = backgroundColour;
            textBox.color = textColour;
        }

        private void Hover()
        {
            background.color = backgroundHoverColour;
            textBox.color = textHoverColour;
        }

        private void Press()
        {
            background.color = backgroundPressedColour;
            textBox.color = textPressedColour;

            onClick.Invoke();
        }

        private void Select()
        {
            background.color = backgroundPressedColour;
            textBox.color = textPressedColour;
        }

        private void MouseTargetInput()
        {
            if (inputTarget.mouseTarget.selected)
            {
                Select();
            }
            else if (inputTarget.mouseTarget.state == MouseTargetState.Idle)
            {
                Idle();
            }
            else if (inputTarget.mouseTarget.state == MouseTargetState.Hover)
            {
                Hover();
            }
            else if (inputTarget.mouseTarget.state == MouseTargetState.Pressed)
            {
                Press();
            }
        }

        public bool SetValueNoNotify(float value)
        {
            value = Mathf.Clamp(value, min, max);

            bool valueChanged = (this.value != value);
            this.value = value;
            UpdateText();

            return valueChanged;
        }
        public bool SetValue(float value)
        {
            bool valueChanged = SetValueNoNotify(value);

            if (valueChanged)
            {
                onValueChange.Invoke();
            }

            return valueChanged;
        }

        public void SubscribeToValueChange(UnityAction call)
        {
            onValueChange.AddListener(call);
        }
    }
}

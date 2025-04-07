using PAC.Input;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PAC.UI.Components.General
{
    [RequireComponent(typeof(InputTarget), typeof(Collider2D))]
    [AddComponentMenu("Custom UI/UI Toggle Button")]
    public class UIToggleButton : MonoBehaviour
    {
        [Header("Toggle")]
        [SerializeField]
        private bool _on = false;
        public bool on
        {
            get
            {
                return _on;
            }
        }

        [Header("Toggle Group")]
        [SerializeField]
        private string _toggleName = "";
        public string toggleName
        {
            get
            {
                return _toggleName;
            }
            set
            {
                _toggleName = value;
            }
        }
        public UIToggleGroup toggleGroup { get; private set; } = null;

        public bool inToggleGroup
        {
            get
            {
                return toggleGroup != null;
            }
        }

        [Header("Size")]
        [Min(0f)]
        public float width = 1f;
        [Min(0f)]
        public float height = 1f;

        [Header("Image")]
        [SerializeField]
        private Sprite _offImage;
        public Sprite offImage
        {
            get
            {
                return _offImage;
            }
            private set
            {
                _offImage = value;
            }
        }
        [SerializeField]
        private Sprite _onImage;
        public Sprite onImage
        {
            get
            {
                return _onImage;
            }
            private set
            {
                _onImage = value;
            }
        }
        [SerializeField]
        private Sprite _hoverImage;
        public Sprite hoverImage
        {
            get
            {
                return _hoverImage;
            }
            private set
            {
                _hoverImage = value;
            }
        }
        [SerializeField]
        [Tooltip("When enabled, hovering over the toggle when off will show the on image and vice versa.")]
        private bool hoverUsesOppositeImage = false;
        [SerializeField]
        private Sprite _pressedImage;
        public Sprite pressedImage
        {
            get
            {
                return _pressedImage;
            }
            private set
            {
                _pressedImage = value;
            }
        }
        [SerializeField]
        [Tooltip("When enabled, the image will change when you release the click instead of when you first click. Only comes into effect when there's no hover and pressed image.")]
        private bool changeImageOnRelease = true;

        [Header("Background")]
        [SerializeField]
        private Color _offBackgroundColour = Color.white;
        public Color offBackgroundColour
        {
            get
            {
                return _offBackgroundColour;
            }
            set
            {
                _offBackgroundColour = value;
                Idle();
            }
        }
        [SerializeField]
        private Color _onBackgroundColour = Color.white;
        public Color onBackgroundColour
        {
            get
            {
                return _onBackgroundColour;
            }
            set
            {
                _onBackgroundColour = value;
                Idle();
            }
        }
        public Color hoverBackgroundTint = Color.white;
        [SerializeField]
        private Color _pressedBackgroundColour = Color.white;
        public Color pressedBackgroundColour
        {
            get
            {
                return _pressedBackgroundColour;
            }
            set
            {
                _pressedBackgroundColour = value;
                Idle();
            }
        }

        [Header("Shadow")]
        [SerializeField]
        private bool showShadow = false;

        [Header("Text")]
        [SerializeField]
        private string text = "";
        [SerializeField]
        private TextAnchor textAlignment = TextAnchor.MiddleCenter;
        [SerializeField]
        private float textPadding = 0.1f;
        [SerializeField]
        private Color onTextColour = Color.black;
        [SerializeField]
        private Color offTextColour = Color.black;
        [SerializeField]
        private Color textHoverColour = Color.black;
        [SerializeField]
        private Color textPressedColour = Color.black;

        [Header("Behaviour")]
        [SerializeField]
        private UnityEvent onTurnOn = new UnityEvent();
        [SerializeField]
        private UnityEvent onTurnOff = new UnityEvent();
        [SerializeField]
        private UnityEvent onHover = new UnityEvent();
        [SerializeField]
        private UnityEvent onLeftClick = new UnityEvent();
        [SerializeField]
        private UnityEvent onRightClick = new UnityEvent();

        private InputTarget inputTarget;

        private Image shadow;
        private Image background;
        private Image imageSpr;
        private BoxCollider2D collider;
        private Text textBox;

        private void Awake()
        {
            inputTarget = GetComponent<InputTarget>();

            GetReferences();
        }

        // Start is called before the first frame update
        void Start()
        {
            inputTarget.mouseTarget.SubscribeToStateChange(MouseTargetInput);

            UpdateDisplay();
        }

        private void GetReferences()
        {
            try
            {
                shadow = transform.Find("Canvas").Find("Shadow").GetComponent<Image>();
            }
            catch { }
        
            background = transform.Find("Canvas").Find("Background").GetComponent<Image>();
            imageSpr = transform.Find("Canvas").Find("Image").GetComponent<Image>();
            collider = GetComponent<BoxCollider2D>();
            textBox = transform.Find("Canvas").Find("Text").GetComponent<Text>();
        }

        private void OnValidate()
        {
            GetReferences();
            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            UpdateSize();

            if (shadow)
            {
                shadow.enabled = showShadow;
            }
        
            textBox.text = text;
            textBox.alignment = textAlignment;
            Idle();
        }

        private void UpdateSize()
        {
            background.transform.localScale = new Vector3(width, height, 1f);

            if (shadow)
            {
                shadow.transform.localScale = background.transform.localScale;
            }
        
            collider.size = new Vector2(width, height);

            textBox.GetComponent<RectTransform>().sizeDelta = new Vector2((width - textPadding * 2f) * 100f, height * 100f);
        }

        private void Idle()
        {
            SetImage(on ? onImage : (offImage ? offImage : onImage));
            background.color = on ? onBackgroundColour : offBackgroundColour;
            textBox.color = on ? onTextColour : offTextColour;
        }

        private void Hover()
        {
            if (hoverUsesOppositeImage)
            {
                SetImage(!on ? onImage : (offImage ? offImage : onImage));
            }
            else
            {
                SetImage(hoverImage ? hoverImage : (on ? onImage : (offImage ? offImage : onImage)));
            }
            background.color = hoverBackgroundTint * (on ? onBackgroundColour : offBackgroundColour);
            textBox.color = textHoverColour;

            onHover.Invoke();
        }

        public void Press()
        {
            _on = !_on;
            if (pressedImage == null)
            {
                if (hoverImage == null)
                {
                    if (hoverUsesOppositeImage)
                    {
                        SetImage((changeImageOnRelease ? on : !on) ? onImage : (offImage ? offImage : onImage));
                    }
                    else
                    {
                        SetImage((changeImageOnRelease ? !on : on) ? onImage : (offImage ? offImage : onImage));
                    }
                }
                else
                {
                    SetImage(hoverImage);
                }
            }
            else
            {
                SetImage(pressedImage);
            }
            background.color = pressedBackgroundColour;
            textBox.color = textPressedColour;

            if (on)
            {
                onTurnOn.Invoke();
            }
            else
            {
                onTurnOff.Invoke();
            }

            onLeftClick.Invoke();
        }

        public void RightClick()
        {
            onRightClick.Invoke();
        }

        private void MouseTargetInput()
        {
            if (inputTarget.mouseTarget.state == MouseTargetState.Idle)
            {
                Idle();
            }
            else if (inputTarget.mouseTarget.state == MouseTargetState.Hover)
            {
                Hover();
            }
            else if (inputTarget.mouseTarget.state == MouseTargetState.Pressed)
            {
                if (inputTarget.mouseTarget.buttonTargetedWith == MouseButton.Left)
                {
                    if (toggleGroup)
                    {
                        if (inputTarget.keyboardTarget.IsHeldExactly(KeyCode.LeftControl) || inputTarget.keyboardTarget.IsHeldExactly(KeyCode.RightControl))
                        {
                            toggleGroup.CtrlPress(this);
                        }
                        else
                        {
                            toggleGroup.Press(this);
                        }
                    }
                    else
                    {
                        Press();
                    }
                }
                else if (inputTarget.mouseTarget.buttonTargetedWith == MouseButton.Right)
                {
                    RightClick();
                }
            }
        }

        private void SetImage(Sprite newImage)
        {
            imageSpr.sprite = newImage;
            if (newImage == null)
            {
                imageSpr.color = new Color(imageSpr.color.r, imageSpr.color.g, imageSpr.color.b, 0f);
            }
            else
            {
                imageSpr.color = new Color(imageSpr.color.r, imageSpr.color.g, imageSpr.color.b, 255f);
            }
        }

        public void SetImages(Sprite offImage, Sprite onImage, Sprite hoverImage, Sprite pressedImage)
        {
            this.offImage = offImage;
            this.onImage = onImage;
            this.hoverImage = hoverImage;
            this.pressedImage = pressedImage;

            if (inputTarget.mouseTarget.state == MouseTargetState.Idle)
            {
                SetImage(on ? onImage : offImage);
            }
            else if (inputTarget.mouseTarget.state == MouseTargetState.Hover)
            {
                SetImage(hoverImage);
            }
            else if (inputTarget.mouseTarget.state == MouseTargetState.Pressed)
            {
                SetImage(pressedImage);
            }
        }

        public void SetOnOff(bool on)
        {
            if (_on != on)
            {
                Press();
            }
            Idle();
        }

        public void SetText(string text)
        {
            this.text = text;

            UpdateDisplay();
        }

        public void SetTextAlignment(TextAnchor textAlignment)
        {
            this.textAlignment = textAlignment;

            UpdateDisplay();
        }

        public void JoinToggleGroup(UIToggleGroup toggleGroup)
        {
            this.toggleGroup = toggleGroup;
        }
        public void LeaveToggleGroup()
        {
            toggleGroup = null;
        }

        public void SubscribeToTurnOn(UnityAction call)
        {
            onTurnOn.AddListener(call);
        }
        public void SubscribeToTurnOff(UnityAction call)
        {
            onTurnOff.AddListener(call);
        }
        public void SubscribeToHover(UnityAction call)
        {
            onHover.AddListener(call);
        }
        public void SubscribeToLeftClick(UnityAction call)
        {
            onLeftClick.AddListener(call);
        }
        public void SubscribeToRightClick(UnityAction call)
        {
            onRightClick.AddListener(call);
        }
    }
}

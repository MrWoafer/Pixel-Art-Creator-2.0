using PAC.Input;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PAC.UI.Components.General
{
    [RequireComponent(typeof(InputTarget), typeof(Collider2D))]
    [AddComponentMenu("Custom UI/UI Button")]
    public class UIButton : MonoBehaviour
    {
        [Header("Size")]
        [SerializeField]
        [Min(0f)]
        private float _width = 1f;
        public float width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                if (!Application.isPlaying) { UpdateSize(); }
            }
        }
        [SerializeField]
        [Min(0f)]
        private float _height = 1f;
        public float height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                if (!Application.isPlaying) { UpdateSize(); }
            }
        }

        [Header("Image")]
        [SerializeField]
        private Sprite _image;
        public Sprite image
        {
            get
            {
                return _image;
            }
            private set
            {
                _image = value;
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
        private Color imageColour = Color.white;

        [Header("Background")]
        public Color backgroundColour = Color.white;
        public Color backgroundHoverColour = Color.white;
        public Color backgroundPressedColour = Color.white;

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
        private Color textColour = Color.black;
        [SerializeField]
        private Color textHoverColour = Color.black;
        [SerializeField]
        private Color textPressedColour = Color.black;

        [Header("Behaviour")]
        [Space()]
        [SerializeField]
        private UnityEvent onIdle = new UnityEvent();
        [SerializeField]
        private UnityEvent onHover = new UnityEvent();
        [SerializeField]
        private UnityEvent onClick = new UnityEvent();
        [SerializeField]
        private UnityEvent onLeftClick = new UnityEvent();
        [SerializeField]
        private UnityEvent onRightClick = new UnityEvent();

        public bool isPressed
        {
            get
            {
                return inputTarget.mouseTarget.state == MouseTargetState.Pressed;
            }
        }

        private InputTarget inputTarget;

        private Image shadow;
        private Image background;
        private Image imageSpr;
        private BoxCollider2D collider;
        private Text textBox;
        private Canvas canvas;

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
            Idle();
        }

        private void GetReferences()
        {
            canvas = transform.Find("Canvas").GetComponent<Canvas>();
            shadow = transform.Find("Canvas").Find("Shadow").GetComponent<Image>();
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
            SetImage(image);
            shadow.enabled = showShadow;
            textBox.text = text;
            textBox.alignment = textAlignment;
            Idle();
        }

        private void UpdateSize()
        {
            background.transform.localScale = new Vector3(width, height, 1f);
            shadow.transform.localScale = background.transform.localScale;
            collider.size = new Vector2(width, height);
            textBox.GetComponent<RectTransform>().sizeDelta = new Vector2((width - textPadding * 2f) * 100f, height * 100f);
        }

        private void Idle()
        {
            SetImage(image);
            background.color = backgroundColour;
            textBox.color = textColour;

            onIdle.Invoke();
        }

        private void Hover()
        {
            if (hoverImage == null)
            {
                SetImage(image);
            }
            else
            {
                SetImage(hoverImage);
            }
            background.color = backgroundHoverColour;
            textBox.color = textHoverColour;

            onHover.Invoke();
        }

        public void Press()
        {
            if (pressedImage == null)
            {
                SetImage(image);
            }
            background.color = backgroundPressedColour;
            textBox.color = textPressedColour;

            onClick.Invoke();
            onLeftClick.Invoke();
        }

        public void RightClick()
        {
            onRightClick.Invoke();
        }

        private void Select()
        {
            if (pressedImage == null)
            {
                SetImage(image);
            }
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
                if (inputTarget.mouseTarget.buttonTargetedWith == MouseButton.Left)
                {
                    Press();
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
                imageSpr.color = new Color(imageColour.r, imageColour.g, imageColour.b, 0f);
            }
            else
            {
                imageSpr.color = new Color(imageColour.r, imageColour.g, imageColour.b, 255f);
            }
        }

        public void SetImages(Sprite image, Sprite hoverImage, Sprite pressedImage)
        {
            this.image = image;
            this.hoverImage = hoverImage;
            this.pressedImage = pressedImage;

            if (inputTarget.mouseTarget.state == MouseTargetState.Idle)
            {
                SetImage(image);
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

        public void SetText(string text)
        {
            this.text = text;

            UpdateDisplay();
        }

        public void SetSortingLayer(string sortingLayer, int sortingOrder)
        {
            SetSortingLayer(sortingLayer);
            SetSortingOrder(sortingOrder);
        }
        public void SetSortingLayer(string sortingLayer)
        {
            canvas.sortingLayerName = sortingLayer;
        }
        public void SetSortingOrder(int sortingOrder)
        {
            canvas.sortingOrder = sortingOrder;
        }

        public void SubscribeToIdle(UnityAction call)
        {
            onIdle.AddListener(call);
        }
        public void SubscribeToHover(UnityAction call)
        {
            onHover.AddListener(call);
        }
        public void SubscribeToClick(UnityAction call)
        {
            onClick.AddListener(call);
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

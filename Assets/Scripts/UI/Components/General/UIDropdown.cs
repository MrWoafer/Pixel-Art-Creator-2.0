using PAC.Input;

using UnityEngine;

namespace PAC.UI.Components.General
{
    public enum DropdownCloseMode
    {
        ClickOff = 0,
        MouseOff = 1
    }

    [AddComponentMenu("Custom UI/UI Dropdown")]
    public class UIDropdown : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private bool _open = false;
        public bool open
        {
            get
            {
                return _open;
            }
            private set
            {
                _open = value;
            }
        }
        [SerializeField]
        private DropdownCloseMode deselectMode = DropdownCloseMode.ClickOff;
        [SerializeField]
        [Tooltip("The button (if any) that causes the dropdown to open.")]
        public GameObject openingButton;

        [HideInInspector]
        public UIDropdown parentDropdown;
        [HideInInspector]
        public UIDropdown rootDropdown
        {
            get
            {
                return isRootDropdown ? this : parentDropdown.rootDropdown;
            }
        }
        public bool isRootDropdown
        {
            get
            {
                return parentDropdown == null;
            }
        }

        private bool beenOpenForAFrame = false;
        private bool initialisedAlready = false;

        private Mouse mouse;
        private UIManager uiManager;
        private UIElement uiElement;

        private void Awake()
        {
            mouse = Finder.mouse;
            uiManager = Finder.uiManager;

            uiElement = GetComponent<UIElement>();

            foreach (Transform child in transform)
            {
                UIDropdown dropdown = child.GetComponent<UIDropdown>();
                if (dropdown)
                {
                    dropdown.parentDropdown = this;
                }
            }
        }

        private void Start()
        {
            if (!initialisedAlready)
            {
                Initialise();
            }
        }

        public void Initialise()
        {
            InitialiseSetActive();

            if (openingButton && isRootDropdown)
            {
                InputTarget inputTarget = openingButton.GetComponent<InputTarget>();
                if (inputTarget)
                {
                    inputTarget.uiElement = rootDropdown.uiElement;
                }
            }

            foreach (Transform child in transform)
            {
                try
                {
                    UIButton button = child.GetComponent<UIButton>();
                    if (button)
                    {
                        button.SubscribeToClick(CloseRoot);
                    }
                }
                catch { }

                try
                {
                    UIToggleButton button = child.GetComponent<UIToggleButton>();
                    if (button)
                    {
                        button.SubscribeToLeftClick(CloseRoot);
                    }
                }
                catch { }

                if (rootDropdown.uiElement)
                {
                    InputTarget inputTarget = child.GetComponent<InputTarget>();
                    if (inputTarget)
                    {
                        inputTarget.uiElement = rootDropdown.uiElement;
                    }
                }
            }

            Close();
            initialisedAlready = true;
        }

        private void InitialiseSetActive()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);

                UIDropdown dropdown = child.GetComponent<UIDropdown>();
                if (dropdown)
                {
                    dropdown.InitialiseSetActive();
                }
            }
        }

        private void Update()
        {
            if (open && beenOpenForAFrame)
            {
                if (MouseOff() && (deselectMode == DropdownCloseMode.MouseOff || (deselectMode == DropdownCloseMode.ClickOff && mouse.click)))
                {
                    Close();
                }
            }

            if (open && !beenOpenForAFrame)
            {
                beenOpenForAFrame = true;
            }
        }

        private void OnValidate()
        {
            SetOpenEditor(open);
        }

        public void Open()
        {
            SetOpen(true);
        }
        public void Close()
        {
            SetOpen(false);
        }
        public void ToggleOpen()
        {
            SetOpen(!open);
        }

        public void SetOpen(bool open)
        {
            this.open = open;

            /// This > -5000 check is to avoid issues with double initialisation (which happens with UIDropdownChoice objects) causing dropdowns to be moved twice.
            if (!open && transform.localPosition.x > -5000f)
            {
                transform.localPosition -= new Vector3(10000f, 0f, 0f);
            }
            else if (open && transform.localPosition.x < -5000f)
            {
                transform.localPosition += new Vector3(10000f, 0f, 0f);
            }


            foreach (Transform child in transform)
            {
                UIDropdown dropdown = child.GetComponent<UIDropdown>();
                if (dropdown)
                {
                    if (!open)
                    {
                        dropdown.Close();
                    }

                    if (!open && transform.localPosition.x > -5000f)
                    {
                        child.localPosition -= new Vector3(10000f, 0f, 0f);
                    }
                    else if (open && transform.localPosition.x < -5000f)
                    {
                        child.localPosition += new Vector3(10000f, 0f, 0f);
                    }
                }
            }

            beenOpenForAFrame = false;

            if (uiElement)
            {
                if (open)
                {
                    uiManager.TryTarget(uiElement);
                }
                else
                {
                    uiManager.TryUntarget(uiElement);
                }
            }
        }

        public void SetOpenEditor(bool open)
        {
            this.open = open;

            foreach (Transform child in transform)
            {
                UIDropdown dropdown = child.GetComponent<UIDropdown>();
                if (dropdown)
                {
                    if (!open)
                    {
                        dropdown.SetOpenEditor(false);
                    }
                }

                child.gameObject.SetActive(open);
            }

            beenOpenForAFrame = false;

            if (uiElement)
            {
                if (open)
                {
                    uiManager.TryTarget(uiElement);
                }
                else
                {
                    uiManager.TryUntarget(uiElement);
                }
            }
        }

        /// <summary>
        /// Opens this dropdown and all child dropdowns, and all their child dropdowns, etc.
        /// </summary>
        public void FullyOpen()
        {
            Open();

            foreach (Transform child in transform)
            {
                UIDropdown dropdown = child.GetComponent<UIDropdown>();
                if (dropdown)
                {
                    dropdown.FullyOpen();
                }
            }

            beenOpenForAFrame = false;
        }
        /// <summary>
        /// Closes the highest-level dropdown containing this one.
        /// </summary>
        public void CloseRoot()
        {
            rootDropdown.Close();
        }

        public bool MouseOff()
        {
            if (openingButton != null)
            {
                InputTarget inputTarget = openingButton.GetComponent<InputTarget>();
                if (inputTarget && inputTarget.mouseTarget.state != MouseTargetState.Idle)
                {
                    return false;
                }
            }

            foreach (Transform child in transform)
            {
                InputTarget inputTarget = child.GetComponent<InputTarget>();
                if (inputTarget && inputTarget.mouseTarget.state != MouseTargetState.Idle)
                {
                    return false;
                }

                UIDropdown dropdown = child.GetComponent<UIDropdown>();
                if (dropdown && dropdown.open && !dropdown.MouseOff())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
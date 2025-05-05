using PAC.Managers;
using PAC.UI.Components;
using PAC.UI.Components.General;

using UnityEngine;
using UnityEngine.Events;

namespace PAC.Input
{
    public class InputTarget : MonoBehaviour
    {
        [Header("UI Element")]
        [Tooltip("Which UI Element this target is part of. Leave as null signifies this is its own UI Element.")]
        public UIElement uiElement;

        [Header("Settings")]
        [SerializeField]
        private string _targetName = "";
        public string targetName
        {
            get
            {
                return _targetName;
            }
        }
        [SerializeField]
        private int _targetTag = 0;
        public int targetTag
        {
            get
            {
                return _targetTag;
            }
        }

        [Header("Mouse Input")]
        public bool mouseInputEnabled = false;
        public bool isHoverTrigger = false;
        public bool allowLeftClick = true;
        public bool allowRightClick = false;
        public bool allowMiddleClick = false;
        public bool allowScroll = false;
        public CursorState cursorHover = CursorState.Hover;
        public CursorState cursorPress = CursorState.Press;
        public CursorState cursorSelected = CursorState.Unspecified;
        [SerializeField]
        private CursorState _cursorScroll = CursorState.Unspecified;
        public CursorState cursorScroll
        {
            get => _cursorScroll;
            set
            {
                _cursorScroll = value;
                UpdateSettings();
            }
        }
        public MouseTargetDeselectMode mouseDeselectMode = MouseTargetDeselectMode.Unclick;
        public bool disableMouseWhenSelected = false;

        public MouseTarget mouseTarget { get; private set; }

        [Header("Keyboard Input")]
        public bool keyboardInputEnabled = false;
        [Tooltip("Whether the keyboard target should receive knowledge of keys that are already being held when the keyboard target is targeted.")]
        public bool receiveAlreadyHeldKeys = false;
        [Tooltip("Whether when holding a key it should keep counting as new presses.")]
        public bool allowHoldingKeySpam = false;

        public KeyboardTarget keyboardTarget { get; private set; }

        [Header("Viewport")]
        public UIViewport viewport;

        private UnityEvent onTarget = new UnityEvent();
        private UnityEvent onUntarget = new UnityEvent();

        private InputSystem inputSystem;

        public Collider2D collider { get; private set; }

        void Awake()
        {
            inputSystem = Finder.inputSystem;

            collider = GetComponent<Collider2D>();

            mouseTarget = new MouseTarget();
            keyboardTarget = new KeyboardTarget();

            UpdateSettings();
        }

        private void Update()
        {
            keyboardTarget.ManualUpdate();
        }

        private void UpdateSettings()
        {
            mouseTarget.isHoverTrigger = isHoverTrigger;
            mouseTarget.allowLeftClick = allowLeftClick;
            mouseTarget.allowRightClick = allowRightClick;
            mouseTarget.allowMiddleClick = allowMiddleClick;
            mouseTarget.allowScroll = allowScroll;

            mouseTarget.cursorHover = cursorHover;
            mouseTarget.cursorPress = cursorPress;
            mouseTarget.cursorSelected = cursorSelected;
            mouseTarget.cursorScroll = cursorScroll;

            mouseTarget.deselectMode = mouseDeselectMode;

            keyboardTarget.receiveAlreadyHeldKeys = receiveAlreadyHeldKeys;
            keyboardTarget.allowHoldingKeySpam = allowHoldingKeySpam;
        }

        public void Target()
        {
            onTarget.Invoke();
        }

        public void Untarget()
        {
            mouseTarget.Untarget();
            keyboardTarget.Untarget();
            onUntarget.Invoke();
        }

        public void GetUntargeted()
        {
            if (inputSystem.inputTarget == this)
            {
                inputSystem.Untarget();
            }
        }

        public void SubscribeToTarget(UnityAction call)
        {
            onTarget.AddListener(call);
        }
        public void SubscribeToUntarget(UnityAction call)
        {
            onUntarget.AddListener(call);
        }
    }
}

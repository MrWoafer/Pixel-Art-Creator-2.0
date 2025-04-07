using PAC.Input;
using PAC.KeyboardShortcuts;
using PAC.UI.Components.General;

using UnityEngine;
using UnityEngine.Events;

namespace PAC.UI.Components.Specialised
{
    public class UIKeyboardShortcut : MonoBehaviour
    {
        [Header("Settings")]
        public string actionName = "";

        private KeyboardShortcut _shortcut = KeyboardShortcut.None;
        public KeyboardShortcut shortcut
        {
            get => _shortcut;
            set
            {
                _shortcut = value;
                button.SetText(shortcut.ToString());
                onShortcutSet.Invoke(_shortcut);
            }
        }
        private KeyboardShortcut newShortcut = KeyboardShortcut.None;

        private UIButton button;

        private InputSystem inputSystem;

        [Header("Events")]
        [Space]
        [SerializeField]
        private UnityEvent<KeyboardShortcut> onShortcutSet = new UnityEvent<KeyboardShortcut>();

        private void Awake()
        {
            button = transform.Find("Button").GetComponent<UIButton>();

            inputSystem = Finder.inputSystem;
        }

        private void Start()
        {
            button.SubscribeToLeftClick(() => newShortcut = KeyboardShortcut.None);
            button.GetComponent<InputTarget>().keyboardTarget.SubscribeToOnKeyDown(OnKeyDown);
            button.GetComponent<InputTarget>().keyboardTarget.SubscribeToOnKeyUp(OnKeyUp);
            button.GetComponent<InputTarget>().keyboardTarget.SubscribeToUntarget(OnUntarget);
        }

        private void OnKeyDown(CustomKeyCode keyCode)
        {
            newShortcut.Add(keyCode);
            button.SetText(newShortcut.ToString());
        }

        private void OnKeyUp(CustomKeyCode keyCode)
        {
            inputSystem.Untarget();
            shortcut = newShortcut;
        }

        private void OnUntarget()
        {
            button.SetText(shortcut.ToString());
        }

        public void SubscribeToOnShortcutSet(UnityAction<KeyboardShortcut> call)
        {
            onShortcutSet.AddListener(call);
        }
    }
}

using PAC.UI;
using UnityEngine;
using UnityEngine.Events;

namespace PAC.Files
{
    public class FileTab : MonoBehaviour
    {
        public File file { get; private set; }
        public bool selected => tileToggle.on;

        public UIToggleButton tileToggle { get; private set; }
        private UITextbox nameTextbox;
        private UIButton closeButton;

        public bool closed { get; set; } = false;

        private UnityEvent onSelect = new UnityEvent();
        private UnityEvent onClose = new UnityEvent();
        private UnityEvent onNameChange = new UnityEvent();

        private void Awake()
        {
            tileToggle = GetComponent<UIToggleButton>();
            nameTextbox = transform.Find("Name").GetComponent<UITextbox>();
            closeButton = transform.Find("Close").GetComponent<UIButton>();
        }

        // Start is called before the first frame update
        void Start()
        {
            tileToggle.SubscribeToTurnOn(Select);
            closeButton.SubscribeToClick(Close);
            nameTextbox.SubscribeToFinishEvent(OnNameChange);
        }

        public void SetFile(File file)
        {
            this.file = file;
            nameTextbox.SetText(file.name);
            file.SubscribeToOnSavedSinceEditChanged(SetSavedNameSuffix);
            SetSavedNameSuffix();
        }

        private void Select()
        {
            onSelect.Invoke();
        }

        private void Close()
        {
            closed = true;
            onClose.Invoke();
        }

        private void OnNameChange()
        {
            file.name = nameTextbox.text;
            onNameChange.Invoke();
        }

        private void SetSavedNameSuffix()
        {
            string suffix = nameTextbox.suffix.TrimEnd('*');
            suffix += file.savedSinceLastEdit ? "" : "*";
            nameTextbox.SetSuffix(suffix);
        }

        public void SubscribeToSelect(UnityAction call)
        {
            onSelect.AddListener(call);
        }
        public void SubscribeToClose(UnityAction call)
        {
            onClose.AddListener(call);
        }
        public void SubscribeToNameChange(UnityAction call)
        {
            onNameChange.AddListener(call);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PAC.UI
{
    public enum DropdownDirection
    {
        up = 1,
        down = -1
    }

    public class UIDropdownChoice : MonoBehaviour
    {
        [Header("Settings")]
        public DropdownDirection direction = DropdownDirection.down;
        public int selectedOptionIndex = 0;
        public List<string> options = new List<string>(new string[] { "Option 1", "Option 2", "Option 3" });
        public string selectedOption
        {
            get
            {
                return options[toggleGroup.currentToggleIndex];
            }
        }

        [Header("References")]
        [SerializeField]
        private GameObject toggleButtonPrefab;

        private UIToggleGroup toggleGroup;
        private UIDropdown dropdown;
        private UIButton openingButton;
        private Transform arrow;

        private UnityEvent onOptionChanged = new UnityEvent();

        private void Awake()
        {
            GetReferences();

            selectedOptionIndex = Mathf.Clamp(selectedOptionIndex, 0, options.Count - 1);
        }

        // Start is called before the first frame update
        void Start()
        {
            toggleGroup.SubscribeToSelectedToggleChange(OnOptionSelected);

            SetUpDropdown();
            dropdown.CloseRoot();
        }

        private void GetReferences()
        {
            toggleGroup = transform.Find("Toggle Group").GetComponent<UIToggleGroup>();
            dropdown = transform.Find("Dropdown").GetComponent<UIDropdown>();
            openingButton = GetComponent<UIButton>();
            arrow = transform.Find("Canvas").Find("Text").Find("Arrow");
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                GetReferences();
                openingButton.SetText(options[selectedOptionIndex]);
            }

            //arrow.eulerAngles = new Vector3(0f, 0f, 90f * (float)direction);
        }

        private void ClearButtons()
        {
            toggleGroup.DestroyToggles();
        }

        public void SetUpDropdown()
        {
            ClearButtons();

            for (int i = 0; i < options.Count; i++)
            {
                UIToggleButton button = Instantiate(toggleButtonPrefab, dropdown.transform).GetComponent<UIToggleButton>();

                button.width = openingButton.width;
                button.height = openingButton.height;

                button.SetText(options[i]);
                button.SetTextAlignment(TextAnchor.MiddleLeft);

                button.UpdateDisplay();

                if (direction == DropdownDirection.down)
                {
                    button.transform.localPosition = new Vector3(0f, -(i + 1) * openingButton.height, 0f);
                }
                else
                {
                    button.transform.localPosition = new Vector3(0f, (options.Count - i) * openingButton.height, 0f);
                }

                toggleGroup.Add(button);
            }

            toggleGroup.Press(selectedOptionIndex);

            dropdown.Initialise();
        }

        public bool Select(string option)
        {
            option = option.ToLower();
            for (int i = 0; i < options.Count; i++)
            {
                if (options[i].ToLower() == option)
                {
                    toggleGroup.Press(i);
                    return true;
                }
            }
            return false;
        }

        private void OnOptionSelected()
        {
            openingButton.SetText(selectedOption);
            onOptionChanged.Invoke();
        }

        public void SubscribeToOptionChanged(UnityAction call)
        {
            onOptionChanged.AddListener(call);
        }
    }
}
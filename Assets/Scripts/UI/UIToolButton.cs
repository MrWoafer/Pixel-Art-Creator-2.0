using System.Linq;
using PAC.Input;
using UnityEngine;

namespace PAC.UI
{
    public class UIToolButton : MonoBehaviour
    {
        public UIButton[] buttons { get; private set; }
        public UIButton currentButton { get; private set; }
        private UIToggleButton toggleButton;
        private Tooltip tooltip;

        private void Awake()
        {
            buttons = transform.GetComponentsInChildren<UIButton>();
            currentButton = buttons[0];
            toggleButton = transform.Find("Toggle Button").GetComponent<UIToggleButton>();
            tooltip = toggleButton.GetComponent<Tooltip>();
        }

        // Start is called before the first frame update
        void Start()
        {
            foreach (UIButton button in buttons)
            {
                UIButton temp = button;
                button.SubscribeToClick(() => {
                    Select(temp);
                    toggleButton.toggleGroup.PressForceEvent(toggleButton);
                });
            }

            toggleButton.SubscribeToTurnOff(() => Select(buttons[0]));
            toggleButton.SubscribeToRightClick(() => currentButton.RightClick());

            Select(currentButton);
        }

        private void Select(UIButton button)
        {
            if (!buttons.Contains(button))
            {
                throw new System.Exception("button not in buttons.");
            }

            bool gonePastButton = false;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] == button)
                {
                    gonePastButton = true;
                }
                else
                {
                    buttons[i].transform.localPosition = new Vector3(i - (gonePastButton ? 1f : 0f), 0f, 0f);
                }
            }
            button.transform.localPosition = new Vector3(-10000f, 0f, 0f);

            toggleButton.SetImages(button.image, button.pressedImage, button.hoverImage, button.pressedImage);
            tooltip.text = button.GetComponent<Tooltip>().text;
            toggleButton.toggleName = button.GetComponent<InputTarget>().targetName.ToLower();

            currentButton = button;
        }
    }
}

using UnityEngine;
using PAC.Input;
using PAC.UI.Components;

namespace PAC.Managers
{
    public class UIManager : MonoBehaviour
    {
        public UIElement selectedUIElement { get; private set; }

        private bool untargetedThisFrame = false;

        private void LateUpdate()
        {
            if (untargetedThisFrame)
            {
                untargetedThisFrame = false;
            }
        }

        public bool TryTarget(UIElement uiElement)
        {
            if (selectedUIElement)
            {
                return false;
            }

            Target(uiElement);
            return true;
        }

        private void Target(UIElement uiElement)
        {
            selectedUIElement = uiElement;

            //Debug.Log("Targeted UI Element: " + uiElement.elementName);
        }

        public bool TryUntarget(UIElement uiElement)
        {
            if (selectedUIElement != uiElement)
            {
                return false;
            }

            selectedUIElement = null;
            untargetedThisFrame = true;

            //Debug.Log("Untargeted UI Element: " + uiElement.elementName);

            return true;
        }

        public bool CanTargetInputTarget(InputTarget inputTarget)
        {
            return !untargetedThisFrame && (selectedUIElement == null || inputTarget.uiElement != null && inputTarget.uiElement == selectedUIElement);
        }
    }
}

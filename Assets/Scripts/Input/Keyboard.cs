using PAC.Keyboard_Shortcuts;
using UnityEngine;

namespace PAC.Input
{
    public class Keyboard : MonoBehaviour
    {
        public bool canInteract = false;

        private InputSystem inputSystem;

        // Start is called before the first frame update
        void Start()
        {
            inputSystem = GameObject.Find("Input System").GetComponent<InputSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            if (canInteract && inputSystem.inputTarget.keyboardInputEnabled)
            {
                foreach (CustomKeyCode key in CustomKeyCode.allKeyCodes)
                {
                    if (key.GetKeyDown())
                    {
                        if (inputSystem.hasInputTarget)
                        {
                            inputSystem.inputTarget.keyboardTarget.KeyDown(key);
                        }
                        else
                        {
                            inputSystem.globalInputTarget.keyboardTarget.KeyDown(key);
                        }
                    }
                    else if (key.GetKeyUp())
                    {
                        if (inputSystem.hasInputTarget)
                        {
                            inputSystem.inputTarget.keyboardTarget.KeyUp(key);
                        }
                        else
                        {
                            inputSystem.globalInputTarget.keyboardTarget.KeyUp(key);
                        }
                    }
                }
            }
        }
    }
}

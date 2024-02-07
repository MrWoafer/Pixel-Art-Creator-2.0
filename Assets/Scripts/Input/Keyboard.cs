using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
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
                else if (Input.GetKeyUp(key))
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputSystem : MonoBehaviour
{
    [Header("Typing Settings")]
    [Min(0f)]
    public float timeUntilKeySpam = 2f;
    [Min(0f)]
    public float timeBetweenKeySpam = 0.5f;

    [Header("References")]
    [SerializeField]
    private Mouse _mouse;
    public Mouse mouse
    {
        get
        {
            return _mouse;
        }
    }
    [SerializeField]
    private Keyboard _keyboard;
    public Keyboard keyboard
    {
        get
        {
            return _keyboard;
        }
    }

    public InputTarget inputTarget { get; private set; }
    public bool hasInputTarget
    {
        get
        {
            return inputTarget != globalInputTarget;
        }
    }

    private UndoRedoManager undoRedoManager;

    public InputTarget globalInputTarget { get; private set; }
    public MouseTarget globalMouseTarget
    {
        get
        {
            return globalInputTarget.mouseTarget;
        }
    }
    public KeyboardTarget globalKeyboardTarget
    {
        get
        {
            return globalInputTarget.keyboardTarget;
        }
    }

    private int lockedForAFrame = 0;

    private UnityEvent globalKeyboardInputEvent = new UnityEvent();
    private UnityEvent globalMouseScrollEvent = new UnityEvent();
    private UnityEvent globalLeftClickEvent = new UnityEvent();

    private void Awake()
    {
        undoRedoManager = Finder.undoRedoManager;

        globalInputTarget = GetComponent<InputTarget>();
    }

    private void Start()
    {
        inputTarget = globalInputTarget;

        globalKeyboardTarget.SubscribeToOnInput(GlobalKeyboardInput);
        mouse.SubscribeToScroll(GlobalMouseScroll);
        mouse.SubscribeToLeftClick(GlobalLeftClick);

        mouse.canInteract = true;
        keyboard.canInteract = true;
    }

    private void LateUpdate()
    {
        if (lockedForAFrame > 0)
        {
            lockedForAFrame--;

            if (lockedForAFrame <= 0)
            {
                lockedForAFrame = 0;
                mouse.canInteract = true;
                keyboard.canInteract = true;
            }
        }
    }

    public bool Target(InputTarget newTarget)
    {
        //Debug.Log("Targeting input target: " + newTarget.targetName);

        inputTarget.Target();

        mouse.canInteract = false;

        inputTarget = newTarget;

        if (inputTarget.keyboardInputEnabled && inputTarget.receiveAlreadyHeldKeys)
        {
            inputTarget.keyboardTarget.KeysDownNoSpamReset(globalInputTarget.keyboardTarget.keysHeld);
        }

        return true;
    }

    public InputTarget Untarget()
    {
        //Debug.Log("Untargeting input target: " + inputTarget.targetName);

        inputTarget.Untarget();

        InputTarget oldTarget = inputTarget;
        inputTarget = globalInputTarget;

        mouse.canInteract = true;
        keyboard.canInteract = true;

        return oldTarget;
    }

    private void GlobalKeyboardInput()
    {
        globalKeyboardInputEvent.Invoke();
    }

    public void LockForAFrame()
    {
        lockedForAFrame = 2;
        mouse.canInteract = false;
        keyboard.canInteract = false;
    }

    public void SubscribeToGlobalKeyboard(UnityAction call)
    {
        globalKeyboardInputEvent.AddListener(call);
    }

    private void GlobalMouseScroll()
    {
        globalMouseScrollEvent.Invoke();
    }

    public void SubscribeToGlobalMouseScroll(UnityAction call)
    {
        globalMouseScrollEvent.AddListener(call);
    }

    private void GlobalLeftClick()
    {
        globalLeftClickEvent.Invoke();
    }

    public void SubscribeToGlobalLeftClick(UnityAction call)
    {
        globalLeftClickEvent.AddListener(call);
    }
}

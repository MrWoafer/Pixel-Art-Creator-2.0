using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class KeyboardTarget
{
    private Dictionary<CustomKeyCode, bool> keyHeldStates = new Dictionary<CustomKeyCode, bool>();

    /// <summary>The keys that are currently held down, in order of when they were pressed (most recent on top).</summary>
    private CustomStack<CustomKeyCode> _keysHeld = new CustomStack<CustomKeyCode>();
    /// <summary>The keys that are currently held down, in order of when they were pressed (most recent first).</summary>
    public CustomKeyCode[] keysHeld { get => _keysHeld.ToArray(); }

    private CustomKeyCode currentKeyHeld { get => _keysHeld.Count == 0 ? KeyCode.None : _keysHeld.Peek(); }

    /// <summary>The keys that have been pressed this frame.</summary>
    private List<CustomKeyCode> _keysPressed = new List<CustomKeyCode>();
    public CustomKeyCode[] keysPressed { get => _keysPressed.ToArray(); }

    public bool receiveAlreadyHeldKeys = false;

    public bool allowHoldingKeySpam = false;
    private float timeHoldingKey = 0f;
    private float keySpamCount = 0;

    public bool inputThisFrame { get; private set; } = false;

    private InputSystem inputSystem;

    private UnityEvent onInput = new UnityEvent();
    private UnityEvent<CustomKeyCode> onKeyDown = new UnityEvent<CustomKeyCode>();
    private UnityEvent<CustomKeyCode> onKeyUp = new UnityEvent<CustomKeyCode>();
    private UnityEvent onUntarget = new UnityEvent();


    public KeyboardTarget()
    {
        inputSystem = Finder.inputSystem;

        foreach (char chr in "abcdefghijklmnopqrstuvwxyz0123456789,.;:<>_/\\?!*")
        {
            CustomKeyCode keyCode = KeyCodeFunctions.StrToKeyCode(chr.ToString());
            if (keyCode != KeyCode.None)
            {
                keyHeldStates.Add(keyCode, false);
            }
        }
        keyHeldStates.Add(KeyCode.Space, false);
        keyHeldStates.Add(KeyCode.Backspace, false);
        keyHeldStates.Add(KeyCode.Escape, false);
        keyHeldStates.Add(KeyCode.Return, false);
        keyHeldStates.Add(CustomKeyCode.Shift, false);
        keyHeldStates.Add(CustomKeyCode.Ctrl, false);
        keyHeldStates.Add(CustomKeyCode.Alt, false);
        keyHeldStates.Add(CustomKeyCode.Plus, false);
        keyHeldStates.Add(CustomKeyCode.Minus, false);
        keyHeldStates.Add(KeyCode.Delete, false);
        keyHeldStates.Add(KeyCode.UpArrow, false);
        keyHeldStates.Add(KeyCode.DownArrow, false);
        keyHeldStates.Add(KeyCode.LeftArrow, false);
        keyHeldStates.Add(KeyCode.RightArrow, false);
        keyHeldStates.Add(CustomKeyCode.GreaterThan, false);
        keyHeldStates.Add(CustomKeyCode.LessThan, false);

        if (keyHeldStates.ContainsKey(KeyCode.None))
        {
            keyHeldStates.Remove(KeyCode.None);
        }
    }

    /// <summary>
    /// Only to be called in InputTarget's Update() method.
    /// </summary>
    public void ManualUpdate()
    {
        if (currentKeyHeld != KeyCode.None)
        {
            timeHoldingKey += Time.deltaTime;
            if (timeHoldingKey > inputSystem.timeUntilKeySpam && allowHoldingKeySpam)
            {
                if (Mathf.Ceil((timeHoldingKey - inputSystem.timeUntilKeySpam) / inputSystem.timeBetweenKeySpam) > keySpamCount)
                {
                    keySpamCount++;
                    KeyDownNoSpamReset(currentKeyHeld);
                }
            }
        }

        if (inputThisFrame)
        {
            onInput.Invoke();
            _keysPressed = new List<CustomKeyCode>();
            inputThisFrame = false;
        }
    }

    /// <summary>
    /// Simulates the key being pressed, if it is a key detectable by KeyboardTarget, without restting the timer until key spamming occurs.
    /// </summary>
    public void KeyDownNoSpamReset(CustomKeyCode key)
    {
        if (keyHeldStates.ContainsKey(key))
        {
            keyHeldStates[key] = true;
            _keysPressed.Add(key);

            inputThisFrame = true;
        }
    }
    /// <summary>
    /// Simulates the keys being pressed, if they are keys detectable by KeyboardTarget, without restting the timer until key spamming occurs.
    /// </summary>
    public void KeysDownNoSpamReset(params CustomKeyCode[] keys)
    {
        foreach (CustomKeyCode key in keys)
        {
            KeyDownNoSpamReset(key);
        }
    }
    /// <summary>
    /// Simulates the key being pressed, if it is a key detectable by KeyboardTarget.
    /// </summary>
    public void KeyDown(CustomKeyCode key)
    {
        if (keyHeldStates.ContainsKey(key))
        {
            KeyDownNoSpamReset(key);

            _keysHeld.Push(key);
            ResetSpamTimer();

            onKeyDown.Invoke(key);
        }
    }
    /// <summary>
    /// Simulates the keys being pressed, if they are keys detectable by KeyboardTarget.
    /// </summary>
    public void KeysDown(params CustomKeyCode[] keys)
    {
        foreach (CustomKeyCode key in keys)
        {
            KeyDown(key);
        }
    }
    /// <summary>
    /// Simulates the key being unpressed.
    /// </summary>
    public void KeyUp(CustomKeyCode key)
    {
        if (keyHeldStates.ContainsKey(key))
        {
            keyHeldStates[key] = false;
            inputThisFrame = true;

            _keysHeld.RemoveAll(key);
            ResetSpamTimer();

            onKeyUp.Invoke(key);
        }
    }
    /// <summary>
    /// Simulates the keys being unpressed.
    /// </summary>
    public void KeysUp(params CustomKeyCode[] keys)
    {
        foreach (CustomKeyCode key in keys)
        {
            KeyUp(key);
        }
    }

    private void ResetSpamTimer()
    {
        timeHoldingKey = 0f;
        keySpamCount = 0;
    }

    /// <summary>
    /// Returns true if all the given key (and potentially some other keys) has been pressed this frame (and is a key detectable by KeyboardTarget).
    /// </summary>
    public bool IsPressed(CustomKeyCode key)
    {
        return keysPressed.Contains(key);
    }
    /// <summary>
    /// Returns true if all the given keys (and potentially some other keys) have been pressed this frame (and are keys detectable by KeyboardTarget).
    /// </summary>
    public bool IsPressed(KeyboardShortcut shortcut) => IsPressed(shortcut.ToArray());
    /// <summary>
    /// Returns true if all the given keys (and potentially some other keys) have been pressed this frame (and are keys detectable by KeyboardTarget).
    /// </summary>
    public bool IsPressed(params CustomKeyCode[] keys)
    {
        foreach (CustomKeyCode key in keys)
        {
            if (!IsPressed(key))
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// Returns true if all the given keys (and potentially some other keys) have been pressed this frame (and are keys detectable by KeyboardTarget) for one of the given keyboard shortcuts.
    /// </summary>
    public bool OneIsPressed(IEnumerable<KeyboardShortcut> shortcuts)
    {
        foreach (KeyboardShortcut shortcut in shortcuts)
        {
            if (IsPressed(shortcut))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns true if all the given key (and potentially some other keys) is held (and is a key detectable by KeyboardTarget).
    /// </summary>
    public bool IsHeld(KeyboardShortcut shortcut) => IsHeld(shortcut.ToArray());
    /// <summary>
    /// Returns true if all the given key (and potentially some other keys) is held (and is a key detectable by KeyboardTarget).
    /// </summary>
    public bool IsHeld(CustomKeyCode key)
    {
        return keyHeldStates.ContainsKey(key) && keyHeldStates[key];
    }
    /// <summary>
    /// Returns true if all the given keys (and potentially some other keys) are held (and are keys detectable by KeyboardTarget).
    /// </summary>
    public bool IsHeld(params CustomKeyCode[] keys)
    {
        foreach(CustomKeyCode key in keys)
        {
            if (!IsHeld(key))
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// Returns true if all the given keys (and potentially some other keys) are held (and are keys detectable by KeyboardTarget) for one of the given keyboard shortcuts.
    /// </summary>
    public bool OneIsHeld(IEnumerable<KeyboardShortcut> shortcuts)
    {
        foreach (KeyboardShortcut shortcut in shortcuts)
        {
            if (IsHeld(shortcut))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns true if all and only the given keys are held (and are keys detectable by KeyboardTarget).
    /// </summary>
    public bool IsHeldExactly(KeyboardShortcut shortcut) => IsHeldExactly(shortcut.ToArray());
    /// <summary>
    /// Returns true if all and only the given keys are held (and are keys detectable by KeyboardTarget).
    /// </summary>
    public bool IsHeldExactly(params CustomKeyCode[] keys)
    {
        int heldKeyCount = 0;
        foreach(CustomKeyCode keyCode in keyHeldStates.Keys)
        {
            if (keyHeldStates[keyCode])
            {
                if (keys.Contains(keyCode))
                {
                    heldKeyCount++;
                }
                else
                {
                    return false;
                }
            }
        }
        return heldKeyCount == keys.Length;
    }
    /// <summary>
    /// Returns true if all and only the given keys are held (and are keys detectable by KeyboardTarget) for one of the given keyboard shortcuts.
    /// </summary>
    public bool OneIsHeldExactly(IEnumerable<KeyboardShortcut> shortcuts)
    {
        foreach (KeyboardShortcut shortcut in shortcuts)
        {
            if (IsHeldExactly(shortcut))
            {
                return true;
            }
        }
        return false;
    }

    public void Untarget()
    {
        _keysHeld.Clear();
        ResetSpamTimer();

        foreach(CustomKeyCode key in keyHeldStates.Keys.ToArray())
        {
            keyHeldStates[key] = false;
        }

        onUntarget.Invoke();
    }

    public void SubscribeToOnInput(UnityAction call)
    {
        onInput.AddListener(call);
    }
    public void SubscribeToOnKeyDown(UnityAction<CustomKeyCode> call)
    {
        onKeyDown.AddListener(call);
    }
    public void SubscribeToOnKeyUp(UnityAction<CustomKeyCode> call)
    {
        onKeyUp.AddListener(call);
    }

    public void SubscribeToUntarget(UnityAction call)
    {
        onUntarget.AddListener(call);
    }
}

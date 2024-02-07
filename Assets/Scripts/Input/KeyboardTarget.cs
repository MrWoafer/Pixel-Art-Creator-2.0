using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class KeyboardTarget
{
    private Dictionary<KeyCode, bool> keyHeldStates = new Dictionary<KeyCode, bool>();

    /// <summary>The keys that are currently held down, in order of when they were pressed (most recent on top).</summary>
    private CustomStack<KeyCode> _keysHeld = new CustomStack<KeyCode>();
    /// <summary>The keys that are currently held down, in order of when they were pressed (most recent first).</summary>
    public KeyCode[] keysHeld { get => _keysHeld.ToArray(); }

    private KeyCode currentKeyHeld { get => _keysHeld.Count == 0 ? KeyCode.None : _keysHeld.Peek(); }

    /// <summary>The keys that have been pressed this frame.</summary>
    private List<KeyCode> _keysPressed = new List<KeyCode>();
    public KeyCode[] keysPressed { get => _keysPressed.ToArray(); }

    public bool receiveAlreadyHeldKeys = false;

    public bool allowHoldingKeySpam = false;
    private float timeHoldingKey = 0f;
    private float keySpamCount = 0;

    public bool inputThisFrame { get; private set; } = false;

    private InputSystem inputSystem;

    private UnityEvent onInput = new UnityEvent();


    public KeyboardTarget()
    {
        inputSystem = Finder.inputSystem;

        foreach (char chr in "abcdefghijklmnopqrstuvwxyz0123456789,.;:<>-_/\\?!*+=")
        {
            KeyCode keyCode = KeyCodeFunctions.StrToKeyCode(chr.ToString());
            if (keyCode != KeyCode.None)
            {
                keyHeldStates.Add(keyCode, false);
            }
        }
        keyHeldStates.Add(KeyCodeFunctions.StrToKeyCode("space"), false);
        keyHeldStates.Add(KeyCodeFunctions.StrToKeyCode("backspace"), false);
        keyHeldStates.Add(KeyCodeFunctions.StrToKeyCode("esc"), false);
        keyHeldStates.Add(KeyCodeFunctions.StrToKeyCode("enter"), false);
        keyHeldStates.Add(KeyCodeFunctions.StrToKeyCode("lshift"), false);
        keyHeldStates.Add(KeyCodeFunctions.StrToKeyCode("rshift"), false);
        keyHeldStates.Add(KeyCodeFunctions.StrToKeyCode("lctrl"), false);
        keyHeldStates.Add(KeyCodeFunctions.StrToKeyCode("rctrl"), false);
        keyHeldStates.Add(KeyCodeFunctions.StrToKeyCode("lalt"), false);
        keyHeldStates.Add(KeyCodeFunctions.StrToKeyCode("ralt"), false);
        keyHeldStates.Add(KeyCode.KeypadPlus, false);
        keyHeldStates.Add(KeyCode.KeypadMinus, false);
        keyHeldStates.Add(KeyCode.Delete, false);
        keyHeldStates.Add(KeyCode.LeftArrow, false);
        keyHeldStates.Add(KeyCode.RightArrow, false);

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
            _keysPressed = new List<KeyCode>();
            inputThisFrame = false;
        }
    }

    /// <summary>
    /// Simulates the key being pressed, if it is a key detectable by KeyboardTarget, without restting the timer until key spamming occurs.
    /// </summary>
    public void KeyDownNoSpamReset(KeyCode key)
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
    public void KeysDownNoSpamReset(params KeyCode[] keys)
    {
        foreach (KeyCode key in keys)
        {
            KeyDownNoSpamReset(key);
        }
    }
    /// <summary>
    /// Simulates the key being pressed, if it is a key detectable by KeyboardTarget.
    /// </summary>
    public void KeyDown(KeyCode key)
    {
        if (keyHeldStates.ContainsKey(key))
        {
            KeyDownNoSpamReset(key);

            _keysHeld.Push(key);
            ResetSpamTimer();
        }
    }
    /// <summary>
    /// Simulates the keys being pressed, if they are keys detectable by KeyboardTarget.
    /// </summary>
    public void KeysDown(params KeyCode[] keys)
    {
        foreach (KeyCode key in keys)
        {
            KeyDown(key);
        }
    }
    /// <summary>
    /// Simulates the key being unpressed.
    /// </summary>
    public void KeyUp(KeyCode key)
    {
        if (keyHeldStates.ContainsKey(key))
        {
            keyHeldStates[key] = false;
            inputThisFrame = true;

            _keysHeld.RemoveAll(key);
            ResetSpamTimer();
        }
    }
    /// <summary>
    /// Simulates the keys being unpressed.
    /// </summary>
    public void KeysUp(params KeyCode[] keys)
    {
        foreach (KeyCode key in keys)
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
    public bool IsPressed(KeyCode key)
    {
        return keysPressed.Contains(key);
    }
    /// <summary>
    /// Returns true if all the given keys (and potentially some other keys) have been pressed this frame (and are keys detectable by KeyboardTarget).
    /// </summary>
    public bool IsPressed(KeyCode[] keys)
    {
        foreach (KeyCode key in keys)
        {
            if (!IsPressed(key))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Returns true if all the given key (and potentially some other keys) is held (and is a key detectable by KeyboardTarget).
    /// </summary>
    public bool IsHeld(KeyCode key)
    {
        return keyHeldStates.ContainsKey(key) && keyHeldStates[key];
    }
    /// <summary>
    /// Returns true if all the given keys (and potentially some other keys) are held (and are keys detectable by KeyboardTarget).
    /// </summary>
    public bool IsHeld(params KeyCode[] keys)
    {
        foreach(KeyCode key in keys)
        {
            if (!IsHeld(key))
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// Returns true if all and only the given keys are held (and are keys detectable by KeyboardTarget).
    /// </summary>
    public bool IsHeldExactly(params KeyCode[] keys)
    {
        int keysDetectable = 0;
        foreach(KeyCode key in keyHeldStates.Keys)
        {
            if (keyHeldStates[key])
            {
                if (!keys.Contains(key))
                {
                    return false;
                }
                keysDetectable++;
            }
        }
        return keysDetectable == keys.Length;
    }

    public void Untarget()
    {
        _keysHeld.Clear();
        ResetSpamTimer();

        foreach(KeyCode key in keyHeldStates.Keys.ToArray())
        {
            keyHeldStates[key] = false;
        }
    }

    public void SubscribeToOnInput(UnityAction call)
    {
        onInput.AddListener(call);
    }
}

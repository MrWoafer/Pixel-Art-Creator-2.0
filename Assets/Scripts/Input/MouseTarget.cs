using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum MouseTargetState
{
    Idle = 0,
    Hover = 1,
    Pressed = 2
}

public enum MouseTargetDeselectMode
{
    Unclick = 0,
    ClickAgain = 1
}

public class MouseTarget
{
    public bool isHoverTrigger = false;
    public bool allowLeftClick = true;
    public bool allowRightClick = false;
    public bool allowMiddleClick = false;
    public bool allowScroll = false;

    public CursorState cursorHover = CursorState.Hover;
    public CursorState cursorPress = CursorState.Press;
    public CursorState cursorSelected = CursorState.Unspecified;
    public CursorState cursorScroll = CursorState.Unspecified;

    public MouseTargetDeselectMode deselectMode = MouseTargetDeselectMode.Unclick;

    private MouseTargetState _state = MouseTargetState.Idle;
    public MouseTargetState state
    {
        get
        {
            return _state;
        }
    }

    private UnityEvent onIdle = new UnityEvent();
    private UnityEvent onHover = new UnityEvent();
    private UnityEvent onPress = new UnityEvent();
    private UnityEvent onLeftClick = new UnityEvent();
    private UnityEvent onRightClick = new UnityEvent();
    private UnityEvent onMiddleClick = new UnityEvent();
    private UnityEvent onSelect = new UnityEvent();
    private UnityEvent onStateChange = new UnityEvent();
    private UnityEvent onScroll = new UnityEvent();

    public bool selected { get; private set; } = false;

    public float timeHovered
    {
        get
        {
            return state == MouseTargetState.Hover || state == MouseTargetState.Pressed ? Time.time - timeStartedHovering : 0f;
        }
    }
    private float timeStartedHovering;

    public MouseButton buttonTargetedWith;

    public void Idle()
    {
        if (_state != MouseTargetState.Idle)
        {
            _state = MouseTargetState.Idle;
            onIdle.Invoke();
            onStateChange.Invoke();
        }
    }

    public void Hover()
    {
        if (_state != MouseTargetState.Hover)
        {
            _state = MouseTargetState.Hover;
            onHover.Invoke();
            onStateChange.Invoke();

            timeStartedHovering = Time.time;
        }
    }

    public void Press()
    {
        if (_state != MouseTargetState.Pressed)
        {
            _state = MouseTargetState.Pressed;

            if (buttonTargetedWith == MouseButton.Left)
            {
                onLeftClick.Invoke();
            }
            else if (buttonTargetedWith == MouseButton.Right)
            {
                onRightClick.Invoke();
            }
            else if (buttonTargetedWith == MouseButton.Middle)
            {
                onMiddleClick.Invoke();
            }

            onPress.Invoke();
            onStateChange.Invoke();
        }
    }

    public void Select()
    {
        if (!selected)
        {
            selected = true;
            onSelect.Invoke();
            onStateChange.Invoke();
        }
    }
    public void Unselect()
    {
        if (selected)
        {
            selected = false;
            onStateChange.Invoke();
        }
    }

    public void Scroll()
    {
        onScroll.Invoke();
    }

    public void SubscribeToIdle(UnityAction call)
    {
        onIdle.AddListener(call);
    }

    public void SubscribeToHover(UnityAction call)
    {
        onHover.AddListener(call);
    }

    public void SubscribeToPress(UnityAction call)
    {
        onPress.AddListener(call);
    }

    public void SubscribeToSelect(UnityAction call)
    {
        onSelect.AddListener(call);
    }

    public void SubscribeToStateChange(UnityAction call)
    {
        onStateChange.AddListener(call);
    }

    public void SubscribeToScroll(UnityAction call)
    {
        onScroll.AddListener(call);
    }

    public void SubscribeToLeftClick(UnityAction call)
    {
        onLeftClick.AddListener(call);
    }
    public void SubscribeToRightClick(UnityAction call)
    {
        onRightClick.AddListener(call);
    }
    public void SubscribeToMiddleClick(UnityAction call)
    {
        onMiddleClick.AddListener(call);
    }

    public void Untarget()
    {
        Unselect();
        Idle();
    }
}

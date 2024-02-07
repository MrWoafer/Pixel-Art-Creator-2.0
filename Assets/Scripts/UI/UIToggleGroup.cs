using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("Custom UI/UI Toggle Group")]
public class UIToggleGroup : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private bool _canSelectMultiple = false;
    public bool canSelectMultiple
    {
        get
        {
            return _canSelectMultiple;
        }
    }
    [SerializeField]
    private bool _canSelectNone = false;
    public bool canSelectNone
    {
        get
        {
            return _canSelectNone;
        }
    }
    [SerializeField]
    private bool _swapClickAndCtrlClick = false;
    public bool swapClickAndCtrlClick
    {
        get
        {
            return _swapClickAndCtrlClick;
        }
    }

    [SerializeField]
    private UIToggleButton _currentToggle = null;
    public UIToggleButton currentToggle
    {
        get => _currentToggle;
        private set => _currentToggle = value;
    }

    public int currentToggleIndex
    {
        get
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                if (toggles[i] == currentToggle)
                {
                    return i;
                }
            }
            return -1;
        }
    }

    [SerializeField]
    private List<UIToggleButton> _toggles = new List<UIToggleButton>();
    public List<UIToggleButton> toggles
    {
        get
        {
            return _toggles;
        }
    }

    public int Count
    {
        get
        {
            return _toggles.Count;
        }
    }

    public UIToggleButton[] selectedToggles
    {
        get
        {
            List<UIToggleButton> selected = new List<UIToggleButton>();

            foreach (UIToggleButton toggle in toggles)
            {
                if (toggle.on)
                {
                    selected.Add(toggle);
                }
            }

            return selected.ToArray();
        }
    }
    public int[] selectedIndices
    {
        get
        {
            List<int> selected = new List<int>();

            for (int i = 0; i < toggles.Count; i++)
            {
                if (toggles[i].on)
                {
                    selected.Add(i);
                }
            }

            return selected.ToArray();
        }
    }

    public bool hasSelectedToggle
    {
        get
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                if (toggles[i].on)
                {
                    return true;
                }
            }
            return false;
        }
    }

    private UnityEvent onSelectedToggleChanged = new UnityEvent();

    private void Start()
    {
        if (currentToggle && !Contains(currentToggle))
        {
            throw new System.Exception("The given starting toggle is not in the toggle group.");
        }
        
        foreach (UIToggleButton toggle in _toggles)
        {
            if (toggle != null)
            {
                toggle.SetOnOff(false);
                toggle.JoinToggleGroup(this);
            }
        }
        if (currentToggle != null)
        {
            currentToggle.SetOnOff(true);
        }
    }

    public bool Add(UIToggleButton toggle)
    {
        _toggles.Add(toggle);
        toggle.JoinToggleGroup(this);

        return true;
    }

    public bool Remove(UIToggleButton toggle)
    {
        if(_toggles.Remove(toggle))
        {
            toggle.LeaveToggleGroup();
            return true;
        }
        return false;
    }

    public bool Contains(UIToggleButton toggle)
    {
        return _toggles.Contains(toggle);
    }

    public bool Press(int index)
    {
        if (index < 0 || index >= toggles.Count)
        {
            throw new System.Exception("Index out of range: " + index);
        }
        return Press(toggles[index]);
    }
    public bool Press(UIToggleButton toggle)
    {
        return PressOrCtrlPress(toggle, swapClickAndCtrlClick);
    }

    public bool CtrlPress(int index)
    {
        if (index < 0 || index >= toggles.Count)
        {
            throw new System.Exception("Index out of range: " + index);
        }
        return CtrlPress(toggles[index]);
    }
    public bool CtrlPress(UIToggleButton toggle)
    {
        return PressOrCtrlPress(toggle, !swapClickAndCtrlClick);
    }

    public bool PressOrCtrlPress(UIToggleButton toggle, bool ctrlClick)
    {
        if (!Contains(toggle))
        {
            return false;
        }

        if (!ctrlClick)
        {
            if (currentToggle == toggle && selectedToggles.Length == 1)
            {
                return false;
            }

            if (currentToggle != null)
            {
                foreach (UIToggleButton selectedToggle in selectedToggles)
                {
                    selectedToggle.SetOnOff(false);
                }
            }
            currentToggle = toggle;
            toggle.SetOnOff(true);

            onSelectedToggleChanged.Invoke();

            return true;
        }
        else
        {
            if (!canSelectMultiple)
            {
                PressOrCtrlPress(toggle, false);
            }

            if (toggle.on)
            {
                if (toggle == currentToggle)
                {
                    if (selectedToggles.Length == 1)
                    {
                        if (!canSelectNone)
                        {
                            return false;
                            
                        }
                        currentToggle = null;
                    }
                    else
                    {
                        currentToggle = selectedToggles[0];
                        if (currentToggle == toggle)
                        {
                            currentToggle = selectedToggles[1];
                        }
                    }
                }

                toggle.SetOnOff(false);

                onSelectedToggleChanged.Invoke();

                return true;
            }
            else
            {
                currentToggle = toggle;
                toggle.SetOnOff(true);

                onSelectedToggleChanged.Invoke();

                return true;
            }
        }

        return false;
    }

    public void Clear()
    {
        _toggles = new List<UIToggleButton>();
        currentToggle = null;
    }

    public void DestroyToggles()
    {
        foreach (UIToggleButton toggle in _toggles)
        {
            Destroy(toggle.gameObject);
        }

        Clear();
    }

    public void SubscribeToSelectedToggleChange(UnityAction call)
    {
        onSelectedToggleChanged.AddListener(call);
    }
}

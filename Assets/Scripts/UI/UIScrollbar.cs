using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum ScrollDirection
{
    Vertical = 0,
    Horizontal = 1
}

[RequireComponent(typeof(InputTarget))]
[AddComponentMenu("Custom UI/UI Scrollbar")]
public class UIScrollbar : MonoBehaviour
{
    [Header("Scroll")]
    [SerializeField]
    [Range(-1f, 1f)]
    private float _scrollAmount = 0f;
    public float scrollAmount
    {
        get
        {
            return _scrollAmount;
        }
        set
        {
            SetScrollAmount(value);
        }
    }
    [SerializeField]
    [Min(0f)]
    private float scrollToTime = 0.5f;

    [Header("Size")]
    [Min(0f)]
    public float width = 1f;
    [Min(0f)]
    public float height = 1f;

    [Header("Background")]
    public Color backgroundColour = Color.white;
    public Color backgroundHoverColour = Color.white;
    public Color backgroundPressedColour = Color.white;

    [Header("Handle")]
    [SerializeField]
    [Min(0f)]
    private float minHandleHeight = 0.12f;
    public Color handleColour = Color.white;
    public Color handleHoverColour = Color.white;
    public Color handlePressedColour = Color.white;

    [Header("Viewport")]
    [SerializeField]
    private UIViewport viewport;

    [Header("Behaviour")]
    [Space()]
    [SerializeField]
    private UnityEvent onIdle = new UnityEvent();
    [SerializeField]
    private UnityEvent onHover = new UnityEvent();
    [SerializeField]
    private UnityEvent onClick = new UnityEvent();

    private RectTransform rectTransform;
    private InputTarget inputTarget;

    private GameObject handle;
    private InputTarget handleInputTarget;

    private BoxCollider2D collider;

    private Image background;
    private Image handleImg;

    private InputSystem inputSystem;
    private Mouse mouse;

    private float scrollToPositionT = 0f;
    private float scrollToPosition;
    private float scrollFromPosition;
    private Vector3 previousMousePos;

    private bool beenRunningForAFrame = false;

    private void Awake()
    {
        GetReferences();
    }

    private void Start()
    {
        inputTarget.mouseTarget.SubscribeToStateChange(MouseTargetInput);
        handleInputTarget.mouseTarget.SubscribeToStateChange(MouseTargetInput);

        inputTarget.mouseTarget.SubscribeToIdle(BackgroundIdle);
        inputTarget.mouseTarget.SubscribeToHover(BackgroundHover);
        inputTarget.mouseTarget.SubscribeToPress(BackgroundPress);

        handleInputTarget.mouseTarget.SubscribeToIdle(HandleIdle);
        handleInputTarget.mouseTarget.SubscribeToHover(HandleHover);
        handleInputTarget.mouseTarget.SubscribeToPress(HandlePress);

        viewport.SubscribeToRefresh(RefreshScroll);

        UpdateDisplay();
    }

    private void Update()
    {
        if (inputTarget.mouseTarget.state == MouseTargetState.Pressed)
        {
            scrollToPositionT += Time.deltaTime;
            SetScrollAmount(Mathf.Lerp(scrollFromPosition, scrollToPosition, Mathf.Clamp(scrollToPositionT / scrollToTime, 0f, 1f)));
        }
        else if (handleInputTarget.mouseTarget.state == MouseTargetState.Pressed)
        {
            if ((mouse.worldPos.y > previousMousePos.y && mouse.worldPos.y > handle.transform.position.y) || (mouse.worldPos.y < previousMousePos.y && mouse.worldPos.y < handle.transform.position.y))
            {
                SetScrollAmount(scrollAmount + (transform.InverseTransformPoint(mouse.worldPos).y - transform.InverseTransformPoint(previousMousePos).y) /
                    (rectTransform.sizeDelta.y - handle.transform.localScale.y) * 2f);
            }

            previousMousePos = mouse.worldPos;
        }

        if (!beenRunningForAFrame)
        {
            beenRunningForAFrame = true;
        }
    }

    private void GetReferences()
    {
        inputSystem = Finder.inputSystem;
        mouse = Finder.mouse;

        rectTransform = GetComponent<RectTransform>();
        inputTarget = GetComponent<InputTarget>();

        handle = transform.Find("Handle").gameObject;
        handleInputTarget = handle.GetComponent<InputTarget>();

        background = GetComponent<Image>();
        handleImg = handle.GetComponent<Image>();

        collider = GetComponent<BoxCollider2D>();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying || beenRunningForAFrame)
        {
            GetReferences();

            UpdateDisplay();
        }
    }

    public void UpdateDisplay()
    {
        UpdateSize();
        RefreshScroll();
        BackgroundIdle();
        HandleIdle();
    }

    private void UpdateSize()
    {
        collider.size = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
    }

    private void BackgroundIdle()
    {
        background.color = backgroundColour;
    }
    private void HandleIdle()
    {
        handleImg.color = handleColour;
    }

    private void BackgroundHover()
    {
        background.color = backgroundHoverColour;
    }
    private void HandleHover()
    {
        handleImg.color = handleHoverColour;
    }

    private void BackgroundPress()
    {
        background.color = backgroundPressedColour;
    }
    private void HandlePress()
    {
        handleImg.color = handlePressedColour;
    }

    private void RefreshScroll()
    {
        SetScrollAmount(scrollAmount);
    }

    public void SetScrollAmount(float scrollAmount)
    {
        scrollAmount = Mathf.Clamp(scrollAmount, -1f, 1f);

        if (!Application.isPlaying)
        {
            GetReferences();
        }

        if (viewport && viewport.boundScrollToContents)
        {
            float maxY, minY;
            Vector2 minMaxY = viewport.GetScrollMinMaxY();
            minY = minMaxY.x;
            maxY = minMaxY.y;

            handle.transform.localScale = new Vector2(handle.transform.localScale.x,
                Mathf.Clamp((rectTransform.sizeDelta.y - Mathf.Max(maxY - minY - viewport.rectTransform.sizeDelta.y, 0f)), minHandleHeight,
                float.MaxValue));

            if (minMaxY.y - minMaxY.x > viewport.rectTransform.sizeDelta.y)
            {
                _scrollAmount = scrollAmount;
            }
            else
            {
                _scrollAmount = (float)viewport.defaultScrollSide;
            }
            viewport.SetScrollAmount(scrollAmount);
        }
        else
        {
            _scrollAmount = scrollAmount;
            viewport?.SetScrollAmount(scrollAmount);
        }

        handle.transform.localPosition = new Vector3(handle.transform.localPosition.x, (rectTransform.sizeDelta.y - handle.transform.localScale.y) / 2f * scrollAmount, handle.transform.localPosition.z);
    }

    public float GetScrollAmount()
    {
        return _scrollAmount;
    }

    private void MouseTargetInput()
    {
        if (inputTarget.mouseTarget.state == MouseTargetState.Pressed)
        {
            scrollToPositionT = 0f;
            scrollToPosition = transform.InverseTransformPoint(mouse.worldPos).y / (rectTransform.sizeDelta.y - handle.transform.localPosition.y) * 2f;
            scrollFromPosition = scrollAmount;
        }
        else if (handleInputTarget.mouseTarget.state == MouseTargetState.Pressed)
        {
            previousMousePos = mouse.worldPos;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for the hue/saturation box of the HSL colour picker.
/// </summary>
public class HSLHueSaturationBox : MonoBehaviour
{
    private float _hue = 0f;
    public float hue
    {
        get => _hue;
        private set
        {
            _hue = hue;
        }
    }

    private float _saturation = 0f;
    public float saturation
    {
        get => _saturation;
        private set
        {
            _saturation = value;
        }
    }

    [Header("References")]
    private HSLColourPicker hslColourPicker;
    private Transform cursor;

    private InputTarget inputTarget;
    private Mouse mouse;

    private Vector2 previousMousePos;

    private void Awake()
    {
        hslColourPicker = transform.parent.GetComponent<HSLColourPicker>();
        cursor = transform.Find("Cursor");

        inputTarget = GetComponent<InputTarget>();
        mouse = Finder.mouse;
    }

    private void Start()
    {
        inputTarget.mouseTarget.SubscribeToStateChange(OnMouseInput);
    }

    private void Update()
    {
        if (inputTarget.mouseTarget.state == MouseTargetState.Pressed)
        {
            UpdateMousePosition();

            previousMousePos = mouse.worldPos;
        }
    }

    public void SetHue(float hue)
    {
        _hue = hue;
        UpdateCursor();
    }
    public void SetSaturation(float saturation)
    {
        _saturation = saturation;
        UpdateCursor();
    }

    /// <summary>
    /// Called when there in mouse input.
    /// </summary>
    private void OnMouseInput()
    {
        if (inputTarget.mouseTarget.state == MouseTargetState.Pressed)
        {
            Vector3 mouseLocalCoords = transform.InverseTransformPoint(mouse.worldPos);

            mouseLocalCoords = new Vector3(Mathf.Clamp(mouseLocalCoords.x, -0.5f, 0.5f), Mathf.Clamp(mouseLocalCoords.y, -0.5f, 0.5f), 0f);

            UpdateValuesFromMouse(mouseLocalCoords);

            previousMousePos = mouse.worldPos;
        }
    }

    /// <summary>
    /// Handles moving the mouse cursor and updating hue/saturation based on how the mouse has moved since last frame.
    /// </summary>
    private void UpdateMousePosition()
    {
        float sensitivity = hslColourPicker.mouseSensitivity;
        if (inputTarget.keyboardTarget.IsHeldExactly(CustomKeyCode.Ctrl))
        {
            sensitivity *= hslColourPicker.slowSensitivityScalar;
        }

        Vector3 mouseLocalCoords = cursor.localPosition + transform.InverseTransformVector(Functions.Vector2ToVector3(mouse.worldPos - previousMousePos)) * sensitivity;

        mouseLocalCoords = new Vector3(Mathf.Clamp(mouseLocalCoords.x, -0.5f, 0.5f), Mathf.Clamp(mouseLocalCoords.y,-0.5f, 0.5f), 0f);

        UpdateValuesFromMouse(mouseLocalCoords);
    }

    /// <summary>
    /// Sets the hue/saturation based on the mouse position.
    /// </summary>
    private void UpdateValuesFromMouse(Vector2 mouseLocalCoords)
    {
        SetHue(mouseLocalCoords.x + 0.5f);
        SetSaturation(mouseLocalCoords.y + 0.5f);

        UpdateCursor();

        hslColourPicker.UpdateColour();
    }

    /// <summary>
    /// Move the cursor to the correct position.
    /// </summary>
    private void UpdateCursor()
    {
        cursor.localPosition = new Vector3(hue - 0.5f, saturation - 0.5f, 0f);
    }
}

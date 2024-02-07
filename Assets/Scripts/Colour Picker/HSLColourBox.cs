using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HSLColourBox : MonoBehaviour
{
    private float _hue = 0f;
    public float hue
    {
        get
        {
            return _hue;
        }
        set
        {
            SetHue(value);
        }
    }

    private float _saturation = 0f;
    public float saturation
    {
        get
        {
            return _saturation;
        }
        set
        {
            SetSaturation(value);
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
        inputTarget.mouseTarget.SubscribeToStateChange(MouseInput);
    }

    private void Update()
    {
        if (inputTarget.mouseTarget.state == MouseTargetState.Pressed)
        {
            UpdateMousePosition();

            previousMousePos = mouse.worldPos;
        }
    }

    private void MouseInput()
    {
        if (inputTarget.mouseTarget.state == MouseTargetState.Pressed)
        {
            Vector3 mouseLocalCoords = transform.InverseTransformPoint(mouse.worldPos);

            mouseLocalCoords = new Vector3(Mathf.Clamp(mouseLocalCoords.x, -0.5f, 0.5f), Mathf.Clamp(mouseLocalCoords.y, -0.5f, 0.5f), 0f);

            UpdateValuesFromMouse(mouseLocalCoords);

            previousMousePos = mouse.worldPos;
        }
    }

    private void UpdateMousePosition()
    {
        float sensitivity = hslColourPicker.mouseSensitivity;
        if (inputTarget.keyboardTarget.IsHeldExactly(KeyCode.LeftControl) || inputTarget.keyboardTarget.IsHeldExactly(KeyCode.RightControl))
        {
            sensitivity *= hslColourPicker.slowSensitivityScalar;
        }

        Vector3 mouseLocalCoords = cursor.localPosition + transform.InverseTransformVector(Functions.Vector2ToVector3(mouse.worldPos - previousMousePos)) * sensitivity;

        mouseLocalCoords = new Vector3(Mathf.Clamp(mouseLocalCoords.x, -0.5f, 0.5f), Mathf.Clamp(mouseLocalCoords.y,-0.5f, 0.5f), 0f);

        UpdateValuesFromMouse(mouseLocalCoords);
    }

    private void UpdateValuesFromMouse(Vector2 mouseLocalCoords)
    {
        hue = mouseLocalCoords.x + 0.5f;
        saturation = mouseLocalCoords.y + 0.5f;

        UpdateCursor();

        hslColourPicker.UpdateColour();
    }

    private void UpdateCursor()
    {
        cursor.localPosition = new Vector3(hue - 0.5f, saturation - 0.5f, 0f);
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
}

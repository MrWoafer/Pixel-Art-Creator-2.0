using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HSLColourSlider : MonoBehaviour
{
    private float _lightness = 0f;
    public float lightness
    {
        get
        {
            return _lightness;
        }
        set
        {
            SetLightness(value);
        }
    }

    [Header("References")]
    private HSLColourPicker hslColourPicker;
    private Transform cursor;
    private Renderer renderer;

    private InputTarget inputTarget;
    private Mouse mouse;

    private Vector2 previousMousePos;

    private void Awake()
    {
        hslColourPicker = transform.parent.GetComponent<HSLColourPicker>();
        cursor = transform.Find("Cursor");
        renderer = GetComponent<Renderer>();

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

            mouseLocalCoords = new Vector3(0f, Mathf.Clamp(mouseLocalCoords.y, -0.5f, 0.5f), 0f);

            UpdateValuesFromMouse(mouseLocalCoords);

            previousMousePos = mouse.worldPos;
        }
    }

    private void UpdateMousePosition()
    {
        float sensitivity = hslColourPicker.mouseSensitivity;
        if (inputTarget.keyboardTarget.IsHeldExactly(KeyCode.LeftControl))
        {
            sensitivity *= hslColourPicker.slowSensitivityScalar;
        }

        Vector3 mouseLocalCoords = cursor.localPosition + transform.InverseTransformVector(Functions.Vector2ToVector3(mouse.worldPos - previousMousePos)) * sensitivity;

        mouseLocalCoords = new Vector3(0f, Mathf.Clamp(mouseLocalCoords.y, -0.5f, 0.5f), 0f);

        UpdateValuesFromMouse(mouseLocalCoords);
    }

    private void UpdateValuesFromMouse(Vector2 mouseLocalCoords)
    {
        lightness = mouseLocalCoords.y + 0.5f;

        UpdateCursor();

        hslColourPicker.UpdateColour();
    }

    public void SetHueSaturation(float hue, float saturation)
    {
        renderer.material.SetFloat("_Hue", hue);
        renderer.material.SetFloat("_Saturation", saturation);
    }

    private void UpdateCursor()
    {
        cursor.localPosition = new Vector3(0f, lightness - 0.5f, 0f);
    }

    public void SetLightness(float lightness)
    {
        _lightness = lightness;
        UpdateCursor();
    }
}

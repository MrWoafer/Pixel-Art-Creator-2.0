using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Theme", menuName = "Custom/Theme")]
public class Theme : ScriptableObject
{
    [Header("Theme")]
    public string themeName = "New Theme";

    [Header("Panels")]
    public Color backgroundColour = Color.white;
    public Color panelColour = Color.white;
    public Color subPanelColour = Color.white;
    public Color shadowColour = new Color(0f, 0f, 0f, 180f / 255f);

    [Header("Buttons")]
    public Color buttonColour = Color.white;
    public Color buttonHoverColour = Color.white;
    public Color buttonPressedColour = Color.white;

    [Header("Toggle Buttons")]
    public Color toggleButtonOffColour = Color.white;
    public Color toggleButtonOnColour = Color.white;
    public Color toggleButtonHoverTint = Color.white;
    public Color toggleButtonPressedColour = Color.white;

    [Header("Textboxes")]
    public Color textboxColour = Color.white;
    public Color textboxHoverColour = Color.white;
    public Color textboxPressedColour = Color.white;
    public Color textboxSelectedColour = Color.white;

    [Header("Scales")]
    public Color scaleColour = Color.white;
    public Color scaleHoverColour = Color.white;
    public Color scalePressedColour = Color.white;

    [Header("Scrollbars")]
    public Color scrollbarHandleColour = Color.white;
    public Color scrollbarHandleHoverColour = Color.white;
    public Color scrollbarHandlePressedColour = Color.white;

    public Color scrollbarBackgroundColour = Color.white;
    public Color scrollbarBackgroundHoverColour = Color.white;
    public Color scrollbarBackgroundPressedColour = Color.white;

    [Header("Layer Tiles")]
    public Color layerTileOffColour = Color.white;
    public Color layerTileOnColour = Color.white;
    public Color layerTileHoverTint = Color.white;
    public Color layerTilePressedColour = Color.white;


    private UnityEvent onChanged = new UnityEvent();

    private void OnValidate()
    {
        onChanged.Invoke();
    }

    public void SubscribeToOnChanged(UnityAction call)
    {
        onChanged.AddListener(call);
    }
}

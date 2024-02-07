using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ThemeObjectType
{
    None = 0,
    Background = 1,
    Panel = 2,
    SubPanel = 3,
    Shadow = 4,
    RadioButton = 5
}

[AddComponentMenu("Custom UI/UI Theme")]
public class UITheme : MonoBehaviour
{
    [SerializeField]
    private bool useTheme = true;
    [SerializeField]
    private ThemeObjectType themeObjectType = ThemeObjectType.None;

    private ThemeManager themeManager;
    private Theme theme { get => themeManager.currentTheme; }

    private void Awake()
    {
        themeManager = Finder.themeManager;
        themeManager.SubscribeToThemeChanged(OnThemeChanged);
    }

    private void Start()
    {
        OnThemeChanged();
    }

    private void OnThemeChanged()
    {
        if (!useTheme || !Application.isPlaying)
        {
            return;
        }

        Image image = GetComponent<Image>();
        if (image)
        {
            if (themeObjectType == ThemeObjectType.Background)
            {
                image.color = theme.backgroundColour;
            }
            else if (themeObjectType == ThemeObjectType.Panel)
            {
                image.color = theme.panelColour;
            }
            else if (themeObjectType == ThemeObjectType.SubPanel)
            {
                image.color = theme.subPanelColour;
            }
            else if (themeObjectType == ThemeObjectType.Shadow)
            {
                image.color = theme.shadowColour;
            }
        }

        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        if (spr)
        {
            if (themeObjectType == ThemeObjectType.Background)
            {
                spr.color = theme.backgroundColour;
            }
            else if (themeObjectType == ThemeObjectType.Panel)
            {
                spr.color = theme.panelColour;
            }
            else if (themeObjectType == ThemeObjectType.SubPanel)
            {
                spr.color = theme.subPanelColour;
            }
        }

        UIButton button = GetComponent<UIButton>();
        if (button)
        {
            button.backgroundColour = theme.buttonColour;
            button.backgroundHoverColour = theme.buttonHoverColour;
            button.backgroundPressedColour = theme.buttonPressedColour;

            button.UpdateDisplay();
        }

        UIToggleButton toggleButton = GetComponent<UIToggleButton>();
        if (toggleButton)
        {
            toggleButton.offBackgroundColour = theme.toggleButtonOffColour;
            if (toggleButton.inToggleGroup && themeObjectType != ThemeObjectType.RadioButton)
            {
                toggleButton.onBackgroundColour = theme.toggleButtonOnColour;
            }
            else
            {
                toggleButton.onBackgroundColour = theme.toggleButtonOffColour;
            }
            toggleButton.hoverBackgroundTint = theme.toggleButtonHoverTint;
            toggleButton.pressedBackgroundColour = theme.toggleButtonPressedColour;

            toggleButton.UpdateDisplay();
        }

        UITextbox textbox = GetComponent<UITextbox>();
        if (textbox)
        {
            textbox.backgroundColour = theme.textboxColour;
            textbox.backgroundHoverColour = theme.textboxHoverColour;
            textbox.backgroundPressedColour = theme.textboxPressedColour;
            textbox.backgroundSelectedColour = theme.textboxSelectedColour;

            textbox.UpdateDisplay();
        }

        UIScale scale = GetComponent<UIScale>();
        if (scale)
        {
            scale.backgroundColour = theme.scaleColour;
            scale.backgroundHoverColour = theme.scaleHoverColour;
            scale.backgroundPressedColour = theme.scalePressedColour;

            scale.UpdateDisplay();
        }

        UIScrollbar scrollbar = GetComponent<UIScrollbar>();
        if (scrollbar)
        {
            scrollbar.handleColour = theme.scrollbarHandleColour;
            scrollbar.handleHoverColour = theme.scrollbarHandleHoverColour;
            scrollbar.handlePressedColour = theme.scrollbarHandlePressedColour;

            scrollbar.backgroundColour = theme.scrollbarBackgroundColour;
            scrollbar.backgroundHoverColour = theme.scrollbarBackgroundHoverColour;
            scrollbar.backgroundPressedColour = theme.scrollbarBackgroundPressedColour;

            scrollbar.UpdateDisplay();
        }

        LayerTile layerTile = GetComponent<LayerTile>();
        if (layerTile)
        {
            layerTile.tileToggle.offBackgroundColour = theme.layerTileOffColour;
            layerTile.tileToggle.onBackgroundColour = theme.layerTileOnColour;
            layerTile.tileToggle.hoverBackgroundTint = theme.layerTileHoverTint;
            layerTile.tileToggle.pressedBackgroundColour = theme.layerTilePressedColour;

            layerTile.tileToggle.UpdateDisplay();
        }
    }
}

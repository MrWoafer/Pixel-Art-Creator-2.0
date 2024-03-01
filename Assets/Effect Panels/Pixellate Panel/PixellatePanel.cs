using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A panel that pixellates the view behind it.
/// </summary>
[RequireComponent(typeof(EffectPanel))]
[AddComponentMenu("Effect Panels/Pixellate Panel")]
public class PixellatePanel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private bool _pixellateOn = false;
    public bool pixellateOn
    {
        get
        {
            return _pixellateOn;
        }
        private set
        {
            _pixellateOn = value;
        }
    }
    [SerializeField]
    [Min(float.Epsilon)]
    private float pixels = 32f;

    private EffectPanel effectPanel;
    private SpriteRenderer sprRen;

    private bool beenRunningForAFrame = false;

    private void Awake()
    {
        effectPanel = GetComponent<EffectPanel>();
        sprRen = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        SetSettings();
        EnableDisable(pixellateOn);
    }

    private void Update()
    {
        if (!beenRunningForAFrame)
        {
            beenRunningForAFrame = true;
        }
    }

    private void OnValidate()
    {
        if (beenRunningForAFrame && Application.isPlaying)
        {
            SetSettings();
            EnableDisable(pixellateOn);
        }
    }

    private void EnableDisable(bool enabled)
    {
        pixellateOn = enabled;
        effectPanel.EnableDisable(enabled);
    }

    private void SetSettings()
    {
        sprRen.material.SetFloat("_Pixels", pixels);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EffectPanel), typeof(SpriteRenderer))]
[AddComponentMenu("Effect Panels/Blur Panel")]
public class BlurPanel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private bool _blurEnabled = false;
    public bool blurEnabled
    {
        get
        {
            return _blurEnabled;
        }
        private set
        {
            _blurEnabled = value;
        }
    }
    [SerializeField]
    [Min(0f)]
    private float blurAmount = 3f;

    private EffectPanel effectPanel;
    private SpriteRenderer sprRen;
    private Collider2D collider;

    private bool beenRunningForAFrame = false;

    private void Awake()
    {
        effectPanel = GetComponent<EffectPanel>();
        sprRen = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
    }

    void Start()
    {
        SetSettings();
        EnableDisable(blurEnabled);
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
            EnableDisable(blurEnabled);
        }
    }

    public void EnableDisable(bool enabled)
    {
        blurEnabled = enabled;
        effectPanel.EnableDisable(enabled);
        sprRen.enabled = enabled;

        if (collider)
        {
            collider.enabled = enabled;
        }
    }

    private void SetSettings()
    {
        sprRen.material.SetFloat("_Blur_Amount", blurAmount / 1000f);
    }
}

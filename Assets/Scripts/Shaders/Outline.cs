using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private bool _outlineEnabled = true;
    public bool outlineEnabled
    {
        get => _outlineEnabled;

        set
        {
            _outlineEnabled = value;
            if (renderer == null)
            {
                GetReferences();
            }
            if (renderer.sharedMaterial != null)
            {
                renderer.sharedMaterial.SetInt("_Enabled", value ? 1 : 0);
            }
        }
    }
    [SerializeField]
    private Color _colour = Color.black;
    public Color colour
    {
        get => _colour;

        set
        {
            _colour = value;
            if (renderer == null)
            {
                GetReferences();
            }
            if (renderer.sharedMaterial != null)
            {
                renderer.sharedMaterial.SetColor("_Colour", value);
            }
        }
    }
    [SerializeField]
    [Min(0f)]
    private float _thickness = 0.02f;
    public float thickness
    {
        get => _thickness;

        set
        {
            _thickness = value;
            if (renderer == null)
            {
                GetReferences();
            }
            if (renderer.sharedMaterial != null)
            {
                renderer.sharedMaterial.SetFloat("_Thickness", value);
            }
        }
    }
    [SerializeField]
    private bool _keepExistingTexture = true;
    public bool keepExistingTexture
    {
        get => _keepExistingTexture;

        set
        {
            _keepExistingTexture = value;
            if (renderer == null)
            {
                GetReferences();
            }
            if (renderer.sharedMaterial != null)
            {
                renderer.sharedMaterial.SetInt("_Keep_Existing_Texture", value ? 1 : 0);
            }
        }
    }

    [Header("References")]
    public Material outlineMaterial;

    private Renderer renderer;

    private bool madeNewMaterial = false;

    private void Awake()
    {
        GetReferences();
        CreateOutline();
    }

    private void OnValidate()
    {
        GetReferences();
        CreateOutline();
    }

    private void GetReferences()
    {
        renderer = gameObject.GetComponent<Renderer>();
    }

    private void CreateOutline()
    {
        if (!madeNewMaterial)
        {
            renderer.material = new Material(outlineMaterial);
            madeNewMaterial = true;
        }
        outlineEnabled = _outlineEnabled;
        colour = _colour;
        thickness = _thickness;
        keepExistingTexture = _keepExistingTexture;
    }
}

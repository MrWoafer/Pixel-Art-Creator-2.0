using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[AddComponentMenu("Effect Panels/Effect Panel")]
public class EffectPanel : MonoBehaviour
{
    [Header("Graphical Settings")]
    [SerializeField]
    [Min(0f)]
    private float resolution = 1f;

    private const float resolutionScaler = 100f;
    private const int depth = 16;

    private Camera renderCamera;
    private RenderTexture renderTexture;
    private SpriteRenderer sprRen;

    private bool beenRunningForAFrame = false;

    private void Awake()
    {
        renderCamera = transform.Find("Render Camera").GetComponent<Camera>();
        sprRen = GetComponent<SpriteRenderer>();

        SetUpRenderTexture(resolution);
        SetUpCamera();
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
            SetUpRenderTexture(resolution);
            SetUpCamera();
        }
    }

    public void EnableDisable(bool enabled)
    {
        renderCamera.enabled = enabled;
        sprRen.enabled = enabled;
    }

    private void SetUpRenderTexture(float resolution)
    {
        renderTexture = new RenderTexture((int)(transform.lossyScale.x * resolution * resolutionScaler), (int)(transform.lossyScale.y * resolution * resolutionScaler), depth);
        renderTexture.name = "Blur Panel Render Texture";
        sprRen.material.SetTexture("_Render_Texture", renderTexture);
    }

    private void SetUpCamera()
    {
        renderCamera.targetTexture = renderTexture;
        renderCamera.orthographic = true;
        renderCamera.orthographicSize = transform.lossyScale.y / 2f;
        renderCamera.transform.localPosition = new Vector3(0f, 0f, 0f);
    }
}

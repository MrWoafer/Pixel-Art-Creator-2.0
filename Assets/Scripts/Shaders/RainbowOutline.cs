using UnityEngine;

namespace PAC.Shaders
{
    public class RainbowOutline : MonoBehaviour
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
        public Material rainbowOutlineMaterial;

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
                renderer.material = new Material(rainbowOutlineMaterial);
                madeNewMaterial = true;
            }
            outlineEnabled = _outlineEnabled;
            thickness = _thickness;
            keepExistingTexture = _keepExistingTexture;
        }
    }
}

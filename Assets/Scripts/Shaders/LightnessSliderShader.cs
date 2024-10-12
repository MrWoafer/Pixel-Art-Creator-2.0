using UnityEngine;

namespace PAC.Shaders
{
    public class LightnessSliderShader : MonoBehaviour
    {
        [Header("References")]
        public Material lightnessSliderMaterial;

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
                renderer.material = new Material(lightnessSliderMaterial);
                madeNewMaterial = true;
            }
        }
    }
}

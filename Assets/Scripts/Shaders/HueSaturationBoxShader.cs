using UnityEngine;

namespace PAC.Shaders
{
    public class HueSaturationBoxShader : MonoBehaviour
    {
        [Header("References")]
        public Material hueSaturationBoxMaterial;

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
                renderer.material = new Material(hueSaturationBoxMaterial);
                madeNewMaterial = true;
            }
        }
    }
}

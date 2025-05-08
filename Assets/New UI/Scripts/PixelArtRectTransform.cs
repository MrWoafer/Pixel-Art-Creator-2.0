using PAC.Extensions.UnityEngine;
using PAC.Maths;

using UnityEngine;
using TMPro;

namespace PAC.NewUI
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("PAC/UI/Pixel Art Rect Transform")]
    public class PixelArtRectTransform : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        [Tooltip("The position relative to the RectTransform's anchors, in asset pixels (not screen pixels).")]
        private Vector2Int _anchoredPosition;
        /// <summary>
        /// The position relative to the <see cref="RectTransform"/>'s anchors, in pixels (pixels in the assets, not on the screen).
        /// </summary>
        public Vector2Int anchoredPosition
        {
            get => _anchoredPosition;
            set
            {
                _anchoredPosition = value;
                Snap();
            }
        }

        [SerializeField]
        [Tooltip("The size delta, in asset pixels (not screen pixels).")]
        private Vector2Int _sizeDelta;
        /// <summary>
        /// The size delta, in pixels (pixels in the assets, not on the screen).
        /// </summary>
        public Vector2Int sizeDelta
        {
            get => _sizeDelta;
            set
            {
                _sizeDelta = value;
                Snap();
            }
        }

        #endregion

        #region References

        private RectTransform rectTransform;

        private Canvas parentCanvas;
        private PixelArtRectTransform parentPixelArtRectTransform;

        #endregion

        #region Methods

        private void OnEnable()
        {
            GetReferences();
        }

        private void OnTransformParentChanged()
        {
            GetReferences();
        }

        private void GetReferences()
        {
            rectTransform = GetComponent<RectTransform>();

            rectTransform.parent.TryGetComponent(out parentCanvas);
            rectTransform.parent.TryGetComponent(out parentPixelArtRectTransform);
        }

        private void Update()
        {
            Snap();
        }

        private void Snap()
        {
            if (!enabled || rectTransform is null)
            {
                return;
            }

            rectTransform.anchoredPosition = _anchoredPosition;
            rectTransform.sizeDelta = _sizeDelta;

            // Adjust to fit pixel art grid

            if (parentCanvas is not null)
            {
                (float x, float y, _) = rectTransform.TransformPoint(rectTransform.rect.min);
                Vector3 offset = new Vector3(
                    x.RoundHalfUp(parentCanvas.scaleFactor),
                    parentCanvas.pixelRect.height - (parentCanvas.pixelRect.height - y).RoundHalfUp(parentCanvas.scaleFactor), // Treat top edge as y = 0 instead of bottom edge, just 'cos I like the rounding better
                    0f
                    )
                    - new Vector3(x, y, 0f);

                rectTransform.position += offset;
            }
            else if (parentPixelArtRectTransform is not null)
            {
                Vector2 minLocal = TryGetComponent(out TextMeshProUGUI tmpro) ? tmpro.textBounds.min : rectTransform.rect.min;
                Vector2 minLocalToParent = rectTransform.InverseTransformPoint(rectTransform.TransformPoint(minLocal));
                Vector3 offset = minLocalToParent - rectTransform.parent.GetComponent<RectTransform>().rect.min;
                Vector3 roundedOffset = new Vector3(offset.x.RoundHalfUp(), offset.y.RoundHalfUp(), offset.z);

                rectTransform.localPosition += roundedOffset - offset;
            }
        }

        #endregion
    }
}

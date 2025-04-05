using PAC.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PAC.UI
{
    public enum ViewportSide
    {
        Negative = -1,
        Centre = 0,
        Positive = 1
    }

    [RequireComponent(typeof(Mask)), RequireComponent(typeof(Image))]
    [AddComponentMenu("Custom UI/UI Viewport")]
    public class UIViewport : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float _scrollAmount = 0f;
        public float scrollAmount
        {
            get
            {
                return _scrollAmount;
            }
            private set
            {
                _scrollAmount = value;
            }
        }
        public ScrollDirection scrollDirection = ScrollDirection.Vertical;
        public bool maxScrollEnabled = true;
        public float maxScrollAmount = 1f;
        public bool minScrollEnabled = true;
        public float minScrollAmount = -1f;
        [Tooltip("When enabled, the max/min amount you can scroll will be such that the top/bottom object are at the top/bottom of the scrollable area.")]
        public bool boundScrollToContents = false;
        [Tooltip("If bound to scroll contents is enabled and the objects don't take up the whole viewport, this is the side the objects will be displayed on.")]
        [SerializeField]
        private ViewportSide _defaultScrollSide = ViewportSide.Positive;
        public ViewportSide defaultScrollSide
        {
            get
            {
                return _defaultScrollSide;
            }
        }

        [Header("Display")]
        [SerializeField]
        private bool displayBackground = false;

        public Transform scrollingArea { get; private set; }

        public RectTransform rectTransform { get; private set; }
        public BoxCollider2D collider { get; private set; }
        private Image background;

        private UnityEvent onRefresh = new UnityEvent();

        private void Awake()
        {
            GetReferences();
        }

        private void GetReferences()
        {
            collider = GetComponent<BoxCollider2D>();
            rectTransform = GetComponent<RectTransform>();
            scrollingArea = transform.Find("Scrolling Area");
            background = transform.Find("Background").GetComponent<Image>();
        }

        private void OnValidate()
        {
            GetReferences();
            SetScrollAmount(scrollAmount);
            collider.size = rectTransform.sizeDelta;
            background.enabled = displayBackground;
        }

        public void RefreshViewport()
        {
            foreach (Transform child in transform)
            {
                InputTarget inputTarget = child.GetComponent<InputTarget>();
                if (inputTarget)
                {
                    inputTarget.viewport = this;
                }

                UIElement uiElement = child.GetComponent<UIElement>();
                if (uiElement)
                {
                    uiElement.viewport = this;
                }
            }

            foreach (Transform child in scrollingArea.transform)
            {
                InputTarget inputTarget = child.GetComponent<InputTarget>();
                if (inputTarget)
                {
                    inputTarget.viewport = this;
                }

                UIElement uiElement = child.GetComponent<UIElement>();
                if (uiElement)
                {
                    uiElement.viewport = this;
                }
            }

            SetScrollAmount(scrollAmount);

            onRefresh.Invoke();
        }

        public void AddScrollAmount(float scrollAmount)
        {
            SetScrollAmount(this.scrollAmount + scrollAmount);
        }
        public void SetScrollAmount(float scrollAmount)
        {
            if (!Application.isPlaying)
            {
                GetReferences();
            }

            if (boundScrollToContents)
            {
                scrollAmount = Mathf.Clamp(scrollAmount, -1f, 1f);
            }
            else
            {
                scrollAmount = Mathf.Clamp(scrollAmount, minScrollEnabled ? minScrollAmount : float.MinValue, maxScrollEnabled ? maxScrollAmount : float.MaxValue);
            }

            if (scrollingArea != null)
            {
                if (boundScrollToContents)
                {
                    if (scrollDirection == ScrollDirection.Vertical)
                    {
                        float maxY, minY;
                        Vector2 minMaxY = GetScrollMinMaxY();
                        minY = minMaxY.x;
                        maxY = minMaxY.y;

                        if (maxY - minY < rectTransform.sizeDelta.y)
                        {
                            if (defaultScrollSide == ViewportSide.Positive)
                            {
                                scrollingArea.transform.localPosition = Vector3.up * (rectTransform.sizeDelta.y / 2f - maxY);
                            }
                            else if (defaultScrollSide == ViewportSide.Negative)
                            {
                                scrollingArea.transform.localPosition = Vector3.up * (-rectTransform.sizeDelta.y / 2f - minY);
                            }
                            else
                            {
                                scrollingArea.transform.localPosition = Vector3.up * (minY + maxY) / 2f;
                            }
                        }
                        else
                        {
                            scrollingArea.transform.localPosition = Vector3.Lerp(Vector3.up * (-rectTransform.sizeDelta.y / 2f - minY),
                                Vector3.up * (rectTransform.sizeDelta.y / 2f - maxY), (scrollAmount + 1f) / 2f);
                        }
                    }
                    else
                    {
                        float maxX, minX;
                        Vector2 minMaxX = GetScrollMinMaxX();
                        minX = minMaxX.x;
                        maxX = minMaxX.y;

                        if (maxX - minX < rectTransform.sizeDelta.x)
                        {
                            if (defaultScrollSide == ViewportSide.Positive)
                            {
                                scrollingArea.transform.localPosition = Vector3.right * (rectTransform.sizeDelta.x / 2f - maxX);
                            }
                            else if (defaultScrollSide == ViewportSide.Negative)
                            {
                                scrollingArea.transform.localPosition = Vector3.right * (-rectTransform.sizeDelta.x / 2f - minX);
                            }
                            else
                            {
                                scrollingArea.transform.localPosition = Vector3.right * (minX + maxX) / 2f;
                            }
                        }
                        else
                        {
                            scrollingArea.transform.localPosition = Vector3.Lerp(Vector3.right * (-rectTransform.sizeDelta.x / 2f - minX),
                                Vector3.right * (rectTransform.sizeDelta.x / 2f - maxX), (scrollAmount + 1f) / 2f);
                        }
                    }
                }
                else
                {
                    scrollingArea.transform.localPosition = (scrollDirection == ScrollDirection.Vertical ? Vector3.up : Vector3.right) * scrollAmount;
                }

                this.scrollAmount = scrollAmount;
            }
        }

        /// <summary>
        /// Get the min and max y value of the space that the objects in this viewport's scroll area occupy. (Scaled to the scale of the viewport object.)
        /// </summary>
        /// <returns>(min y, max y)</returns>
        public Vector2 GetScrollMinMaxY()
        {
            float maxY;
            float minY;
            if (scrollingArea.childCount == 0)
            {
                maxY = 0f;
                minY = 0f;
            }
            else
            {
                maxY = float.MinValue;
                minY = float.MaxValue;
            }

            foreach (Transform child in scrollingArea.transform)
            {
                /// So it doesn't bound to stuff that's in an inactive tab for example
                if (child.transform.position.sqrMagnitude >= 10000)
                {
                    continue;
                }

                Vector3 position = Vector3.Scale(child.transform.localPosition, scrollingArea.transform.localScale);

                Vector3 size = Vector3.Scale(child.transform.localScale, scrollingArea.transform.localScale);

                RectTransform rectTransform = child.GetComponent<RectTransform>();
                if (rectTransform)
                {
                    size = Vector3.Scale(Vector3.Scale(rectTransform.sizeDelta, child.localScale), scrollingArea.transform.localScale);
                }

                UIButton button = child.GetComponent<UIButton>();
                if (button)
                {
                    size = Vector3.Scale(Vector3.Scale(child.localScale, new Vector3(1f, button.height, 1f)), scrollingArea.transform.localScale);
                }

                UIToggleButton toggleButton = child.GetComponent<UIToggleButton>();
                if (toggleButton)
                {
                    size = Vector3.Scale(Vector3.Scale(child.localScale, new Vector3(1f, toggleButton.height, 1f)), scrollingArea.transform.localScale);
                }

                if (position.y + size.y / 2f > maxY)
                {
                    maxY = position.y + size.y / 2f;
                }
                if (position.y - size.y / 2f < minY)
                {
                    minY = position.y - size.y / 2f;
                }
            }

            return new Vector2(minY, maxY);
        }

        /// <summary>
        /// Get the min and max x value of the space that the objects in this viewport's scroll area occupy. (Scaled to the scale of the viewport object.)
        /// </summary>
        /// <returns>(min x, max x)</returns>
        public Vector2 GetScrollMinMaxX()
        {
            float maxX;
            float minX;
            if (scrollingArea.childCount == 0)
            {
                maxX = 0f;
                minX = 0f;
            }
            else
            {
                maxX = float.MinValue;
                minX = float.MaxValue;
            }

            foreach (Transform child in scrollingArea.transform)
            {
                /// So it doesn't bound to stuff that's in an inactive tab for example
                if (child.transform.position.sqrMagnitude >= 10000)
                {
                    continue;
                }

                Vector3 position = Vector3.Scale(child.transform.localPosition, scrollingArea.transform.localScale);

                Vector3 size = Vector3.Scale(child.transform.localScale, scrollingArea.transform.localScale);

                RectTransform rectTransform = child.GetComponent<RectTransform>();
                if (rectTransform)
                {
                    size = Vector3.Scale(Vector3.Scale(rectTransform.sizeDelta, child.localScale), scrollingArea.transform.localScale);
                }

                UIButton button = child.GetComponent<UIButton>();
                if (button)
                {
                    size = Vector3.Scale(Vector3.Scale(child.localScale, new Vector3(button.width, 1f, 1f)), scrollingArea.transform.localScale);
                }

                UIToggleButton toggleButton = child.GetComponent<UIToggleButton>();
                if (toggleButton)
                {
                    size = Vector3.Scale(Vector3.Scale(child.localScale, new Vector3(toggleButton.width, 1f, 1f)), scrollingArea.transform.localScale);
                }

                if (position.x + size.x / 2f > maxX)
                {
                    maxX = position.x + size.x / 2f;
                }
                if (position.x - size.x / 2f < minX)
                {
                    minX = position.x - size.x / 2f;
                }
            }

            return new Vector2(minX, maxX);
        }

        public void SubscribeToRefresh(UnityAction call)
        {
            onRefresh.AddListener(call);
        }
    }
}
using UnityEngine;

namespace PAC.UI
{
    public enum CollapsedState
    {
        Uncollapsed = 0,
        Collapsed = 1
    }

    [AddComponentMenu("Custom UI/UI Collapser")]
    public class UICollapser : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private CollapsedState collapsedState = CollapsedState.Uncollapsed;
        [SerializeField]
        private GameObject uncollapsedObject;
        [SerializeField]
        private GameObject collapsedObject;

        private Vector3 uncollapsedPos;
        private Vector3 collapsedPos;
        private Vector3 idlePos = new Vector3(-10000f, 0f, 0f);

        private bool beenRunningAFrame = false;

        private void Awake()
        {
            uncollapsedObject.SetActive(true);
            collapsedObject.SetActive(true);

            uncollapsedPos = uncollapsedObject.transform.localPosition;
            collapsedPos = collapsedObject.transform.localPosition;

            UpdatePositions();
        }

        private void Update()
        {
            if (!beenRunningAFrame)
            {
                beenRunningAFrame = true;
            }
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                SetCollapsedEditor(collapsedState);
            }
            else if (beenRunningAFrame)
            {
                if (collapsedState == CollapsedState.Uncollapsed)
                {
                    collapsedState = CollapsedState.Collapsed;
                    SetCollapsed(CollapsedState.Uncollapsed);
                }
                else
                {
                    collapsedState = CollapsedState.Uncollapsed;
                    SetCollapsed(CollapsedState.Collapsed);
                }
            }
        }

        public void Collapse()
        {
            SetCollapsed(CollapsedState.Collapsed);
        }
        public void Uncollapse()
        {
            SetCollapsed(CollapsedState.Uncollapsed);
        }
        public void SetCollapsed(CollapsedState collapsedState)
        {
            if (collapsedState != this.collapsedState)
            {
                this.collapsedState = collapsedState;

                if (collapsedState == CollapsedState.Uncollapsed)
                {
                    collapsedPos = collapsedObject.transform.localPosition;
                }
                else
                {
                    uncollapsedPos = uncollapsedObject.transform.localPosition;
                }

                UpdatePositions();
            }
        }

        private void UpdatePositions()
        {
            if (collapsedState == CollapsedState.Uncollapsed)
            {
                uncollapsedObject.transform.localPosition = uncollapsedPos;
                collapsedObject.transform.position = idlePos;
            }
            else
            {
                collapsedObject.transform.localPosition = collapsedPos;
                uncollapsedObject.transform.position = idlePos;
            }
        }

        private void SetCollapsedEditor(CollapsedState collapsedState)
        {
            this.collapsedState = collapsedState;

            if (collapsedState == CollapsedState.Uncollapsed)
            {
                uncollapsedObject.SetActive(true);
                collapsedObject.SetActive(false);
            }
            else
            {
                uncollapsedObject.SetActive(false);
                collapsedObject.SetActive(true);
            }
        }
    }
}
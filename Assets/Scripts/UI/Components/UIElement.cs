using PAC.Input;
using UnityEngine;

namespace PAC.UI
{
    public enum UIElementSelectMode
    {
        Manual = 0,
        OnInputTargetTargeted = 1
    }

    public enum UIElementDeselectMode
    {
        Manual = 0,
        OnInputTargetUntargeted = 1
    }

    public class UIElement : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private string _elementName = "";
        public string elementName
        {
            get
            {
                return _elementName;
            }
        }
        [SerializeField]
        private UIElementSelectMode selectMode = UIElementSelectMode.Manual;
        [SerializeField]
        private UIElementDeselectMode deselectMode = UIElementDeselectMode.Manual;

        [Header("Viewport")]
        public UIViewport viewport;

        private UIManager uiManager;

        private void Awake()
        {
            uiManager = Finder.uiManager;
        }

        private void Start()
        {
            if (selectMode == UIElementSelectMode.OnInputTargetTargeted)
            {
                InputTarget inputTarget = GetComponent<InputTarget>();
                if (!inputTarget)
                {
                    throw new System.Exception("To use the OnInputTargetTargeted UI Element select mode, the object must have an InputTarget.");
                }
                else
                {
                    inputTarget.SubscribeToTarget(() => uiManager.TryTarget(this));
                }
            }

            if (deselectMode == UIElementDeselectMode.OnInputTargetUntargeted)
            {
                InputTarget inputTarget = GetComponent<InputTarget>();
                if (!inputTarget)
                {
                    throw new System.Exception("To use the OnInputTargetUntargeted UI Element deselect mode, the object must have an InputTarget.");
                }
                else
                {
                    inputTarget.SubscribeToUntarget(() => uiManager.TryUntarget(this));
                }
            }
        }
    }
}
using PAC.UI.Components.General;
using PAC.Input;
using PAC.UI.Components.General.Tooltip;

using UnityEngine;
using UnityEngine.Events;
using PAC.Managers;
using PAC.Image.Layers;

namespace PAC.UI.Components.Specialised
{
    public class LayerTile : MonoBehaviour
    {
        public Layer layer { get; private set; }
        public bool selected
        {
            get
            {
                return tileToggle.on;
            }
        }

        public UIToggleButton tileToggle { get; private set; }
        private UIToggleButton visibleToggle;
        private UIToggleButton lockedToggle;
        private UITextbox nameTextbox;
        private UnityEngine.UI.Image layerTypeImage;

        private InputTarget visibilityInputTarget;
        private LayerManager layerManager;

        private UnityEvent onVisibilityChange = new UnityEvent();
        private UnityEvent onLockChange = new UnityEvent();
        private UnityEvent onNameChange = new UnityEvent();
        private UnityEvent onRightClick = new UnityEvent();

        private void Awake()
        {
            tileToggle = GetComponent<UIToggleButton>();
            visibleToggle = transform.Find("Visible").GetComponent<UIToggleButton>();
            lockedToggle = transform.Find("Locked").GetComponent<UIToggleButton>();
            nameTextbox = transform.Find("Name").GetComponent<UITextbox>();
            layerTypeImage = transform.Find("Canvas").Find("Layer Type").GetComponent<UnityEngine.UI.Image>();

            visibilityInputTarget = transform.Find("Visible").GetComponent<InputTarget>();
            layerManager = Finder.layerManager;
        }

        // Start is called before the first frame update
        void Start()
        {
            visibleToggle.SubscribeToLeftClick(OnVisibilityChange);
            lockedToggle.SubscribeToLeftClick(OnLockChange);
            nameTextbox.SubscribeToFinishEvent(OnNameChange);
            tileToggle.SubscribeToRightClick(() => { onRightClick.Invoke(); });
        }

        public void SetLayer(Layer layer)
        {
            this.layer = layer;
            visibleToggle.SetOnOff(!layer.visible);
            lockedToggle.SetOnOff(layer.locked);
            nameTextbox.SetText(layer.name);

            if (layer.layerType == LayerType.Normal)
            {
                layerTypeImage.sprite = layerManager.layerTypeSprite;
                layerTypeImage.GetComponent<Tooltip>().text = "Normal Layer";
            }
            else if (layer.layerType == LayerType.Tile)
            {
                layerTypeImage.sprite = layerManager.tileLayerTypeSprite;
                layerTypeImage.GetComponent<Tooltip>().text = "Tile Layer";
            }
            else
            {
                throw new System.Exception("Unknown / unimplemented layer type: " + layer.GetType());
            }
        }

        private void OnVisibilityChange()
        {
            if (visibilityInputTarget.keyboardTarget.IsHeldExactly(KeyCode.LeftControl) || visibilityInputTarget.keyboardTarget.IsHeldExactly(KeyCode.RightControl))
            {
                bool wasVisible = layer.visible;

                layerManager.HideAllBut(layer);

                if (wasVisible == layer.visible)
                {
                    onVisibilityChange.Invoke();
                }
            }
            else
            {
                layer.visible = !visibleToggle.on;
                onVisibilityChange.Invoke();
            }
        }
        private void OnLockChange()
        {
            layer.locked = lockedToggle.on;
            onLockChange.Invoke();
        }
        private void OnNameChange()
        {
            layer.name = nameTextbox.text;
            onNameChange.Invoke();
        }

        public void SubscribeToVisibilityChange(UnityAction call)
        {
            onVisibilityChange.AddListener(call);
        }
        public void SubscribeToLockChange(UnityAction call)
        {
            onLockChange.AddListener(call);
        }
        public void SubscribeToNameChange(UnityAction call)
        {
            onNameChange.AddListener(call);
        }
        public void SubscribeToRightClick(UnityAction call)
        {
            onRightClick.AddListener(call);
        }
    }
}

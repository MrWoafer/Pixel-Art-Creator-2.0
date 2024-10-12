using System.Collections;
using PAC.ColourPicker;
using PAC.Drawing;
using PAC.Input;
using PAC.Screen;
using UnityEngine;
using UnityEngine.Events;

namespace PAC.UI
{
    [AddComponentMenu("Custom UI/UI Colour Picker")]
    public class UIColourPicker : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private Color startingColour = Color.red;
        private bool colourSet = false;

        public Color colour => colourPreview ? colourPreview.colour : hslColourPicker.color;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onColourChanged = new UnityEvent();
        [SerializeField]
        private UnityEvent onClose = new UnityEvent();

        [Header("References")]
        [HideInInspector]
        public ColourPreview colourPreview;
        private ColourPreview[] selectedColoursPreviews = new ColourPreview[0];
        private HSLColourPicker hslColourPicker;
        private GlobalColourPicker globalColourPicker;

        private Toolbar toolbar;

        private UIScale rScale;
        private UIScale gScale;
        private UIScale bScale;
        private UIScale aScale;

        private InputSystem inputSystem;

        private bool usingGlobalEyeDropper = false;
        private bool selectedGlobalEyeDropperThisFrame = false;

        private void Awake()
        {
            try
            {
                colourPreview = transform.Find("Canvas").Find("Colour Preview").GetComponent<ColourPreview>();
            }
            catch { }

            try
            {
                selectedColoursPreviews = transform.Find("Canvas").Find("Selected Colours").GetComponentsInChildren<ColourPreview>();
            }
            catch { }

            hslColourPicker = transform.Find("Canvas").Find("HSL Colour Picker").GetComponent<HSLColourPicker>();
            globalColourPicker = Finder.colourPicker;

            toolbar = Finder.toolbar;

            rScale = transform.Find("Canvas").Find("RGBA Scales").Find("R Scale").Find("Scale").GetComponent<UIScale>();
            gScale = transform.Find("Canvas").Find("RGBA Scales").Find("G Scale").Find("Scale").GetComponent<UIScale>();
            bScale = transform.Find("Canvas").Find("RGBA Scales").Find("B Scale").Find("Scale").GetComponent<UIScale>();
            aScale = transform.Find("Canvas").Find("RGBA Scales").Find("A Scale").Find("Scale").GetComponent<UIScale>();

            inputSystem = Finder.inputSystem;

            hslColourPicker.SubscribeToOnColourChange(() => UpdateColour(hslColourPicker.hsl.color));
        }

        // Start is called before the first frame update
        void Start()
        {
            rScale.SubscribeToValueChange(ColourChangedByScales);
            gScale.SubscribeToValueChange(ColourChangedByScales);
            bScale.SubscribeToValueChange(ColourChangedByScales);
            aScale.SubscribeToValueChange(ColourChangedByScales);

            inputSystem.SubscribeToGlobalLeftClick(OnGlobalLeftClick);

            foreach (ColourPreview selectedColourPreview in selectedColoursPreviews)
            {
                selectedColourPreview.SubscribeToOnToggle(() => { SetColour(selectedColourPreview.colour); });
            }

            for (int i = 0; i < selectedColoursPreviews.Length && i < globalColourPicker.numOfColourPreviews; i++)
            {
                int j = i;
                globalColourPicker.SubscribeToOnColourChange(() => { selectedColoursPreviews[j].SetColour(globalColourPicker.GetColour(j)); });
            }

            if (!colourSet)
            {
                SetColour(startingColour);
            }
        }

        private void Update()
        {
            if (selectedGlobalEyeDropperThisFrame)
            {
                selectedGlobalEyeDropperThisFrame = false;
            }
        }

        public void UpdateColour(Color colour)
        {
            Color oldColour = this.colour;

            colourPreview?.SetColour(colour);

            rScale.SetValueNoNotify(colour.r * 255f);
            gScale.SetValueNoNotify(colour.g * 255f);
            bScale.SetValueNoNotify(colour.b * 255f);
            aScale.SetValueNoNotify(colour.a * 255f);

            if (oldColour != colour)
            {
                onColourChanged.Invoke();
            }
        }

        public void SetColour(Color colour)
        {
            colourSet = true;
            hslColourPicker.SetColour(colour);
        }

        private void ColourChangedByScales()
        {
            SetColour(new Color(rScale.value / 255f, gScale.value / 255f, bScale.value / 255f, aScale.value / 255f));
        }

        public void Close()
        {
            onClose.Invoke();
        }

        public void SelectDeselectEyeDropper()
        {
            if (toolbar.selectedTool == Tool.GlobalEyeDropper)
            {
                usingGlobalEyeDropper = false;
                toolbar.DeselectGlobalEyeDropper();
            }
            else
            {
                usingGlobalEyeDropper = true;
                selectedGlobalEyeDropperThisFrame = true;
                inputSystem.Untarget();
                toolbar.SelectGlobalEyeDropper();
            }
        }

        private void OnGlobalLeftClick()
        {
            if (toolbar.selectedTool == Tool.GlobalEyeDropper && usingGlobalEyeDropper && !selectedGlobalEyeDropperThisFrame)
            {
                usingGlobalEyeDropper = false;
                toolbar.DeselectGlobalEyeDropper();
                StartCoroutine(UseGlobalEyeDropper());
            }
        }

        private IEnumerator UseGlobalEyeDropper()
        {
            yield return new WaitForEndOfFrame();

            SetColour(ScreenInfo.GetScreenPixelColour((int)UnityEngine.Input.mousePosition.x, (int)UnityEngine.Input.mousePosition.y));
        }

        public void SubscribeToColourChange(UnityAction call)
        {
            onColourChanged.AddListener(call);
        }

        public void SubscribeToClose(UnityAction call)
        {
            onClose.AddListener(call);
        }
    }
}

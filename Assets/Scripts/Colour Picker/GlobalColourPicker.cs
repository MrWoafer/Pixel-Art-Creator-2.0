using PAC.Drawing;
using PAC.Input;
using PAC.KeyboardShortcuts;
using PAC.UI;
using UnityEngine;
using UnityEngine.Events;

namespace PAC.ColourPicker
{
    /// <summary>
    /// A class for the main colour picker in the program - the one that appears in the main view.
    /// </summary>
    [RequireComponent(typeof(UIColourPicker))]
    public class GlobalColourPicker : MonoBehaviour
    {
        /// <summary>The currenty-selected colour.</summary>
        public Color colour => uiColourPicker.colour;

        /// <summary>The current primary colour.</summary>
        public Color primaryColour => colourPreviews[0].colour;
        /// <summary>The current secondary colour.</summary>
        public Color secondaryColour => colourPreviews[1].colour;

        [Header("Events")]
        [SerializeField]
        /// <summary>Called when the selected colour changes.</summary>
        private UnityEvent onColourChanged = new UnityEvent();

        [Header("References")]
        private ColourPreview[] colourPreviews;
        private int currentColourPreviewIndex;
        private ColourPreview currentColourPreview => colourPreviews[currentColourPreviewIndex];
        private UIToggleGroup colourPreviewToggleGroup;
        private UIColourPicker uiColourPicker;

        public int numOfColourPreviews => colourPreviews.Length;

        private UIScale rScale;
        private UIScale gScale;
        private UIScale bScale;
        private UIScale aScale;

        private InputSystem inputSystem;
        private Toolbar toolbar;

        private void Awake()
        {
            colourPreviewToggleGroup = transform.Find("Canvas").Find("Colour Preview Toggle Group").GetComponent<UIToggleGroup>();
            colourPreviews = colourPreviewToggleGroup.GetComponentsInChildren<ColourPreview>();
            uiColourPicker = GetComponent<UIColourPicker>();

            rScale = transform.Find("Canvas").Find("RGBA Scales").Find("R Scale").Find("Scale").GetComponent<UIScale>();
            gScale = transform.Find("Canvas").Find("RGBA Scales").Find("G Scale").Find("Scale").GetComponent<UIScale>();
            bScale = transform.Find("Canvas").Find("RGBA Scales").Find("B Scale").Find("Scale").GetComponent<UIScale>();
            aScale = transform.Find("Canvas").Find("RGBA Scales").Find("A Scale").Find("Scale").GetComponent<UIScale>();

            inputSystem = Finder.inputSystem;
            toolbar = Finder.toolbar;
        }

        private void Start()
        {
            rScale.SubscribeToValueChange(OnColourChangedByScales);
            gScale.SubscribeToValueChange(OnColourChangedByScales);
            bScale.SubscribeToValueChange(OnColourChangedByScales);
            aScale.SubscribeToValueChange(OnColourChangedByScales);

            foreach (ColourPreview colourPreview in colourPreviews)
            {
                colourPreview.SubscribeToOnSelect(OnSelectedColourPreview);
            }

            inputSystem.SubscribeToGlobalKeyboard(CheckKeyboardShortcuts);

            uiColourPicker.SubscribeToColourChange(() => { onColourChanged.Invoke(); });

            colourPreviews[0].SetColour(Preferences.startupPrimaryColour);
            colourPreviews[1].SetColour(Preferences.startupSecondaryColour);
            SelectColourPreview(0);
        }

        /// <summary>
        /// Sets the current colour.
        /// </summary>
        public void SetColour(Color colour)
        {
            uiColourPicker.SetColour(colour);
        }

        /// <summary>
        /// Gets the chosen colour at the given index: 0 - primary; 1 - secondary.
        /// </summary>
        public Color GetColour(int colourPreviewIndex)
        {
            if (colourPreviewIndex < 0 || colourPreviewIndex >= colourPreviews.Length)
            {
                throw new System.IndexOutOfRangeException("colourPreviewIndex out of range: " + colourPreviewIndex);
            }

            return colourPreviews[colourPreviewIndex].colour;
        }

        /// <summary>
        /// Selects the given colour preview: : 0 - primary; 1 - secondary.
        /// </summary>
        /// <param name="colourPreviewIndex"></param>
        private void SelectColourPreview(int colourPreviewIndex)
        {
            uiColourPicker.colourPreview = colourPreviews[colourPreviewIndex];
            colourPreviewToggleGroup.Press(colourPreviewIndex);
        }

        /// <summary>
        /// Moves through the colour previews by the specified amount.
        /// Currently, there are only two colour previews, so this just swaps between primary/secondary colour if numOfSteps is odd, else does nothing.
        /// </summary>
        private void CycleColourPreview(int numOfSteps)
        {
            SelectColourPreview(Functions.Mod(currentColourPreviewIndex + numOfSteps, colourPreviews.Length));
        }

        /// <summary>
        /// Handles checking keyboard shortcuts and enacting the relevant actions.
        /// </summary>
        private void CheckKeyboardShortcuts()
        {
            if (toolbar.selectedTool != Tool.Move)
            {
                if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("selected colour left")))
                {
                    CycleColourPreview(-1);
                }
                else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("selected colour right")))
                {
                    CycleColourPreview(1);
                }
            }

            if (toolbar.selectedTool == Tool.Pencil || toolbar.selectedTool == Tool.EyeDropper || toolbar.selectedTool == Tool.Fill)
            {
                if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha1))
                {
                    SelectColourPreview(0);
                }
                else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha2))
                {
                    SelectColourPreview(1);
                }
            }
        }

        /// <summary>
        /// Caled when a new colour preview is selected.
        /// </summary>
        private void OnSelectedColourPreview()
        {
            OnSelectedColourPreview(colourPreviewToggleGroup.currentToggle.GetComponent<ColourPreview>());
        }
        /// <summary>
        /// Caled when a new colour preview is selected.
        /// </summary>
        private void OnSelectedColourPreview(int colourPreviewIndex)
        {
            if (colourPreviewIndex < 0 || colourPreviewIndex > colourPreviews.Length)
            {
                throw new System.Exception("Index out of range: " + colourPreviewIndex);
            }

            currentColourPreviewIndex = colourPreviewIndex;
            uiColourPicker.colourPreview = colourPreviews[colourPreviewIndex];
            SetColour(colourPreviews[colourPreviewIndex].colour);
        }
        /// <summary>
        /// Caled when a new colour preview is selected.
        /// </summary>
        private void OnSelectedColourPreview(ColourPreview colourPreview)
        {
            for (int i = 0; i < colourPreviews.Length; i++)
            {
                if (colourPreview == colourPreviews[i])
                {
                    OnSelectedColourPreview(i);
                    return;
                }
            }
            throw new System.Exception("Couldn't find given colour preview.");
        }

        /// <summary>
        /// Called when the selected colour has been changed by the RGBA scales.
        /// </summary>
        private void OnColourChangedByScales()
        {
            SetColour(new Color(rScale.value / 255f, gScale.value / 255f, bScale.value / 255f, aScale.value / 255f));
        }

        /// <summary>
        /// Event is invoked when the selected colour changes.
        /// </summary>
        public void SubscribeToOnColourChange(UnityAction call)
        {
            onColourChanged.AddListener(call);
        }
    }
}

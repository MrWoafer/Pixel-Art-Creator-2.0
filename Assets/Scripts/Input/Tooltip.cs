using PAC.UI;
using UnityEngine;

namespace PAC.Input
{
    [RequireComponent(typeof(InputTarget))]
    public class Tooltip : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private string _text = "";
        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                tooltip?.SetText(_text);
            }
        }
        public float hoverTimeUntilTooltip = 2f;
        public Vector2 padding = Vector2.zero;

        [Header("References")]
        [SerializeField]
        private GameObject uiTooltipPrefab;

        public bool tooltipVisible { get; private set; } = false;

        private InputSystem inputSystem;
        private InputTarget inputTarget;

        private Transform tooltips;
        private UITooltip tooltip;

        private void Awake()
        {
            inputSystem = Finder.inputSystem;
            inputTarget = GetComponent<InputTarget>();

            tooltips = GameObject.Find("Tooltips").transform;
        }

        // Start is called before the first frame update
        void Start()
        {
            tooltip = Instantiate(uiTooltipPrefab, tooltips).GetComponent<UITooltip>();
            tooltip.SetText(text);
            tooltip.padding = padding;

            HideTooltip();
        }

        // Update is called once per frame
        void Update()
        {
            if (!tooltipVisible && inputTarget.mouseTarget.timeHovered >= hoverTimeUntilTooltip)
            {
                ShowTooltip(inputSystem.mouse.worldPos);
            }

            if (tooltipVisible && inputTarget.mouseTarget.state != MouseTargetState.Hover && inputTarget.mouseTarget.state != MouseTargetState.Pressed)
            {
                HideTooltip();
            }
        }

        public void ShowTooltip(Vector2 worldCoords)
        {
            tooltip.transform.position = Functions.Vector2ToVector3(worldCoords) + new Vector3(tooltip.globalWidth / 2f, tooltip.globalHeight / 2f, -0.01f);

            int iterations = 0;
            while (tooltip.GoesOffLeftOfScreen() && iterations < 100000)
            {
                tooltip.transform.position += new Vector3(0.1f, 0f, 0f);
                iterations++;
            }

            iterations = 0;
            while (tooltip.GoesOffRightOfScreen() && iterations < 100000)
            {
                tooltip.transform.position += new Vector3(-0.1f, 0f, 0f);
                iterations++;
            }

            iterations = 0;
            while (tooltip.GoesOffBottomOfScreen() && iterations < 100000)
            {
                tooltip.transform.position += new Vector3(0f, 0.1f, 0f);
                iterations++;
            }

            iterations = 0;
            while (tooltip.GoesOffTopOfScreen() && iterations < 100000)
            {
                tooltip.transform.position += new Vector3(0f, -0.1f, 0f);
                iterations++;
            }

            tooltipVisible = true;
        }

        public void HideTooltip()
        {
            tooltip.transform.position = new Vector3(-10000f, 0f, 0f);
            tooltipVisible = false;
        }
    }
}

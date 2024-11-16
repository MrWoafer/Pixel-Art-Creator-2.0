using UnityEngine;
using UnityEngine.Events;
using PAC.Utils;

namespace PAC.UI
{
    public class UINumberField : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float startingValue = 0f;
        [SerializeField]
        private float increment = 1f;
        [SerializeField]
        [Min(0)]
        [Tooltip("The maximum number of decimal placs that will be displayed.")]
        private int textDecimalPlaces = 0;
        [SerializeField]
        private bool useMax = false;
        [SerializeField]
        private float _max = 1f;
        public float max
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
                this.value = this.value;
            }
        }
        [SerializeField]
        private bool useMin = true;
        [SerializeField]
        private float _min = 0f;
        public float min
        {
            get
            {
                return _min;
            }
            set
            {
                _min = value;
                this.value = this.value;
            }
        }

        private float _value;
        public float value
        {
            get
            {
                return _value;
            }
            set
            {
                float oldValue = _value;

                _value = Mathf.Clamp(value, useMin ? min : float.MinValue, useMax ? max : float.MaxValue);
                UpdateDisplay();

                if (oldValue != _value)
                {
                    onValueChanged.Invoke();
                    valueChanged = true;
                }
            }
        }

        private UITextbox textbox;

        private bool valueChanged = false;
        private bool beenRunningAFrame = false;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onValueChanged = new UnityEvent();

        private void Awake()
        {
            GetReferences();
        }

        private void Start()
        {
            textbox.SubscribeToFinishEvent(GetValueFromTextbox);

            if (!valueChanged)
            {
                value = startingValue;
            }
        }

        private void Update()
        {
            if (!beenRunningAFrame)
            {
                beenRunningAFrame = true;
            }
        }

        private void GetReferences()
        {
            textbox = transform.Find("Textbox").GetComponent<UITextbox>();
        }

        private void OnValidate()
        {
            if (!Application.isPlaying || beenRunningAFrame)
            {
                GetReferences();

                max = useMax ? Mathf.Max(max, useMin ? min : float.MinValue) : max;
                startingValue = Mathf.Clamp(startingValue, useMin ? min : float.MinValue, useMax ? max : float.MaxValue);
                if (!Application.isPlaying)
                {
                    value = startingValue;
                }

                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            textbox.SetText(UtilFunctions.RoundDecimalPlaces(value, textDecimalPlaces).ToString());
        }

        public void AddNumOfIncrements(int numOfIncrements)
        {
            value += increment * numOfIncrements;
        }
        public void Increment()
        {
            AddNumOfIncrements(1);
        }
        public void Decrement()
        {
            AddNumOfIncrements(-1);
        }

        private void GetValueFromTextbox()
        {
            try
            {
                value = float.Parse(textbox.text);
            }
            catch
            {
                UpdateDisplay();
            }
        }

        public void SubscribeToValueChanged(UnityAction call)
        {
            onValueChanged.AddListener(call);
        }
    }
}

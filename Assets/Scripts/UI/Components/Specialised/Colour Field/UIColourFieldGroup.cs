using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace PAC.UI.Components.Specialised.ColourField
{
    [AddComponentMenu("Custom UI/UI Colour Field Group")]
    public class UIColourFieldGroup : MonoBehaviour
    {
        [SerializeField]
        private List<UIColourField> _colourFields = new List<UIColourField>();
        public List<UIColourField> colourFields
        {
            get
            {
                return _colourFields;
            }
        }

        public UIColourField currentOpenColourField
        {
            get
            {
                foreach (UIColourField colourField in _colourFields)
                {
                    if (colourField.colourPickerOpen)
                    {
                        return colourField;
                    }
                }
                return null;
            }
        }

        public int Count
        {
            get
            {
                return _colourFields.Count;
            }
        }

        private UnityEvent onColourFieldOpened = new UnityEvent();

        private void Start()
        {
            Initialise();
        }

        private void Initialise()
        {
            foreach (UIColourField colourField in _colourFields)
            {
                if (colourField != null)
                {
                    UIColourField temp = colourField;
                    colourField.SubscribeToColourPickerOpen(() => Opened(temp));
                }
            }
        }

        public bool Add(UIColourField colourField)
        {
            _colourFields.Add(colourField);

            return true;
        }

        public bool Remove(UIColourField colourField)
        {
            if (_colourFields.Remove(colourField))
            {
                return true;
            }
            return false;
        }

        public bool Contains(UIColourField colourField)
        {
            return _colourFields.Contains(colourField);
        }

        public void Clear()
        {
            _colourFields = new List<UIColourField>();
        }

        public void DestroyColourFields()
        {
            foreach (UIColourField colourField in _colourFields)
            {
                Destroy(colourField.gameObject);
            }

            Clear();
        }

        public void Opened(UIColourField openedColourField)
        {
            foreach (UIColourField colourField in _colourFields)
            {
                if (colourField != openedColourField)
                {
                    colourField.CloseColourPicker();
                }
            }
        }

        public void SubscribeToColourFieldOpen(UnityAction call)
        {
            onColourFieldOpened.AddListener(call);
        }
    }
}

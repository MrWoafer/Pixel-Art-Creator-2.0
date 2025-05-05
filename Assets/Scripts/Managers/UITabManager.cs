using System.Collections.Generic;

using UnityEngine;

namespace PAC.Managers
{
    public class UITabManager : MonoBehaviour
    {
        [Header("Tabs")]
        [SerializeField]
        [Min(0)]
        private int selectedTabIndex = 0;
        [SerializeField]
        private List<GameObject> tabs;

        private bool tabSelectedAlready = false;

        private void Awake()
        {
            foreach (GameObject tab in tabs)
            {
                tab.SetActive(true);
            }
        }

        private void Start()
        {
            if (!tabSelectedAlready && tabs.Count != 0)
            {
                SelectTab(selectedTabIndex);
            }
        }

        private void OnValidate()
        {
            if (tabs.Count > 0)
            {
                selectedTabIndex = Mathf.Clamp(selectedTabIndex, 0, tabs.Count - 1);
                SelectTabEditor(selectedTabIndex);
            }
            else
            {
                selectedTabIndex = 0;
            }
        }

        public void AddTab(GameObject tab)
        {
            tabs.Add(tab);
        }

        public void SelectTab(int tabIndex)
        {
            if (tabIndex < 0 || tabIndex >= tabs.Count)
            {
                throw new System.IndexOutOfRangeException("Tab index out of range: " + tabIndex);
            }

            for (int i = 0; i < tabs.Count; i++)
            {
                if (i == tabIndex && tabs[i].transform.position.x < -5000f)
                {
                    tabs[i].transform.position += new Vector3(10000f, 0f, 0f);
                }
                else if (i != tabIndex && tabs[i].transform.position.x > -5000f)
                {
                    tabs[i].transform.position -= new Vector3(10000f, 0f, 0f);
                }
            }

            tabSelectedAlready = true;
        }

        public void SelectTabEditor(int tabIndex)
        {
            if (tabIndex < 0 || tabIndex >= tabs.Count)
            {
                throw new System.IndexOutOfRangeException("Tab index out of range: " + tabIndex);
            }

            for (int i = 0; i < tabs.Count; i++)
            {
                if (i == tabIndex)
                {
                    tabs[i].SetActive(true);
                }
                else
                {
                    tabs[i].SetActive(false);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITabManager : MonoBehaviour
{
    [Header("Tabs")]
    [SerializeField]
    [Min(0)]
    private int selectedTabIndex = 0;
    [SerializeField]
    private GameObject[] tabs;

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
        if (!tabSelectedAlready)
        {
            SelectTab(selectedTabIndex);
        }
    }

    private void OnValidate()
    {
        if (tabs.Length > 0)
        {
            selectedTabIndex = Mathf.Clamp(selectedTabIndex, 0, tabs.Length - 1);
            SelectTabEditor(selectedTabIndex);
        }
        else
        {
            selectedTabIndex = 0;
        }
    }

    public void SelectTab(int tabIndex)
    {
        if (tabIndex < 0 || tabIndex >= tabs.Length)
        {
            throw new System.IndexOutOfRangeException("Tab index out of range: " + tabIndex);
        }

        for (int i = 0; i < tabs.Length; i++)
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
        if (tabIndex < 0 || tabIndex >= tabs.Length)
        {
            throw new System.IndexOutOfRangeException("Tab index out of range: " + tabIndex);
        }

        for (int i = 0; i < tabs.Length; i++)
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

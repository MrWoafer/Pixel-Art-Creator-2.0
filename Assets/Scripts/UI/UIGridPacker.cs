using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridAlignment
{
    Left = -1,
    Centre = 0,
    Right = 1
}

public class UIGridPacker : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private bool refresh = false;
    [SerializeField]
    private GridAlignment alignment = GridAlignment.Centre;
    [SerializeField]
    [Min(0)]
    private int maxPerRow = 10;
    [SerializeField]
    [Min(0f)]
    private float ySpacing = 0.2f;
    [SerializeField]
    [Min(0f)]
    private float xSpacing = 0.2f;

    private RectTransform rect;
    private float width => rect.sizeDelta.x;
    private float height => rect.sizeDelta.y;

    private void Awake()
    {
        GetReferences();
    }

    private void GetReferences()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Repack()
    {
        float y = height / 2f;

        int rowStartIndex = 0;
        float rowWidth = 0f;
        float rowMaxY = 0f;
        float rowMinY = 0f;
        int rowObjCount = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            Vector2 widthHeight = GetWidthHeight(child);
            float childWidth = widthHeight.x;
            float childHeight = widthHeight.y;
            if (childWidth == -1 || childHeight == -1)
            {
                throw new System.Exception("Couldn't get a width/height for child: " + child.name);
            }

            if (rowWidth + childWidth > width || rowObjCount >= maxPerRow)
            {
                y -= rowMaxY;
                rowWidth -= xSpacing;

                PackRow(rowStartIndex, i - 1, y, rowWidth);

                y += rowMinY - ySpacing;

                rowStartIndex = i;
                rowWidth = 0f;
                rowMaxY = 0f;
                rowMinY = 0f;
                rowObjCount = 0;
            }

            child.transform.localPosition = new Vector3(rowWidth + childWidth / 2f - width / 2f, transform.localPosition.y, transform.localPosition.z);

            rowWidth += childWidth + xSpacing;
            rowMaxY = Mathf.Max(rowMaxY, childHeight / 2f);
            rowMinY = Mathf.Min(rowMinY, -childHeight / 2f);
            rowObjCount++;
        }

        y -= rowMaxY;
        rowWidth -= xSpacing;
        PackRow(rowStartIndex, transform.childCount - 1, y, rowWidth);
    }

    private void PackRow(int rowStartIndex, int rowEndIndex, float y, float rowWidth)
    {
        for (int j = rowStartIndex; j <= rowEndIndex; j++)
        {
            Transform rowChild = transform.GetChild(j);

            if (alignment == GridAlignment.Left)
            {
                rowChild.localPosition = new Vector3(rowChild.localPosition.x, y, rowChild.localPosition.z);
            }
            else if (alignment == GridAlignment.Right)
            {
                rowChild.localPosition = new Vector3(rowChild.localPosition.x + width - rowWidth, y, rowChild.localPosition.z);
            }
            else if (alignment == GridAlignment.Centre)
            {
                rowChild.localPosition = new Vector3(rowChild.localPosition.x + (width - rowWidth) / 2f, y, rowChild.localPosition.z);
            }
        }
    }

    private Vector2 GetWidthHeight(Transform obj) => GetWidthHeight(obj.gameObject);
    private Vector2 GetWidthHeight(GameObject obj)
    {
        float width = -1;
        float height = -1;

        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            width = rectTransform.sizeDelta.x * obj.transform.localScale.x;
            height = rectTransform.sizeDelta.y * obj.transform.localScale.y;
        }

        UIButton button = obj.GetComponent<UIButton>();
        if (button != null)
        {
            width = button.width * obj.transform.localScale.x;
            height = button.height * obj.transform.localScale.y;
        }

        UIToggleButton toggle = obj.GetComponent<UIToggleButton>();
        if (toggle != null)
        {
            width = toggle.width * obj.transform.localScale.x;
            height = toggle.height * obj.transform.localScale.y;
        }

        UITileIcon tileIcon = obj.GetComponent<UITileIcon>();
        if (tileIcon != null)
        {
            width = tileIcon.width * obj.transform.localScale.x;
            height = tileIcon.height * obj.transform.localScale.y;
        }

        return new Vector2(width, height);
    }

    private void OnValidate()
    {
        GetReferences();
        Repack();
    }
}

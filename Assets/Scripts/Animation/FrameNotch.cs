using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameNotch : MonoBehaviour
{
    private Text numberText;

    public int frameNum
    {
        get
        {
            return int.Parse(numberText.text);
        }
        set
        {
            SetFrameNumber(value);
        }
    }

    private void Awake()
    {
        numberText = transform.Find("Text").GetComponent<Text>();
    }

    public void SetFrameNumber(int num)
    {
        numberText.text = num.ToString();
    }
}

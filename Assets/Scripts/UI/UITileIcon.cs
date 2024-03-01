using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UITileIcon : MonoBehaviour
{
    [Header("Settings")]
    private File _file;
    public File file
    {
        get => _file;
        private set
        {
            _file = value;
        }
    }

    public float width
    {
        get
        {
            if (Application.isEditor)
            {
                GetReferences();
            }
            return button.width;
        }
    }
    public float height
    {
        get
        {
            if (Application.isEditor)
            {
                GetReferences();
            }
            return button.height;
        }
    }

    private UIButton button;
    private Text nameText;

    private FileManager fileManager;

    [Header("Events")]
    private UnityEvent onClick = new UnityEvent();
    private UnityEvent onLeftClick = new UnityEvent();
    private UnityEvent onRightClick = new UnityEvent();

    private void Awake()
    {
        GetReferences();
    }

    private void GetReferences()
    {
        button = transform.Find("Button").GetComponent<UIButton>();
        nameText = button.transform.Find("Canvas").Find("Name").GetComponent<Text>();

        fileManager = Finder.fileManager;
    }

    void Start()
    {
        button.SetImages(null, null, null);

        SetFile(File.OpenFile("D:\\Games\\Pixel-Art-Creator-2.0\\Test Images\\Cube.png"));

        button.SubscribeToClick(onClick.Invoke);
        button.SubscribeToLeftClick(onLeftClick.Invoke);
        button.SubscribeToRightClick(onRightClick.Invoke);
        button.SubscribeToRightClick(() => fileManager.OpenFile(file));
    }

    public void SetFile(File file)
    {
        this.file = file;
        nameText.text = file.name;

        file.liveRender.Apply();
        button.SetImages(Tex2DSprite.Tex2DToSprite(file.liveRender), null, null);
        file.SubscribeToOnPixelsChanged((x, y, z) => file.liveRender.Apply());
    }

    public void SubscribeToOnClick(UnityAction call)
    {
        onClick.AddListener(call);
    }
    public void SubscribeToOnLeftClick(UnityAction call)
    {
        onLeftClick.AddListener(call);
    }
    public void SubscribeToOnRightClick(UnityAction call)
    {
        onRightClick.AddListener(call);
    }
}

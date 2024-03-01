using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TilesetManager : MonoBehaviour
{
    private UITileIcon[] tileIcons;

    private UIGridPacker gridPacker;

    [Header("Events")]
    private UnityEvent<File> onTileIconSelected = new UnityEvent<File>();

    private void Awake()
    {
        gridPacker = GetComponentInChildren<UIGridPacker>();
    }

    void Start()
    {
        tileIcons = GetComponentsInChildren<UITileIcon>();

        foreach (UITileIcon tileIcon in tileIcons)
        {
            tileIcon.SubscribeToOnLeftClick(() => onTileIconSelected.Invoke(tileIcon.file));
        }
    }

    public void SubscribeToOnTileIconSelected(UnityAction<File> call)
    {
        onTileIconSelected.AddListener(call);
    }
}

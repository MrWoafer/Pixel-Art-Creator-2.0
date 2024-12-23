using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom items for the 'Assets > Create' menu.
/// </summary>
public static class CreateAssetMenuItems
{
    [MenuItem("Assets/Create/Files/Text File")]
    public static void CreateEmptyTextAsset() => CreateAssetMenuItemsHelper.CreateAssetWithRename<TextAsset>("Create text file", "New Text File.txt", "");
}

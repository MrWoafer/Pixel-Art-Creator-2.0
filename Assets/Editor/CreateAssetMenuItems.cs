using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom items for the 'Assets > Create' menu.
/// </summary>
public static class CreateAssetMenuItems
{
    [MenuItem("Assets/Create/Files/Text File")]
    public static void CreateEmptyTextAsset() => CreateAssetMenuItemsHelper.CreateAssetWithRename<TextAsset>("Create text file", "New Text File.txt", "");

    [MenuItem("Assets/Create/Scripting/Completely Empty C# Script")]
    public static void CreateCompletelyEmptyScript() => CreateAssetMenuItemsHelper.CreateAssetWithRename<MonoScript>("Create completely empty C# script", "New Script.cs", "");
}

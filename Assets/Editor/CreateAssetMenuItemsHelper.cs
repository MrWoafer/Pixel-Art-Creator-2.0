using System;
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

/// <summary>
/// A helper class for adding custom items to the 'Assets > Create' menu.
/// </summary>
public static class CreateAssetMenuItemsHelper
{
    /// <summary>
    /// A subclass of <see cref="EndNameEditAction"/> that makes it easy, using delegates, to specify what happens when renaming an asset is finished/cancelled.
    /// </summary>
    private class CustomisableEndNameEditAction : EndNameEditAction
    {
        /// <summary>
        /// Takes in:
        /// <list type="bullet">
        /// <item>int instanceId</item>
        /// <item>string filePath</item>
        /// <item>string resourceFile</item>
        /// </list>
        /// </summary>
        private Action<int, string, string> endNameEditAction;
        /// <summary>
        /// Takes in:
        /// <list type="bullet">
        /// <item>int instanceId</item>
        /// <item>string filePath</item>
        /// <item>string resourceFile</item>
        /// </list>
        /// </summary>
        private Action<int, string, string> cancelNameEditAction;

        /// <summary>
        /// A builder method that sets what happens when the user finishes renaming.
        /// </summary>
        /// <param name="endNameEditAction">
        /// Takes in:
        /// <list type="bullet">
        /// <item>int instanceId</item>
        /// <item>string filePath</item>
        /// <item>string resourceFile</item>
        /// </list>
        /// </param>
        public CustomisableEndNameEditAction SetEndAction(Action<int, string, string> action)
        {
            endNameEditAction = action;
            return this;
        }
        /// <summary>
        /// A builder method that sets what happens when the user cancels renaming.
        /// </summary>
        /// <param name="endNameEditAction">
        /// Takes in:
        /// <list type="bullet">
        /// <item>int instanceId</item>
        /// <item>string filePath</item>
        /// <item>string resourceFile</item>
        /// </list>
        /// </param>
        public CustomisableEndNameEditAction SetCancelAction(Action<int, string, string> action)
        {
            cancelNameEditAction = action;
            return this;
        }

        public override void Action(int instanceId, string filePath, string resourceFile)
        {
            endNameEditAction?.Invoke(instanceId, filePath, resourceFile);
        }
        public override void Cancelled(int instanceId, string filePath, string resourceFile)
        {
            cancelNameEditAction?.Invoke(instanceId, filePath, resourceFile);
        }
    }

    /// <summary>
    /// Creates an asset of the given type in the currently-open asset folder, and makes the user choose a name for it.
    /// </summary>
    /// <typeparam name="AssetType">The type of asset to create.</typeparam>
    /// <param name="actionName">The name of the action being performed. Used when registering the asset creation with the undo system.</param>
    /// <param name="defaultFilename">
    /// <para>
    /// The name of the asset before the user renames it.
    /// </para>
    /// <para>
    /// Must not include any directories.
    /// </para>
    /// <para>
    /// Must include the file extension, which must be a valid extension for <typeparamref name="AssetType"/>.
    /// </para>
    /// </param>
    /// <param name="contents">The string to write into the asset.</param>
    public static void CreateAssetWithRename<AssetType>(string actionName, string defaultFilename, string contents) where AssetType : UnityEngine.Object
        => CreateAssetWithRename<AssetType>(actionName, defaultFilename, (fileName) => fileName, (_) => contents);
    /// <summary>
    /// Creates an asset of the given type in the currently-open asset folder, and makes the user choose a name for it.
    /// </summary>
    /// <typeparam name="AssetType">The type of asset to create.</typeparam>
    /// <param name="actionName">The name of the action being performed. Used when registering the asset creation with the undo system.</param>
    /// <param name="defaultFilename">
    /// <para>
    /// The name of the asset before the user renames it.
    /// </para>
    /// <para>
    /// Must not include any directories.
    /// </para>
    /// <para>
    /// Must include the file extension, which must be a valid extension for <typeparamref name="AssetType"/>.
    /// </para>
    /// </param>
    /// <param name="fileNameGenerator">
    /// A function to generate the file name of the asset based on what the user named it - e.g. to add a suffix.
    /// Takes in the file name of the asset (without the extension), once the asset's name has been chosen.
    /// </param>
    /// <param name="contentGenerator">
    /// A function to generate the string to write into the asset.
    /// Takes in the file name of the asset (without the extension), once the asset's name has been chosen.
    /// </param>
    public static void CreateAssetWithRename<AssetType>(string actionName, string defaultFilename, Func<string, string> fileNameGenerator, Func<string, string> contentGenerator)
        where AssetType : UnityEngine.Object
    {
        if (!Path.HasExtension(defaultFilename))
        {
            throw new ArgumentException($"{nameof(defaultFilename)} does not have a file extension.", nameof(defaultFilename));
        }
        if (Path.GetFileName(defaultFilename) != defaultFilename)
        {
            throw new ArgumentException($"{nameof(defaultFilename)} must not include any directories.", nameof(defaultFilename));
        }

        string folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrWhiteSpace(folderPath) || !AssetDatabase.IsValidFolder(folderPath))
        {
            throw new ArgumentException("Unable to get folder path.");
        }

        string defaultFilePath = Path.Combine(folderPath, defaultFilename);

        CustomisableEndNameEditAction endNameEditAction = ScriptableObject.CreateInstance<CustomisableEndNameEditAction>().SetEndAction((int instanceId, string filePath, string resourceFile) =>
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            filePath = ChangeFileName(filePath, fileNameGenerator.Invoke(fileName));
            string contents = contentGenerator.Invoke(fileName);
            Selection.activeObject = CreateAssetAtPath<AssetType>(actionName, filePath, contents);
        });

        Texture2D icon = GetIconForAssetType<AssetType>();

        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, endNameEditAction, defaultFilePath, icon, null);
    }

    /// <summary>
    /// Creates a new asset at the given path with the given contents. Will not overwrite existing files.
    /// </summary>
    /// <typeparam name="AssetType">The type of asset to create.</typeparam>
    /// <param name="actionName">The name of the action being performed. Used when registering the asset creation with the undo system.</param>
    /// <param name="filePath">
    /// <para>
    /// The file path to create the asset at.
    /// </para>
    /// <para>
    /// Must include the file extension, which must be a valid extension for <typeparamref name="AssetType"/>.
    /// </para>
    /// </param>
    /// <param name="contents">The string to write into the asset.</param>
    private static AssetType CreateAssetAtPath<AssetType>(string actionName, string filePath, string contents) where AssetType : UnityEngine.Object
    {
        if (contents is null)
        {
            throw new ArgumentNullException(nameof(contents), $"{nameof(contents)} is null.");
        }
        if (!Path.HasExtension(filePath))
        {
            throw new ArgumentException($"{nameof(filePath)} does not have a file extension.", nameof(filePath));
        }

        // Create an empty file, throwing an error if it already exists
        new FileStream(filePath, FileMode.CreateNew).Dispose();
        // Write the contents
        File.WriteAllText(filePath, contents);

        AssetDatabase.Refresh();
        AssetType createdAsset = AssetDatabase.LoadAssetAtPath<AssetType>(filePath);

        // Undoing doesn't seem to work properly
        //Undo.RegisterCreatedObjectUndo(createdAsset, actionName);

        return createdAsset;
    }

    /// <summary>
    /// Gets the icon that is used in the project explorer for the given asset type.
    /// </summary>
    private static Texture2D GetIconForAssetType<AssetType>() where AssetType : UnityEngine.Object
    {
        // This default name won't work for all asset types. It will work for at least:
        // TextAsset
        // SceneAsset
        // AudioClip
        // Material
        // Texture2D
        string iconName = $"{typeof(AssetType).Name} Icon";

        if (typeof(AssetType) == typeof(MonoScript))
        {
            iconName = "cs Script Icon";
        }
        if (typeof(AssetType) == typeof(GameObject))
        {
            iconName = "Prefab Icon";
        }

        return EditorGUIUtility.IconContent(iconName).image as Texture2D;
    }

    /// <summary>
    /// Changes the file name of the file path, without affecting the directories or extension.
    /// </summary>
    /// <param name="newFileName">
    /// <para>
    /// Must not contain any directories.
    /// </para>
    /// <para>
    /// A file extension will not change the extension of the path, but will be included in the file name.
    /// E.g. <paramref name="filePath"/> = "Folder/old_file.cs" and <paramref name="newFileName"/> = "new_file.txt" will become "Folder/new_file.txt.cs"
    /// </para>
    /// </param>
    private static string ChangeFileName(string filePath, string newFileName)
    {
        if (Path.GetFileName(newFileName) != newFileName)
        {
            throw new ArgumentException($"{nameof(newFileName)} must not include any directories.", nameof(newFileName));
        }

        string directory = Path.GetDirectoryName(filePath);
        string extension = Path.GetExtension(filePath);

        return Path.ChangeExtension(Path.Combine(directory, newFileName), extension);
    }
}

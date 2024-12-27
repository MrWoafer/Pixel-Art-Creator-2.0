using System;
using System.IO;
using System.Linq;
using System.Text;

using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom items for the 'Assets > Create' menu.
/// </summary>
public static class CreateAssetMenuItems
{
    private const int scriptTabSize = 4;

    [MenuItem("Assets/Create/Files/Text File")]
    public static void CreateEmptyTextAsset() => CreateAssetMenuItemsHelper.CreateAssetWithRename<TextAsset>("Create text file", "New Text File.txt", "");

    [MenuItem("Assets/Create/Scripting/Completely Empty C# Script")]
    public static void CreateCompletelyEmptyScript() => CreateAssetMenuItemsHelper.CreateAssetWithRename<MonoScript>("Create completely empty C# script", "New Script.cs", "");

    [MenuItem("Assets/Create/Scripting/Extension Methods Script")]
    public static void CreateExtensionMethodsScript()
    {
        const string suffix = "Extensions";

        static (string className, string typeName, string variableName) NameGenerator(string fileName)
        {
            if (fileName.Any(c => char.IsWhiteSpace(c)))
            {
                throw new ArgumentException("File name cannot contain whitespace.", nameof(fileName));
            }

            // Add suffix to file name if it doesn't already have it
            string className = fileName + (fileName.EndsWith(suffix) ? "" : suffix);
            // Remove suffix from the end of the class name
            string typeName = className.Remove(className.Length - suffix.Length);
            // Turn type name into camel case
            string variableName = char.ToLower(typeName[0]) + (typeName.Length > 1 ? typeName[1..] : "");

            return (className, typeName, variableName);
        }

        static string FileNameGenerator(string fileName) => NameGenerator(fileName).className;

        static string ContentGenerator(string fileName)
        {
            (string className, string typeName, string variableName) = NameGenerator(fileName);

            StringBuilder contents = new StringBuilder();

            contents.AppendLine($"namespace {EditorSettings.projectGenerationRootNamespace}.Extensions");
            contents.AppendLine("{");
            contents.AppendLine($"\t/// <summary>");
            contents.AppendLine($"\t/// Extension methods for <see cref=\"{typeName}\"/>.");
            contents.AppendLine($"\t/// </summary>");
            contents.AppendLine($"\tpublic static class {className}");
            contents.AppendLine("\t{");
            contents.AppendLine($"\t\tpublic static void ExtensionMethod(this {typeName} {variableName})");
            contents.AppendLine("\t\t{");
            contents.AppendLine();
            contents.AppendLine("\t\t}");
            contents.AppendLine("\t}");
            contents.AppendLine("}");

            return contents.ToString().Replace("\t", new string(' ', scriptTabSize));
        }

        CreateAssetMenuItemsHelper.CreateAssetWithRename<MonoScript>("Create extension methods script", "New Extension Methods Script.cs", FileNameGenerator, ContentGenerator);
    }

    [MenuItem("Assets/Create/Scripting/Interface Script")]
    public static void CreateInterfaceScript()
    {
        static string FileNameGenerator(string fileName)
        {
            if (fileName.Any(c => char.IsWhiteSpace(c)))
            {
                throw new ArgumentException("File name cannot contain whitespace.", nameof(fileName));
            }

            // Add "I" prefix if the file name doesn't already include it
            if (fileName[0] != 'I' || fileName.Length == 1 || char.IsLower(fileName[1]))
            {
                return "I" + fileName;
            }
            return fileName;
        }

        static string ContentGenerator(string fileName)
        {
            string className = FileNameGenerator(fileName);

            StringBuilder contents = new StringBuilder();

            contents.AppendLine($"namespace {EditorSettings.projectGenerationRootNamespace}");
            contents.AppendLine("{");
            contents.AppendLine($"\tpublic interface {className}");
            contents.AppendLine("\t{");
            contents.AppendLine();
            contents.AppendLine("\t}");
            contents.AppendLine("}");

            return contents.ToString().Replace("\t", new string(' ', scriptTabSize));
        }

        CreateAssetMenuItemsHelper.CreateAssetWithRename<MonoScript>("Create interface script", "New Interface.cs", FileNameGenerator, ContentGenerator);
    }

    [MenuItem("Assets/Create/Scripting/Abstract Class Script")]
    public static void CreateAbstractClassScript()
    {
        static string FileNameGenerator(string fileName) => fileName;

        static string ContentGenerator(string fileName)
        {
            StringBuilder contents = new StringBuilder();

            contents.AppendLine($"namespace {EditorSettings.projectGenerationRootNamespace}");
            contents.AppendLine("{");
            contents.AppendLine($"\tpublic abstract class {fileName}");
            contents.AppendLine("\t{");
            contents.AppendLine();
            contents.AppendLine("\t}");
            contents.AppendLine("}");

            return contents.ToString().Replace("\t", new string(' ', scriptTabSize));
        }

        CreateAssetMenuItemsHelper.CreateAssetWithRename<MonoScript>("Create abstract class script", "New Abstract Class.cs", FileNameGenerator, ContentGenerator);
    }

    [MenuItem("Assets/Create/Testing/C# Edit Mode Test Script")]
    public static void CreateEditModeTestScript()
    {
        const string suffix = "Tests";

        static (string className, string typeName) NameGenerator(string fileName)
        {
            if (fileName.Any(c => char.IsWhiteSpace(c)))
            {
                throw new ArgumentException("File name cannot contain whitespace.", nameof(fileName));
            }

            // Add suffix to file name if it doesn't already have it
            string className = fileName + (fileName.EndsWith(suffix) ? "" : suffix);
            // Remove suffix from the end of the class name
            string typeName = className.Remove(className.Length - suffix.Length);
            
            return (className, typeName);
        }

        static string FileNameGenerator(string fileName) => NameGenerator(fileName).className;

        static string ContentGenerator(string fileName)
        {
            (string className, string typeName) = NameGenerator(fileName);

            StringBuilder contents = new StringBuilder();

            contents.AppendLine("using NUnit.Framework;");
            contents.AppendLine();
            contents.AppendLine($"namespace {EditorSettings.projectGenerationRootNamespace}.Tests");
            contents.AppendLine("{");
            contents.AppendLine($"\t/// <summary>");
            contents.AppendLine($"\t/// Tests for <see cref=\"{typeName}\"/>.");
            contents.AppendLine($"\t/// </summary>");
            contents.AppendLine($"\tpublic class {className}");
            contents.AppendLine("\t{");
            contents.AppendLine("\t\t[Test]");
            contents.AppendLine("\t\tpublic void ExampleTest()");
            contents.AppendLine("\t\t{");
            contents.AppendLine("\t\t\tAssert.Pass();");
            contents.AppendLine("\t\t}");
            contents.AppendLine("\t}");
            contents.AppendLine("}");

            return contents.ToString().Replace("\t", new string(' ', scriptTabSize));
        }

        CreateAssetMenuItemsHelper.CreateAssetWithRename<MonoScript>("Create edit mode test script", "New Edit Mode Test Script.cs", FileNameGenerator, ContentGenerator);
    }
}

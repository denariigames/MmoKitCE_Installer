using UnityEditor;
using UnityEngine;

public static class ExportProjectSettings
{
    [MenuItem("MMORPG KIT/MmoKitCE/Export Project Settings", false, 10200)]
    static void Export()
    {
        string[] paths = new string[]
        {
            "ProjectSettings/DynamicsManager.asset",
            "ProjectSettings/InputManager.asset",
            "ProjectSettings/Physics2DManager.asset",
            "ProjectSettings/ProjectSettings.asset",
            "ProjectSettings/QualitySettings.asset",
            "ProjectSettings/TagManager.asset",
            "ProjectSettings/TimeManager.asset",
        };

        AssetDatabase.ExportPackage(
            paths,
            "MmoKitCE_Settings.unitypackage",
            ExportPackageOptions.Interactive |
            ExportPackageOptions.Recurse |
            ExportPackageOptions.IncludeDependencies
            // ExportPackageOptions.IncludeLibraryAssets   // ← usually NOT needed for this
        );
    }
}
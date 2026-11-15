using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class BuildSceneList
{
    private static readonly string[] GameScenesInOrder =
    {
        "Assets/Scenes/Menu_Screen.unity",
        "Assets/Scenes/Part 1.unity",
        "Assets/Scenes/Part 2.unity",
    };

    static BuildSceneList()
    {
        EditorApplication.delayCall += EnsureGameScenesAreListed;
    }

    private static void EnsureGameScenesAreListed()
    {
        EditorBuildSettingsScene[] current = EditorBuildSettings.scenes;
        List<EditorBuildSettingsScene> updated = new List<EditorBuildSettingsScene>();

        foreach (string path in GameScenesInOrder.Where(File.Exists))
        {
            updated.Add(new EditorBuildSettingsScene(path, true));
        }
        foreach (EditorBuildSettingsScene scene in current)
        {
            if (File.Exists(scene.path) && updated.All(listed => listed.path != scene.path))
            {
                updated.Add(scene);
            }
        }

        bool unchanged = updated.Count == current.Length
            && updated.Zip(current, (wanted, existing) => wanted.path == existing.path && wanted.enabled == existing.enabled).All(same => same);
        if (unchanged)
        {
            return;
        }

        EditorBuildSettings.scenes = updated.ToArray();
        Debug.Log("Build scene list set to: " + string.Join(", ", updated.Select(scene => Path.GetFileNameWithoutExtension(scene.path))));
    }
}

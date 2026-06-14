using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildPlayer
{
    [MenuItem("SilverEagle/Build Windows")]
    public static void PerformBuild()
    {
        string buildFolder = Path.Combine(Directory.GetCurrentDirectory(), "Builds/Win64");
        if (!Directory.Exists(buildFolder))
        {
            Directory.CreateDirectory(buildFolder);
        }

        string buildPath = Path.Combine(buildFolder, "SilverEagle.exe");

        // Filter scenes to exclude dev/temp scenes (like EnemyAiScene)
        List<string> activeScenes = new List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                string path = scene.path.ToLower();
                // Exclude scenes with "dev", "temp", or "enemyai" in the name
                if (path.Contains("enemyai") || path.Contains("dev") || path.Contains("temp"))
                {
                    continue;
                }
                activeScenes.Add(scene.path);
            }
        }

        if (activeScenes.Count == 0)
        {
            Debug.LogError("Build Error: No active scenes found for build!");
            return;
        }

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = activeScenes.ToArray();
        buildPlayerOptions.locationPathName = buildPath;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        Debug.Log("Starting StandaloneWindows64 build to: " + buildPath);
        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        
        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("Build Succeeded! Total size: " + report.summary.totalSize + " bytes");
        }
        else
        {
            Debug.LogError("Build Failed! Result: " + report.summary.result);
        }
    }
}

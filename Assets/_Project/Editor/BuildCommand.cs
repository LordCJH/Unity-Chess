using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ChessGame.Editor
{
    public static class BuildCommand
    {
        public static void PerformBuild()
        {
            string[] args = Environment.GetCommandLineArgs();
            string buildPath = "Build/Linux/chinese_chess";
            bool headless = false;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-buildPath" && i + 1 < args.Length)
                    buildPath = args[i + 1];
                if (args[i] == "-headless")
                    headless = true;
            }

            BuildPlayerOptions options = new BuildPlayerOptions();
            options.scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();
            options.locationPathName = buildPath;
            options.target = BuildTarget.StandaloneLinux64;
            options.options = BuildOptions.None;

            if (headless)
                options.options |= BuildOptions.EnableHeadlessMode;

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Build succeeded: {summary.totalSize} bytes");
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError("Build failed.");
                EditorApplication.Exit(1);
            }
        }
    }
}

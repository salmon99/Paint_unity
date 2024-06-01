using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.iOS.Xcode;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

static class PostProcessBuildClass {
    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string path) {
    public static void ChangeXcodeSettings(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            // Fix for missing UnityFramework.h Xcode issue
            string mainAppPath = Path.Combine(pathToBuiltProject, "MainApp", "main.mm");
            string mainContent = File.ReadAllText(mainAppPath);
            string newContent = mainContent.Replace("#include <UnityFramework/UnityFramework.h>", @"#include ""../UnityFramework/UnityFramework.h""");
            File.WriteAllText(mainAppPath, newContent);
        }
    }
}
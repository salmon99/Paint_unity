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
        Debug.Log("Post process build ChangeXcodePlist");
        if (buildTarget == BuildTarget.iOS) {
            string mainAppPath = Path.Combine(path, "UnityFramework", "UnityFramework.h");
            string mainContent = File.ReadAllText(mainAppPath);
            string newContent = mainContent.Replace("#import <UnityFramework/UnityAppController.h>", "#import <Classes/UnityAppController.h>");
            File.WriteAllText(mainAppPath, newContent);
        }
    }
}
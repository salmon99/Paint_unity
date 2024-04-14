using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Discord;
using System.Threading.Tasks;

[InitializeOnLoad]
public class DiscordController
{
    private static Discord.Discord discord;
    private static string projectName { get { return Application.productName; } }
    private static string version { get { return Application.unityVersion; } }
    private static RuntimePlatform platform { get { return Application.platform; } }
    private static string activeSceneName { get { return EditorSceneManager.GetActiveScene().name; } }
    private static long lastTimestamp;

    private const string applicationId = "1228960923478397042";

    static DiscordController()
    {
        DelayInit();
    }

    private static async void DelayInit(int delay = 1000)
    {
        await Task.Delay(delay);
        SetupDiscord();
    }

    private static void SetupDiscord()
    {
        discord = new Discord.Discord(long.Parse(applicationId), (ulong)CreateFlags.Default);
        lastTimestamp = GetTimestamp();
        UpdateActivity();

        EditorApplication.update += EditorUpdate;
        EditorSceneManager.sceneOpened += SceneOpened;
    }

    private static void EditorUpdate()
    {
        discord.RunCallbacks();
    }

    private static void SceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode sceneMode)
    {
        UpdateActivity();
    }

    private static void UpdateActivity()
    {
        ActivityManager activityManager = discord.GetActivityManager();
        Activity activity = new Activity
        {
            Details = "Editing " + projectName,
            State = activeSceneName + " | " + platform,
            Timestamps =
            {
                Start = lastTimestamp
            },
            Assets =
            {
                LargeImage = "20240414_155515",
                LargeText = version,
                SmallImage = "20240414_155515",
                SmallText = version
            }
        };

        activityManager.UpdateActivity(activity, result =>
        {
            Debug.Log("Discord result : " + result);
        });
    }

    private static long GetTimestamp()
    {
        long unixTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        return unixTimestamp;
    }
}

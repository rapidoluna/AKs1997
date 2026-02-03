using Discord;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiscordRPCManager : MonoBehaviour
{
    public long applicationId = 123456789012345678;
    private Discord.Discord discord;
    private ActivityManager activityManager;

    void Start()
    {
        try
        {
            discord = new Discord.Discord(applicationId, (System.UInt64)CreateFlags.NoRequireDiscord);
            activityManager = discord.GetActivityManager();

            UpdatePresenceByScene();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Discord RPC Error: " + e.Message);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdatePresenceByScene();
    }

    public void UpdatePresenceByScene()
    {
        if (activityManager == null) return;

        string currentSceneName = SceneManager.GetActiveScene().name;
        string detailsText = "being watched by Odares";

        if (currentSceneName == "MainScene")
        {
            detailsText = "타이틀 화면";
        }
        else if (currentSceneName == "MenuScene")
        {
            detailsText = "메인 메뉴";
        }
        else if (currentSceneName == "PlayScene")
        {
            detailsText = "캐시러시 플레이 중";
        }
        else if (currentSceneName == "ResultScene")
        {
            detailsText = "결과 화면";
        }
        else if (currentSceneName == "rapidoluna")
        {
            detailsText = "개발 중";
        }
        else if (currentSceneName == "rkfaorlaksen")
        {
            detailsText = "개발 중";
        }

        var activity = new Activity
        {
            State = "플레이 중",
            Details = detailsText,
            Assets = {
                LargeImage = "icon",
                LargeText = "Digital World's Greatest 2.5D FPS Game"
            },
            Timestamps = {
                Start = System.DateTimeOffset.Now.ToUnixTimeSeconds()
            }
        };

        activityManager.UpdateActivity(activity, (result) => {
            if (result == Result.Ok) Debug.Log("Discord Presence Updated: " + detailsText);
        });
    }

    void Update()
    {
        if (discord != null) discord.RunCallbacks();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (discord != null) discord.Dispose();
    }
}
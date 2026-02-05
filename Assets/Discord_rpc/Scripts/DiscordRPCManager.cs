using Discord;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiscordRPCManager : MonoBehaviour
{
    //디스코드 RPC 매니저, 무조건 MainScene에만 프리팹을 배치할 것.
    public long applicationId = 123456789012345678;
    private Discord.Discord discord;
    private ActivityManager activityManager;

    private void Awake()
    {
        var existingManagers = Object.FindObjectsByType<DiscordRPCManager>(FindObjectsSortMode.None);

        if (existingManagers.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

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
            detailsText = "메뉴 화면";
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
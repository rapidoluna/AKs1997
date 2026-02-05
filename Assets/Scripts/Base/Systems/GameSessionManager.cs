using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance;

    public int kills;
    public int score; // 최종 합산 점수
    public int damageTaken;
    public bool isExtracted;
    public float playTime;

    private bool _isTimerRunning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (_isTimerRunning) playTime += Time.deltaTime;
    }

    public void StartSession()
    {
        ResetSession();
        _isTimerRunning = true;
    }

    public void StopSession() => _isTimerRunning = false;

    public void ResetSession()
    {
        kills = 0; score = 0; damageTaken = 0;
        playTime = 0f; isExtracted = false;
        _isTimerRunning = false;
    }

    public void AddInstantScore(int amount) => score += amount;

    public void AddDepositScore(int amount) => score += amount;

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(playTime / 60F);
        int seconds = Mathf.FloorToInt(playTime % 60F);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
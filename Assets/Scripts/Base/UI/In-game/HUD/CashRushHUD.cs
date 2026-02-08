using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CashRushHUD : MonoBehaviour
{
    public static CashRushHUD Instance;

    [SerializeField] private TextMeshProUGUI scoreText;//플레이어 점수
    [SerializeField] private TextMeshProUGUI timerText;//캐시러시 완료까지 남은 시간
    [SerializeField] private TextMeshProUGUI payoutText;//캐시러시를 통해 추가될 점수
    [SerializeField] private TextMeshProUGUI notifyText;//알림
    [SerializeField] private GameObject notifyPanel;//알림 패널
    [SerializeField] private GameObject timerPanel;//캐시러시 동안 표시될 패널

    private int _totalScore = 0;//플레이어 총 점수

    public int CurrentScore => _totalScore;

    private void Awake()
    {
        Instance = this;
        if (notifyPanel != null) notifyPanel.SetActive(false);
        if (timerPanel != null) timerPanel.SetActive(false);
        UpdateScoreUI();
    }

    public void SetTimerActive(bool active, int potentialAmount = 0)
    {
        if (timerPanel != null) timerPanel.SetActive(active);

        if (active && payoutText != null)
        {
            payoutText.text = $"+ {potentialAmount:N0}";
        }
    }

    public void UpdateTimer(float time)
    {
        if (timerText != null)
            timerText.text = $"캐시러시 완료까지 : {Mathf.CeilToInt(time)}s";
    }

    public void AddScore(int amount)
    {
        _totalScore += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"{_totalScore:N0}";
    }

    public void ShowNotification(string message)
    {
        if (notifyText == null || notifyPanel == null) return;
        notifyText.text = message;
        StopAllCoroutines();
        StartCoroutine(NotifyRoutine());
    }

    private IEnumerator NotifyRoutine()
    {
        notifyPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        notifyPanel.SetActive(false);
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CashRushHUD : MonoBehaviour
{
    public static CashRushHUD Instance;

    [SerializeField] private Text scoreText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text notifyText;
    [SerializeField] private GameObject notifyPanel;
    [SerializeField] private GameObject timerPanel;

    private int _totalScore = 0;

    private void Awake()
    {
        Instance = this;
        if (notifyPanel != null) notifyPanel.SetActive(false);
        if (timerPanel != null) timerPanel.SetActive(false);
        UpdateScoreUI();
    }

    public void SetTimerActive(bool active)
    {
        if (timerPanel != null) timerPanel.SetActive(active);
    }

    public void UpdateTimer(float time)
    {
        if (timerText != null)
            timerText.text = $"완료까지 : {Mathf.CeilToInt(time)}s";
    }

    public void AddScore(int amount)
    {
        _totalScore += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"현재 점수 : {_totalScore:N0}";
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
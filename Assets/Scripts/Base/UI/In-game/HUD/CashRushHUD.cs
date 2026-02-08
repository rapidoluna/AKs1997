using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CashRushHUD : MonoBehaviour
{
    public static CashRushHUD Instance;

    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image scoreGaugeImage;
    [SerializeField] private float targetScore = 50000f;
    [SerializeField] private float gaugeSpeed = 5f;

    [Header("Timer UI")]
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI payoutText;

    [Header("Notification UI")]
    [SerializeField] private GameObject notifyPanel;
    [SerializeField] private TextMeshProUGUI notifyText;

    private int _totalScore = 0;
    private float _currentGaugeFill = 0f;

    public int CurrentScore => _totalScore;

    private void Awake()
    {
        Instance = this;

        if (notifyPanel != null) notifyPanel.SetActive(false);
        if (timerPanel != null) timerPanel.SetActive(false);

        UpdateScoreText();
        if (scoreGaugeImage != null) scoreGaugeImage.fillAmount = 0f;
    }

    private void Update()
    {
        if (scoreGaugeImage != null)
        {
            float targetFill = Mathf.Clamp01((float)_totalScore / targetScore);
            _currentGaugeFill = Mathf.Lerp(_currentGaugeFill, targetFill, Time.deltaTime * gaugeSpeed);
            scoreGaugeImage.fillAmount = _currentGaugeFill;
        }
    }

    public void SetTimerActive(bool active, int potentialAmount = 0)
    {
        if (timerPanel != null)
            timerPanel.SetActive(active);

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
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = $"{_totalScore:N0}";
    }

    public void ShowNotification(string message)
    {
        if (notifyText == null || notifyPanel == null) return;

        notifyText.text = message;
        notifyPanel.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(HideNotificationRoutine());
    }

    private IEnumerator HideNotificationRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        if (notifyPanel != null)
            notifyPanel.SetActive(false);
    }
}
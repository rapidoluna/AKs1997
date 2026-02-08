using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealingHUD : MonoBehaviour
{
    [SerializeField] private PlayerHealing playerHealing;
    [SerializeField] private GameObject healingStatusPanel;
    [SerializeField] private Slider healingProgressBar;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI statusText;

    private float _localTimer;

    private void Update()
    {
        if (playerHealing == null) return;

        if (playerHealing.IsHealing)
        {
            if (!healingStatusPanel.activeSelf)
            {
                healingStatusPanel.SetActive(true);
                _localTimer = 0f;
            }

            _localTimer += Time.deltaTime;

            float duration = playerHealing.HealDuration;
            float progress = Mathf.Clamp01(_localTimer / duration);

            if (healingProgressBar != null)
                healingProgressBar.value = progress;

            if (timeText != null)
            {
                float remaining = Mathf.Max(0f, duration - _localTimer);
                timeText.text = $"{remaining:F1}s";
            }
        }
        else
        {
            if (healingStatusPanel.activeSelf)
            {
                healingStatusPanel.SetActive(false);
                _localTimer = 0f;
            }
        }
    }
}
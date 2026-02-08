using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealingHUD : MonoBehaviour
{
    [SerializeField] private PlayerHealing playerHealing;

    [Header("Healing Status UI")]
    [SerializeField] private GameObject healingPanel;
    [SerializeField] private Slider healingSlider;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Medkit Info UI")]
    [SerializeField] private GameObject[] medkitCells;
    [SerializeField] private Image medkitIcon;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color emptyColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    private float _timer;

    private void Update()
    {
        if (playerHealing == null) return;

        UpdateHealingStatus();
        UpdateMedkitInfo();
    }

    private void UpdateHealingStatus()
    {
        if (playerHealing.IsHealing)
        {
            if (healingPanel != null && !healingPanel.activeSelf)
            {
                healingPanel.SetActive(true);
                _timer = 0f;
            }

            _timer += Time.deltaTime;

            if (healingSlider != null)
            {
                healingSlider.value = Mathf.Clamp01(_timer / playerHealing.HealDuration);
            }

            if (timeText != null)
            {
                float remaining = Mathf.Max(0f, playerHealing.HealDuration - _timer);
                timeText.text = $"{remaining:F1}s";
            }
        }
        else
        {
            if (healingPanel != null && healingPanel.activeSelf)
            {
                healingPanel.SetActive(false);
                _timer = 0f;
            }
        }
    }

    private void UpdateMedkitInfo()
    {
        int currentCount = playerHealing.CurrentMedkits;

        if (medkitCells != null)
        {
            for (int i = 0; i < medkitCells.Length; i++)
            {
                if (medkitCells[i] != null)
                {
                    bool shouldBeActive = i < currentCount;
                    if (medkitCells[i].activeSelf != shouldBeActive)
                    {
                        medkitCells[i].SetActive(shouldBeActive);
                    }
                }
            }
        }

        if (medkitIcon != null)
        {
            bool isAvailable = currentCount > 0 && !playerHealing.IsHealing;
            medkitIcon.color = isAvailable ? normalColor : emptyColor;
        }
    }
}
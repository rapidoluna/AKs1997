using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private PlayerHealing playerHealing;

    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image healPreviewBarFill;
    [SerializeField] private Image bonusHealthBarFill;
    [SerializeField] private float lerpSpeed = 5f;
    [SerializeField] private float maxBonusDisplay = 100f;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            playerHealing = player.GetComponent<PlayerHealing>();
        }
    }

    void Update()
    {
        if (playerHealth == null) return;

        float currentHealthRatio = playerHealth.CurrentHealth / playerHealth.BaseMaxHealth;

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, currentHealthRatio, Time.deltaTime * lerpSpeed);
        }

        if (healPreviewBarFill != null)
        {
            if (playerHealing != null && playerHealing.IsHealing)
            {
                float healAmount = 30f;
                float previewTarget = Mathf.Min((playerHealth.CurrentHealth + healAmount) / playerHealth.BaseMaxHealth, 1f);

                healPreviewBarFill.fillAmount = Mathf.Lerp(healPreviewBarFill.fillAmount, previewTarget, Time.deltaTime * lerpSpeed);

                if (!healPreviewBarFill.gameObject.activeSelf)
                    healPreviewBarFill.gameObject.SetActive(true);
            }
            else
            {
                healPreviewBarFill.fillAmount = healthBarFill.fillAmount;
                if (healPreviewBarFill.gameObject.activeSelf && Mathf.Abs(healPreviewBarFill.fillAmount - healthBarFill.fillAmount) < 0.01f)
                {
                    healPreviewBarFill.gameObject.SetActive(false);
                }
            }
        }

        if (bonusHealthBarFill != null)
        {
            float bonusTarget = (playerHealth.BonusMaxHealth > 0)
                ? (playerHealth.CurrentBonusHealth / maxBonusDisplay)
                : 0f;

            bonusHealthBarFill.fillAmount = Mathf.Lerp(bonusHealthBarFill.fillAmount, bonusTarget, Time.deltaTime * lerpSpeed);

            if (bonusHealthBarFill.fillAmount > 0.001f || playerHealth.BonusMaxHealth > 0)
            {
                if (!bonusHealthBarFill.gameObject.activeSelf)
                    bonusHealthBarFill.gameObject.SetActive(true);
            }
            else
            {
                if (bonusHealthBarFill.gameObject.activeSelf)
                {
                    bonusHealthBarFill.fillAmount = 0;
                    bonusHealthBarFill.gameObject.SetActive(false);
                }
            }
        }
    }
}
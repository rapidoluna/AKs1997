using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    private PlayerHealth playerHealth;

    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image bonusHealthBarFill;
    [SerializeField] private float lerpSpeed = 5f;
    [SerializeField] private float maxBonusDisplay = 100f;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (playerHealth == null) return;

        if (healthBarFill != null)
        {
            float healthTarget = playerHealth.CurrentHealth / playerHealth.BaseMaxHealth;
            healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, healthTarget, Time.deltaTime * lerpSpeed);
        }

        if (bonusHealthBarFill != null)
        {
            bool hasBonus = playerHealth.BonusMaxHealth > 0;

            if (bonusHealthBarFill.gameObject.activeSelf != hasBonus)
                bonusHealthBarFill.gameObject.SetActive(hasBonus);

            if (hasBonus)
            {
                float bonusTarget = playerHealth.CurrentBonusHealth / maxBonusDisplay;
                bonusHealthBarFill.fillAmount = Mathf.Lerp(bonusHealthBarFill.fillAmount, bonusTarget, Time.deltaTime * lerpSpeed);
            }
        }
    }
}
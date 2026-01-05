using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    private PlayerHealth playerHealth;

    [SerializeField] private Image healthBarFill;
    [SerializeField] private float lerpSpeed = 5f;

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
        if (playerHealth == null || healthBarFill == null) return;

        float targetFill = playerHealth.CurrentHealth / playerHealth.MaxHealth;
        healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, targetFill, Time.deltaTime * lerpSpeed);
    }
}
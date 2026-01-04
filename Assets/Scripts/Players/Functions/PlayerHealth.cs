using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PlayerWalking walking;
    private float currentHealth;
    private float maxHealth;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    void Awake()
    {
        walking = GetComponent<PlayerWalking>();
    }

    void Start()
    {
        if (walking != null && walking.characterData != null)
        {
            maxHealth = walking.characterData.MaxHealth;
            currentHealth = maxHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player Dead");
    }
}
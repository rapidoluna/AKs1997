using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private CharacterData characterData;
    private float currentHealth;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => characterData != null ? characterData.MaxHealth : 100f;

    void Awake()
    {
        if (characterData != null)
        {
            currentHealth = characterData.MaxHealth;
        }
        else
        {
            currentHealth = 100f;
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player Dead");
    }
}
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyData data;
    private int _currentHealth;

    private void Awake()
    {
        if (data != null)
        {
            _currentHealth = data.maxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0) Die();
    }

    private void Die()
    {
        if (data.dropItemPrefab != null && Random.Range(0, 100) <= data.dropRate)
        {
            Instantiate(data.dropItemPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
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

    private bool _isDead = false;

    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        DropItem();

        Destroy(gameObject);
    }

    private void DropItem()
    {
        if (data == null || data.dropItemPrefab == null) return;

        // 확률 체크 (EnemyData의 dropRate 사용)
        float randomValue = Random.Range(0f, 100f);
        if (randomValue <= data.dropRate)
        {
            Instantiate(data.dropItemPrefab, transform.position, Quaternion.identity);
        }
    }
}
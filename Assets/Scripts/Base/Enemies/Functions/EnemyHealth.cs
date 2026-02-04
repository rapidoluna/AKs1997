using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyData data;
    private int _currentHealth;
    private bool _isDead = false;
    private EnemySpawn _mySpawner;

    private void Awake()
    {
        if (data != null) _currentHealth = data.maxHealth;
    }

    public void SetSpawner(EnemySpawn spawner) => _mySpawner = spawner;

    public void TakeDamage(int damage)
    {
        if (_isDead) return;
        _currentHealth -= damage;
        if (_currentHealth <= 0) Die();
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        if (GameSessionManager.Instance != null) GameSessionManager.Instance.kills++;

        STSNGStation station = Object.FindAnyObjectByType<STSNGStation>();
        if (station != null)
        {
            station.AddStoredCashFromEnemy(data.maxHealth * 10);
        }

        if (_mySpawner != null) _mySpawner.OnEnemyDestroyed();
        DropItems();
        Destroy(gameObject);
    }

    private void DropItems()
    {
        if (data == null || data.dropTable == null) return;
        foreach (var drop in data.dropTable)
        {
            if (Random.Range(0f, 100f) <= drop.dropRate)
                Instantiate(drop.itemPrefab, transform.position, Quaternion.identity);
        }
    }
}
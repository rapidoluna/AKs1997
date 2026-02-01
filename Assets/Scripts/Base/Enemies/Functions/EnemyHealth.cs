using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyData data;
    private int _currentHealth;
    private bool _isDead = false;
    private EnemySpawn _mySpawner;

    private void Awake()
    {
        if (data != null)
        {
            _currentHealth = data.maxHealth;
        }
    }

    public void SetSpawner(EnemySpawn spawner)
    {
        _mySpawner = spawner;
    }

    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        EnemyPatternController pattern = GetComponent<EnemyPatternController>();
        if (pattern != null) pattern.TryDodge();

        EnemyDetect detector = GetComponent<EnemyDetect>();
        if (detector != null)
        {
            detector.StartForceTracking(10f);

            EnemyController controller = GetComponent<EnemyController>();
            if (controller != null && controller.player != null && EnemyGroup.Instance != null)
            {
                EnemyGroup.Instance.ReportTarget(controller.player.position, detector);
            }
        }

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

        if (_mySpawner != null)
        {
            _mySpawner.OnEnemyDestroyed();
        }

        DropItems();
        Destroy(gameObject);
    }

    private void DropItems()
    {
        if (data == null || data.dropTable == null || data.dropTable.Count == 0) return;

        foreach (var drop in data.dropTable)
        {
            if (drop.itemPrefab == null) continue;

            float randomValue = Random.Range(0f, 100f);
            if (randomValue <= drop.dropRate)
            {
                Instantiate(drop.itemPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
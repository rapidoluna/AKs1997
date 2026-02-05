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
        if (_currentHealth <= 0) Die();
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        if (GameSessionManager.Instance != null && data != null)
        {
            int killScore = data.maxHealth * 10;
            GameSessionManager.Instance.kills++;
            GameSessionManager.Instance.AddInstantScore(killScore);

            if (CashRushHUD.Instance != null)
            {
                CashRushHUD.Instance.AddScore(killScore);
            }
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
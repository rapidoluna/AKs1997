using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int maxEnemies = 10;

    private int _currentEnemyCount;

    public GameObject SpawnEnemyExplicitly()
    {
        if (_currentEnemyCount >= maxEnemies) return null;

        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        RegisterEnemy(enemy);
        _currentEnemyCount++;
        return enemy;
    }

    private void RegisterEnemy(GameObject enemy)
    {
        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.SetSpawner(this);
        }
    }

    public void OnEnemyDestroyed()
    {
        _currentEnemyCount--;
    }
}
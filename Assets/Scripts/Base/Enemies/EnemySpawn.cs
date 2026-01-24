using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxEnemies = 10;

    private float _lastSpawnTime;
    private int _currentEnemyCount;

    private void Update()
    {
        if (_currentEnemyCount < maxEnemies && Time.time >= _lastSpawnTime + spawnInterval)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        _lastSpawnTime = Time.time;
        _currentEnemyCount++;
    }
}
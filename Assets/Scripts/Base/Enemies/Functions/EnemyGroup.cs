using UnityEngine;
using System.Collections.Generic;

public class EnemyGroup : MonoBehaviour
{
    public static EnemyGroup Instance { get; private set; }

    [SerializeField] private float shareRange = 20f;
    private List<EnemyDetect> _activeEnemies = new List<EnemyDetect>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterEnemy(EnemyDetect enemy)
    {
        if (!_activeEnemies.Contains(enemy))
            _activeEnemies.Add(enemy);
    }

    public void UnregisterEnemy(EnemyDetect enemy)
    {
        if (_activeEnemies.Contains(enemy))
            _activeEnemies.Remove(enemy);
    }

    public void ReportTarget(Vector3 targetPosition, EnemyDetect reporter)
    {
        foreach (var enemy in _activeEnemies)
        {
            if (enemy == reporter || enemy == null) continue;

            float distance = Vector3.Distance(reporter.transform.position, enemy.transform.position);
            if (distance <= shareRange)
            {
                enemy.OnProjectileDetected(targetPosition);
            }
        }
    }
}
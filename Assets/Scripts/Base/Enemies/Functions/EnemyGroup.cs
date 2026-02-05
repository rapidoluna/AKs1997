using UnityEngine;
using System.Collections.Generic;

public class EnemyGroup : MonoBehaviour
{
    public static EnemyGroup Instance { get; private set; }

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
        if (reporter == null) return;

        EnemyController controller = reporter.GetComponent<EnemyController>();
        if (controller == null || controller.data == null) return;

        float range = controller.data.shareRange;

        foreach (var enemy in _activeEnemies)
        {
            if (enemy == reporter || enemy == null) continue;

            float distance = Vector3.Distance(reporter.transform.position, enemy.transform.position);
            if (distance <= range)
            {
                enemy.OnProjectileDetected(targetPosition);
            }
        }
    }
}
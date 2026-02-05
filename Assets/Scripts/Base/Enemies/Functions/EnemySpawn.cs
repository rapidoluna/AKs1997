using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject dropPodPrefab;
    [SerializeField] private int maxEnemies = 3;

    private int _currentEnemyCount;

    public GameObject SpawnEnemyExplicitly()
    {
        if (_currentEnemyCount >= maxEnemies) return null;

        GameObject podObj = Instantiate(dropPodPrefab, transform.position, Quaternion.identity);
        DropPod pod = podObj.GetComponent<DropPod>();

        if (pod != null)
        {
            pod.Init(transform.position, this);
        }

        return podObj;
    }

    public void AddEnemyCount(int count)
    {
        _currentEnemyCount += count;
    }

    public void OnEnemyDestroyed()
    {
        _currentEnemyCount--;
    }
}
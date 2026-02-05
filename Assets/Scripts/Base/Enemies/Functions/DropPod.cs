using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropPod : MonoBehaviour
{
    [System.Serializable]
    public struct WeightedEnemy
    {
        public GameObject enemyPrefab;
        public int weight;
    }

    [SerializeField] private WeightedEnemy[] weightedEnemies;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float fallSpeed = 60f;
    [SerializeField] private GameObject impactEffect;

    private Vector3 _targetPosition;
    private bool _hasLanded = false;
    private EnemySpawn _sourceSpawner;

    public void Init(Vector3 landingPos, EnemySpawn spawner)
    {
        _targetPosition = landingPos;
        _sourceSpawner = spawner;
        transform.position = landingPos + Vector3.up * 100f;
    }

    private void Update()
    {
        if (_hasLanded) return;
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, fallSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _targetPosition) < 0.1f) Land();
    }

    private void Land()
    {
        _hasLanded = true;
        if (impactEffect) Instantiate(impactEffect, transform.position, Quaternion.identity);
        StartCoroutine(OpenAndSpawn());
    }

    private IEnumerator OpenAndSpawn()
    {
        yield return new WaitForSeconds(0.8f);

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject selectedPrefab = GetWeightedRandomEnemy();
            if (selectedPrefab != null)
            {
                GameObject enemy = Instantiate(selectedPrefab, spawnPoints[i].position, Quaternion.identity);
                StartCoroutine(DelayedAIActivation(enemy));

                EnemyHealth health = enemy.GetComponent<EnemyHealth>();
                if (health != null && _sourceSpawner != null)
                {
                    health.SetSpawner(_sourceSpawner);
                    _sourceSpawner.AddEnemyCount(1);
                }
            }
            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    private IEnumerator DelayedAIActivation(GameObject enemy)
    {
        var controller = enemy.GetComponent<EnemyPatternController>();
        var detect = enemy.GetComponent<EnemyDetect>();

        if (controller != null) controller.enabled = false;
        if (detect != null) detect.enabled = false;

        yield return new WaitForSeconds(1.0f);

        if (controller != null) controller.enabled = true;
        if (detect != null) detect.enabled = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && detect != null)
        {
            detect.OnProjectileDetected(player.transform.position);
        }
    }

    private GameObject GetWeightedRandomEnemy()
    {
        int totalWeight = 0;
        foreach (var we in weightedEnemies) totalWeight += we.weight;
        if (totalWeight <= 0) return null;

        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var we in weightedEnemies)
        {
            currentWeight += we.weight;
            if (randomValue < currentWeight) return we.enemyPrefab;
        }
        return null;
    }
}
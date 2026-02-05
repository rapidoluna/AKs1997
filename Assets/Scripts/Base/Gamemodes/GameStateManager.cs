using UnityEngine;
using System;
using System.Collections;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    [SerializeField] private int targetScore = 50000;
    [SerializeField] private GameObject dropPodPrefab;
    [SerializeField] private float spawnRadius = 15f;
    private bool _isEscapeReady = false;

    public bool IsEscapeReady => _isEscapeReady;
    public event Action OnEscapeAvailable;

    private void Awake()
    {
        Instance = this;
    }

    public void CheckScore(int currentScore)
    {
        if (_isEscapeReady) return;

        if (currentScore >= targetScore)
        {
            ActivateEscape();
        }
    }

    private void ActivateEscape()
    {
        _isEscapeReady = true;
        OnEscapeAvailable?.Invoke();

        if (CashRushHUD.Instance != null)
            CashRushHUD.Instance.ShowNotification("≈ª√‚ ∞°¥…");
    }

    public void TriggerCashRushArrival(Vector3 stationPos)
    {
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(DelayedCall(stationPos, i * 1.2f));
        }
    }

    private IEnumerator DelayedCall(Vector3 stationPos, float delay)
    {
        yield return new WaitForSeconds(delay);
        CallRandomDropPod(stationPos);
    }

    private void CallRandomDropPod(Vector3 centerPos)
    {
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(8f, spawnRadius);
        Vector3 spawnPos = centerPos + new Vector3(randomPoint.x, 0, randomPoint.y);

        RaycastHit hit;
        if (Physics.Raycast(spawnPos + Vector3.up * 20f, Vector3.down, out hit, 40f))
        {
            spawnPos = hit.point;
        }

        GameObject podObj = Instantiate(dropPodPrefab, spawnPos, Quaternion.identity);
        DropPod pod = podObj.GetComponent<DropPod>();
        if (pod != null) pod.Init(spawnPos, null);
    }
}
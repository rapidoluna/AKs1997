using UnityEngine;

public class DamageIndicatorManager : MonoBehaviour
{
    public GameObject indicatorPrefab;
    public Transform playerTransform;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        if (playerTransform != null)
            playerHealth = playerTransform.GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnDamageTaken += CreateIndicator;
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnDamageTaken -= CreateIndicator;
    }

    private void CreateIndicator(Vector3 attackerPosition)
    {
        if (indicatorPrefab == null) return;
        GameObject newIndicator = Instantiate(indicatorPrefab, transform);
        DamageIndicator indicatorScript = newIndicator.GetComponent<DamageIndicator>();
        if (indicatorScript != null)
        {
            indicatorScript.SetTarget(attackerPosition, playerTransform);
        }
    }
}
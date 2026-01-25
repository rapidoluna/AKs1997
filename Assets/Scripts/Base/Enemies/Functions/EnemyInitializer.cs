using UnityEngine;

public class EnemyInitializer : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    private EnemyController _controller;
    private EnemyAttack _attack;

    private void Awake()
    {
        _controller = GetComponent<EnemyController>();
        _attack = GetComponent<EnemyAttack>();

        if (enemyData == null || enemyData.enemyPrefab == null) return;

        GameObject model = Instantiate(enemyData.enemyPrefab, transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;

        Transform fp = model.transform.Find("FirePoint");
        if (fp != null && _attack != null) _attack.SetFirePoint(fp);
    }
}
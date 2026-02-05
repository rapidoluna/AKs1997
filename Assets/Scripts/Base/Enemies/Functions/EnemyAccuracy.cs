using UnityEngine;

public class EnemyAccuracy : MonoBehaviour
{
    [SerializeField] private float spreadAngle = 3.5f;

    public Quaternion GetSpreadRotation(Vector3 direction)
    {
        float x = Random.Range(-spreadAngle, spreadAngle);
        float y = Random.Range(-spreadAngle, spreadAngle);
        return Quaternion.LookRotation(direction) * Quaternion.Euler(x, y, 0);
    }
}
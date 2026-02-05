using UnityEngine;
using System.Collections;

public class EnemyWalking : MonoBehaviour
{
    private EnemyController _controller;

    private void Awake()
    {
        _controller = GetComponent<EnemyController>();
    }

    private void OnEnable()
    {
        _controller.agent.speed = _controller.data.walkSpeed;
        StartCoroutine(LookAroundRoutine());
    }

    private IEnumerator LookAroundRoutine()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(Random.Range(2f, 4f));
            float scanAngle = Random.Range(-30f, 30f);
            transform.Rotate(Vector3.up, scanAngle);
        }
    }
}
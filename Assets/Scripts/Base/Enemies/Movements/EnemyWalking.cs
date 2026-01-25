using UnityEngine;

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
    }

    private void Update()
    {
    }
}
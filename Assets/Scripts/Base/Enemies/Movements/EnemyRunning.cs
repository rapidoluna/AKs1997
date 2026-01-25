using UnityEngine;

public class EnemyRunning : MonoBehaviour
{
    private EnemyController _controller;

    private void Awake()
    {
        _controller = GetComponent<EnemyController>();
    }

    private void OnEnable()
    {
        _controller.agent.speed = _controller.data.runSpeed;
        _controller.agent.stoppingDistance = _controller.data.attackRange * 0.8f;
    }

    private void Update()
    {
        if (_controller.player != null)
        {
            _controller.agent.SetDestination(_controller.player.position);
        }
    }
}
using UnityEngine;

public class EnemyDetect : MonoBehaviour
{
    private EnemyController _controller;
    private EnemyRunning _running;
    private EnemyWalking _walking;

    private void Awake()
    {
        _controller = GetComponent<EnemyController>();
        _running = GetComponent<EnemyRunning>();
        _walking = GetComponent<EnemyWalking>();
    }

    private void Update()
    {
        if (_controller.player == null || PlayerHealth.IsDead)
        {
            _running.enabled = false;
            _walking.enabled = true;
            return;
        }

        float distance = Vector3.Distance(transform.position, _controller.player.position);

        if (distance <= _controller.data.detectRange)
        {
            _running.enabled = true;
            _walking.enabled = false;
        }
        else
        {
            _running.enabled = false;
            _walking.enabled = true;
        }
    }
}
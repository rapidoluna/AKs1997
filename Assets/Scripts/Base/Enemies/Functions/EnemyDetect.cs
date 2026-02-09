using UnityEngine;
using System.Collections;

public class EnemyDetect : MonoBehaviour
{
    [SerializeField] private EnemyController _controller;
    [SerializeField] private EnemyRunning _running;
    [SerializeField] private EnemyWalking _walking;

    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float chaseMaintainDistance = 20f;

    private bool _isForceTracking = false;
    private Coroutine _forceTrackCoroutine;

    private void Awake()
    {
        if (_controller == null) _controller = GetComponent<EnemyController>();
        if (_running == null) _running = GetComponent<EnemyRunning>();
        if (_walking == null) _walking = GetComponent<EnemyWalking>();
    }

    private void Start()
    {
        if (EnemyGroup.Instance != null) EnemyGroup.Instance.RegisterEnemy(this);
    }

    private void OnDestroy()
    {
        if (EnemyGroup.Instance != null) EnemyGroup.Instance.UnregisterEnemy(this);
    }

    public Transform GetPlayerTransform() => _controller.player;

    private void Update()
    {
        if (_controller.player == null || PlayerHealth.IsDead)
        {
            EnterPatrolMode();
            return;
        }

        float distance = Vector3.Distance(transform.position, _controller.player.position);

        if (_running.enabled || _isForceTracking)
        {
            if (distance > chaseMaintainDistance)
            {
                EnterPatrolMode();
                return;
            }

            if (!_isForceTracking)
            {
                _controller.agent.SetDestination(_controller.player.position);
            }

            if (CanSeePlayer())
            {
                StopForceTracking();
                StartCombat(distance);
            }
            return;
        }

        if (CanSeePlayer())
        {
            StartCombat(distance);
        }
    }

    private void StartCombat(float distance)
    {
        _running.enabled = true;
        _walking.enabled = false;
        _controller.agent.SetDestination(_controller.player.position);

        if (_controller.data.pattern == EnemyPattern.Tactical && EnemyGroup.Instance != null)
        {
            EnemyGroup.Instance.ReportTarget(_controller.player.position, this);
        }
    }

    private void EnterPatrolMode()
    {
        StopForceTracking();
        if (_running) _running.enabled = false;
        if (_walking) _walking.enabled = true;
        if (_controller.agent.hasPath) _controller.agent.ResetPath();
    }

    private bool CanSeePlayer()
    {
        float distance = Vector3.Distance(transform.position, _controller.player.position);
        if (distance > _controller.data.detectRange) return false;

        Vector3 dir = (_controller.player.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dir) <= viewAngle * 0.5f)
        {
            RaycastHit hit;
            Vector3 eye = transform.position + Vector3.up * 1.5f;
            Vector3 target = _controller.player.position + Vector3.up * 1.5f;
            if (Physics.Raycast(eye, (target - eye).normalized, out hit, _controller.data.detectRange))
            {
                if (hit.collider.CompareTag("Player")) return true;
            }
        }
        return false;
    }

    public void OnProjectileDetected(Vector3 origin)
    {
        if (Vector3.Distance(transform.position, origin) > chaseMaintainDistance) return;
        StartForceTracking(origin, 7f);
    }

    public void StartForceTracking(Vector3 targetPos, float duration)
    {
        if (_forceTrackCoroutine != null) StopCoroutine(_forceTrackCoroutine);
        _forceTrackCoroutine = StartCoroutine(ForceTrackRoutine(targetPos, duration));
    }

    public IEnumerator ForceTrackRoutine(Vector3 targetPos, float duration)
    {
        _isForceTracking = true;
        _running.enabled = true;
        _walking.enabled = false;

        _controller.agent.SetDestination(targetPos);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (Vector3.Distance(transform.position, targetPos) < 1.5f)
            {
                transform.Rotate(Vector3.up, 360f * Time.deltaTime);

                if (CanSeePlayer())
                {
                    _isForceTracking = false;
                    StartCombat(Vector3.Distance(transform.position, _controller.player.position));
                    yield break;
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        _isForceTracking = false;
    }

    public void StopForceTracking()
    {
        _isForceTracking = false;
        if (_forceTrackCoroutine != null)
        {
            StopCoroutine(_forceTrackCoroutine);
            _forceTrackCoroutine = null;
        }
    }
}
using UnityEngine;
using System.Collections;

public class EnemyDetect : MonoBehaviour
{
    private EnemyController _controller;
    private EnemyRunning _running;
    private EnemyWalking _walking;
    private bool _isForceTracking = false;
    private Coroutine _forceTrackCoroutine;
    private Vector3 _lastKnownPosition;
    private bool _isSearching = false;

    [SerializeField] private float viewAngle = 90f;

    private void Awake()
    {
        _controller = GetComponent<EnemyController>();
        _running = GetComponent<EnemyRunning>();
        _walking = GetComponent<EnemyWalking>();
    }

    private void Start()
    {
        if (EnemyGroup.Instance != null)
            EnemyGroup.Instance.RegisterEnemy(this);
    }

    private void OnDestroy()
    {
        if (EnemyGroup.Instance != null)
            EnemyGroup.Instance.UnregisterEnemy(this);
    }

    private void Update()
    {
        if (_controller.player == null || PlayerHealth.IsDead)
        {
            StopForceTracking();
            _isSearching = false;
            _running.enabled = false;
            _walking.enabled = true;
            return;
        }

        if (_isForceTracking) return;

        if (CanSeePlayer())
        {
            _isSearching = false;
            _running.enabled = true;
            _walking.enabled = false;

            if (EnemyGroup.Instance != null)
                EnemyGroup.Instance.ReportTarget(_controller.player.position, this);
        }
        else
        {
            if (!_isSearching && _running.enabled)
            {
                StartCoroutine(SearchLastPosition());
            }
        }
    }

    private bool CanSeePlayer()
    {
        float distance = Vector3.Distance(transform.position, _controller.player.position);
        if (distance > _controller.data.detectRange) return false;

        Vector3 directionToPlayer = (_controller.player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle <= viewAngle * 0.5f)
        {
            RaycastHit hit;
            Vector3 startPos = transform.position + Vector3.up * 1.5f;
            Vector3 targetPos = _controller.player.position + Vector3.up * 1.5f;
            Vector3 rayDirection = (targetPos - startPos).normalized;

            if (Physics.Raycast(startPos, rayDirection, out hit, _controller.data.detectRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void OnProjectileDetected(Vector3 soundOrigin)
    {
        if (_isForceTracking) return;
        _lastKnownPosition = soundOrigin;
        StartForceTracking(8f);
    }

    public void StartForceTracking(float duration)
    {
        if (_forceTrackCoroutine != null) StopCoroutine(_forceTrackCoroutine);
        _forceTrackCoroutine = StartCoroutine(ForceTrackRoutine(duration));
    }

    private IEnumerator ForceTrackRoutine(float duration)
    {
        _isForceTracking = true;
        _running.enabled = true;
        _walking.enabled = false;

        if (_controller.player != null)
            _controller.agent.SetDestination(_controller.player.position);

        yield return new WaitForSeconds(duration);

        _isForceTracking = false;
    }

    private IEnumerator SearchLastPosition()
    {
        _isSearching = true;
        _controller.agent.SetDestination(_controller.player.position);

        while (_controller.agent.remainingDistance > 0.5f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        _running.enabled = false;
        _walking.enabled = true;
        _isSearching = false;
    }

    private void StopForceTracking()
    {
        _isForceTracking = false;
        if (_forceTrackCoroutine != null)
        {
            StopCoroutine(_forceTrackCoroutine);
            _forceTrackCoroutine = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (_controller == null || _controller.data == null)
        {
            _controller = GetComponent<EnemyController>();
            if (_controller == null || _controller.data == null) return;
        }

        float range = _controller.data.detectRange;
        Vector3 eyePos = transform.position + Vector3.up * 1.5f;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(eyePos, range);

        Gizmos.color = Color.yellow;
        Vector3 leftDir = Quaternion.AngleAxis(-viewAngle * 0.5f, Vector3.up) * transform.forward;
        Vector3 rightDir = Quaternion.AngleAxis(viewAngle * 0.5f, Vector3.up) * transform.forward;

        Gizmos.DrawRay(eyePos, leftDir * range);
        Gizmos.DrawRay(eyePos, rightDir * range);

        if (_running != null && _running.enabled)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(eyePos, _controller.player.position + Vector3.up * 1.5f);
        }
    }
}
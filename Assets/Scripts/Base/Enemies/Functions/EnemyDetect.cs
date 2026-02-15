using UnityEngine;
using System.Collections;

public class EnemyDetect : MonoBehaviour
{
    [SerializeField] private EnemyController _controller;
    [SerializeField] private EnemyRunning _running;
    [SerializeField] private EnemyWalking _walking;

    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float chaseMaintainDistance = 20f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionGauge = 0f;
    [SerializeField] private float detectionThreshold = 100f;
    [SerializeField] private float detectionIncreaseRate = 50f;
    [SerializeField] private float detectionDecreaseRate = 20f;
    [SerializeField] private float suspectThreshold = 30f;

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
            detectionGauge = 0f;
            EnterPatrolMode();
            return;
        }

        float distance = Vector3.Distance(transform.position, _controller.player.position);
        bool canSee = CanSeePlayer();

        if (canSee)
        {
            float distanceFactor = 1f - Mathf.Clamp01(distance / chaseMaintainDistance);
            detectionGauge += detectionIncreaseRate * distanceFactor * Time.deltaTime;

            if (detectionGauge >= detectionThreshold)
            {
                if (!_running.enabled && !_isForceTracking)
                {
                    StartCombat(distance);
                }
            }
            else if (detectionGauge >= suspectThreshold)
            {
                if (!_isForceTracking)
                {
                    StartForceTracking(_controller.player.position, 3f);
                }
            }
        }
        else
        {
            if (!_running.enabled && !_isForceTracking)
            {
                detectionGauge = Mathf.Max(0, detectionGauge - detectionDecreaseRate * Time.deltaTime);
            }
        }

        if (_running.enabled || _isForceTracking)
        {
            if (distance > chaseMaintainDistance && !_isForceTracking)
            {
                detectionGauge = 0f;
                EnterPatrolMode();
            }
        }
    }

    public bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (_controller.player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle < viewAngle * 0.5f)
        {
            if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out RaycastHit hit, chaseMaintainDistance))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void EnterPatrolMode()
    {
        _running.enabled = false;
        _walking.enabled = true;
    }

    private void StartCombat(float distance)
    {
        _isForceTracking = false;
        if (_forceTrackCoroutine != null) StopCoroutine(_forceTrackCoroutine);

        _running.enabled = true;
        _walking.enabled = false;

        if (EnemyGroup.Instance != null)
        {
            EnemyGroup.Instance.ReportTarget(_controller.player.position, this);
        }
    }

    public void OnProjectileDetected(Vector3 targetPosition)
    {
        detectionGauge += 40f;
        if (detectionGauge >= detectionThreshold)
        {
            StartCombat(Vector3.Distance(transform.position, targetPosition));
        }
        else
        {
            StartForceTracking(targetPosition, 5f);
        }
    }

    public void OnDamageTaken(Vector3 origin)
    {
        detectionGauge = detectionThreshold;
        if (EnemyGroup.Instance != null)
        {
            EnemyGroup.Instance.ReportTarget(origin, this);
        }
        StartCombat(Vector3.Distance(transform.position, origin));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            Vector3 sourcePos = other.transform.position - other.transform.forward * 2f;
            OnProjectileDetected(sourcePos);
        }
    }

    public void OnSoundHeard(Vector3 origin, float volume)
    {
        float distance = Vector3.Distance(transform.position, origin);
        if (distance <= volume)
        {
            float noiseImpact = (1f - (distance / volume)) * 50f;
            detectionGauge += noiseImpact;

            if (!_running.enabled)
            {
                StartForceTracking(origin, 5f);
            }
        }
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
                    detectionGauge = detectionThreshold;
                    _isForceTracking = false;
                    StartCombat(Vector3.Distance(transform.position, _controller.player.position));
                    yield break;
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        _isForceTracking = false;
        if (detectionGauge < detectionThreshold) EnterPatrolMode();
    }

    public void StopForceTracking()
    {
        _isForceTracking = false;
        if (_forceTrackCoroutine != null) StopCoroutine(_forceTrackCoroutine);
        EnterPatrolMode();
    }
}
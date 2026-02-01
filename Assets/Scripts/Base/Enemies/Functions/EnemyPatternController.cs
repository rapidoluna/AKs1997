using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyPatternController : MonoBehaviour
{
    private EnemyController _controller;
    private bool _isDodging = false;
    private bool _calledBackup = false;
    private bool _isCovering = false;
    private float _lastMeleeTime;

    private void Awake()
    {
        _controller = GetComponent<EnemyController>();
    }

    private void Update()
    {
        if (_controller.player == null || PlayerHealth.IsDead) return;

        float distance = Vector3.Distance(transform.position, _controller.player.position);

        if (_controller.data.pattern == EnemyPattern.Tactical && !_calledBackup)
        {
            if (distance <= _controller.data.detectRange)
            {
                CallBackup();
            }
        }

        if (_controller.data.pattern == EnemyPattern.Tactical && !_isCovering && distance <= _controller.data.attackRange)
        {
            FindCover();
        }

        if (distance <= _controller.data.meleeRange)
        {
            if (_controller.data.pattern == EnemyPattern.Agile)
            {
                MaintainDistance();
            }
            else if (_controller.data.pattern == EnemyPattern.Tanker)
            {
                TryMeleeAttack();
            }
        }
    }

    public void TryDodge()
    {
        if (_isDodging || _controller.data.pattern != EnemyPattern.Agile) return;

        if (Random.value <= _controller.data.dodgeChance)
        {
            StartCoroutine(DodgeRoutine());
        }
    }

    private IEnumerator DodgeRoutine()
    {
        _isDodging = true;
        Vector3 dodgeDir = transform.right * (Random.value > 0.5f ? 1f : -1f);
        _controller.agent.velocity = dodgeDir * 12f;
        yield return new WaitForSeconds(0.4f);
        _isDodging = false;
    }

    private void MaintainDistance()
    {
        Vector3 directionFromPlayer = (transform.position - _controller.player.position).normalized;
        Vector3 targetPos = transform.position + directionFromPlayer * 5f;
        _controller.agent.SetDestination(targetPos);
    }

    private void TryMeleeAttack()
    {
        if (Time.time >= _lastMeleeTime + _controller.data.meleeCooldown)
        {
            _lastMeleeTime = Time.time;
            PlayerHealth playerHealth = _controller.player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_controller.data.meleeDamage);
            }
        }
    }

    private void FindCover()
    {
        GameObject[] covers = GameObject.FindGameObjectsWithTag("Cover");
        if (covers.Length == 0) return;

        GameObject bestCover = null;
        float closestDist = float.MaxValue;

        foreach (var cover in covers)
        {
            float dist = Vector3.Distance(transform.position, cover.transform.position);
            Vector3 dirToPlayer = (_controller.player.position - cover.transform.position).normalized;
            float dot = Vector3.Dot(cover.transform.forward, dirToPlayer);

            if (dist < closestDist && dot < 0)
            {
                closestDist = dist;
                bestCover = cover;
            }
        }

        if (bestCover != null)
        {
            _isCovering = true;
            _controller.agent.SetDestination(bestCover.transform.position);
            StartCoroutine(ResetCover());
        }
    }

    private IEnumerator ResetCover()
    {
        yield return new WaitForSeconds(5f);
        _isCovering = false;
    }

    private void CallBackup()
    {
        _calledBackup = true;
        EnemySpawn[] spawners = FindObjectsByType<EnemySpawn>(FindObjectsSortMode.None);
        Vector3 callPosition = transform.position;

        foreach (var spawner in spawners)
        {
            GameObject spawnedEnemy = spawner.SpawnEnemyExplicitly();
            if (spawnedEnemy != null)
            {
                EnemyController ec = spawnedEnemy.GetComponent<EnemyController>();
                if (ec != null && ec.agent != null)
                {
                    ec.agent.SetDestination(callPosition);
                }
            }
        }
    }
}
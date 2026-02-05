using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    private EnemyController _controller;
    private float _lastAttackTime;
    private float _lastMeleeTime;
    private Transform _firePoint;
    private bool _isMeleeAttacking = false;

    private void Awake()
    {
        _controller = GetComponent<EnemyController>();
    }

    public void SetFirePoint(Transform fp)
    {
        _firePoint = fp;
    }

    private void Update()
    {
        if (_controller.player == null || PlayerHealth.IsDead || _controller.data == null || _isMeleeAttacking) return;

        float distance = Vector3.Distance(transform.position, _controller.player.position);
        Vector3 dirToPlayer = (_controller.player.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dirToPlayer);

        if (dot > 0.5f)
        {
            if (_controller.data.pattern == EnemyPattern.Tanker && distance <= _controller.data.meleeRange)
            {
                if (Time.time >= _lastMeleeTime + _controller.data.meleeCooldown)
                {
                    StartCoroutine(MeleeAttackRoutine());
                }
            }
            else if (distance <= _controller.data.attackRange)
            {
                if (Time.time >= _lastAttackTime + _controller.data.attackCooldown)
                {
                    LaunchProjectile();
                }
            }
        }
    }

    private IEnumerator MeleeAttackRoutine()
    {
        _isMeleeAttacking = true;
        _lastMeleeTime = Time.time;

        Vector3 dashPos = transform.position + (transform.forward * 1.5f);
        float elapsed = 0f;
        while (elapsed < 0.15f)
        {
            _controller.agent.Move(transform.forward * Time.deltaTime * 10f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (Vector3.Distance(transform.position, _controller.player.position) <= _controller.data.meleeRange + 1f)
        {
            if (_controller.player.TryGetComponent(out PlayerHealth ph))
            {
                ph.TakeDamage(_controller.data.meleeDamage);
                if (_controller.player.TryGetComponent(out PlayerWalking pw))
                {
                    pw.ApplyStun(0.6f);
                    Vector3 knockbackDir = (_controller.player.position - transform.position).normalized;
                    knockbackDir.y = 0.2f;
                    pw.ApplyKnockback(knockbackDir * 15f);
                }
            }
        }

        yield return new WaitForSeconds(0.35f);
        _isMeleeAttacking = false;
    }

    private void LaunchProjectile()
    {
        _lastAttackTime = Time.time;
        if (BulletPool.Instance == null || _firePoint == null) return;

        GameObject bullet = BulletPool.Instance.GetBullet();
        bullet.transform.position = _firePoint.position;
        Vector3 direction = (_controller.player.position - _firePoint.position).normalized;
        bullet.transform.rotation = Quaternion.LookRotation(direction);

        Projectile projectile = bullet.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Init(_controller.data.bulletSpeed / 60f, _controller.data.attackDamage, _controller.data.attackRange * 2f, gameObject);
        }
    }
}
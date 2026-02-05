using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    private EnemyController _controller;
    private EnemyAccuracy _accuracy;
    private float _lastAttackTime;
    private float _lastMeleeTime;
    private Transform _firePoint;
    private bool _isMeleeAttacking = false;
    private bool _isBursting = false;

    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstInterval = 0.25f;

    private void Awake()
    {
        _controller = GetComponent<EnemyController>();
        _accuracy = GetComponent<EnemyAccuracy>();
    }

    public void SetFirePoint(Transform fp) => _firePoint = fp;

    private void Update()
    {
        if (_controller.player == null || PlayerHealth.IsDead || _controller.data == null || _isMeleeAttacking || _isBursting) return;

        float distance = Vector3.Distance(transform.position, _controller.player.position);

        Vector3 dirToPlayer = (_controller.player.position - transform.position).normalized;
        if (Vector3.Dot(transform.forward, dirToPlayer) < 0.3f) return;

        if (_controller.data.pattern == EnemyPattern.Tanker && distance <= _controller.data.meleeRange)
        {
            if (Time.time >= _lastMeleeTime + _controller.data.meleeCooldown)
                StartCoroutine(MeleeAttackRoutine());
        }
        else if (distance <= _controller.data.attackRange)
        {
            if (Time.time >= _lastAttackTime + _controller.data.attackCooldown)
                StartCoroutine(BurstFireRoutine());
        }
    }

    private IEnumerator BurstFireRoutine()
    {
        _isBursting = true;
        for (int i = 0; i < burstCount; i++)
        {
            if (_controller.player == null || PlayerHealth.IsDead) break;
            LaunchProjectile();
            yield return new WaitForSeconds(burstInterval);
        }
        _lastAttackTime = Time.time;
        _isBursting = false;
    }

    private IEnumerator MeleeAttackRoutine()
    {
        _isMeleeAttacking = true;
        _lastMeleeTime = Time.time;

        float elapsed = 0f;
        while (elapsed < 0.15f)
        {
            _controller.agent.Move(transform.forward * Time.deltaTime * 8f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (Vector3.Distance(transform.position, _controller.player.position) <= _controller.data.meleeRange + 1.2f)
        {
            if (_controller.player.TryGetComponent(out PlayerHealth ph))
            {
                ph.TakeDamage(_controller.data.meleeDamage);
                if (_controller.player.TryGetComponent(out PlayerWalking pw))
                {
                    pw.ApplyStun(0.5f);
                    Vector3 knockback = (_controller.player.position - transform.position).normalized;
                    pw.ApplyKnockback(knockback * 20f);
                }
            }
        }
        yield return new WaitForSeconds(0.4f);
        _isMeleeAttacking = false;
    }

    private void LaunchProjectile()
    {
        if (BulletPool.Instance == null || _firePoint == null) return;
        GameObject bullet = BulletPool.Instance.GetBullet();
        bullet.transform.position = _firePoint.position;
        Vector3 direction = (_controller.player.position - _firePoint.position).normalized;
        bullet.transform.rotation = _accuracy != null ? _accuracy.GetSpreadRotation(direction) : Quaternion.LookRotation(direction);

        Projectile projectile = bullet.GetComponent<Projectile>();
        if (projectile != null)
            projectile.Init(_controller.data.bulletSpeed / 60f, _controller.data.attackDamage, _controller.data.attackRange * 2f, gameObject);
    }
}
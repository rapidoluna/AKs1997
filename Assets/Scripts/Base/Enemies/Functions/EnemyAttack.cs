using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private EnemyController _controller;
    private float _lastAttackTime;
    private Transform _firePoint;

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
        if (_controller.player == null || PlayerHealth.IsDead || _firePoint == null || _controller.data == null) return;

        float distance = Vector3.Distance(transform.position, _controller.player.position);

        if (distance <= _controller.data.attackRange && Time.time >= _lastAttackTime + _controller.data.attackCooldown)
        {
            LaunchProjectile();
        }
    }

    private void LaunchProjectile()
    {
        _lastAttackTime = Time.time;
        if (BulletPool.Instance == null) return;

        GameObject bullet = BulletPool.Instance.GetBullet();
        bullet.transform.position = _firePoint.position;

        Vector3 direction = (_controller.player.position - _firePoint.position).normalized;
        bullet.transform.rotation = Quaternion.LookRotation(direction);

        Projectile projectile = bullet.GetComponent<Projectile>();
        if (projectile != null)
        {
            float speed = _controller.data.bulletSpeed / 60f;
            projectile.Init(speed, _controller.data.attackDamage, _controller.data.attackRange * 2f, gameObject);

            Collider[] enemyColliders = GetComponentsInChildren<Collider>();
            Collider bulletCollider = bullet.GetComponent<Collider>();

            if (bulletCollider != null)
            {
                foreach (var col in enemyColliders)
                {
                    Physics.IgnoreCollision(bulletCollider, col);
                }
            }
        }
    }
}
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private EnemyController _controller;
    private float _lastAttackTime;

    [SerializeField] private Transform firePoint;

    private void Awake()
    {
        _controller = GetComponent<EnemyController>();
    }

    private void Update()
    {
        if (_controller.player == null) return;

        float distance = Vector3.Distance(transform.position, _controller.player.position);

        // 공격 사거리 안에 있고 쿨타임이 지났을 때 실행
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

        bullet.transform.position = firePoint.position;

        Vector3 direction = (_controller.player.position - firePoint.position).normalized;
        bullet.transform.rotation = Quaternion.LookRotation(direction);

        Projectile projectile = bullet.GetComponent<Projectile>();
        if (projectile != null)
        {
            float speed = _controller.data.bulletSpeed / 60f;
            projectile.Init(speed, _controller.data.attackDamage, _controller.data.attackRange * 2f, gameObject);
        }
    }
}
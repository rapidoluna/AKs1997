using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _speed;
    private float _damage;
    private Vector3 _direction;
    private float _lifeTime = 3f;
    private float _spawnTime;

    public void Initialize(Vector3 direction, float damage, float speed)
    {
        _direction = direction;
        _damage = damage;
        _speed = speed;
        _spawnTime = Time.time;
    }

    private void Update()
    {
        transform.position += _direction * _speed * Time.deltaTime;

        if (Time.time - _spawnTime >= _lifeTime)
        {
            DisableBullet();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage((int)_damage);
        }

        DisableBullet();
    }

    private void DisableBullet()
    {
        if (BulletPool.Instance != null)
            BulletPool.Instance.ReturnBullet(gameObject);
        else
            Destroy(gameObject);
    }
}
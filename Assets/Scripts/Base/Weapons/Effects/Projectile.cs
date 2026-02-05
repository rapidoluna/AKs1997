using UnityEngine;

public class Projectile : MonoBehaviour
{
    protected float _speed;
    protected int _damage;
    protected float _range;
    protected GameObject _owner;
    protected Vector3 _startPosition;

    [SerializeField] protected float soundRadius = 5f;

    public virtual void Init(float speed, int damage, float range, GameObject owner)
    {
        _speed = speed;
        _damage = damage;
        _range = range;
        _owner = owner;
        _startPosition = transform.position;
    }

    protected virtual void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);

        if (_owner != null && _owner.CompareTag("Player"))
        {
            DetectNearbyEnemies();
        }

        if (Vector3.Distance(_startPosition, transform.position) >= _range)
        {
            DisableProjectile();
        }
    }

    protected virtual void DetectNearbyEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, soundRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                EnemyDetect detector = hitCollider.GetComponent<EnemyDetect>();
                if (detector != null)
                {
                    detector.OnProjectileDetected(_owner.transform.position);
                }
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (_owner == null) return;

        if (other.gameObject == _owner || other.transform.IsChildOf(_owner.transform)) return;

        if (other.CompareTag(_owner.tag)) return;

        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(_damage);
            DisableProjectile();
        }
        else if (!other.isTrigger)
        {
            DisableProjectile();
        }
    }

    protected void DisableProjectile()
    {
        gameObject.SetActive(false);
    }
}
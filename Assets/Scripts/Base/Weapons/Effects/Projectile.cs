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

        ToggleVisibility(true);
    }

    protected virtual void OnEnable()
    {
        ToggleVisibility(true);
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
                EnemyDetect detector = hitCollider.GetComponentInParent<EnemyDetect>();
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
        if (other.gameObject == _owner || other.transform.IsChildOf(_owner.transform) || other.CompareTag(_owner.tag)) return;

        IDamageable damageable = other.GetComponentInParent<IDamageable>() ?? other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_damage);
            DisableProjectile();
        }
        else if (!other.isTrigger)
        {
            DisableProjectile();
        }
    }

    protected void ToggleVisibility(bool visible)
    {
        if (TryGetComponent<Renderer>(out var r)) r.enabled = visible;
        if (TryGetComponent<Collider>(out var c)) c.enabled = visible;

        foreach (var childRenderer in GetComponentsInChildren<Renderer>())
            childRenderer.enabled = visible;

        foreach (var trail in GetComponentsInChildren<TrailRenderer>())
        {
            if (visible) trail.Clear();
            trail.enabled = visible;
        }
    }

    protected void DisableProjectile()
    {
        ToggleVisibility(false);
        gameObject.SetActive(false);
    }
}
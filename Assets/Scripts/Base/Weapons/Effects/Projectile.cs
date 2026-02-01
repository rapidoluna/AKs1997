using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _speed;
    private int _damage;
    private float _range;
    private GameObject _owner;
    private Vector3 _startPosition;

    [SerializeField] private float soundRadius = 5f;

    public void Init(float speed, int damage, float range, GameObject owner)
    {
        _speed = speed;
        _damage = damage;
        _range = range;
        _owner = owner;
        _startPosition = transform.position;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);

        if (_owner != null && _owner.CompareTag("Player"))
        {
            DetectNearbyEnemies();
        }

        if (Vector3.Distance(_startPosition, transform.position) >= _range)
        {
            gameObject.SetActive(false);
        }
    }

    private void DetectNearbyEnemies()
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

    private void OnTriggerEnter(Collider other)
    {
        if (_owner == null) return;

        if (other.gameObject == _owner || other.transform.IsChildOf(_owner.transform)) return;

        if (other.CompareTag(_owner.tag)) return;

        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(_damage);
            gameObject.SetActive(false);
        }
        else if (!other.isTrigger)
        {
            gameObject.SetActive(false);
        }
    }
}
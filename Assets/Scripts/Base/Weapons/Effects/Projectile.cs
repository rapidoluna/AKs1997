using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _speed;
    private int _damage;
    private float _range;
    private GameObject _owner;
    private Vector3 _startPosition;

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

        if (Vector3.Distance(_startPosition, transform.position) >= _range)
        {
            gameObject.SetActive(false);
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
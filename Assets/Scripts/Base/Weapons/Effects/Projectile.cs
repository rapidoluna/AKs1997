using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _speed;
    private int _damage;
    private float _range;
    private Vector3 _startPos;
    private Rigidbody _rb;
    private GameObject _owner;

    public void Init(float speed, int damage, float range, GameObject owner)
    {
        _speed = speed;
        _damage = damage;
        _range = range;
        _owner = owner;
        _startPos = transform.position;
        _rb = GetComponent<Rigidbody>();

        gameObject.SetActive(true);
        if (_rb != null)
        {
            _rb.linearVelocity = transform.forward * _speed;
        }
    }

    private void Update()
    {
        if (Vector3.Distance(_startPos, transform.position) >= _range)
        {
            Deactivate();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == _owner) return;

        IDamageable target = collision.gameObject.GetComponentInParent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(_damage);
        }

        Deactivate();
    }

    private void Deactivate()
    {
        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
        gameObject.SetActive(false);
    }
}
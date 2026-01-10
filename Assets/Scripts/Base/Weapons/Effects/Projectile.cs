using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float _speed;
    private int _damage;
    private float _range;
    private Vector3 _startPos;
    private Rigidbody _rb;

    public void Init(float speed, int damage, float range)
    {
        _speed = speed;
        _damage = damage;
        _range = range;
        _startPos = transform.position;
        _rb = GetComponent<Rigidbody>();

        if (_rb != null)
        {
            _rb.linearVelocity = transform.forward * _speed;
        }
    }

    private void Update()
    {
        if (Vector3.Distance(_startPos, transform.position) >= _range)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
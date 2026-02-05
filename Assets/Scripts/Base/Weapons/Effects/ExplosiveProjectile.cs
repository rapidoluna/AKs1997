using UnityEngine;
using System.Collections.Generic;

public class ExplosiveProjectile : Projectile
{
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private GameObject explosionVFX;

    protected override void DetectNearbyEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, soundRadius, targetLayer);
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

    protected override void OnTriggerEnter(Collider other)
    {
        if (_owner == null) return;

        if (other.gameObject == _owner || other.transform.IsChildOf(_owner.transform)) return;
        if (other.CompareTag(_owner.tag)) return;

        if (!other.isTrigger || other.TryGetComponent<IDamageable>(out _))
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, targetLayer);

        HashSet<GameObject> damagedObjects = new HashSet<GameObject>();

        foreach (var hit in hitColliders)
        {
            if (hit.gameObject == _owner) continue;

            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                GameObject rootObj = hit.transform.root.gameObject;
                if (!damagedObjects.Contains(rootObj))
                {
                    damageable.TakeDamage(_damage);
                    damagedObjects.Add(rootObj);
                }
            }
        }

        DisableProjectile();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
    }
}
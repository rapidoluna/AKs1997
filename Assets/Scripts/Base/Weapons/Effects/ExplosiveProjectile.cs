using UnityEngine;
using System.Collections.Generic;

public class ExplosiveProjectile : Projectile
{
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float knockbackForce = 15f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private GameObject explosionVFX;

    protected override void OnTriggerEnter(Collider other)
    {
        if (_owner == null) return;

        if (other.gameObject == _owner || other.transform.IsChildOf(_owner.transform) || other.CompareTag(_owner.tag)) return;

        if (other.gameObject.layer == gameObject.layer || other.CompareTag("Projectile")) return;

        IDamageable directTarget = other.GetComponentInParent<IDamageable>() ?? other.GetComponent<IDamageable>();

        if (directTarget != null)
        {
            directTarget.TakeDamage(_damage);
        }

        if (!other.isTrigger || directTarget != null)
        {
            Explode(directTarget);
        }
    }

    private void Explode(IDamageable excludeTarget)
    {
        if (explosionVFX != null)
            Instantiate(explosionVFX, transform.position, Quaternion.identity);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        HashSet<IDamageable> damagedEntities = new HashSet<IDamageable>();

        if (excludeTarget != null) damagedEntities.Add(excludeTarget);

        foreach (var hit in hitColliders)
        {
            if (hit.gameObject.layer == gameObject.layer || hit.CompareTag("Projectile")) continue;

            Rigidbody rb = hit.GetComponentInParent<Rigidbody>() ?? hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(knockbackForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
            }

            if (((1 << hit.gameObject.layer) & targetLayer) == 0) continue;

            if (_owner != null)
            {
                if (hit.gameObject == _owner || hit.transform.IsChildOf(_owner.transform) || hit.CompareTag(_owner.tag)) continue;
            }

            IDamageable damageable = hit.GetComponentInParent<IDamageable>() ?? hit.GetComponent<IDamageable>();
            if (damageable != null && !damagedEntities.Contains(damageable))
            {
                damageable.TakeDamage(_damage / 2);
                damagedEntities.Add(damageable);
            }
        }
        DisableProjectile();
    }
}
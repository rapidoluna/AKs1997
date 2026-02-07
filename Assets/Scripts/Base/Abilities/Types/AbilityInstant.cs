using UnityEngine;

public class AbilityInstant : AbilityBase
{
    public override void Execute(KeyCode inputKey)
    {
        if (abilityData.maxHealthBonus > 0 && PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.Heal(abilityData.maxHealthBonus);
        }

        if (abilityData.areaSize != Vector3.zero)
        {
            HitArea();
        }
    }

    private void HitArea()
    {
        if (firePoint == null) return;

        Collider[] hitColliders = Physics.OverlapBox(firePoint.position + firePoint.forward * (abilityData.areaSize.z / 2), abilityData.areaSize / 2, firePoint.rotation);

        foreach (var hit in hitColliders)
        {
            if (hit.transform.root == firePoint.root) continue;

            IDamageable target = hit.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage((int)abilityData.abilityDamage);
            }

            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (hit.transform.position - firePoint.position).normalized;
                rb.AddForce(direction * 500f, ForceMode.Impulse);
            }
        }
    }
}
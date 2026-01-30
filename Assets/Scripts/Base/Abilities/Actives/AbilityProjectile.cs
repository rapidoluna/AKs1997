using UnityEngine;

public class AbilityProjectile : AbilityBase
{
    public override void Execute()
    {
        if (abilityData.abilityPrefab != null)
            Instantiate(abilityData.abilityPrefab, firePoint.position, firePoint.rotation);
    }
}
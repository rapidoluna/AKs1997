using UnityEngine;

public class AbilityBuff : AbilityBase
{
    public override void Execute()
    {
        if (movement != null)
        {
            movement.ApplyAbilitySpeed(abilityData.moveSpeedMultiplier);
        }

        WeaponShooting[] shooters = transform.root.GetComponentsInChildren<WeaponShooting>();
        foreach (var shooter in shooters)
        {
            shooter.SetDamageMultiplier(abilityData.damageMultiplier);
        }

        PlayerHealth health = transform.root.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.ApplyHealthBuff(abilityData.maxHealthBonus);
        }
    }

    public override void StopAbility()
    {
        if (movement != null)
        {
            movement.ResetSpeed();
        }

        WeaponShooting[] shooters = transform.root.GetComponentsInChildren<WeaponShooting>();
        foreach (var shooter in shooters)
        {
            shooter.SetDamageMultiplier(1f);
        }

        PlayerHealth health = transform.root.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.ResetHealthBuff();
        }
    }
}
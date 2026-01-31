using UnityEngine;

public class AbilityEquip : AbilityBase
{
    private WeaponController _weaponController;
    private AbilityProcessor _specialityProcessor;
    private UltimateProcessor _ultimateProcessor;

    private void Awake()
    {
        _weaponController = transform.root.GetComponentInChildren<WeaponController>();
        _specialityProcessor = GetComponent<AbilityProcessor>();
        _ultimateProcessor = GetComponent<UltimateProcessor>();
    }

    public override void Execute()
    {
        if (_weaponController == null)
            _weaponController = transform.root.GetComponentInChildren<WeaponController>();

        if (_weaponController == null || abilityData.rewardWeapon == null)
        {
            Debug.LogError($"[AbilityEquip] 실행 불가: WC={(_weaponController != null)}, Weapon={(abilityData.rewardWeapon != null)}");
            return;
        }

        _weaponController.SetAbilityWeapon(abilityData.rewardWeapon);

        GameObject spawnedWeapon = _weaponController.Slots[2];
        if (spawnedWeapon != null)
        {
            var shooter = spawnedWeapon.GetComponentInChildren<WeaponShooting>();
            if (shooter != null)
            {
                shooter.SetDamageMultiplier(abilityData.damageMultiplier);
                if (abilityData.useReloadLimit)
                {
                    shooter.SetReloadLimit(abilityData.maxReloadCount);
                    shooter.OnResourceExhausted += HandleResourceExhausted;
                }
            }
        }

        _weaponController.SwitchToSlot(2, true);
    }

    private void HandleResourceExhausted()
    {
        if (abilityData.type == AbilityType.Speciality && _specialityProcessor != null)
            _specialityProcessor.StopEffect();
        else if (abilityData.type == AbilityType.Ultimate && _ultimateProcessor != null)
            _ultimateProcessor.StopUltimate();
    }

    public override void StopAbility()
    {
        if (_weaponController == null) return;

        GameObject spawnedWeapon = _weaponController.Slots[2];
        if (spawnedWeapon != null)
        {
            var shooter = spawnedWeapon.GetComponentInChildren<WeaponShooting>();
            if (shooter != null) shooter.OnResourceExhausted -= HandleResourceExhausted;
        }

        _weaponController.ClearAbilitySlot();
        _weaponController.UnlockWeapon();
        _weaponController.SwitchToSlot(0);
    }
}
using UnityEngine;

public class AbilityEquip : AbilityBase
{
    private WeaponController _weaponController;

    public override void Initialize(AbilityData data, PlayerWalking move, Transform point, MonoBehaviour runnerScript)
    {
        base.Initialize(data, move, point, runnerScript);

        if (firePoint != null)
            _weaponController = firePoint.GetComponent<WeaponController>();
    }

    public override void Execute(KeyCode key)
    {
        if (_weaponController == null || abilityData.rewardWeapon == null) return;

        _weaponController.SetAbilityWeapon(abilityData.rewardWeapon);
        _weaponController.SwitchToSlot(2, true);
    }

    public override void StopAbility()
    {
        if (_weaponController == null) return;

        _weaponController.UnlockWeapon();
        _weaponController.SwitchToSlot(0);
        _weaponController.ClearAbilitySlot();
    }
}
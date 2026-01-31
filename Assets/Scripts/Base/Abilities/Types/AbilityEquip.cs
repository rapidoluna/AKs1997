using UnityEngine;
using System.Collections;

public class AbilityEquip : AbilityBase
{
    private WeaponController _weaponController;
    private AbilityProcessor _specialityProcessor;
    private UltimateProcessor _ultimateProcessor;
    private int _previousSlot = 0;

    private void Awake()
    {
        _weaponController = transform.root.GetComponentInChildren<WeaponController>();
        _specialityProcessor = GetComponent<AbilityProcessor>();
        _ultimateProcessor = GetComponent<UltimateProcessor>();
    }

    public override void Execute()
    {
        if (_weaponController == null) _weaponController = transform.root.GetComponentInChildren<WeaponController>();
        if (_weaponController == null || abilityData.rewardWeapon == null) return;

        _previousSlot = _weaponController.CurrentIndex;
        _weaponController.SetAbilityWeapon(abilityData.rewardWeapon);
        _weaponController.SwitchToSlot(2, true);
    }

    public override void StopAbility()
    {
        if (_weaponController == null) return;

        _weaponController.UnlockWeapon();
        _weaponController.SwitchToSlot(_previousSlot);

        StartCoroutine(DelayedClear());
    }

    private IEnumerator DelayedClear()
    {
        yield return new WaitForSeconds(0.65f);
        _weaponController.ClearAbilitySlot();
    }

    private void HandleResourceExhausted()
    {
        if (abilityData.type == AbilityType.Speciality && _specialityProcessor != null)
            _specialityProcessor.StopEffect();
        else if (abilityData.type == AbilityType.Ultimate && _ultimateProcessor != null)
            _ultimateProcessor.StopUltimate();
    }
}
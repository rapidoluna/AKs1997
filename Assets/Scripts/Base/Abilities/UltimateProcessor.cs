using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UltimateProcessor : MonoBehaviour
{
    private AbilityData _abilityData;
    private List<AbilityBase> _abilityEffects = new List<AbilityBase>();
    private UltimateCharge _chargeSystem;
    private bool _isDurationActive = false;

    private void Start()
    {
        PlayerInitializer init = GetComponentInParent<PlayerInitializer>();
        PlayerWalking walk = GetComponentInParent<PlayerWalking>();
        Transform firePoint = transform;

        if (init != null && init.CharacterData != null)
        {
            _abilityData = init.CharacterData.characterUltimate;
            if (_abilityData != null)
            {
                _chargeSystem = gameObject.AddComponent<UltimateCharge>();
                _chargeSystem.Initialize(_abilityData);
                CreateEffects(_abilityData, walk, firePoint);
            }
        }
    }

    private void CreateEffects(AbilityData data, PlayerWalking walk, Transform firePoint)
    {
        foreach (var type in data.activeTypes)
        {
            AbilityBase effect = type switch
            {
                AbilityActiveType.Buff => gameObject.AddComponent<AbilityBuff>(),
                AbilityActiveType.Equip => gameObject.AddComponent<AbilityEquip>(),
                AbilityActiveType.Overlay => gameObject.AddComponent<AbilityOverlay>(),
                AbilityActiveType.Charge => gameObject.AddComponent<AbilityCharge>(),
                _ => null
            };

            if (effect != null)
            {
                effect.Initialize(data, walk, firePoint);
                _abilityEffects.Add(effect);
            }
        }
    }

    private void Update()
    {
        if (_abilityData == null || _abilityEffects.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_isDurationActive)
            {
                if (_abilityData.abilityDuration <= 0) StopUltimate();
            }
            else if (_chargeSystem.IsReady)
            {
                StartUltimate();
            }
        }
    }

    private void StartUltimate()
    {
        _isDurationActive = true;
        _chargeSystem.ResetGauge();
        _chargeSystem.SetLock(true);

        foreach (var effect in _abilityEffects)
        {
            effect.Execute();
        }

        if (_abilityData.abilityDuration > 0)
        {
            StartCoroutine(DurationRoutine(_abilityData.abilityDuration));
        }
    }

    private IEnumerator DurationRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (_isDurationActive) StopUltimate();
    }

    public void StopUltimate()
    {
        if (!_isDurationActive) return;

        foreach (var effect in _abilityEffects)
        {
            effect.StopAbility();
        }

        _isDurationActive = false;
        _chargeSystem.SetLock(false);
    }

    public bool IsActive() => _isDurationActive;
}
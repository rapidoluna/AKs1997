using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityProcessor : MonoBehaviour
{
    private AbilityData _abilityData;
    private List<AbilityBase> _abilityEffects = new List<AbilityBase>();

    private float _cooldownTimer = 0f;
    private bool _isDurationActive = false;
    private bool _isInitialized = false;

    public void Initialize(AbilityData data, PlayerWalking walk, Transform firePoint)
    {
        if (data == null) return;

        _abilityData = data;
        CreateEffects(_abilityData, walk, firePoint);
        _isInitialized = true;
    }

    private void CreateEffects(AbilityData data, PlayerWalking walk, Transform firePoint)
    {
        _abilityEffects.Clear();
        foreach (var type in data.activeTypes)
        {
            AbilityBase effect = type switch
            {
                AbilityActiveType.Buff => new AbilityBuff(),
                AbilityActiveType.Equip => new AbilityEquip(),
                AbilityActiveType.Overlay => new AbilityOverlay(),
                AbilityActiveType.Charge => new AbilityCharge(),
                AbilityActiveType.Instant => new AbilityInstant(),
                _ => null
            };

            if (effect != null)
            {
                effect.Initialize(data, walk, firePoint, this);
                _abilityEffects.Add(effect);
            }
        }
    }

    private void Update()
    {
        if (!_isInitialized) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (_isDurationActive)
            {
                if (_abilityData.abilityDuration <= 0) StopEffect();
            }
            else if (Time.time >= _cooldownTimer)
            {
                StartEffect();
            }
        }
    }

    private void StartEffect()
    {
        _isDurationActive = true;

        foreach (var effect in _abilityEffects)
        {
            effect.Execute(KeyCode.Q);
        }

        if (_abilityData.abilityDuration > 0)
        {
            StartCoroutine(DurationRoutine(_abilityData.abilityDuration));
        }
    }

    private IEnumerator DurationRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (_isDurationActive) StopEffect();
    }

    public void StopEffect()
    {
        if (!_isDurationActive) return;

        foreach (var effect in _abilityEffects)
        {
            effect.StopAbility();
        }

        _isDurationActive = false;
        _cooldownTimer = Time.time + _abilityData.abilityCooltime;
    }

    public bool IsActive() => _isDurationActive;
    public AbilityData GetData() => _abilityData;

    public float GetCooldownRatio()
    {
        if (_isDurationActive) return 0f;
        float totalCd = _abilityData.abilityCooltime;
        if (totalCd <= 0) return 1f;

        float elapsed = Time.time - (_cooldownTimer - totalCd);
        return Mathf.Clamp01(elapsed / totalCd);
    }

    public float GetRemainingCooldown()
    {
        float remaining = _cooldownTimer - Time.time;
        return remaining > 0 ? remaining : 0;
    }
}
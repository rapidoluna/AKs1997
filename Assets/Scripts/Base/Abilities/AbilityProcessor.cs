using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityProcessor : MonoBehaviour
{
    private AbilityData _abilityData;
    private List<AbilityBase> _abilityEffects = new List<AbilityBase>();

    private float _cooldownTimer = 0f;
    private bool _isDurationActive = false;

    private void Start()
    {
        PlayerInitializer init = GetComponentInParent<PlayerInitializer>();
        PlayerWalking walk = GetComponentInParent<PlayerWalking>();
        Transform firePoint = transform;

        if (init != null && init.CharacterData != null)
        {
            _abilityData = init.CharacterData.characterSpeciality;
            if (_abilityData != null)
            {
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
        foreach (var effect in _abilityEffects)
        {
            effect.Execute();
        }
        _isDurationActive = true;

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
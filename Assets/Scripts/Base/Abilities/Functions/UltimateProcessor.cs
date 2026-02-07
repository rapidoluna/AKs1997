using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UltimateProcessor : MonoBehaviour
{
    private AbilityData _abilityData;
    private List<AbilityBase> _abilityEffects = new List<AbilityBase>();

    private bool _isDurationActive = false;
    private bool _isInitialized = false;

    private float _currentGauge = 0f;
    private float _maxGauge = 100f;
    private bool _isGaugeLocked = false;

    public float CurrentGauge => _currentGauge;
    public float MaxGauge => _maxGauge;
    public float GaugeRatio => _maxGauge > 0 ? Mathf.Clamp01(_currentGauge / _maxGauge) : 0f;
    public bool IsReady => !_isGaugeLocked && _currentGauge >= _maxGauge;
    public bool IsActive => _isDurationActive;

    public AbilityData GetData() => _abilityData;

    public void Initialize(AbilityData data, PlayerWalking walk, Transform firePoint)
    {
        if (data == null) return;

        _abilityData = data;
        _maxGauge = data.maxReloadCount > 0 ? data.maxReloadCount : 100f;
        _currentGauge = 0f;

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

        if (!_isGaugeLocked && _currentGauge < _maxGauge && _abilityData.ultimateChargeSpeed > 0)
        {
            _currentGauge += _abilityData.ultimateChargeSpeed * Time.deltaTime;
            if (_currentGauge > _maxGauge) _currentGauge = _maxGauge;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_isDurationActive)
            {
                if (_abilityData.abilityDuration <= 0) StopUltimate();
            }
            else if (IsReady)
            {
                StartUltimate();
            }
        }
    }

    public void AddCharge(float amount)
    {
        if (_isGaugeLocked || _currentGauge >= _maxGauge) return;

        _currentGauge += amount;
        if (_currentGauge > _maxGauge) _currentGauge = _maxGauge;
    }

    private void StartUltimate()
    {
        _isDurationActive = true;
        _currentGauge = 0f;
        _isGaugeLocked = true;

        foreach (var effect in _abilityEffects)
        {
            effect.Execute(KeyCode.E);
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
        _isGaugeLocked = false;
    }
}
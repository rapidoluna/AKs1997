using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityProcessor : MonoBehaviour
{
    private AbilityData _qData;
    private AbilityData _eData;
    private AbilityBase _qEffect;
    private AbilityBase _eEffect;

    [SerializeField] private Transform weaponHoldPoint;
    [SerializeField] private Transform firePoint;

    private Dictionary<AbilityType, float> _cooldowns = new Dictionary<AbilityType, float>();
    private Dictionary<AbilityType, bool> _isCharging = new Dictionary<AbilityType, bool>();
    private Dictionary<AbilityType, bool> _isDurationActive = new Dictionary<AbilityType, bool>();
    private Dictionary<AbilityType, List<GameObject>> _targetLists = new Dictionary<AbilityType, List<GameObject>>();
    private Dictionary<AbilityType, GameObject> _spawnedDevices = new Dictionary<AbilityType, GameObject>();

    private float _ultimateGauge = 0f;
    private const float MAX_GAUGE = 100f;
    [SerializeField] private float gaugeChargeSpeed = 10f;

    public float UltimateGaugeNormalized => _ultimateGauge / MAX_GAUGE;

    private void Start()
    {
        PlayerInitializer initializer = GetComponentInParent<PlayerInitializer>();
        PlayerWalking movement = GetComponentInParent<PlayerWalking>();

        if (initializer != null && initializer.CharacterData != null)
        {
            _qData = initializer.CharacterData.characterSpeciality;
            _eData = initializer.CharacterData.characterUltimate;

            _qEffect = AssignAbilityComponent(_qData, movement);
            _eEffect = AssignAbilityComponent(_eData, movement);
        }

        _cooldowns[AbilityType.Speciality] = 0;
        _cooldowns[AbilityType.Ultimate] = 0;
    }

    private AbilityBase AssignAbilityComponent(AbilityData data, PlayerWalking walk)
    {
        if (data == null) return null;
        AbilityBase ability = null;
        switch (data.activeType)
        {
            case AbilityActiveType.Projectile: ability = gameObject.AddComponent<AbilityProjectile>(); break;
            case AbilityActiveType.Instant: ability = gameObject.AddComponent<AbilityInstant>(); break;
            case AbilityActiveType.Equip: ability = gameObject.AddComponent<AbilityEquip>(); break;
            case AbilityActiveType.Overlay: ability = gameObject.AddComponent<AbilityOverlay>(); break;
            case AbilityActiveType.Buff: ability = gameObject.AddComponent<AbilityBuff>(); break;
            case AbilityActiveType.Charge: ability = gameObject.AddComponent<AbilityCharge>(); break;
        }
        if (ability != null) ability.Initialize(data, walk, firePoint);
        return ability;
    }

    private void Update()
    {
        if (_ultimateGauge < MAX_GAUGE)
        {
            _ultimateGauge += Time.deltaTime * gaugeChargeSpeed;
            _ultimateGauge = Mathf.Min(_ultimateGauge, MAX_GAUGE);
        }

        HandleInput(KeyCode.Q, _qData, _qEffect, AbilityType.Speciality);
        HandleInput(KeyCode.E, _eData, _eEffect, AbilityType.Ultimate);
    }

    private void HandleInput(KeyCode key, AbilityData data, AbilityBase effect, AbilityType type)
    {
        if (data == null || effect == null) return;

        if (Input.GetKeyDown(key) && _isDurationActive.ContainsKey(type) && _isDurationActive[type])
        {
            if (data.abilityDuration <= 0)
            {
                StopDurationAbility(type, data);
                return;
            }
        }

        if (IsAbilityInCooldown(type)) return;
        if (_isDurationActive.ContainsKey(type) && _isDurationActive[type]) return;

        if (Input.GetKeyDown(key))
        {
            if (data.activeType == AbilityActiveType.Charge)
                StartCharge(data, type, effect);
            else
                StartAbility(type, data, effect);
        }

        if (_isCharging.ContainsKey(type) && _isCharging[type])
        {
            if (Input.GetKeyUp(key))
            {
                effect.OnChargeRelease(_targetLists[type]);
                StartAbility(type, data, null);
                StopCharge(type);
            }
        }
    }

    private void StartAbility(AbilityType type, AbilityData data, AbilityBase effect)
    {
        if (effect != null) effect.Execute();
        _isDurationActive[type] = true;

        if (data.abilityDuration > 0)
            StartCoroutine(DurationCoroutine(type, data));
    }

    private IEnumerator DurationCoroutine(AbilityType type, AbilityData data)
    {
        yield return new WaitForSeconds(data.abilityDuration);
        StopDurationAbility(type, data);
    }

    private void StopDurationAbility(AbilityType type, AbilityData data)
    {
        _isDurationActive[type] = false;
        if (type == AbilityType.Ultimate) _ultimateGauge = 0;
        else _cooldowns[type] = Time.time + data.abilityCooltime;
    }

    private void StartCharge(AbilityData data, AbilityType type, AbilityBase effect)
    {
        _isCharging[type] = true;
        _targetLists[type] = new List<GameObject>();
        effect.OnChargeStart();
        if (data.rewardWeapon?.weaponPrefab != null)
            _spawnedDevices[type] = Instantiate(data.rewardWeapon.weaponPrefab, weaponHoldPoint);
    }

    private void StopCharge(AbilityType type)
    {
        _isCharging[type] = false;
        if (_spawnedDevices.ContainsKey(type) && _spawnedDevices[type] != null)
            Destroy(_spawnedDevices[type]);
    }

    private bool IsAbilityInCooldown(AbilityType type)
    {
        if (type == AbilityType.Ultimate) return _ultimateGauge < MAX_GAUGE;
        return _cooldowns.ContainsKey(type) && Time.time < _cooldowns[type];
    }

    public float GetCooldownNormalized(AbilityType type)
    {
        if (type == AbilityType.Ultimate) return 1f - UltimateGaugeNormalized;
        AbilityData data = (type == AbilityType.Speciality) ? _qData : _eData;
        if (data == null || data.abilityCooltime <= 0) return 0;
        float remaining = _cooldowns.ContainsKey(type) ? _cooldowns[type] - Time.time : 0;
        return Mathf.Clamp01(remaining / data.abilityCooltime);
    }

    public bool IsExecuting(AbilityType type)
    {
        bool charging = _isCharging.ContainsKey(type) && _isCharging[type];
        bool duration = _isDurationActive.ContainsKey(type) && _isDurationActive[type];
        return charging || duration;
    }
}
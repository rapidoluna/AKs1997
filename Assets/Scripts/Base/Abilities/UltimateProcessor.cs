using UnityEngine;
using System.Collections;

public class UltimateProcessor : MonoBehaviour
{
    private AbilityData _abilityData;
    private AbilityBase _abilityEffect;
    private UltimateCharge _chargeSystem;

    private bool _isDurationActive = false;

    private void Start()
    {
        PlayerInitializer init = GetComponentInParent<PlayerInitializer>();
        PlayerWalking walk = GetComponentInParent<PlayerWalking>();

        if (init != null && init.CharacterData != null)
        {
            _abilityData = init.CharacterData.characterUltimate;
            if (_abilityData != null)
            {
                _chargeSystem = gameObject.AddComponent<UltimateCharge>();
                _chargeSystem.Initialize(_abilityData);

                _abilityEffect = CreateEffect(_abilityData, walk);
            }
        }
    }

    private AbilityBase CreateEffect(AbilityData data, PlayerWalking walk)
    {
        AbilityBase effect = data.activeType switch
        {
            AbilityActiveType.Buff => gameObject.AddComponent<AbilityBuff>(),
            _ => null
        };

        if (effect != null) effect.Initialize(data, walk, null);
        return effect;
    }

    private void Update()
    {
        if (_abilityData == null || _abilityEffect == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!_isDurationActive && _chargeSystem.IsReady)
            {
                StartUltimate();
            }
            else if (!_chargeSystem.IsReady)
            {
                Debug.Log($"[Ultimate] 게이지 부족: {(_chargeSystem.CurrentGauge):F1}/100");
            }
        }
    }

    private void StartUltimate()
    {
        Debug.Log($"[Ultimate] {_abilityData.abilityName} 발동!");
        _abilityEffect.Execute();
        _isDurationActive = true;
        _chargeSystem.ResetGauge();

        if (_abilityData.abilityDuration > 0)
        {
            StartCoroutine(DurationRoutine(_abilityData.abilityDuration));
        }
    }

    private IEnumerator DurationRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        StopUltimate();
    }

    private void StopUltimate()
    {
        if (!_isDurationActive) return;
        _abilityEffect.StopAbility();
        _isDurationActive = false;
        Debug.Log($"[Ultimate] {_abilityData.abilityName} 종료");
    }
}
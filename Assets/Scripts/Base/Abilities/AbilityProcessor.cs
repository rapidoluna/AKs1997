using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityProcessor : MonoBehaviour
{
    private AbilityData _abilityData;
    private AbilityBase _abilityEffect;

    private float _cooldownTimer = 0f;
    private bool _isDurationActive = false;

    private void Start()
    {
        PlayerInitializer init = GetComponentInParent<PlayerInitializer>();
        PlayerWalking walk = GetComponentInParent<PlayerWalking>();

        if (init != null && init.CharacterData != null)
        {
            _abilityData = init.CharacterData.characterSpeciality;

            if (_abilityData != null)
            {
                Debug.Log($"[AbilityProcessor] 능력 초기화 완료: {_abilityData.abilityName} (Type: {_abilityData.activeType})");
                _abilityEffect = CreateEffect(_abilityData, walk);
            }
            else
            {
                Debug.LogWarning("[AbilityProcessor] Speciality 데이터가 CharacterData에 없습니다.");
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

        if (effect != null)
        {
            effect.Initialize(data, walk, null);
        }
        return effect;
    }

    private void Update()
    {
        if (_abilityData == null || _abilityEffect == null) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (_isDurationActive)
            {
                if (_abilityData.abilityDuration <= 0)
                {
                    Debug.Log($"[AbilityProcessor] {_abilityData.abilityName} 토글 종료 시도");
                    StopEffect();
                }
            }
            else if (Time.time >= _cooldownTimer)
            {
                Debug.Log($"[AbilityProcessor] {_abilityData.abilityName} 실행 시도");
                StartEffect();
            }
            else
            {
                Debug.Log($"[AbilityProcessor] 쿨타임 대기 중: {(_cooldownTimer - Time.time):F1}초 남음");
            }
        }
    }

    private void StartEffect()
    {
        _abilityEffect.Execute();
        _isDurationActive = true;
        Debug.Log($"[AbilityProcessor] {_abilityData.abilityName} 활성화됨");

        if (_abilityData.abilityDuration > 0)
        {
            Debug.Log($"[AbilityProcessor] 지속 시간 타이머 시작: {_abilityData.abilityDuration}초");
            StartCoroutine(DurationRoutine(_abilityData.abilityDuration));
        }
    }

    private IEnumerator DurationRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        Debug.Log($"[AbilityProcessor] 지속 시간 종료");
        StopEffect();
    }

    private void StopEffect()
    {
        if (!_isDurationActive) return;

        _abilityEffect.StopAbility();
        _isDurationActive = false;
        _cooldownTimer = Time.time + _abilityData.abilityCooltime;
        Debug.Log($"[AbilityProcessor] {_abilityData.abilityName} 비활성화됨. 쿨타임 시작: {_abilityData.abilityCooltime}초");
    }
}
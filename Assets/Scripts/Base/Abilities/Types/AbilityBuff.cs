using UnityEngine;

public class AbilityBuff : AbilityBase
{
    private bool _isBuffActive = false;

    public override void Execute(KeyCode key)
    {
        if (abilityData == null || _isBuffActive) return;

        // 1. 체력 관련 버프
        if (PlayerHealth.Instance != null)
        {
            if (abilityData.maxHealthBonus > 0)
            {
                PlayerHealth.Instance.ApplyHealthBuff(abilityData.maxHealthBonus);
                PlayerHealth.Instance.Heal(abilityData.maxHealthBonus);
            }
        }

        // 2. 이동 속도 버프
        if (playerMove != null && abilityData.moveSpeedMultiplier > 1f)
        {
            playerMove.SetSpeedMultiplier(abilityData.moveSpeedMultiplier);
        }

        _isBuffActive = true;
    }

    public override void StopAbility()
    {
        if (!_isBuffActive) return;

        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.ApplyHealthBuff(0);
        }

        if (playerMove != null)
        {
            playerMove.SetSpeedMultiplier(1f);
        }

        _isBuffActive = false;
    }
}
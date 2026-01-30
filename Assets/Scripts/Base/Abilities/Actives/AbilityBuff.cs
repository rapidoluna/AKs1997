using UnityEngine;
using System.Collections;

public class AbilityBuff : AbilityBase
{
    public override void Execute()
    {
        StartCoroutine(BuffRoutine());
    }

    private IEnumerator BuffRoutine()
    {
        if (movement != null) movement.ApplyAbilitySpeed(abilityData.moveSpeedMultiplier);
        yield return new WaitForSeconds(abilityData.abilityDuration);
        if (movement != null) movement.ResetSpeed();
    }
}
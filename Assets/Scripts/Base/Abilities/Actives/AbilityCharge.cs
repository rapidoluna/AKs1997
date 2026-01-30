using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityCharge : AbilityBase
{
    public override void Execute() { }

    public override void OnChargeStart()
    {
        if (movement != null) movement.ApplyAbilitySpeed(abilityData.moveSpeedMultiplier);
    }

    public override void OnChargeRelease(List<GameObject> targets)
    {
        foreach (var target in targets)
        {
            if (target == null) continue;
            if (abilityData.abilityPrefab != null)
                Instantiate(abilityData.abilityPrefab, firePoint.position, Quaternion.LookRotation(target.transform.position - firePoint.position));

            StartCoroutine(PullRoutine(target));
        }
        if (movement != null) movement.ResetSpeed();
    }

    private IEnumerator PullRoutine(GameObject target)
    {
        float elapsed = 0;
        float duration = abilityData.abilityDuration > 0 ? abilityData.abilityDuration : 0.6f;

        while (elapsed < duration && target != null)
        {
            target.transform.position = Vector3.Lerp(target.transform.position, transform.position + transform.forward * 2f, Time.deltaTime * abilityData.pullForce);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
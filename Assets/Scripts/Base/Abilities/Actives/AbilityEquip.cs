using UnityEngine;
using System.Collections;

public class AbilityEquip : AbilityBase
{
    public override void Execute()
    {
        StartCoroutine(EquipRoutine());
    }

    private IEnumerator EquipRoutine()
    {
        Debug.Log($"{abilityData.rewardWeapon.name} ÀåÂø (½½·Ô: {abilityData.slotTarget})");

        yield return new WaitForSeconds(abilityData.abilityDuration);

        Debug.Log($"{abilityData.rewardWeapon.name} È¸¼ö");
    }
}
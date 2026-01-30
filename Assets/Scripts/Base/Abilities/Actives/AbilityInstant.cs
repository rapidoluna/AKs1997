using UnityEngine;

public class AbilityInstant : AbilityBase
{
    public override void Execute()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, abilityData.abilityRange);
        foreach (var t in targets)
        {
            if (t.CompareTag("Enemy"))
            {
                Debug.Log($"{t.name}에게 {abilityData.abilityDamage}의 즉발 피해");
            }
        }
    }
}
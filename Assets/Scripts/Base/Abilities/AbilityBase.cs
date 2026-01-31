using UnityEngine;
using System.Collections.Generic;

public abstract class AbilityBase : MonoBehaviour
{
    protected AbilityData abilityData;
    protected PlayerWalking movement;
    protected Transform firePoint;

    public virtual void Initialize(AbilityData data, PlayerWalking walk, Transform fp)
    {
        abilityData = data;
        movement = walk;
        firePoint = fp;
    }

    public abstract void Execute();
    public virtual void OnChargeStart() { }
    public virtual void OnChargeRelease(List<GameObject> targets) { }
    public virtual void StopAbility() { }
}
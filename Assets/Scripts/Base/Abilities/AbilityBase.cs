using UnityEngine;

public abstract class AbilityBase
{
    protected AbilityData abilityData;
    protected PlayerWalking playerMove;
    protected Transform firePoint;
    protected MonoBehaviour runner;

    public virtual void Initialize(AbilityData data, PlayerWalking move, Transform point, MonoBehaviour runnerScript)
    {
        abilityData = data;
        playerMove = move;
        firePoint = point;
        runner = runnerScript;
    }

    public abstract void Execute(KeyCode inputKey);
    public virtual void StopAbility() { }
}
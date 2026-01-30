using UnityEngine;
using System.Collections;

public class AbilityOverlay : AbilityBase
{
    private bool _isOverlayActive = false;

    public override void Execute()
    {
        if (_isOverlayActive) return;
        StartCoroutine(OverlayRoutine());
    }

    private IEnumerator OverlayRoutine()
    {
        _isOverlayActive = true;
        Debug.Log($"{abilityData.AbilityName} 오버레이 활성화");

        yield return new WaitForSeconds(abilityData.abilityDuration);

        _isOverlayActive = false;
        Debug.Log($"{abilityData.AbilityName} 오버레이 종료");
    }
}
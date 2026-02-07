using UnityEngine;

public class AbilityInstant : AbilityBase
{
    private Vector3 _lastCenter;
    private Vector3 _lastSize;
    private Quaternion _lastRotation = Quaternion.identity;
    private bool _hasExecuted = false;

    private AbilityProcessor _specialityProcessor;
    private UltimateProcessor _ultimateProcessor;

    private void Awake()
    {
        _specialityProcessor = GetComponent<AbilityProcessor>();
        _ultimateProcessor = GetComponent<UltimateProcessor>();
    }

    public override void Execute()
    {
        _lastCenter = firePoint.position + firePoint.forward * abilityData.abilityRange;
        _lastRotation = firePoint.rotation;
        _lastSize = abilityData.areaSize;
        _hasExecuted = true;

        if (abilityData.abilityPrefab != null)
        {
            Instantiate(abilityData.abilityPrefab, _lastCenter, _lastRotation);
        }

        Collider[] colliders = Physics.OverlapBox(_lastCenter, _lastSize * 0.5f, _lastRotation);

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Enemy"))
            {
                col.SendMessage("TakeDamage", (int)abilityData.abilityDamage, SendMessageOptions.DontRequireReceiver);
            }
        }

        if (abilityData.type == AbilityType.Speciality && _specialityProcessor != null)
            _specialityProcessor.StopEffect();
        else if (abilityData.type == AbilityType.Ultimate && _ultimateProcessor != null)
            _ultimateProcessor.StopUltimate();
    }

    private void OnDrawGizmos()
    {
        if (!_hasExecuted) return;
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(_lastCenter, _lastRotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, _lastSize);
    }

    public override void StopAbility()
    {
    }
}
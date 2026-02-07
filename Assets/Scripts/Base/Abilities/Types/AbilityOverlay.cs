using UnityEngine;
using System.Collections;

public class AbilityOverlay : AbilityBase
{
    private GameObject _akimboWeaponInstance;
    private WeaponShooting _akimboShooting;

    public override void Initialize(AbilityData data, PlayerWalking move, Transform point, MonoBehaviour runnerScript)
    {
        base.Initialize(data, move, point, runnerScript);

        if (data.rewardWeapon != null && _akimboWeaponInstance == null)
        {
            _akimboWeaponInstance = Object.Instantiate(data.rewardWeapon, point);

            // 왼손 위치 등으로 오프셋 조정 필요 (임시 좌표)
            _akimboWeaponInstance.transform.localPosition = new Vector3(-0.5f, 0f, 0.5f);
            _akimboWeaponInstance.transform.localRotation = Quaternion.identity;

            _akimboShooting = _akimboWeaponInstance.GetComponent<WeaponShooting>();
            _akimboWeaponInstance.SetActive(false);
        }
    }

    public override void Execute(KeyCode inputKey)
    {
        if (_akimboWeaponInstance == null) return;

        _akimboWeaponInstance.SetActive(true);

        if (runner != null)
            runner.StartCoroutine(AkimboFireRoutine());
    }

    private IEnumerator AkimboFireRoutine()
    {
        float duration = abilityData.abilityDuration > 0 ? abilityData.abilityDuration : 5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (Input.GetMouseButton(0) && _akimboShooting != null)
            {
                _akimboShooting.Shoot();
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        StopAbility();
    }

    public override void StopAbility()
    {
        if (_akimboWeaponInstance != null)
            _akimboWeaponInstance.SetActive(false);
    }
}
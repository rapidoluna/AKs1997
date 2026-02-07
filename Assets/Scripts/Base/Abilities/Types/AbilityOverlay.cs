using UnityEngine;

public class AbilityOverlay : AbilityBase
{
    private GameObject _spawnedOverlayWeapon;

    public override void Execute()
    {
        if (abilityData != null && abilityData.rewardWeapon != null)
        {
            Transform weaponHolder = transform.root.GetComponentInChildren<PlayerWalking>()?.transform.Find("Weapon Holder");

            Transform targetParent = weaponHolder != null ? weaponHolder : firePoint;

            _spawnedOverlayWeapon = Instantiate(abilityData.rewardWeapon, targetParent);

            _spawnedOverlayWeapon.transform.localPosition = abilityData.rewardWeapon.transform.localPosition;
            _spawnedOverlayWeapon.transform.localRotation = abilityData.rewardWeapon.transform.localRotation;

            Debug.Log($"[AbilityOverlay] {abilityData.rewardWeapon.name} 장착 완료 (부모: {targetParent.name})");
        }
        else
        {
            Debug.LogWarning("[AbilityOverlay] rewardWeapon이 설정되지 않았습니다.");
        }
    }

    public override void StopAbility()
    {
        if (_spawnedOverlayWeapon != null)
        {
            Destroy(_spawnedOverlayWeapon);
            Debug.Log("[AbilityOverlay] 오버레이 무기 해제");
        }
    }
}
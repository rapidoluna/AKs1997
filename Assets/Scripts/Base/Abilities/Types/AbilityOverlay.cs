using UnityEngine;

public class AbilityOverlay : AbilityBase
{
    private GameObject _spawnedOverlayWeapon;

    public override void Execute()
    {
        if (abilityData != null && abilityData.rewardWeapon != null)
        {
            Transform targetParent = null;

            if (movement != null)
            {
                WeaponController weaponCtrl = movement.GetComponentInChildren<WeaponController>();
                if (weaponCtrl != null)
                {
                    targetParent = weaponCtrl.transform;
                }
            }

            if (targetParent == null)
            {
                Transform searchRoot = movement != null ? movement.transform : transform.root;
                targetParent = FindChildRecursive(searchRoot, "Weapon Holder");
            }

            if (targetParent == null)
            {
                targetParent = firePoint;
            }

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

    private Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform result = FindChildRecursive(child, name);
            if (result != null) return result;
        }
        return null;
    }
}
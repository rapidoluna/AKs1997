using UnityEngine;

public enum AbilityType { Speciality, Ultimate }
public enum AbilityActiveType { Projectile, Instant, Buff, Charge, Equip, Overlay }

[CreateAssetMenu(fileName = "AbilityData", menuName = "AKs97/AbilityData")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    public Sprite abilityIcon;
    public AbilityType type;
    public AbilityActiveType activeType;

    public float abilityCooltime;
    public float abilityDuration; // 0이면 무한 지속(토글), 0보다 크면 타이머 적용
    public float abilityDamage;
    public float abilityRange;

    public float moveSpeedMultiplier = 1f;
    public float maxHealthBonus = 0f;
    public float damageMultiplier = 1f;

    public float minChargeTime;
    public int maxTargets;
    public float targetAcquisitionSpeed;
    public Vector3 areaSize;
    public float pullForce;

    public GameObject abilityPrefab;   // 발사체 또는 효과 이펙트
    public GameObject rewardWeapon;    // 장착용 무기 또는 손에 생성될 장치
    public GameObject trackingMark;    // 타겟팅 표식
}
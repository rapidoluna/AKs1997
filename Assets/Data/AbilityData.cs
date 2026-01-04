using UnityEngine;

//능력이 주특기인지 특수기인지 구분
public enum AbilityType
{
    Speciality,
    Ultimate
}

public enum AbilityActiveType
{
    Projectile,
    Instant,
    Equip,
    Buff
}

[CreateAssetMenu(fileName = "AbilityData", menuName = "AKs97/AbilityData")]
public class AbilityData : ScriptableObject
{
    //능력 정보
    public AbilityType abilityType;
    public AbilityActiveType activeType;
    [SerializeField] private string abilityName;
    [SerializeField] private string abilityDescription;

    //쿨타임 및 지속시간, 특수기 요구량, 특수기 충전량
    public float abilityCooltime;
    public float abilityDuration;
    public float requiredGauge = 100;
    public float gaugeChargeMultiplier = 0.01f;

    //능력 범위 및 대미지
    public float abilityRange;
    public float abilityDamage;

    //능력 관련 비주얼 요소
    public GameObject abilityPrefab;
    public Sprite abilityIcon;

    //무기 지급 능력일 시 무기 데이터 받아옴
    public WeaponData rewardWeapon;

    //주특기일 때 쿨타임 사용, 특수기일 때 게이지 사용
    private void OnValidate()
    {
        if (abilityType == AbilityType.Speciality) requiredGauge = 0;
        else abilityCooltime = 0;
    }

    //프로퍼티
    public string AbilityName => abilityName;
    public string AbilityDescription => abilityDescription;
    public WeaponData RewardWeapon => rewardWeapon;
}

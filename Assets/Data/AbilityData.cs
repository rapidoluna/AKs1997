using UnityEngine;

//능력이 주특기인지 특수기인지 선택
public enum AbilityType
{
    Speciality,
    Ultimate
}

//능력 유형 선택
public enum AbilityActiveType
{
    Projectile,//발사체
    Instant,//즉발
    Equip,//장착
    Overlay,//동시 사용
    Buff,//버프
    Charge//능력 사용 키를 꾹 눌렀다가 사용
}

//무기 슬롯 결정
public enum WeaponSlotTarget
{
    None,
    Primary,
    Secondary,
    SpecialSlot,//제 3의 슬롯
    Temporary//임시로 사용(능력 유지 시간 지나면 사라짐)
}

//능력 범위 유형 선택
public enum AbilityAreaType
{
    Point,//
    Sphere,//원형
    Box,//사각형
    Cone//부채꼴
}

[CreateAssetMenu(fileName = "AbilityData", menuName = "AKs97/AbilityData")]
public class AbilityData : ScriptableObject
{
    public AbilityType abilityType;//주특기인지 특수기인지
    public AbilityActiveType activeType;//능력 유형
    [SerializeField] private string abilityName;//능력 이름
    [SerializeField, TextArea] private string abilityDescription;//능력 설명

    public float abilityCooltime;//능력 쿨타임
    public float abilityDuration;//능력 지속 시간
    public float requiredGauge = 100;//특수기인 경우 필요한 게이지
    public float gaugeChargeMultiplier = 0.01f;//특수기인 경우 게이지 충전 속도

    public WeaponData rewardWeapon;//능력이 무기를 사용하는 경우 지급받는 무기 //스마트 윈치 클로의 경우 본체 할당
    public WeaponSlotTarget slotTarget;//무기 슬롯
    public bool allowOriginalWeaponInput;//기존 무기와 동시에 사용하는 경우 true

    public float moveSpeedMultiplier = 1.0f;//능력 사용 시 이속 증가 정도
    public float damageMultiplier = 1.0f;//능력 사용 시 대미지 증가 정도

    public AbilityAreaType areaType;//범위 공격 능력인 경우 능력 범위
    public Vector3 areaSize = Vector3.one;//범위 크기
    public float abilityRange;//능력 사거리
    public float abilityDamage;//능력 대미지

    public GameObject abilityPrefab;//능력 모델 프리팹(무기가 아닌 경우 할당) //스마트 윈치 클로의 경우 발사체 할당
    public Sprite abilityIcon;//능력 아이콘

    //스마트 윈치 클로 관련
    public int maxTargets = 3;//스마트 윈치 클로의 최대 타겟 수
    public float minChargeTime = 0.5f;//최소한으로 눌러야 하는 시간
    public float targetAcquisitionSpeed = 0.2f;//타겟 하나를 잡는 데 걸리는 간격

    public GameObject trackingMarkPrefab;//적에게 남길 표식 프리팹

    public float pullForce = 20f;//적을 끌고 오는 힘의 세기
    public float stunDuration = 1.5f;//적 스턴 시간

    //주특기인 경우 필요 게이지 0, 특수기인 경우 쿨타임 0
    private void OnValidate()
    {
        if (abilityType == AbilityType.Speciality) requiredGauge = 0;
        else abilityCooltime = 0;
    }

    //프로퍼티
    public string AbilityName => abilityName;
    public string AbilityDescription => abilityDescription;
}
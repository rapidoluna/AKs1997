using UnityEngine;

public enum WeaponType
{
    Rifle,
    Shotgun,
    Smg,
    Pistol,
    Special
}

public enum FiringType
{
    Semi,
    Full,
    Burst,
    Charge,
    Accelerate
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "AKs97/WeaponData")]
public class WeaponData : ScriptableObject
{
    //무기 정보
    [SerializeField] private WeaponType weapon;
    [SerializeField] private FiringType[] firing;
    [SerializeField] private string weaponName;
    [SerializeField] private string weaponDescription;

    //무기 관련 비주얼 요소
    //추후에 사운드, 애니메이션 등의 요소도 추가 필요
    public GameObject weaponPrefab;
    public Sprite weaponIcon;

    //장탄 수, 대미지, 유효 사거리, 탄속, 연사력
    //탄속과 연사력은 모두 분당 계산임.
    [Min(1)] public int magSize;
    public int weaponDamage;
    public float effectiveRange;
    public float bulletSpeed;
    [Min(0.01f)] public float fireRate;

    //전술 재장전 시간, 공탄 재장전 시간
    public float tacticalReloadTimer;
    public float emptyReloadTimer;

    //표기 상 소모 탄환 수, 실제 발사되는 탄환 수
    public int usingBullet;
    public int firingBullet;

    [Header("Burst Weapon Options")]
    //점사식 무기 관련
    //점사 탄환 수, 점사 간격
    public int burstBullet;
    public float burstInterval;

    [Header("Charge Weapon Options")]
    //충전식 무기 관련
    //충전 시간, 최대 충전 정도
    public float chargeTime;
    public float maxCharge = 100;

    [Header("Accelerate Weapon Options")]
    //가속형 무기 관련
    //가속 시간, 가속 정도, 최대 가속 탄속
    public float accelerateTime;
    public float maxAccelerate;
    public float maxAccelerateSpeed;

    //프로퍼티
    public string WeaponName => weaponName;
    public FiringType[] Firing => firing;
}

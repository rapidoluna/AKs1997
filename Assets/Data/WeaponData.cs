using UnityEngine;

public enum WeaponType
{
    Rifle,
    SMG,
    Shotgun,
    Pistol,
    Special
}

public enum FireMode
{
    Semi,
    Full,
    Burst,
    Charge
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "AKs97/WeaponData")]
public class WeaponData : ScriptableObject
{
    //총기 정보
    public WeaponType weaponType;
    [SerializeField] private FireMode[] availableFireModes;
    [SerializeField] private string weaponName;
    [SerializeField] private string weaponDescription;

    //장탄 수, RPM, 탄환 당 대미지, 유효 사거리, 소모 탄환 수
    public int magSize;
    public float maxRPM;
    public int bulletDamage;
    public float effectiveRange;
    public float usingBullet;

    //발사 탄환 개수, 탈출 시 제거, 가속 여부
    public int firingBullets;
    [SerializeField] private bool removeWhenExtract;
    [SerializeField] private bool hasAcceleration;

    //몇 점사인가, 가속 시 최저 RPM, 가속 시 최대 RPM, 가속 속도
    public int burstBullets;
    public float minAccelerate;
    [SerializeField] private float maxAccelerate;
    [SerializeField] private float accelerateSpeed;

    //무기 관련 비주얼 요소
    public GameObject weaponPrefab;
    public Sprite weaponIcon;

    //재장전 시간
    [SerializeField] private float tacticalReloadTime;
    [SerializeField] private float emptyReloadTime;
    [SerializeField] private string tacticalReloadAnimTrigger = "TacticalReload";
    [SerializeField] private string emptyReloadAnimTrigger = "EmptyReload";

    //프로퍼티
    public string WeaponName => weaponName;
    public string Description => weaponDescription;
    public FireMode[] AvailableFireModes => availableFireModes;
    public float TacticalReloadTime => tacticalReloadTime;
    public float EmptyReloadTime => emptyReloadTime;
    public string TacticalReloadAnimTrigger => tacticalReloadAnimTrigger;
    public string EmptyReloadAnimTrigger => emptyReloadAnimTrigger;
    public bool RemoveWhenExtract => removeWhenExtract;
    public bool HasAcceleration => hasAcceleration;
}

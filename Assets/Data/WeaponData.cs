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
    Burst
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "AKs97/WeaponData")]
public class WeaponData : ScriptableObject
{
    //무기 정보
    //무기 종류, 발사 형식, 이름, 설명
    public WeaponType weaponType;
    [SerializeField] private FireMode[] availableFireModes;
    [SerializeField] private string weaponName;
    [SerializeField] private string weaponDescription;

    //전투 관련 정보
    //장탄 수, 발당 대미지, 유효 사거리, 표기 상 사용 탄환 수, 실제 발사되는 탄환 수, 탄속, 상대를 맞췄을 때 밀어내는 정도
    public int magSize;
    public int bulletDamage;
    public float effectiveRange;
    public float usingBullet;
    public int firingBullets;
    public float bulletSpeed;
    public float impactForce;

    //사격 관련 정보
    //수직 반동, 수평 반동, 반동 복구 정도, 시작 탄퍼짐, 최대 탄퍼짐
    public float verticalRecoil;
    public float horizontalRecoil;
    public float recoilRecovery;
    public float baseSpread;
    public float maxSpread;

    //조작 관련 정보
    //조준 속도, 조준 시 FOV, 무기를 들고 있을 때 속도, 탈출 시 제거 여부
    public float adsSpeed;
    public float adsZoomFOV;
    public float movementWeight;
    [SerializeField] private bool removeWhenExtract;

    //비주얼 및 소리
    //무기 모델, 아이콘, 사격음, 공탄 사격음, 재장전 소리
    public GameObject weaponPrefab;
    public Sprite weaponIcon;
    public AudioClip fireSound;
    public AudioClip emptySound;
    public AudioClip reloadSound;

    //재장전
    //전술 재장전 시간, 공탄 재장전 시간, 전술 재장전 애니메이션, 공탄 재장전 애니메이션
    [SerializeField] private float tacticalReloadTime;
    [SerializeField] private float emptyReloadTime;
    [SerializeField] private string tacticalReloadAnimTrigger = "TacticalReload";
    [SerializeField] private string emptyReloadAnimTrigger = "EmptyReload";


    //프로퍼티, 외부 참조용 변수
    public string WeaponName => weaponName;
    public string Description => weaponDescription;
    public FireMode[] AvailableFireModes => availableFireModes;
    public float TacticalReloadTime => tacticalReloadTime;
    public float EmptyReloadTime => emptyReloadTime;
    public string TacticalReloadAnimTrigger => tacticalReloadAnimTrigger;
    public string EmptyReloadAnimTrigger => emptyReloadAnimTrigger;
    public bool RemoveWhenExtract => removeWhenExtract;
}
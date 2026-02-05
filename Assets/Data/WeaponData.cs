using UnityEngine;

public enum WeaponType
{
    Rifle,
    Shotgun,
    Smg,
    Pistol,
    Special,
    Melee
}

public enum FiringType
{
    Semi,
    Full,
    Burst,
    Charge,
    Accelerate,
    Swing
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "AKs97/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("General")]
    [SerializeField] private WeaponType weapon;
    [SerializeField] private FiringType[] firing;
    [SerializeField] private string weaponName;
    [SerializeField] private string weaponDescription;

    [Header("Visuals")]
    public GameObject weaponPrefab;
    public GameObject bulletPrefab;
    public Sprite weaponIcon;

    [Header("Stats")]
    public int weaponDamage;
    public float effectiveRange;
    [Min(0.01f)] public float fireRate;

    [Header("Gun Settings")]
    [Min(0)] public int magSize;
    public float bulletSpeed;
    public float tacticalReloadTimer;
    public float emptyReloadTimer;
    public int usingBullet;
    public int firingBullet;

    [Header("Burst Settings")]
    public int burstBullet;
    public float burstInterval;

    [Header("Charge Settings")]
    public float chargeTime;
    public float maxCharge = 100;
    public float maxHold;

    [Header("Accelerate Settings")]
    public float accelerateTime;
    public float maxAccelerate;
    public float maxAccelerateSpeed;

    [Header("Melee Settings")]
    public MeleeData meleeData;

    public string WeaponName => weaponName;
    public FiringType[] Firing => firing;
    public WeaponType Type => weapon;
}

[System.Serializable]
public struct MeleeData
{
    public float attackRadius;
    public int maxComboCount;
    public float comboResetTime;
    public float dashForce;
    public LayerMask hitLayer;
}
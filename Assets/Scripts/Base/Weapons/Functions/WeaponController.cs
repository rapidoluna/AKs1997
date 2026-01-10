using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;

    private WeaponAmmo _ammo;
    private WeaponShooting _shooting;
    private WeaponReloading _reloading;

    private void Awake()
    {
        _ammo = GetComponent<WeaponAmmo>();
        _shooting = GetComponent<WeaponShooting>();
        _reloading = GetComponent<WeaponReloading>();
    }

    private void Start()
    {
        if (weaponData != null)
        {
            InitializeModules(weaponData);
        }
    }

    public void InitializeModules(WeaponData newData)
    {
        weaponData = newData;

        _ammo.Init(weaponData);
        _shooting.Init(weaponData);
        _reloading.Init(weaponData);

        // 무기 정보 수신 확인 로그
        Debug.Log($"<color=cyan>[Weapon System]</color> 데이터 수신 완료: {weaponData.WeaponName}");
        Debug.Log($"[정보] 공격력: {weaponData.weaponDamage} | 장탄수: {weaponData.magSize} | 연사속도(분당): {weaponData.fireRate} | 탄속(분당) : {weaponData.bulletSpeed}");
    }

    public WeaponData GetWeaponData()
    {
        return weaponData;
    }
}
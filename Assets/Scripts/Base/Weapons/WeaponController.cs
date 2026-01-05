using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponData currentWeaponData;

    public WeaponShooting Shooting { get; private set; }
    public WeaponAmmo Ammo { get; private set; }
    public WeaponReloading Reloading { get; private set; }
    public WeaponAiming Aiming { get; private set; }
    public WeaponRecoil Recoil { get; private set; }
    public WeaponEffects Effects { get; private set; }
    public WeaponFireModeSelect FireModeSelect { get; private set; }

    private void Awake()
    {
        Shooting = GetComponent<WeaponShooting>();
        Ammo = GetComponent<WeaponAmmo>();
        Reloading = GetComponent<WeaponReloading>();
        Aiming = GetComponent<WeaponAiming>();
        Recoil = GetComponent<WeaponRecoil>();
        Effects = GetComponent<WeaponEffects>();
        FireModeSelect = GetComponent<WeaponFireModeSelect>();

        if (currentWeaponData != null)
        {
            ApplyWeaponData(currentWeaponData);
        }
    }

    private void Update()
    {
        if (currentWeaponData == null) return;

        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButton(0))
        {
            Shooting.TryFire();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reloading.StartReload();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            FireModeSelect.ChangeFireMode();
        }
    }

    public void ApplyWeaponData(WeaponData data)
    {
        currentWeaponData = data;
        Ammo.Initialize(data.magSize);
    }

    public WeaponData GetWeaponData() => currentWeaponData;
}
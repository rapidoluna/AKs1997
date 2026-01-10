using System.Collections;
using UnityEngine;

public class WeaponShooting : MonoBehaviour
{
    private WeaponData _data;
    private WeaponAmmo _ammo;
    private WeaponReloading _reloading;
    private WeaponAiming _aiming;
    private float _lastFireTime;

    [SerializeField] private WeaponRecoilCamera recoilCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float baseSpread = 2.0f;

    public bool IsShooting { get; private set; }

    private void Awake()
    {
        _ammo = GetComponent<WeaponAmmo>();
        _reloading = GetComponent<WeaponReloading>();
        _aiming = GetComponent<WeaponAiming>();
    }

    public void Init(WeaponData data)
    {
        _data = data;
        if (firePoint == null)
        {
            FirePointSettings settings = Object.FindFirstObjectByType<FirePointSettings>();
            if (settings != null) firePoint = settings.transform;
        }
    }

    private void Update()
    {
        if (_data == null) return;

        bool isFullAuto = _data.Firing[0] == FiringType.Full;
        IsShooting = isFullAuto ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        HandleFiringInput();
    }

    private void HandleFiringInput()
    {
        float fireInterval = 60f / _data.fireRate;
        if (Time.time < _lastFireTime + fireInterval) return;
        if (_reloading != null && _reloading.IsReloading) return;
        if (_ammo != null && _ammo.IsEmpty) return;

        if (IsShooting)
        {
            if (_data.Firing[0] == FiringType.Burst) StartCoroutine(BurstRoutine());
            else ExecuteShoot();
        }
    }

    private void ExecuteShoot()
    {
        if (_ammo.ConsumeAmmo(_data.usingBullet))
        {
            _lastFireTime = Time.time;
            float recoilMult = _aiming != null ? _aiming.RecoilMultiplier : 1f;
            if (recoilCamera != null) recoilCamera.TriggerRecoil(recoilMult);

            float speed = _data.bulletSpeed / 60f;
            float spread = baseSpread * (_aiming != null ? _aiming.SpreadMultiplier : 1f);

            for (int i = 0; i < _data.firingBullet; i++)
            {
                GenerateProjectile(speed, spread);
            }
        }
    }

    private IEnumerator BurstRoutine()
    {
        _lastFireTime = Time.time + (_data.burstBullet * _data.burstInterval);
        float speed = _data.bulletSpeed / 60f;
        float recoilMult = _aiming != null ? _aiming.RecoilMultiplier : 1f;
        float spread = baseSpread * (_aiming != null ? _aiming.SpreadMultiplier : 1f);

        for (int i = 0; i < _data.burstBullet; i++)
        {
            if (_ammo.IsEmpty) break;
            if (_ammo.ConsumeAmmo(_data.usingBullet))
            {
                if (recoilCamera != null) recoilCamera.TriggerRecoil(recoilMult);
                for (int j = 0; j < _data.firingBullet; j++) GenerateProjectile(speed, spread);
            }
            yield return new WaitForSeconds(_data.burstInterval);
        }
    }

    private void GenerateProjectile(float speed, float spreadRange)
    {
        if (_data.bulletPrefab == null || firePoint == null) return;
        Quaternion spread = Quaternion.Euler(Random.Range(-spreadRange, spreadRange), Random.Range(-spreadRange, spreadRange), 0);
        GameObject bullet = Instantiate(_data.bulletPrefab, firePoint.position, firePoint.rotation * spread);
        Projectile projectile = bullet.GetComponent<Projectile>();
        if (projectile != null) projectile.Init(speed, _data.weaponDamage, _data.effectiveRange);
    }
}
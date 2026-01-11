using System.Collections;
using UnityEngine;

public class WeaponShooting : MonoBehaviour
{
    [SerializeField] private WeaponData _data;
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

    private void OnEnable()
    {
        if (_reloading == null) _reloading = GetComponent<WeaponReloading>();
        if (_reloading != null)
            _reloading.OnReloadComplete += SetPostReloadDelay;
    }

    private void OnDisable()
    {
        IsShooting = false;
        if (_reloading != null)
            _reloading.OnReloadComplete -= SetPostReloadDelay;
    }

    private void SetPostReloadDelay(float delay)
    {
        _lastFireTime = Time.time + delay;
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
        if (_data == null || _reloading.IsReloading) return;
        bool isFullAuto = _data.Firing[0] == FiringType.Full;
        IsShooting = isFullAuto ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);
        HandleFiringInput();
    }

    private void HandleFiringInput()
    {
        if (!IsShooting || Time.time < _lastFireTime || _ammo.IsEmpty) return;
        float fireInterval = 60f / _data.fireRate;
        _lastFireTime = Time.time + fireInterval;
        if (_data.Firing[0] == FiringType.Burst) StartCoroutine(BurstRoutine());
        else if (_ammo.ConsumeAmmo(_data.usingBullet)) Fire();
    }

    private void Fire()
    {
        float speed = _data.bulletSpeed / 60f;
        float recoilMult = _aiming != null ? _aiming.RecoilMultiplier : 1f;
        if (recoilCamera != null) recoilCamera.TriggerRecoil(recoilMult);
        for (int i = 0; i < _data.firingBullet; i++)
        {
            float spread = baseSpread * (_aiming != null ? _aiming.SpreadMultiplier : 1f);
            GenerateProjectile(speed, spread);
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
                GenerateProjectile(speed, spread);
            }
            yield return new WaitForSeconds(_data.burstInterval);
        }
    }

    private void GenerateProjectile(float speed, float spreadRange)
    {
        if (BulletPool.Instance == null) return;
        GameObject bullet = BulletPool.Instance.GetBullet();
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation * Quaternion.Euler(Random.Range(-spreadRange, spreadRange), Random.Range(-spreadRange, spreadRange), 0);
        Projectile projectile = bullet.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Init(speed, _data.weaponDamage, _data.effectiveRange, transform.root.gameObject);
        }
    }

    public WeaponData GetWeaponData()
    {
        return _data;
    }
}
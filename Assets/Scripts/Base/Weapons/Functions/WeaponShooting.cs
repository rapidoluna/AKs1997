using System.Collections;
using System.Linq;
using UnityEngine;

public class WeaponShooting : MonoBehaviour
{
    [SerializeField] private WeaponData _data;
    private WeaponAmmo _ammo;
    private WeaponReloading _reloading;
    private WeaponAiming _aiming;
    private float _lastFireTime;
    private Camera _mainCamera;

    [SerializeField] private WeaponRecoilCamera recoilCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float baseSpread = 2.0f;

    private float _currentCharge = 0f;
    private float _accelerationTimer = 0f;

    public bool IsShooting { get; private set; }

    private void Awake()
    {
        _ammo = GetComponent<WeaponAmmo>();
        _reloading = GetComponent<WeaponReloading>();
        _aiming = GetComponent<WeaponAiming>();
        _mainCamera = Camera.main;
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
        _currentCharge = 0f;
        _accelerationTimer = 0f;
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
        if (firePoint == null) firePoint = transform.Find("FirePoint");
    }

    private void Update()
    {
        if (PlayerHealth.IsDead || _data == null || _reloading.IsReloading)
        {
            IsShooting = false;
            _currentCharge = 0f;
            return;
        }

        HandleFiringMode();
    }

    private void HandleFiringMode()
    {
        FiringType primaryMode = _data.Firing[0];

        switch (primaryMode)
        {
            case FiringType.Semi:
                if (Input.GetMouseButtonDown(0)) TryFire();
                break;
            case FiringType.Full:
                if (Input.GetMouseButton(0)) TryFire();
                break;
            case FiringType.Burst:
                if (Input.GetMouseButtonDown(0)) TryFire();
                break;
            case FiringType.Charge:
                HandleChargeMode();
                break;
            case FiringType.Accelerate:
                HandleAccelerateMode();
                break;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _currentCharge = 0f;
            _accelerationTimer = 0f;
            IsShooting = false;
        }
    }

    private void HandleChargeMode()
    {
        bool isFullAutoCharge = _data.Firing.Contains(FiringType.Full);

        if (Input.GetMouseButton(0))
        {
            _currentCharge += Time.deltaTime;
            float chargeRatio = Mathf.Clamp01(_currentCharge / _data.chargeTime);

            if (isFullAutoCharge && chargeRatio >= 1f)
            {
                IsShooting = true;
                TryFire();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!isFullAutoCharge && _currentCharge >= 0.1f)
            {
                FireChargeSingle();
            }
            _currentCharge = 0f;
            IsShooting = false;
        }
    }

    private void HandleAccelerateMode()
    {
        if (Input.GetMouseButton(0))
        {
            IsShooting = true;
            _accelerationTimer += Time.deltaTime;
            TryFire();
        }
        else
        {
            _accelerationTimer = 0f;
            IsShooting = false;
        }
    }

    private void TryFire()
    {
        if (Time.time < _lastFireTime || _ammo.IsEmpty) return;

        float rpm = _data.fireRate;
        if (_data.Firing[0] == FiringType.Accelerate)
        {
            float t = Mathf.Clamp01(_accelerationTimer / _data.accelerateTime);
            rpm = Mathf.Lerp(_data.fireRate, _data.maxAccelerate, t);
        }

        float fireInterval = 60f / rpm;
        _lastFireTime = Time.time + fireInterval;

        if (_data.Firing[0] == FiringType.Burst) StartCoroutine(BurstRoutine());
        else if (_ammo.ConsumeAmmo(_data.usingBullet)) Fire();
    }

    private void FireChargeSingle()
    {
        if (_ammo.IsEmpty) return;
        float chargeRatio = Mathf.Clamp01(_currentCharge / _data.chargeTime);
        if (_ammo.ConsumeAmmo(_data.usingBullet))
        {
            float speed = _data.bulletSpeed / 60f;
            int finalDamage = (int)(_data.weaponDamage * (1f + chargeRatio * 2f));
            GenerateProjectile(speed, baseSpread * (_aiming != null ? _aiming.SpreadMultiplier : 1f), finalDamage);
        }
    }

    private void Fire()
    {
        float speed = _data.bulletSpeed / 60f;
        if (_data.Firing[0] == FiringType.Accelerate)
        {
            float t = Mathf.Clamp01(_accelerationTimer / _data.accelerateTime);
            speed = Mathf.Lerp(_data.bulletSpeed, _data.maxAccelerateSpeed, t) / 60f;
        }

        float recoilMult = _aiming != null ? _aiming.RecoilMultiplier : 1f;
        if (recoilCamera != null) recoilCamera.TriggerRecoil(recoilMult);

        for (int i = 0; i < _data.firingBullet; i++)
        {
            float spread = baseSpread * (_aiming != null ? _aiming.SpreadMultiplier : 1f);
            GenerateProjectile(speed, spread, _data.weaponDamage);
        }

        if (CrosshairManager.Instance != null) CrosshairManager.Instance.FireExertion();
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
                GenerateProjectile(speed, spread, _data.weaponDamage);
            }
            yield return new WaitForSeconds(_data.burstInterval);
        }
    }

    private void GenerateProjectile(float speed, float spreadRange, int damage)
    {
        if (BulletPool.Instance == null) return;
        GameObject bullet = BulletPool.Instance.GetBullet();

        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, _data.effectiveRange)
            ? hit.point : ray.GetPoint(_data.effectiveRange);

        bullet.transform.position = firePoint.position;
        Vector3 direction = (targetPoint - firePoint.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        float spreadX = Random.Range(-spreadRange, spreadRange);
        float spreadY = Random.Range(-spreadRange, spreadRange);
        bullet.transform.rotation = targetRotation * Quaternion.Euler(spreadX, spreadY, 0);

        Projectile projectile = bullet.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Init(speed, damage, _data.effectiveRange, transform.root.gameObject);
        }
    }

    public WeaponData GetWeaponData() => _data;
}
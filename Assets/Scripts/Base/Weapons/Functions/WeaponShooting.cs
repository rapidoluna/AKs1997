using System;
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
    [SerializeField] private Transform shellEjectPoint;
    [SerializeField] private float baseSpread = 2.0f;

    private float _currentCharge = 0f;
    private float _accelerationTimer = 0f;
    private float _damageMultiplier = 1f;

    private int _remainingReloads = -1;
    public event Action OnResourceExhausted;

    public bool IsShooting { get; private set; }
    public Transform FirePoint => firePoint;

    private void Awake()
    {
        _ammo = GetComponent<WeaponAmmo>();
        _reloading = GetComponent<WeaponReloading>();
        _aiming = GetComponent<WeaponAiming>();
        _mainCamera = Camera.main;

        if (recoilCamera == null && _mainCamera != null)
        {
            recoilCamera = _mainCamera.GetComponent<WeaponRecoilCamera>();
            if (recoilCamera == null) recoilCamera = _mainCamera.GetComponentInChildren<WeaponRecoilCamera>();
        }
    }

    private void OnEnable()
    {
        if (_reloading == null) _reloading = GetComponent<WeaponReloading>();
        if (_reloading != null) _reloading.OnReloadComplete += SetPostReloadDelay;
    }

    private void OnDisable()
    {
        IsShooting = false;
        _currentCharge = 0f;
        _accelerationTimer = 0f;
        if (_reloading != null) _reloading.OnReloadComplete -= SetPostReloadDelay;
    }

    private void SetPostReloadDelay(float delay)
    {
        _lastFireTime = Time.time + delay;
        if (_remainingReloads > 0) _remainingReloads--;
    }

    public void Init(WeaponData data)
    {
        _data = data;
        if (firePoint == null) firePoint = transform.Find("FirePoint");
    }

    public WeaponData GetWeaponData() => _data;
    public void SetDamageMultiplier(float multiplier) => _damageMultiplier = multiplier;
    public void SetReloadLimit(int maxReloads) => _remainingReloads = maxReloads;

    private void Update()
    {
        if (PlayerHealth.IsDead || _data == null || (_reloading != null && _reloading.IsReloading))
        {
            IsShooting = false;
            _currentCharge = 0f;
            return;
        }

        if (_remainingReloads == 0 && _ammo != null && _ammo.IsEmpty && !IsShooting)
        {
            OnResourceExhausted?.Invoke();
            _remainingReloads = -1;
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
        bool isBurstCharge = _data.Firing.Contains(FiringType.Burst);

        if (Input.GetMouseButton(0))
        {
            if (_currentCharge < 0) return;
            _currentCharge += Time.deltaTime;

            if (!isFullAutoCharge && _currentCharge > _data.chargeTime + _data.maxHold)
            {
                if (isBurstCharge)
                {
                    if (Time.time >= _lastFireTime && !_ammo.IsEmpty) StartCoroutine(BurstRoutine());
                }
                else FireChargeSingle();

                _currentCharge = -1f;
                IsShooting = false;
                if (CrosshairManager.Instance != null) CrosshairManager.Instance.SetChargeRatio(0f);
                return;
            }

            float chargeRatio = Mathf.Clamp01(_currentCharge / _data.chargeTime);
            if (CrosshairManager.Instance != null) CrosshairManager.Instance.SetChargeRatio(chargeRatio);

            if (isFullAutoCharge && chargeRatio >= 1f)
            {
                IsShooting = true;
                TryFire();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (_currentCharge >= 0f)
            {
                if (isBurstCharge)
                {
                    if (_currentCharge >= _data.chargeTime * 0.5f && Time.time >= _lastFireTime && !_ammo.IsEmpty) StartCoroutine(BurstRoutine());
                }
                else if (!isFullAutoCharge && !isBurstCharge && _currentCharge >= 0.1f) FireChargeSingle();
            }

            if (CrosshairManager.Instance != null) CrosshairManager.Instance.SetChargeRatio(0f);
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
        if (Time.time < _lastFireTime || (_ammo != null && _ammo.IsEmpty)) return;

        float rpm = _data.fireRate;
        if (_data.Firing[0] == FiringType.Accelerate)
        {
            float t = Mathf.Clamp01(_accelerationTimer / _data.accelerateTime);
            rpm = Mathf.Lerp(_data.fireRate, _data.maxAccelerate, t);
        }

        float fireInterval = 60f / rpm;
        _lastFireTime = Time.time + fireInterval;

        if (_data.Firing[0] == FiringType.Burst) StartCoroutine(BurstRoutine());
        else if (_ammo != null && _ammo.ConsumeAmmo(_data.usingBullet))
        {
            for (int i = 0; i < _data.usingBullet; i++) EjectCasing();
            Fire();
        }
    }

    private void FireChargeSingle()
    {
        if (_ammo != null && _ammo.IsEmpty) return;
        float chargeRatio = Mathf.Clamp01(_currentCharge / _data.chargeTime);
        if (_ammo != null && _ammo.ConsumeAmmo(_data.usingBullet))
        {
            for (int i = 0; i < _data.usingBullet; i++) EjectCasing();

            float recoilMult = _aiming != null ? _aiming.RecoilMultiplier : 1f;
            if (recoilCamera != null) recoilCamera.TriggerRecoil(recoilMult * (1f + chargeRatio));

            float speed = _data.bulletSpeed / 60f;
            int finalDamage = (int)(_data.weaponDamage * (1f + chargeRatio * 2f) * _damageMultiplier);
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

        int finalDamage = (int)(_data.weaponDamage * _damageMultiplier);
        for (int i = 0; i < _data.firingBullet; i++)
        {
            float spread = baseSpread * (_aiming != null ? _aiming.SpreadMultiplier : 1f);
            GenerateProjectile(speed, spread, finalDamage);
        }
        if (CrosshairManager.Instance != null) CrosshairManager.Instance.FireExertion();
    }

    private IEnumerator BurstRoutine()
    {
        _lastFireTime = Time.time + (_data.burstBullet * _data.burstInterval);
        float speed = _data.bulletSpeed / 60f;
        float recoilMult = _aiming != null ? _aiming.RecoilMultiplier : 1f;
        float spread = baseSpread * (_aiming != null ? _aiming.SpreadMultiplier : 1f);
        int finalDamage = (int)(_data.weaponDamage * _damageMultiplier);

        for (int i = 0; i < _data.burstBullet; i++)
        {
            if (_ammo != null && _ammo.IsEmpty) break;
            if (_ammo != null && _ammo.ConsumeAmmo(_data.usingBullet))
            {
                for (int j = 0; j < _data.usingBullet; j++) EjectCasing();
                if (recoilCamera != null) recoilCamera.TriggerRecoil(recoilMult);
                GenerateProjectile(speed, spread, finalDamage);
            }
            yield return new WaitForSeconds(_data.burstInterval);
        }
    }

    private void GenerateProjectile(float speed, float spreadRange, int damage)
    {
        if (BulletPool.Instance == null) return;
        GameObject bullet = BulletPool.Instance.GetBullet();
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, _data.effectiveRange) ? hit.point : ray.GetPoint(_data.effectiveRange);

        bullet.transform.position = firePoint.position;
        float randX = UnityEngine.Random.Range(-spreadRange, spreadRange);
        float randY = UnityEngine.Random.Range(-spreadRange, spreadRange);
        bullet.transform.rotation = Quaternion.LookRotation((targetPoint - firePoint.position).normalized) * Quaternion.Euler(randX, randY, 0);

        Projectile p = bullet.GetComponent<Projectile>();
        if (p != null) p.Init(speed, damage, _data.effectiveRange, transform.root.gameObject);
    }

    private void EjectCasing()
    {
        if (_data == null || !_data.ejectCasing || _data.casingPrefab == null || shellEjectPoint == null) return;

        GameObject casing = Instantiate(_data.casingPrefab, shellEjectPoint.position, shellEjectPoint.rotation);
        Rigidbody rb = casing.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float randX = UnityEngine.Random.Range(0.8f, 1.2f);
            float randY = UnityEngine.Random.Range(0.3f, 0.7f);
            float randZ = UnityEngine.Random.Range(-0.2f, 0.2f);

            Vector3 ejectDir = (shellEjectPoint.right * randX + shellEjectPoint.up * randY + shellEjectPoint.forward * randZ).normalized;
            rb.AddForce(ejectDir * _data.ejectionForce, ForceMode.Impulse);
            rb.AddTorque(UnityEngine.Random.insideUnitSphere * 15f, ForceMode.Impulse);
        }
        Destroy(casing, 2f);
    }
}
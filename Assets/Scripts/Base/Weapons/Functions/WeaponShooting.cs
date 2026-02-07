using UnityEngine;

public class WeaponShooting : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Transform firePoint;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private bool isAIControlled = false;

    private float _nextFireTime = 0f;
    private WeaponAmmo _ammo;
    private WeaponRecoilCamera _recoilCamera;

    public WeaponData GetWeaponData() => weaponData;

    private void Awake()
    {
        _ammo = GetComponent<WeaponAmmo>();

        if (Camera.main != null)
            _recoilCamera = Camera.main.GetComponent<WeaponRecoilCamera>();
    }

    private void Update()
    {
        if (isAIControlled || PlayerHealth.IsDead) return;

        if (weaponData.isAutomatic)
        {
            if (Input.GetButton("Fire1") && Time.time >= _nextFireTime)
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= _nextFireTime)
            {
                Shoot();
            }
        }
    }

    public void Shoot()
    {
        if (_ammo != null && !_ammo.ConsumeAmmo()) return;

        _nextFireTime = Time.time + (1f / weaponData.fireRate);

        if (muzzleFlash != null) muzzleFlash.Play();

        SpawnBullet();
        ApplyRecoil();
    }

    private void SpawnBullet()
    {
        Vector3 targetPoint;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, weaponData.range))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(weaponData.range);

        Vector3 direction = (targetPoint - firePoint.position).normalized;

        if (BulletPool.Instance != null)
        {
            GameObject bullet = BulletPool.Instance.GetBullet(firePoint.position, Quaternion.LookRotation(direction));

            Projectile projectile = bullet.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Initialize(direction, weaponData.damage, weaponData.bulletSpeed);
            }
        }
    }

    private void ApplyRecoil()
    {
        if (!isAIControlled && _recoilCamera != null)
        {
            _recoilCamera.AddRecoil(weaponData.recoilX, weaponData.recoilY);
        }
    }
}
using System.Collections;
using UnityEngine;

public class WeaponShooting : MonoBehaviour
{
    //무기를 사용하기 위한 스크립트, 무기 프리팹에 부착.

    [SerializeField] private WeaponData _data;//무기 데이터
    private WeaponAmmo _ammo;//WeaponAmmo 스크립트
    private WeaponReloading _reloading;//WeaponReloading 스크립트
    private WeaponAiming _aiming;//WeaponAiming 스크립트
    private float _lastFireTime;//마지막으로 쏜 시간
    private Camera _mainCamera;//카메라

    [SerializeField] private WeaponRecoilCamera recoilCamera;//반동을 줄 카메라
    [SerializeField] private Transform firePoint;//발사 지점
    [SerializeField] private float baseSpread = 2.0f;//기본 탄퍼짐

    public bool IsShooting { get; private set; }//쏘는 상태인지 아닌지

    private void Awake()
    {
        //위에서 언급한 스크립트들과 메인 카메라를 가져옴
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
        if (_reloading != null)
            _reloading.OnReloadComplete -= SetPostReloadDelay;
    }

    private void SetPostReloadDelay(float delay)
    {
        _lastFireTime = Time.time + delay;//재장전 후 딜레이 시간 계산
    }

    public void Init(WeaponData data)
    {
        _data = data;

        if (firePoint == null)
        {
            firePoint = transform.Find("FirePoint");//발사 지점 누락 시 발사 지점 오브젝트 검색
        }

        if (firePoint == null)
        {
            Debug.LogError($"{gameObject.name} Error!");//검색 후에도 발사 지점 누락 시 에러
        }
    }

    private void Update()
    {
        if (PlayerHealth.IsDead)
        {
            IsShooting = false;//플레이어 사망 시 총기 발사 멈춤
            return;
        }

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
        if (CrosshairManager.Instance != null)
        {
            CrosshairManager.Instance.FireExertion();
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

        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, _data.effectiveRange))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(_data.effectiveRange);
        }

        bullet.transform.position = firePoint.position;

        Vector3 direction = (targetPoint - firePoint.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        float spreadX = Random.Range(-spreadRange, spreadRange);
        float spreadY = Random.Range(-spreadRange, spreadRange);
        bullet.transform.rotation = targetRotation * Quaternion.Euler(spreadX, spreadY, 0);

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
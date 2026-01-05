using System.Collections;
using UnityEngine;

public class WeaponShooting : MonoBehaviour
{
    private WeaponData _data;
    private WeaponAmmo _ammo;
    private WeaponReloading _reloading;
    private float _lastFireTime;

    private void Awake()
    {
        _ammo = GetComponent<WeaponAmmo>();
        _reloading = GetComponent<WeaponReloading>();
    }

    public void Init(WeaponData data)
    {
        _data = data;
    }

    private void Update()
    {
        if (_data == null) return;
        HandleFiringInput();
    }

    private void HandleFiringInput()
    {
        float fireInterval = 60f / _data.fireRate;

        if (Time.time < _lastFireTime + fireInterval) return;
        if (_reloading.IsReloading || _ammo.IsEmpty) return;
        if (_data.Firing == null || _data.Firing.Length == 0) return;

        FiringType currentMode = _data.Firing[0];

        switch (currentMode)
        {
            case FiringType.Full:
                if (Input.GetMouseButton(0)) ExecuteShoot();
                break;
            case FiringType.Semi:
                if (Input.GetMouseButtonDown(0)) ExecuteShoot();
                break;
            case FiringType.Burst:
                if (Input.GetMouseButtonDown(0)) StartCoroutine(BurstRoutine());
                break;
        }
    }

    private void ExecuteShoot()
    {
        if (_ammo.ConsumeAmmo(_data.usingBullet))
        {
            _lastFireTime = Time.time;
            float convertedBulletSpeed = _data.bulletSpeed / 60f;

            for (int i = 0; i < _data.firingBullet; i++)
            {
                GenerateProjectile(convertedBulletSpeed);
            }
        }
    }

    private IEnumerator BurstRoutine()
    {
        float fireInterval = 60f / _data.fireRate;
        _lastFireTime = Time.time + (_data.burstBullet * _data.burstInterval);
        float convertedBulletSpeed = _data.bulletSpeed / 60f;

        for (int i = 0; i < _data.burstBullet; i++)
        {
            if (_ammo.IsEmpty) break;
            if (_ammo.ConsumeAmmo(_data.usingBullet))
            {
                for (int j = 0; j < _data.firingBullet; j++)
                {
                    GenerateProjectile(convertedBulletSpeed);
                }
            }
            yield return new WaitForSeconds(_data.burstInterval);
        }
    }

    private void GenerateProjectile(float speed)
    {
        Debug.Log($"[Shooting] {_data.WeaponName} ¹ß»çµÊ. (Åº¼Ó: {speed} m/s) (ÇöÀç ÀÜÅº ¼ö: {_ammo.CurrentAmmo})");
    }
}
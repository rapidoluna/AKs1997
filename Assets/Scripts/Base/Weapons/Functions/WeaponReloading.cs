using System.Collections;
using UnityEngine;
using System;

public class WeaponReloading : MonoBehaviour
{
    private WeaponData _data;
    private WeaponAmmo _ammo;
    private WeaponShooting _shooting;
    private bool _isReloading;

    public event Action OnReloadStart;
    public event Action<float> OnReloadComplete;

    public bool IsReloading => _isReloading;

    private void Awake()
    {
        _ammo = GetComponent<WeaponAmmo>();
        _shooting = GetComponent<WeaponShooting>();
    }

    public void Init(WeaponData data)
    {
        _data = data;
        _isReloading = false;
    }

    private void Update()
    {
        if (_data == null || _isReloading || (_shooting != null && _shooting.IsShooting)) return;

        if (Input.GetKeyDown(KeyCode.R) && !_ammo.IsFull)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    private IEnumerator ReloadRoutine()
    {
        _isReloading = true;
        OnReloadStart?.Invoke();

        float reloadTime = _ammo.IsEmpty ? _data.emptyReloadTimer : _data.tacticalReloadTimer;
        yield return new WaitForSeconds(reloadTime);

        _ammo.Refill();
        _isReloading = false;

        OnReloadComplete?.Invoke(0.5f);
    }
}
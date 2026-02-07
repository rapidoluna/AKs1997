using UnityEngine;
using System;

public class WeaponAmmo : MonoBehaviour
{
    private WeaponData _data;
    private int _currentAmmo;

    public event Action<int, int> OnAmmoChanged;

    public int CurrentAmmo
    {
        get => _currentAmmo;
        private set
        {
            if (_data == null) return;
            _currentAmmo = Mathf.Clamp(value, 0, _data.magSize);
            OnAmmoChanged?.Invoke(_currentAmmo, _data.magSize);
        }
    }

    public bool IsEmpty => _currentAmmo <= 0;
    public bool IsFull => _data != null && _currentAmmo >= _data.magSize;

    private void Start()
    {
        if (_data == null)
        {
            WeaponShooting shooting = GetComponent<WeaponShooting>();
            if (shooting != null) Init(shooting.GetWeaponData());
        }
    }

    public void Init(WeaponData data)
    {
        if (data == null) return;
        _data = data;
        CurrentAmmo = _data.magSize;
    }

    public bool ConsumeAmmo(int amount)
    {
        if (_data == null || IsEmpty) return false;
        CurrentAmmo -= amount;
        return true;
    }

    public void Refill()
    {
        if (_data != null) CurrentAmmo = _data.magSize;
    }
}
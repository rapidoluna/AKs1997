using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    [SerializeField] private int maxAmmo = 30;
    private int _currentAmmo;
    private bool _isReloading = false;

    public int CurrentAmmo => _currentAmmo;

    private void Start()
    {
        _currentAmmo = maxAmmo;
    }

    public bool ConsumeAmmo()
    {
        if (_isReloading || _currentAmmo <= 0) return false;

        _currentAmmo--;
        return true;
    }

    public void Reload()
    {
        _isReloading = true;
        Invoke(nameof(FinishReload), 1.5f);
    }

    private void FinishReload()
    {
        _currentAmmo = maxAmmo;
        _isReloading = false;
    }
}
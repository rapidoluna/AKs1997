using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private Image weaponIconImage;
    [SerializeField] private Text ammoText;

    private WeaponController _controller;
    private WeaponAmmo _ammo;
    private WeaponData _data;

    private void Awake()
    {
        _controller = GetComponentInParent<WeaponController>();
        _ammo = GetComponentInParent<WeaponAmmo>();
    }

    private void OnEnable()
    {
        _ammo.OnAmmoChanged += UpdateAmmoUI;
    }

    private void OnDisable()
    {
        _ammo.OnAmmoChanged -= UpdateAmmoUI;
    }

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        _data = _controller.GetWeaponData();

        if (_data != null)
        {
            weaponIconImage.sprite = _data.weaponIcon;
            UpdateAmmoUI(_ammo.CurrentAmmo, _data.magSize);
        }
    }

    private void UpdateAmmoUI(int current, int max)
    {
        ammoText.text = current.ToString() + " / " + max.ToString();
    }
}
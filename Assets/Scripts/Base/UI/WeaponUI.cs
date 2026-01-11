using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private Image weaponIconImage;
    [SerializeField] private Text ammoText;
    [SerializeField] private GameObject reloadPromptPanel;
    [SerializeField] private int promptThreshold = 2;

    private WeaponController _controller;
    private WeaponAmmo _ammo;
    private WeaponReloading _reloading;
    private WeaponData _data;

    private void Awake()
    {
        _controller = GetComponentInParent<WeaponController>();
        _ammo = GetComponentInParent<WeaponAmmo>();
        _reloading = GetComponentInParent<WeaponReloading>();
    }

    private void OnEnable()
    {
        if (_ammo != null) _ammo.OnAmmoChanged += UpdateAmmoUI;
        if (_reloading != null)
        {
            _reloading.OnReloadStart += HideReloadPrompt;
            _reloading.OnReloadComplete += HandleReloadComplete;
        }
    }

    private void OnDisable()
    {
        if (_ammo != null) _ammo.OnAmmoChanged -= UpdateAmmoUI;
        if (_reloading != null)
        {
            _reloading.OnReloadStart -= HideReloadPrompt;
            _reloading.OnReloadComplete -= HandleReloadComplete;
        }
    }

    private void HandleReloadComplete(float delay)
    {
        CheckReloadPrompt();
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
        ammoText.text = $"{current} / {max}";
        CheckReloadPrompt(current, max);
    }

    private void CheckReloadPrompt(int current, int max)
    {
        if (_reloading != null && _reloading.IsReloading)
        {
            HideReloadPrompt();
            return;
        }

        bool shouldShow = current <= promptThreshold && current < max;

        if (reloadPromptPanel != null)
        {
            reloadPromptPanel.SetActive(shouldShow);
        }
    }

    private void CheckReloadPrompt()
    {
        if (_ammo != null && _data != null)
        {
            CheckReloadPrompt(_ammo.CurrentAmmo, _data.magSize);
        }
    }

    private void HideReloadPrompt()
    {
        if (reloadPromptPanel != null)
        {
            reloadPromptPanel.SetActive(false);
        }
    }
}
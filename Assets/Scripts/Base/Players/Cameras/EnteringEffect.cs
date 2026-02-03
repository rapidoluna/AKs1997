using UnityEngine;
using System.Collections;

public class EnteringEffect : MonoBehaviour
{
    [SerializeField] private float entryDuration = 2.5f;
    [SerializeField] private Material effectMaterial;

    private PlayerWalking _walking;
    private PlayerCameraEffects _cameraEffects;
    private WeaponController _weaponController;
    private WeaponAiming _aiming;

    private void Awake()
    {
        _walking = GetComponent<PlayerWalking>();
        _cameraEffects = GetComponentInChildren<PlayerCameraEffects>();
        _weaponController = GetComponentInChildren<WeaponController>();
        _aiming = GetComponentInChildren<WeaponAiming>();
    }

    private void Start()
    {
        if (effectMaterial != null)
        {
            effectMaterial.SetFloat("_EffectProgress", 0f);
            StartCoroutine(StartEntrySequence());
        }
    }

    private IEnumerator StartEntrySequence()
    {
        SetPlayerControl(false);

        float elapsed = 0f;
        while (elapsed < entryDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / entryDuration;

            effectMaterial.SetFloat("_EffectProgress", progress);
            yield return null;
        }

        SetPlayerControl(true);
        effectMaterial.SetFloat("_EffectProgress", 1f);
    }

    private void SetPlayerControl(bool state)
    {
        if (_walking != null) _walking.enabled = state;
        if (_cameraEffects != null) _cameraEffects.enabled = state;
        if (_aiming != null) _aiming.enabled = state;

        if (_weaponController != null)
        {
            _weaponController.enabled = state;

            GameObject currentWeapon = _weaponController.Slots[_weaponController.CurrentIndex];
            if (currentWeapon != null)
            {
                if (currentWeapon.TryGetComponent(out WeaponShooting shooting)) shooting.enabled = state;
                if (currentWeapon.TryGetComponent(out WeaponReloading reloading)) reloading.enabled = state;
            }
        }
    }
}
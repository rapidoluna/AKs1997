using UnityEngine;
using System.Collections;

public class EnteringEffect : MonoBehaviour
{
    [SerializeField] private float entryDuration = 2.5f;
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private ModeData modeData;
    [SerializeField] private MapIntroUI introUI;

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

        if (fadeGroup != null) fadeGroup.alpha = 1f;
    }

    private void Start()
    {
        StartCoroutine(StartEntrySequence());
    }

    private IEnumerator StartEntrySequence()
    {
        SetPlayerControl(false);

        float elapsed = 0f;
        while (elapsed < entryDuration)
        {
            elapsed += Time.deltaTime;
            if (fadeGroup != null)
            {
                fadeGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / entryDuration);
            }
            yield return null;
        }

        if (fadeGroup != null) fadeGroup.alpha = 0f;

        SetPlayerControl(true);

        if (introUI != null && modeData != null)
        {
            introUI.ShowIntro(modeData);
        }
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
                if (currentWeapon.TryGetComponent(out WeaponShooting s)) s.enabled = state;
                if (currentWeapon.TryGetComponent(out WeaponReloading r)) r.enabled = state;
            }
        }
    }
}
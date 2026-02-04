using UnityEngine;
using System.Collections;

public class ExtractionEffect : MonoBehaviour
{
    [SerializeField] private float extractionDuration = 0.5f;
    [SerializeField] private CanvasGroup fadeGroup;

    private PlayerWalking _walking;
    private PlayerCameraEffects _cameraEffects;
    private WeaponController _weaponController;
    private WeaponAiming _aiming;
    private ExtractionCameraEffect _camEffect;

    private void Awake()
    {
        _walking = GetComponent<PlayerWalking>();
        _cameraEffects = GetComponentInChildren<PlayerCameraEffects>();
        _weaponController = GetComponentInChildren<WeaponController>();
        _aiming = GetComponentInChildren<WeaponAiming>();
        _camEffect = GetComponentInChildren<ExtractionCameraEffect>();

        if (fadeGroup != null) fadeGroup.alpha = 0f;
    }

    public void StartExtraction()
    {
        StartCoroutine(ExtractionSequence());
    }

    private IEnumerator ExtractionSequence()
    {
        SetPlayerControl(false);

        if (_camEffect != null)
        {
            _camEffect.Play(extractionDuration);
        }

        float elapsed = 0f;
        float fadeStartTime = extractionDuration * 0.5f;

        while (elapsed < extractionDuration)
        {
            elapsed += Time.deltaTime;

            if (fadeGroup != null && elapsed >= fadeStartTime)
            {
                float t = (elapsed - fadeStartTime) / (extractionDuration - fadeStartTime);
                fadeGroup.alpha = Mathf.Clamp01(t);
            }

            yield return null;
        }

        if (fadeGroup != null) fadeGroup.alpha = 1f;
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
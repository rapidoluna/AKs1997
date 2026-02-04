using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExtractionCameraEffect : MonoBehaviour
{
    private Camera _cam;

    [SerializeField] private Image overlayImage;
    [SerializeField] private CanvasGroup fadeGroup;

    private PlayerWalking _walking;
    private PlayerCameraEffects _cameraEffects;
    private WeaponController _weaponController;
    private WeaponAiming _aiming;

    private void Awake()
    {
        _cam = GetComponent<Camera>();

        _walking = GetComponentInParent<PlayerWalking>();
        _cameraEffects = GetComponentInParent<PlayerCameraEffects>();
        _weaponController = GetComponentInParent<WeaponController>();
        _aiming = GetComponentInParent<WeaponAiming>();

        if (overlayImage != null)
        {
            Color c = overlayImage.color;
            c.a = 0f;
            overlayImage.color = c;
        }

        if (fadeGroup != null) fadeGroup.alpha = 0f;
    }

    public void Play(float duration)
    {
        StartCoroutine(ExtractionRoutine(duration));
    }

    private IEnumerator ExtractionRoutine(float duration)
    {
        SetPlayerControl(false);

        float elapsed = 0f;
        float fadeStartTime = duration * 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (overlayImage != null)
            {
                Color c = overlayImage.color;
                c.a = Mathf.Clamp01(t);
                overlayImage.color = c;
            }

            if (fadeGroup != null)
            {
                if (elapsed >= fadeStartTime)
                {
                    float fadeT = (elapsed - fadeStartTime) / (duration - fadeStartTime);
                    fadeGroup.alpha = Mathf.Clamp01(fadeT);
                }
                else
                {
                    fadeGroup.alpha = 0f;
                }
            }

            yield return null;
        }

        if (fadeGroup != null) fadeGroup.alpha = 1f;
        if (overlayImage != null)
        {
            Color c = overlayImage.color;
            c.a = 1f;
            overlayImage.color = c;
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
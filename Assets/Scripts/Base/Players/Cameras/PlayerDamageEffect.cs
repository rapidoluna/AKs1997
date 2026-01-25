using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerDamageEffect : MonoBehaviour
{
    public static PlayerDamageEffect Instance;

    [Header("Digital FX Settings")]
    [SerializeField] private Image glitchOverlay;
    [SerializeField] private float fadeSpeed = 1f;

    [Header("Camera Shake")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float shakeDuration = 0.1f;
    [SerializeField] private float shakeAmount = 0.05f;

    private Vector3 _originalPos;
    private Coroutine _fadeRoutine;

    private void Awake()
    {
        Instance = this;
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        _originalPos = cameraTransform.localPosition;

        if (glitchOverlay != null)
        {
            Color c = glitchOverlay.color;
            c.a = 0f;
            glitchOverlay.color = c;
        }
    }

    public void OnHit()
    {
        if (PlayerHealth.IsDead) return;

        StopCoroutine(nameof(ShakeRoutine));
        StartCoroutine(nameof(ShakeRoutine));

        if (glitchOverlay != null)
        {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(FadeGlitch());
        }
    }

    public void StartDeathFade()
    {
        if (glitchOverlay == null) return;

        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        StartCoroutine(DeathFadeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            cameraTransform.localPosition = _originalPos + Random.insideUnitSphere * shakeAmount;
            elapsed += Time.deltaTime;
            yield return null;
        }
        cameraTransform.localPosition = _originalPos;
    }

    private IEnumerator FadeGlitch()
    {
        Color c = glitchOverlay.color;
        c.a = 0.3f;
        glitchOverlay.color = c;

        while (c.a > 0)
        {
            c.a -= Time.deltaTime * fadeSpeed;
            glitchOverlay.color = c;
            yield return null;
        }
    }

    private IEnumerator DeathFadeRoutine()
    {
        Color c = glitchOverlay.color;
        float currentAlpha = c.a;

        while (currentAlpha < 1.0f)
        {
            currentAlpha += Time.deltaTime * 0.5f;
            c.a = currentAlpha;
            glitchOverlay.color = c;
            yield return null;
        }
    }
}
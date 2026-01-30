using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

public class ExtractionCameraEffect : MonoBehaviour
{
    private Camera _cam;
    private Vector3 _startPos;
    private Quaternion _startRot;
    private float _startFOV;

    [SerializeField] private Image overlayImage;

    [SerializeField] private float targetPitch = -65f;//시선 올라가는 각도
    [SerializeField] private float liftHeight = 3f;//카메라 올라가는 높이
    [SerializeField] private float fovIncrease = 60f;//확장될 FOV 정도
    [SerializeField] private float shakePower = 0.15f;//화면 떨림 정도

    private void Awake()
    {
        _cam = GetComponent<Camera>();

        if (overlayImage != null)
        {
            Color c = overlayImage.color;
            c.a = 0f;
            overlayImage.color = c;
        }
    }

    public void Play(float duration)
    {
        _startPos = transform.localPosition;
        _startRot = transform.localRotation;
        _startFOV = _cam.fieldOfView;

        StartCoroutine(ExtractionRoutine(duration));
    }

    private IEnumerator ExtractionRoutine(float duration)
    {
        float elapsed = 0f;
        Quaternion targetRot = Quaternion.Euler(targetPitch, _startRot.eulerAngles.y, 0f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curve = t * t;

            float currentShake = t * shakePower;
            Vector3 shakeOffset = Random.insideUnitSphere * currentShake;
            transform.localPosition = _startPos + (Vector3.up * (curve * liftHeight)) + shakeOffset;

            transform.localRotation = Quaternion.Slerp(_startRot, targetRot, t);
            _cam.fieldOfView = Mathf.Lerp(_startFOV, _startFOV + fovIncrease, curve);

            if (overlayImage != null)
            {
                Color c = overlayImage.color;
                c.a = Mathf.Lerp(0f, 1f, t * t);
                overlayImage.color = c;
            }

            yield return null;
        }
    }
}
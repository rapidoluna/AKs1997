using UnityEngine;

public class WeaponAiming : MonoBehaviour
{
    [SerializeField] private float adsFov = 40f;
    [SerializeField] private float adsSpeed = 10f;

    [Header("Aiming Buffs")]
    [SerializeField] private float adsRecoilMultiplier = 0.5f;
    [SerializeField] private float adsSpreadMultiplier = 0.2f;

    [Header("Offsets")]
    [SerializeField] private Vector3 hipOffset = new Vector3(0.5f, -0.4f, 1f);
    [SerializeField] private Vector3 adsOffset = new Vector3(0f, -0.1f, 1.2f);

    private Camera _mainCamera;
    private float _defaultFov;

    public bool IsAiming { get; private set; }

    public float RecoilMultiplier => IsAiming ? adsRecoilMultiplier : 1f;
    public float SpreadMultiplier => IsAiming ? adsSpreadMultiplier : 1f;

    private void Awake()
    {
        _mainCamera = Camera.main;
        if (_mainCamera != null) _defaultFov = _mainCamera.fieldOfView;
    }

    private void Update()
    {
        if (PlayerHealth.IsDead)
        {
            IsAiming = false;
            ResetAimingVisuals();
            return;
        }

        IsAiming = Input.GetMouseButton(1);

        if (_mainCamera != null)
        {
            float targetFov = IsAiming ? adsFov : _defaultFov;
            _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, targetFov, Time.deltaTime * adsSpeed);
        }

        Vector3 targetOffset = IsAiming ? adsOffset : hipOffset;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetOffset, Time.deltaTime * adsSpeed);
    }

    private void ResetAimingVisuals()
    {
        if (_mainCamera != null)
        {
            _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, _defaultFov, Time.deltaTime * adsSpeed);
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, hipOffset, Time.deltaTime * adsSpeed);
    }
}
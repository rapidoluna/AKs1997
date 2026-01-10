using UnityEngine;

public class WeaponAiming : MonoBehaviour
{
    [Header("ADS Settings")]
    [SerializeField] private float adsFov = 40f;
    [SerializeField] private float adsSpeed = 10f;

    [Header("Multipliers")]
    [SerializeField] private float adsRecoilMultiplier = 0.5f;
    [SerializeField] private float adsSpreadMultiplier = 0.2f;

    [Header("Fire Point Offsets")]
    [SerializeField] private Vector3 hipOffset = new Vector3(0.5f, -0.4f, 1f);
    [SerializeField] private Vector3 adsOffset = new Vector3(0f, -0.2f, 1.2f);

    private Camera _mainCamera;
    private float _defaultFov;

    public bool IsAiming { get; private set; }
    public float RecoilMultiplier => IsAiming ? adsRecoilMultiplier : 1f;
    public float SpreadMultiplier => IsAiming ? adsSpreadMultiplier : 1f;
    public Vector3 CurrentOffset => IsAiming ? adsOffset : hipOffset;

    private void Awake()
    {
        _mainCamera = Camera.main;
        if (_mainCamera != null) _defaultFov = _mainCamera.fieldOfView;
    }

    private void Update()
    {
        IsAiming = Input.GetMouseButton(1);
        UpdateCameraFov();
    }

    private void UpdateCameraFov()
    {
        if (_mainCamera == null) return;
        float targetFov = IsAiming ? adsFov : _defaultFov;
        _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, targetFov, Time.deltaTime * adsSpeed);
    }
}
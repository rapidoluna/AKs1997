using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    public static CrosshairManager Instance;

    [Header("Crosshair Elements")]
    [SerializeField] private RectTransform bottomBar;//하단 막대
    [SerializeField] private RectTransform leftBar;//왼쪽 막대
    [SerializeField] private RectTransform rightBar;//오른쪽 막대
    [SerializeField] private CanvasGroup crosshairGroup;//조준점 그룹

    [Header("Settings")]
    [SerializeField] private float baseSpread = 20f;//기본적으로 조준점이 벌려져있을 정도
    [SerializeField] private float minChargeSpread = 5f;//충전식 무기 사용 시 조준점이 모이는 최소 정도
    [SerializeField] private float maxSpread = 80f;//사격 시 조준점이 벌어지는 최대 정도
    [SerializeField] private float restoreSpeed = 10f;//복구 속도
    [SerializeField] private float jumpExertion = 20f;//조준점 탄성
    [SerializeField] private float fadeSpeed = 15f;//조준점 사라지는 속도

    private float _currentSpread;
    private float _targetAlpha = 1f;
    private float _chargeRatio = 0f; //충전 정도

    private void Awake()
    {
        Instance = this;
        _currentSpread = baseSpread;
        if (crosshairGroup == null) crosshairGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        float target = Mathf.Lerp(baseSpread, minChargeSpread, _chargeRatio);
        _currentSpread = Mathf.Lerp(_currentSpread, target, Time.deltaTime * restoreSpeed);

        ApplySpread(_currentSpread);

        if (crosshairGroup != null)
        {
            crosshairGroup.alpha = Mathf.Lerp(crosshairGroup.alpha, _targetAlpha, Time.deltaTime * fadeSpeed);
        }
    }

    public void SetChargeRatio(float ratio)
    {
        _chargeRatio = Mathf.Clamp01(ratio);
    }

    public void SetCrosshairVisibility(bool visible)
    {
        _targetAlpha = visible ? 1f : 0f;
    }

    public void FireExertion()
    {
        _currentSpread += jumpExertion;
        _currentSpread = Mathf.Clamp(_currentSpread, baseSpread, maxSpread);
    }

    private void ApplySpread(float spread)
    {
        bottomBar.anchoredPosition = new Vector2(0, -spread);
        leftBar.anchoredPosition = new Vector2(-spread, 0);
        rightBar.anchoredPosition = new Vector2(spread, 0);
    }
}
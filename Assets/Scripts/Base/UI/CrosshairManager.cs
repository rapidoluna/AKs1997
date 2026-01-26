using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    public static CrosshairManager Instance;

    [SerializeField] private RectTransform bottomBar;//하단 막대
    [SerializeField] private RectTransform leftBar;//왼쪽 막대
    [SerializeField] private RectTransform rightBar;//오른쪽 막대
    [SerializeField] private CanvasGroup crosshairGroup;//조준점 묶음

    [SerializeField] private float baseSpread = 20f;//기본 탄퍼짐
    [SerializeField] private float maxSpread = 100f;//최대 탄퍼짐 표시
    [SerializeField] private float restoreSpeed = 10f;//원래 상태로 돌아오는 속도
    [SerializeField] private float jumpExertion = 20f;//사격 시 조준점 벌어지는 정도
    [SerializeField] private float fadeSpeed = 15f;//조준 시 조준점 사라지는 속도

    private float _currentSpread;
    private float _targetAlpha = 1f;

    private void Awake()
    {
        Instance = this;
        _currentSpread = baseSpread;
        if (crosshairGroup == null) crosshairGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        _currentSpread = Mathf.Lerp(_currentSpread, baseSpread, Time.deltaTime * restoreSpeed);
        ApplySpread(_currentSpread);

        if (crosshairGroup != null)
        {
            crosshairGroup.alpha = Mathf.Lerp(crosshairGroup.alpha, _targetAlpha, Time.deltaTime * fadeSpeed);
        }
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
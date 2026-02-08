using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] private Image specialityIcon;
    [SerializeField] private Image specialityDarkOverlay;
    [SerializeField] private Image specialityCooldownFill;
    [SerializeField] private TextMeshProUGUI specialityCooldownText;

    [SerializeField] private Image ultimateIcon;
    [SerializeField] private Image ultimateDarkOverlay;
    [SerializeField] private Image ultimateGaugeFill;
    [SerializeField] private TextMeshProUGUI ultimateReadyText;

    private AbilityProcessor _specialityProcessor;
    private UltimateProcessor _ultimateProcessor;
    public UltimateCharge _ultimateCharge;
    public float a;

    private float _lerpSpeed = 5f;
    private Color _activeColor = Color.white;
    private Color _inactiveColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    private bool _isInitialized = false;
    private bool _wasReady = false;

    private void Start()
    {
        StartCoroutine(FindPlayerRoutine());
    }

    private IEnumerator FindPlayerRoutine()
    {
        while (GameObject.FindGameObjectWithTag("Player") == null)
        {
            yield return null;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _specialityProcessor = player.GetComponentInChildren<AbilityProcessor>();
        _ultimateProcessor = player.GetComponentInChildren<UltimateProcessor>();

        if (_ultimateProcessor != null)
            _ultimateCharge = _ultimateProcessor.GetComponent<UltimateCharge>();

        yield return new WaitForSeconds(0.1f);
        InitIcons();
        _isInitialized = true;
    }

    private void InitIcons()
    {
        if (_specialityProcessor != null && _specialityProcessor.GetData() != null)
        {
            if (specialityIcon != null)
                specialityIcon.sprite = _specialityProcessor.GetData().abilityIcon;
        }

        if (_ultimateProcessor != null && _ultimateProcessor.GetData() != null)
        {
            if (ultimateIcon != null)
                ultimateIcon.sprite = _ultimateProcessor.GetData().abilityIcon;
        }
    }

    private void Update()
    {
        if (!_isInitialized) return;

        UpdateSpecialityUI();
        UpdateUltimateUI();
    }

    private void UpdateSpecialityUI()
    {
        if (_specialityProcessor == null) return;

        bool isActive = _specialityProcessor.IsActive();
        bool isCooldown = _specialityProcessor.GetCooldownRatio() < 1f;

        if (specialityIcon != null)
            specialityIcon.color = (isActive || isCooldown) ? _inactiveColor : _activeColor;

        if (specialityDarkOverlay != null)
            specialityDarkOverlay.gameObject.SetActive(isActive || isCooldown);

        if (specialityCooldownFill != null)
            specialityCooldownFill.fillAmount = 1f - _specialityProcessor.GetCooldownRatio();

        if (specialityCooldownText != null)
        {
            float timeLeft = _specialityProcessor.GetRemainingCooldown();
            specialityCooldownText.text = timeLeft > 0 ? timeLeft.ToString("F1") : "";
        }
    }

    private void UpdateUltimateUI()
    {
        if (_ultimateCharge == null) return;

        bool isReady = _ultimateCharge.IsReady;
        bool isUsing = _ultimateProcessor != null && _ultimateProcessor.IsActive();

        if (ultimateIcon != null)
            ultimateIcon.color = isReady ? _activeColor : _inactiveColor;

        if (ultimateDarkOverlay != null)
            ultimateDarkOverlay.gameObject.SetActive(!isReady || isUsing);

        if (ultimateGaugeFill != null)
            ultimateGaugeFill.fillAmount = Mathf.Lerp(ultimateGaugeFill.fillAmount, _ultimateCharge.GaugeRatio, Time.deltaTime * _lerpSpeed);

        if (ultimateReadyText != null)
        {
            if (isUsing)
            {
                ultimateReadyText.gameObject.SetActive(false);
                _wasReady = false;
            }
            else if (isReady)
            {
                if (!_wasReady)
                {
                    StartCoroutine(ReadyTextSequence());
                    _wasReady = true;
                }
            }
            else
            {
                ultimateReadyText.text = Mathf.FloorToInt(_ultimateCharge.CurrentGauge).ToString();
                Color c = ultimateReadyText.color;
                c.a = 1f;
                ultimateReadyText.color = c;

                ultimateReadyText.gameObject.SetActive(true);
                _wasReady = false;
            }
        }
    }

    private IEnumerator ReadyTextSequence()
    {
        ultimateReadyText.text = "Ready";
        Color textColor = ultimateReadyText.color;
        textColor.a = 1f;
        ultimateReadyText.color = textColor;
        ultimateReadyText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        float duration = 1.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            textColor.a = Mathf.Lerp(1f, 0f, elapsed / duration);
            ultimateReadyText.color = textColor;
            yield return null;
        }

        ultimateReadyText.gameObject.SetActive(false);
    }
}
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
    private UltimateCharge _ultimateCharge;

    private float _lerpSpeed = 5f;
    private Color _activeColor = Color.white;
    private Color _inactiveColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    private Coroutine _readyTextCoroutine;

    private void Start()
    {
        FindPlayerReferences();
        InitIcons();
    }

    private void FindPlayerReferences()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _specialityProcessor = player.GetComponentInChildren<AbilityProcessor>();
            _ultimateProcessor = player.GetComponentInChildren<UltimateProcessor>();

            if (_ultimateProcessor != null)
                _ultimateCharge = _ultimateProcessor.GetComponent<UltimateCharge>();
        }
    }

    private void InitIcons()
    {
        var init = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInitializer>();
        if (init == null || init.CharacterData == null) return;

        if (init.CharacterData.characterSpeciality != null && specialityIcon != null)
            specialityIcon.sprite = init.CharacterData.characterSpeciality.abilityIcon;

        if (init.CharacterData.characterUltimate != null && ultimateIcon != null)
            ultimateIcon.sprite = init.CharacterData.characterUltimate.abilityIcon;
    }

    private void Update()
    {
        if (_ultimateCharge == null)
        {
            FindPlayerReferences();
            return;
        }

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
                if (_readyTextCoroutine != null)
                {
                    StopCoroutine(_readyTextCoroutine);
                    _readyTextCoroutine = null;
                }
                ultimateReadyText.gameObject.SetActive(false);
            }
            else if (isReady)
            {
                if (_readyTextCoroutine == null)
                {
                    _readyTextCoroutine = StartCoroutine(ReadyTextSequence());
                }
            }
            else
            {
                if (_readyTextCoroutine != null)
                {
                    StopCoroutine(_readyTextCoroutine);
                    _readyTextCoroutine = null;
                }

                Color c = ultimateReadyText.color;
                c.a = 1f;
                ultimateReadyText.color = c;
                ultimateReadyText.text = Mathf.FloorToInt(_ultimateCharge.CurrentGauge).ToString();
                ultimateReadyText.gameObject.SetActive(true);
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
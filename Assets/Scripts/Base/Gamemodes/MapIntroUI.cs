using UnityEngine;
using TMPro;
using System.Collections;

public class MapIntroUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup uiGroup;
    [SerializeField] private TextMeshProUGUI mapNameText;
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private TextMeshProUGUI descText;

    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private float stayTime = 3f;
    [SerializeField] private float fadeOutTime = 1f;

    public void ShowIntro(ModeData data)
    {
        if (data == null) return;

        mapNameText.text = data.mapName;
        modeText.text = data.gameModeName;
        descText.text = data.description;

        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        float elapsed = 0f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            uiGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInTime);
            yield return null;
        }

        yield return new WaitForSeconds(stayTime);

        elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            uiGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutTime);
            yield return null;
        }
    }
}
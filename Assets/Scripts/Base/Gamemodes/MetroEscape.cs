using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MetroEscape : MonoBehaviour
{
    private bool _isExiting = false;
    [SerializeField] private string resultSceneName = "ResultScene";

    public void OnInteractComplete()
    {
        if (_isExiting) return;
        if (GameStateManager.Instance != null && GameStateManager.Instance.IsEscapeReady)
        {
            StartCoroutine(EscapeSequence());
        }
    }

    private IEnumerator EscapeSequence()
    {
        _isExiting = true;

        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.isExtracted = true;
        }

        ExtractionCameraEffect effect = Camera.main.GetComponent<ExtractionCameraEffect>();
        if (effect != null)
        {
            effect.Play(6f);
        }

        if (CashRushHUD.Instance != null)
        {
            CashRushHUD.Instance.ShowNotification("탈출 중...");
        }

        yield return new WaitForSeconds(6.2f);

        if (!Application.CanStreamedLevelBeLoaded(resultSceneName))
        {
            Debug.LogError($"Result scene '{resultSceneName}' is not available in Build Settings.");
            yield break;
        }

        SceneManager.LoadScene(resultSceneName);
    }
}

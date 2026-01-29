using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MetroEscape : MonoBehaviour
{
    private bool _isExiting = false;

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

        ExtractionCameraEffect effect = Camera.main.GetComponent<ExtractionCameraEffect>();
        if (effect != null) effect.Play(4.0f);

        if (CashRushHUD.Instance != null)
            CashRushHUD.Instance.ShowNotification("≈ª√‚ ¡ﬂ...");

        yield return new WaitForSeconds(4.0f);

        SceneManager.LoadScene("ResultScene");
    }
}
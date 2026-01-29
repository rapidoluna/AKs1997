using UnityEngine;
using System.Collections;

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

        Debug.Log("탈출 시퀀스 시작... 2초 대기");

        if (CashRushHUD.Instance != null)
            CashRushHUD.Instance.ShowNotification("탈출 중...");

        yield return new WaitForSeconds(2.0f);

        Debug.Log("탈출 성공! 결과 화면으로 이동.");
        UnityEngine.SceneManagement.SceneManager.LoadScene("ResultScene");
    }
}
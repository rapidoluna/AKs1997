using UnityEngine;
using System;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    [SerializeField] private int targetScore = 50000;
    private bool _isEscapeReady = false;

    public bool IsEscapeReady => _isEscapeReady;
    public event Action OnEscapeAvailable;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (_isEscapeReady) return;

        if (CashRushHUD.Instance != null)
        {
            if (CashRushHUD.Instance.CurrentScore >= targetScore)
            {
                ActivateEscape();
            }
        }
    }

    private void ActivateEscape()
    {
        _isEscapeReady = true;
        OnEscapeAvailable?.Invoke();

        if (CashRushHUD.Instance != null)
            CashRushHUD.Instance.ShowNotification("≈ª√‚ ∞°¥…");

        Debug.Log("≈ª√‚ ¡∂∞« √Ê¡∑");
    }
}
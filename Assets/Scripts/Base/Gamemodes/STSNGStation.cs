using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STSNGStation : MonoBehaviour
{
    private bool _isProcessing = false;
    [SerializeField] private float _maxTimer = 60f;
    private float _currentTimer;
    private int _storedCash = 0;
    private int _currentDepositCount = 0;
    [SerializeField] private int maxCapacity = 3;

    public bool IsProcessing => _isProcessing;

    public void AddStoredCashFromEnemy(int amount)
    {
        _storedCash += amount;

        if (CashRushHUD.Instance != null)
        {
            CashRushHUD.Instance.AddScore(_storedCash);
            CashRushHUD.Instance.ShowNotification("적 처치");
        }
    }

    public bool CanAcceptItems(int count)
    {
        bool isEscapeReady = GameStateManager.Instance != null && GameStateManager.Instance.IsEscapeReady;
        return !_isProcessing && count <= maxCapacity && !isEscapeReady;
    }

    public void DepositItems(List<ItemData> items)
    {
        if (_isProcessing || items.Count > maxCapacity) return;

        _currentTimer = _maxTimer;
        int totalValue = 0;
        _currentDepositCount = items.Count;

        foreach (var item in items)
        {
            totalValue += item.cashValue;
        }

        _storedCash = totalValue;
        StartCoroutine(CashRushRoutine());
    }

    private IEnumerator CashRushRoutine()
    {
        _isProcessing = true;

        if (ItemInventoryUI.Instance != null)
            ItemInventoryUI.Instance.ClearIcons();

        if (CashRushHUD.Instance != null)
        {
            CashRushHUD.Instance.ShowNotification("캐시러시 시작");
            CashRushHUD.Instance.SetTimerActive(true, _storedCash);
        }

        while (_currentTimer > 0)
        {
            _currentTimer -= Time.deltaTime;
            if (CashRushHUD.Instance != null)
            {
                CashRushHUD.Instance.UpdateTimer(_currentTimer);
            }
            yield return null;
        }

        CompleteCashRush();
    }

    private void CompleteCashRush()
    {
        _isProcessing = false;

        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.AddScore(_storedCash);
        }

        if (CashRushHUD.Instance != null)
        {
            CashRushHUD.Instance.AddScore(_storedCash);
            CashRushHUD.Instance.ShowNotification("캐시러시 완료");
            CashRushHUD.Instance.SetTimerActive(false);
        }
        _storedCash = 0;
        _currentDepositCount = 0;
    }
    public void FinalizeRemainingCash()
    {
        if (_storedCash > 0 && GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.AddScore(_storedCash);
            _storedCash = 0;
        }
    }
}
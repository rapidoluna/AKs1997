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

    public bool CanAcceptItems(int count)
    {
        bool isEscapeReady = GameStateManager.Instance != null && GameStateManager.Instance.IsEscapeReady;
        return !_isProcessing && count <= maxCapacity && !isEscapeReady;
    }

    public void DepositItems(List<ItemData> items)
    {
        if (_isProcessing || items.Count > maxCapacity) return;

        _isProcessing = true;
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
        if (ItemInventoryUI.Instance != null)
            ItemInventoryUI.Instance.ClearIcons();

        if (CashRushHUD.Instance != null)
        {
            CashRushHUD.Instance.ShowNotification("캐시러시 시작");
            CashRushHUD.Instance.SetTimerActive(true, _storedCash);
        }

        StartCoroutine(DelayedDropPodCall(2f));

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

    private IEnumerator DelayedDropPodCall(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.TriggerCashRushArrival(transform.position);
        }
    }

    private void CompleteCashRush()
    {
        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.AddDepositScore(_storedCash);
        }

        if (CashRushHUD.Instance != null)
        {
            CashRushHUD.Instance.AddScore(_storedCash);
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.CheckScore(CashRushHUD.Instance.CurrentScore);
            }
            CashRushHUD.Instance.ShowNotification("캐시러시 완료");
            CashRushHUD.Instance.SetTimerActive(false);
        }
        _storedCash = 0;
        _isProcessing = false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STSNGStation : MonoBehaviour
{
    private bool _isProcessing = false;
    [SerializeField] private float _timer = 60f;
    private int _storedCash = 0;
    private int _currentDepositCount = 0;
    [SerializeField] private int maxCapacity = 3;

    public bool CanAcceptItems(int count)
    {
        return !_isProcessing && count <= maxCapacity;
    }

    public void DepositItems(List<ItemData> items)
    {
        if (_isProcessing || items.Count > maxCapacity) return;

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
            CashRushHUD.Instance.ShowNotification($"캐시러시 시작");
            CashRushHUD.Instance.SetTimerActive(true, _storedCash);
        }

        while (_timer > 0)
        {
            _timer -= Time.deltaTime;
            if (CashRushHUD.Instance != null)
            {
                CashRushHUD.Instance.UpdateTimer(_timer);
            }
            yield return null;
        }

        CompleteCashRush();
    }

    private void CompleteCashRush()
    {
        _isProcessing = false;
        if (CashRushHUD.Instance != null)
        {
            CashRushHUD.Instance.AddScore(_storedCash);
            CashRushHUD.Instance.ShowNotification("캐시러시 완료");
            CashRushHUD.Instance.SetTimerActive(false);
        }
        _storedCash = 0;
        _currentDepositCount = 0;
    }
    //프로퍼티
    public bool IsProcessing => _isProcessing;
}
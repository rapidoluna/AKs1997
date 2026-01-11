using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STSNGStation : MonoBehaviour
{
    private bool _isProcessing = false;
    [SerializeField] private float maxTime = 60f;
    private float _timer;
    private int _storedCash = 0;

    public void DepositItems(List<ItemData> items)
    {
        if (_isProcessing) return;

        int totalValue = 0;
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
        _timer = maxTime;

        if (CashRushHUD.Instance != null)
        {
            CashRushHUD.Instance.ShowNotification("캐시러시 시작");
            CashRushHUD.Instance.SetTimerActive(true);
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
    }
}
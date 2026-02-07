using TMPro;
using UnityEngine;

public class UltimateCharge : MonoBehaviour
{
    [SerializeField]private float _currentGauge = 0f;
    private const float MaxGauge = 100f;
    private AbilityData _ultimateData;
    private bool _isLocked = false;
    public TextMeshProUGUI ultText;

    public float CurrentGauge => _currentGauge;
    public float GaugeRatio => _currentGauge / MaxGauge;
    public bool IsReady => _currentGauge >= MaxGauge;


    public void Initialize(AbilityData data)
    {
        _ultimateData = data;
        _currentGauge = 0f;
    }

    public void SetLock(bool lockStatus)
    {
        _isLocked = lockStatus;
        if (lockStatus) Debug.Log("[UltimateCharge] 게이지 충전 일시 정지 (궁극기 활성화)");
        else Debug.Log("[UltimateCharge] 게이지 충전 재개");
    }

    private void Update()
    {
        AddGauge(_ultimateData.ultimateChargeSpeed * Time.deltaTime);
    }

    public void AddGauge(float amount)
    {
        if (IsReady || _isLocked) return;
        _currentGauge = Mathf.Min(_currentGauge + amount, MaxGauge);
    }

    public void ResetGauge()
    {
        _currentGauge = 0f;
    }
}
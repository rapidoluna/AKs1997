using UnityEngine;

public class UltimateCharge : MonoBehaviour
{
    private float _currentGauge = 0f;
    private const float MaxGauge = 100f;
    private AbilityData _ultimateData;

    public float CurrentGauge => _currentGauge;
    public float GaugeRatio => _currentGauge / MaxGauge;
    public bool IsReady => _currentGauge >= MaxGauge;

    public void Initialize(AbilityData data)
    {
        _ultimateData = data;
        _currentGauge = 0f;
    }

    private void Update()
    {
        if (_ultimateData == null || IsReady) return;

        AddGauge(_ultimateData.ultimateChargeSpeed * Time.deltaTime);
    }

    public void AddGauge(float amount)
    {
        if (IsReady) return;
        _currentGauge = Mathf.Min(_currentGauge + amount, MaxGauge);
    }

    public void ResetGauge()
    {
        _currentGauge = 0f;
    }
}
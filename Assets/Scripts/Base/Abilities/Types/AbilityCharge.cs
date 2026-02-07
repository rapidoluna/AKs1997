using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityCharge : AbilityBase
{
    private GameObject _deviceInstance;
    private Transform _dynamicFirePoint;
    private List<GameObject> _targets = new List<GameObject>();
    private Dictionary<GameObject, float> _targetingProgress = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> _lostTargetTimers = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, GameObject> _marks = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, SpriteRenderer> _markRenderers = new Dictionary<GameObject, SpriteRenderer>();

    private bool _isCharging = false;
    private KeyCode _inputKey;
    private float _viewAngle = 80f;
    private float _scanInterval = 0.05f;
    private float _targetLostGraceTime = 0.5f;
    private Camera _mainCam;
    private List<GameObject> _cleanupList = new List<GameObject>();

    public override void Execute()
    {
        _inputKey = (abilityData.type == AbilityType.Ultimate) ? KeyCode.E : KeyCode.Q;
        _mainCam = Camera.main;

        if (abilityData.rewardWeapon != null)
        {
            Transform weaponHolder = transform.root.GetComponentInChildren<PlayerWalking>()?.transform.Find("Weapon Holder");
            _deviceInstance = Instantiate(abilityData.rewardWeapon, weaponHolder != null ? weaponHolder : firePoint);
            _deviceInstance.transform.localPosition = Vector3.zero;
            _deviceInstance.transform.localRotation = Quaternion.identity;

            WeaponShooting rewardWS = _deviceInstance.GetComponent<WeaponShooting>();
            if (rewardWS == null) rewardWS = _deviceInstance.GetComponentInChildren<WeaponShooting>();
            if (rewardWS != null) _dynamicFirePoint = rewardWS.FirePoint;
        }

        if (_dynamicFirePoint == null) _dynamicFirePoint = firePoint;

        _isCharging = true;
        _targets.Clear();
        _targetingProgress.Clear();
        _lostTargetTimers.Clear();
        _marks.Clear();
        _markRenderers.Clear();

        StartCoroutine(ChargeRoutine());
        StartCoroutine(TargetingUpdateRoutine());
    }

    private IEnumerator ChargeRoutine()
    {
        float elapsed = 0f;
        while (_isCharging && Input.GetKey(_inputKey))
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (elapsed >= abilityData.minChargeTime && _targets.Count > 0)
        {
            FireSingleWinchClaw();
        }

        _isCharging = false;

        if (abilityData.type == AbilityType.Ultimate)
            GetComponent<UltimateProcessor>()?.StopUltimate();
        else
            GetComponent<AbilityProcessor>()?.StopEffect();
    }

    private IEnumerator TargetingUpdateRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(_scanInterval);
        while (_isCharging)
        {
            UpdateTargetingLogic(_scanInterval);
            yield return wait;
        }
    }

    private void UpdateTargetingLogic(float deltaTime)
    {
        _cleanupList.Clear();
        foreach (var enemy in _targetingProgress.Keys) _cleanupList.Add(enemy);

        for (int i = 0; i < _cleanupList.Count; i++)
        {
            GameObject enemy = _cleanupList[i];
            if (IsTargetInvalid(enemy))
            {
                if (!_lostTargetTimers.ContainsKey(enemy)) _lostTargetTimers[enemy] = 0f;
                _lostTargetTimers[enemy] += deltaTime;
                if (_lostTargetTimers[enemy] > _targetLostGraceTime) RemoveTarget(enemy);
            }
            else _lostTargetTimers[enemy] = 0f;
        }

        Collider[] overlapEnemies = Physics.OverlapSphere(_mainCam.transform.position, abilityData.abilityRange);
        foreach (var col in overlapEnemies)
        {
            if (!col.CompareTag("Enemy")) continue;
            GameObject enemy = col.gameObject;
            if (_targets.Contains(enemy)) continue;

            if (!IsTargetInvalid(enemy))
            {
                if (!_targetingProgress.ContainsKey(enemy))
                {
                    _targetingProgress[enemy] = 0f;
                    CreateMark(enemy);
                }
                _targetingProgress[enemy] += deltaTime * abilityData.targetAcquisitionSpeed;
                float progress = Mathf.Clamp01(_targetingProgress[enemy]);
                UpdateMarkAlpha(enemy, progress);
                if (progress >= 1f && _targets.Count < abilityData.maxTargets) _targets.Add(enemy);
            }
        }
    }

    private bool IsTargetInvalid(GameObject enemy)
    {
        if (enemy == null || _mainCam == null) return true;
        float dist = Vector3.Distance(_mainCam.transform.position, enemy.transform.position);
        if (dist > abilityData.abilityRange * 1.1f) return true;
        Vector3 dirToEnemy = (enemy.transform.position - _mainCam.transform.position).normalized;
        if (Vector3.Angle(_mainCam.transform.forward, dirToEnemy) > _viewAngle * 0.5f) return true;
        if (Physics.Raycast(_mainCam.transform.position, dirToEnemy, out RaycastHit hit, abilityData.abilityRange + 2f))
        {
            if (hit.collider.gameObject != enemy && !hit.collider.CompareTag("Enemy")) return true;
        }
        else return true;
        return false;
    }

    private void RemoveTarget(GameObject enemy)
    {
        _targets.Remove(enemy);
        _targetingProgress.Remove(enemy);
        _lostTargetTimers.Remove(enemy);
        if (_marks.TryGetValue(enemy, out GameObject mark))
        {
            if (mark != null) Destroy(mark);
            _marks.Remove(enemy);
        }
        _markRenderers.Remove(enemy);
    }

    private void CreateMark(GameObject enemy)
    {
        if (abilityData.trackingMark == null) return;
        Vector3 markPos = enemy.transform.position + Vector3.up * 2.0f;
        GameObject mark = Instantiate(abilityData.trackingMark, markPos, Quaternion.identity, enemy.transform);
        if (!mark.GetComponent<Billboard>()) mark.AddComponent<Billboard>();
        _marks.Add(enemy, mark);
        SpriteRenderer sr = mark.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            _markRenderers.Add(enemy, sr);
            Color c = sr.color; c.a = 0f; sr.color = c;
        }
    }

    private void UpdateMarkAlpha(GameObject enemy, float alpha)
    {
        if (_markRenderers.TryGetValue(enemy, out SpriteRenderer sr) && sr != null)
        {
            Color c = sr.color; c.a = alpha; sr.color = c;
        }
    }

    private void FireSingleWinchClaw()
    {
        if (abilityData.abilityPrefab == null || _dynamicFirePoint == null) return;
        GameObject proj = Instantiate(abilityData.abilityPrefab, _dynamicFirePoint.position, _dynamicFirePoint.rotation);
        WinchProjectile winch = proj.GetComponent<WinchProjectile>();
        if (winch != null)
        {
            List<Transform> tList = new List<Transform>();
            foreach (var t in _targets) if (t != null) tList.Add(t.transform);
            winch.SetupMultiTarget(tList, transform.root, _dynamicFirePoint, abilityData.pullForce);
        }
    }

    public override void StopAbility()
    {
        _isCharging = false;
        if (_deviceInstance != null) Destroy(_deviceInstance);
        _dynamicFirePoint = null;
        foreach (var mark in _marks.Values) if (mark != null) Destroy(mark);
        _targets.Clear();
        _targetingProgress.Clear();
        _lostTargetTimers.Clear();
        _marks.Clear();
        _markRenderers.Clear();
    }
}
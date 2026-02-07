using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityCharge : AbilityBase
{
    private CharacterController _characterController;
    private List<LineRenderer> _activeWinches = new List<LineRenderer>();

    public override void Initialize(AbilityData data, PlayerWalking move, Transform point, MonoBehaviour runnerScript)
    {
        base.Initialize(data, move, point, runnerScript);
        if (playerMove != null)
            _characterController = playerMove.GetComponent<CharacterController>();
    }

    public override void Execute(KeyCode inputKey)
    {
        if (runner != null)
            runner.StartCoroutine(SmartWinchRoutine(inputKey));
    }

    private IEnumerator SmartWinchRoutine(KeyCode key)
    {
        if (_characterController == null || abilityData.abilityPrefab == null) yield break;

        float chargeTime = 0f;
        while (Input.GetKey(key) && chargeTime < abilityData.minChargeTime)
        {
            chargeTime += Time.deltaTime;
            yield return null;
        }

        if (!Input.GetKey(key) && chargeTime < abilityData.minChargeTime) yield break;

        List<Transform> targets = new List<Transform>();
        float searchRadius = 0f;
        float maxRadius = 50f;

        while (Input.GetKey(key))
        {
            searchRadius += abilityData.targetAcquisitionSpeed * Time.deltaTime;
            if (searchRadius > maxRadius) searchRadius = maxRadius;

            Collider[] hits = Physics.OverlapSphere(firePoint.position, searchRadius);
            foreach (var hit in hits)
            {
                if (hit.transform.root != firePoint.root && targets.Count < abilityData.maxTargets)
                {
                    if (hit.GetComponent<Rigidbody>() != null || hit.GetComponent<IDamageable>() != null)
                    {
                        if (!targets.Contains(hit.transform))
                        {
                            targets.Add(hit.transform);
                            CreateWinchVisual(hit.transform.position);
                        }
                    }
                }
            }

            if (targets.Count > 0)
            {
                ApplyPullForce(targets);
            }

            UpdateWinchVisuals(targets);
            yield return null;
        }

        StopAbility();
    }

    private void ApplyPullForce(List<Transform> targets)
    {
        foreach (var target in targets)
        {
            if (target == null) continue;

            Vector3 pullDir = (firePoint.position - target.position).normalized;
            Rigidbody rb = target.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(pullDir * abilityData.pullForce * Time.deltaTime, ForceMode.VelocityChange);
            }
            else
            {
                Vector3 moveDir = (target.position - playerMove.transform.position).normalized;
                _characterController.Move(moveDir * (abilityData.pullForce * 0.1f) * Time.deltaTime);
            }
        }
    }

    private void CreateWinchVisual(Vector3 targetPos)
    {
        GameObject winchObj = Object.Instantiate(abilityData.abilityPrefab, firePoint.position, Quaternion.identity);
        LineRenderer lr = winchObj.GetComponent<LineRenderer>();
        if (lr != null) _activeWinches.Add(lr);
    }

    private void UpdateWinchVisuals(List<Transform> targets)
    {
        for (int i = 0; i < _activeWinches.Count; i++)
        {
            if (i < targets.Count && targets[i] != null && _activeWinches[i] != null)
            {
                _activeWinches[i].SetPosition(0, firePoint.position);
                _activeWinches[i].SetPosition(1, targets[i].position);
            }
        }
    }

    public override void StopAbility()
    {
        foreach (var lr in _activeWinches)
        {
            if (lr != null) Object.Destroy(lr.gameObject);
        }
        _activeWinches.Clear();
    }
}
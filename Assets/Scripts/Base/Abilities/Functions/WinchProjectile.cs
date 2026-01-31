using UnityEngine;
using System.Collections.Generic;

public class WinchProjectile : MonoBehaviour
{
    private List<Transform> _targets;
    private Transform _playerRoot;
    private Transform _firePoint;
    private float _pullForce;

    private bool _isAttached = false;
    private Vector3 _targetCenter;
    private List<LineRenderer> _ropes = new List<LineRenderer>();

    public void SetupMultiTarget(List<Transform> targets, Transform playerRoot, Transform firePoint, float pullForce)
    {
        _targets = targets;
        _playerRoot = playerRoot;
        _firePoint = firePoint;
        _pullForce = pullForce;

        CreateRope();
    }

    private void Update()
    {
        if (_targets == null || _targets.Count == 0 || _playerRoot == null)
        {
            Destroy(gameObject);
            return;
        }

        _targetCenter = Vector3.zero;
        int activeCount = 0;
        foreach (var t in _targets)
        {
            if (t != null) { _targetCenter += t.position; activeCount++; }
        }
        if (activeCount > 0) _targetCenter /= activeCount;

        if (!_isAttached)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetCenter, 50f * Time.deltaTime);
            if (Vector3.Distance(transform.position, _targetCenter) < 1f)
            {
                _isAttached = true;
                for (int i = 0; i < _targets.Count - 1; i++) CreateRope();
            }
        }
        else
        {
            bool allClose = true;
            for (int i = 0; i < _targets.Count; i++)
            {
                if (_targets[i] == null) continue;

                float dist = Vector3.Distance(_playerRoot.position, _targets[i].position);
                if (dist > 2.5f)
                {
                    Vector3 dir = (_playerRoot.position - _targets[i].position).normalized;
                    if (_targets[i].TryGetComponent<Rigidbody>(out Rigidbody rb))
                    {
                        rb.constraints = RigidbodyConstraints.FreezeRotation;
                        rb.linearVelocity = dir * _pullForce;
                    }
                    else
                        _targets[i].position += dir * _pullForce * Time.deltaTime;

                    allClose = false;
                }
            }

            transform.position = _targetCenter;

            if (allClose) Destroy(gameObject);
        }

        UpdateRopes();
    }

    private void CreateRope()
    {
        GameObject ropeObj = new GameObject("Rope");
        ropeObj.transform.SetParent(transform);
        var lr = ropeObj.AddComponent<LineRenderer>();
        lr.startWidth = 0.05f; lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.positionCount = 2;
        _ropes.Add(lr);
    }

    private void UpdateRopes()
    {
        if (_ropes.Count == 0) return;

        Vector3 startPos = (_firePoint != null) ? _firePoint.position : _playerRoot.position + (Vector3.up * 0.7f);

        _ropes[0].SetPosition(0, startPos);
        _ropes[0].SetPosition(1, transform.position);

        if (_isAttached)
        {
            for (int i = 0; i < _targets.Count; i++)
            {
                if (i + 1 < _ropes.Count && _targets[i] != null)
                {
                    _ropes[i + 1].SetPosition(0, transform.position);
                    _ropes[i + 1].SetPosition(1, _targets[i].position);
                }
            }
        }
    }
}
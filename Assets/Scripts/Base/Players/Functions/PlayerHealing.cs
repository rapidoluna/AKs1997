using System.Collections;
using UnityEngine;

public class PlayerHealing : MonoBehaviour
{
    [SerializeField] private float healDuration = 1.0f;
    [SerializeField] private float healAmount = 30f;
    [SerializeField] private int maxMedkits = 5;

    private int _currentMedkits;
    private bool _isHealing = false;
    private Coroutine _healCoroutine;

    private PlayerHealth _health;
    private WeaponController _weaponController;

    public bool IsHealing => _isHealing;

    private void Awake()
    {
        _health = GetComponent<PlayerHealth>();
        _weaponController = GetComponentInChildren<WeaponController>();
        _currentMedkits = maxMedkits;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3) && !_isHealing)
        {
            TryStartHealing();
        }

        if (_isHealing && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            CancelHealing();
        }
    }

    private void TryStartHealing()
    {
        if (_currentMedkits <= 0 || _health == null) return;

        if (_health.IsFullHealth())
        {
            Debug.Log("Health is already full.");
            return;
        }

        _healCoroutine = StartCoroutine(HealRoutine());
    }

    private IEnumerator HealRoutine()
    {
        _isHealing = true;

        if (_weaponController != null)
        {
            _weaponController.SwitchToSlot(_weaponController.CurrentIndex, true);
        }

        yield return new WaitForSeconds(healDuration);

        if (_health != null)
        {
            _health.Heal(healAmount);
            _currentMedkits--;
        }

        _isHealing = false;

        if (_weaponController != null)
        {
            _weaponController.UnlockWeapon();
        }
    }

    private void CancelHealing()
    {
        if (_healCoroutine != null)
        {
            StopCoroutine(_healCoroutine);
            _isHealing = false;

            if (_weaponController != null)
            {
                _weaponController.UnlockWeapon();
            }
        }
    }
}
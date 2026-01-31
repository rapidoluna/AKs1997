using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private GameObject[] weaponSlots = new GameObject[3];
    private int _currentWeaponIndex = 0;
    private bool _isWeaponLocked = false;
    private bool _isSwapping = false;

    public GameObject[] Slots => weaponSlots;
    public int CurrentIndex => _currentWeaponIndex;

    private void Start()
    {
        InitializeWeapons();
    }

    private void InitializeWeapons()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] != null)
                weaponSlots[i].SetActive(false);
        }

        if (weaponSlots.Length > 0 && weaponSlots[0] != null)
        {
            _currentWeaponIndex = 0;
            weaponSlots[0].SetActive(true);
        }
    }

    private void Update()
    {
        if (_isWeaponLocked || _isSwapping) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWithTimer(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWithTimer(1);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            int nextSlot = _currentWeaponIndex == 0 ? 1 : 0;
            SwitchWithTimer(nextSlot);
        }
    }

    public void SwitchWithTimer(int index)
    {
        if (index == _currentWeaponIndex || index < 0 || index >= weaponSlots.Length || weaponSlots[index] == null) return;

        StartCoroutine(SwapCooldownRoutine(index));
    }

    private IEnumerator SwapCooldownRoutine(int index)
    {
        _isSwapping = true;

        if (weaponSlots[_currentWeaponIndex] != null)
            weaponSlots[_currentWeaponIndex].SetActive(false);

        _currentWeaponIndex = index;
        weaponSlots[_currentWeaponIndex].SetActive(true);

        yield return new WaitForSeconds(0.65f);
        _isSwapping = false;
    }

    public void SwitchToSlot(int index, bool lockWeapon = false)
    {
        if (index < 0 || index >= weaponSlots.Length || weaponSlots[index] == null) return;

        if (weaponSlots[_currentWeaponIndex] != null)
            weaponSlots[_currentWeaponIndex].SetActive(false);

        _currentWeaponIndex = index;
        weaponSlots[_currentWeaponIndex].SetActive(true);
        _isWeaponLocked = lockWeapon;
    }

    public void UnlockWeapon() => _isWeaponLocked = false;

    public void SetAbilityWeapon(GameObject weaponPrefab)
    {
        if (weaponSlots.Length > 2 && weaponSlots[2] != null)
        {
            Destroy(weaponSlots[2]);
        }

        GameObject newWeapon = Instantiate(weaponPrefab, transform);
        newWeapon.SetActive(false);
        weaponSlots[2] = newWeapon;
    }

    public void ClearAbilitySlot()
    {
        if (weaponSlots.Length > 2 && weaponSlots[2] != null)
        {
            Destroy(weaponSlots[2]);
            weaponSlots[2] = null;
        }
    }
}
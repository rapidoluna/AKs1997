using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private GameObject[] weaponSlots = new GameObject[3];
    private int _currentWeaponIndex = 0;
    private bool _isWeaponLocked = false;

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
        if (_isWeaponLocked) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchToSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchToSlot(1);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            int nextSlot = _currentWeaponIndex + (scroll > 0 ? -1 : 1);
            if (nextSlot < 0) nextSlot = 1;
            if (nextSlot > 1) nextSlot = 0;
            SwitchToSlot(nextSlot);
        }
    }

    public void SwitchToSlot(int index, bool lockWeapon = false)
    {
        if (index < 0 || index >= weaponSlots.Length) return;

        if (weaponSlots[index] == null)
        {
            Debug.LogWarning($"[WeaponController] Slot {index} is empty.");
            return;
        }

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
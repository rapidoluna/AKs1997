using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Slots")]
    [SerializeField] private GameObject[] weaponSlots = new GameObject[2];
    private int _currentWeaponIndex = 0;

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

        if (weaponSlots[0] != null)
        {
            _currentWeaponIndex = 0;
            weaponSlots[0].SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchToSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchToSlot(1);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            int nextSlot = _currentWeaponIndex + (scroll > 0 ? -1 : 1);

            if (nextSlot < 0) nextSlot = weaponSlots.Length - 1;
            if (nextSlot >= weaponSlots.Length) nextSlot = 0;

            SwitchToSlot(nextSlot);
        }
    }

    public void SwitchToSlot(int index)
    {
        if (index < 0 || index >= weaponSlots.Length || index == _currentWeaponIndex) return;
        if (weaponSlots[index] == null) return;

        weaponSlots[_currentWeaponIndex].SetActive(false);
        _currentWeaponIndex = index;
        weaponSlots[_currentWeaponIndex].SetActive(true);
    }
}
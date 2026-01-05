using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    private WeaponController controller;

    private void Start()
    {
        controller = GetComponent<WeaponController>();
    }

    public void ApplyRecoil()
    {
        WeaponData data = controller.GetWeaponData();
    }
}
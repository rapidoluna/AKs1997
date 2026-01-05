using UnityEngine;

public class WeaponFireModeSelect : MonoBehaviour
{
    private WeaponController controller;
    private int currentModeIndex;

    private void Start()
    {
        controller = GetComponent<WeaponController>();
    }

    public void ChangeFireMode()
    {
        WeaponData data = controller.GetWeaponData();
        currentModeIndex = (currentModeIndex + 1) % data.AvailableFireModes.Length;
    }
}
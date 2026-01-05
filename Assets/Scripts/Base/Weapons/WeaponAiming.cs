using UnityEngine;

public class WeaponAiming : MonoBehaviour
{
    private WeaponController controller;

    private void Start()
    {
        controller = GetComponent<WeaponController>();
    }
}
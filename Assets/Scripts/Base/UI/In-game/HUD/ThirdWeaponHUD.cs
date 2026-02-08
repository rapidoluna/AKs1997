using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThirdWeaponHUD : MonoBehaviour
{
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private GameObject hudParent;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI ammoText;

    private void Update()
    {
        if (weaponController == null) return;

        GameObject weaponObj = null;
        if (weaponController.Slots != null && weaponController.Slots.Length > 2)
        {
            weaponObj = weaponController.Slots[2];
        }

        if (weaponObj != null)
        {
            if (hudParent != null && !hudParent.activeSelf)
                hudParent.SetActive(true);

            var shooting = weaponObj.GetComponent<WeaponShooting>();
            WeaponData data = null;

            if (shooting != null)
            {
                data = shooting.GetWeaponData();
            }

            if (data != null && weaponIcon != null)
            {
                if (weaponIcon.sprite != data.weaponIcon)
                    weaponIcon.sprite = data.weaponIcon;
            }

            var ammo = weaponObj.GetComponent<WeaponAmmo>();
            if (ammoText != null)
            {
                if (ammo != null && data != null)
                    ammoText.text = $"{ammo.CurrentAmmo} / {data.magSize}";
                else
                    ammoText.text = "- / -";
            }
        }
        else
        {
            if (hudParent != null && hudParent.activeSelf)
                hudParent.SetActive(false);
        }
    }
}
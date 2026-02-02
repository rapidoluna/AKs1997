using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponInventoryHUD : MonoBehaviour
{
    [SerializeField] private WeaponController controller;

    [Header("Slot 1 (Main)")]
    [SerializeField] private RectTransform slot1Transform;
    [SerializeField] private Image slot1Icon;
    [SerializeField] private TextMeshProUGUI slot1AmmoText; // TMP 타입 유지

    [Header("Slot 2 (Sub)")]
    [SerializeField] private RectTransform slot2Transform;
    [SerializeField] private Image slot2Icon;
    [SerializeField] private TextMeshProUGUI slot2AmmoText; // TMP 타입 유지

    [Header("Common Reload Settings")]
    [SerializeField] private GameObject commonReloadPrompt;
    [SerializeField] private int promptThreshold = 2;

    [Header("Visual Settings")]
    [SerializeField] private Vector2 activeSize = new Vector2(250, 80);
    [SerializeField] private Vector2 inactiveSize = new Vector2(180, 50);
    [SerializeField] private float lerpSpeed = 10f;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);

    private void Update()
    {
        if (controller == null) return;

        UpdateSlot(0, slot1Transform, slot1Icon, slot1AmmoText);
        UpdateSlot(1, slot2Transform, slot2Icon, slot2AmmoText);

        UpdateCommonReloadPrompt();
    }

    // 매개변수 타입을 TextMeshProUGUI로 수정
    private void UpdateSlot(int index, RectTransform rect, Image icon, TextMeshProUGUI ammoText)
    {
        if (rect == null) return;

        GameObject slotObj = (index < controller.Slots.Length) ? controller.Slots[index] : null;
        bool isActive = controller.CurrentIndex == index;

        if (slotObj == null)
        {
            rect.gameObject.SetActive(false);
            return;
        }

        rect.gameObject.SetActive(true);
        WeaponShooting shooting = slotObj.GetComponent<WeaponShooting>();
        WeaponAmmo ammo = slotObj.GetComponent<WeaponAmmo>();

        if (shooting == null || ammo == null) return;

        WeaponData data = shooting.GetWeaponData();

        rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, isActive ? activeSize : inactiveSize, Time.deltaTime * lerpSpeed);

        if (icon != null && data != null)
        {
            icon.sprite = data.weaponIcon;
            icon.color = Color.Lerp(icon.color, isActive ? activeColor : inactiveColor, Time.deltaTime * lerpSpeed);
        }

        if (ammoText != null)
        {
            if (isActive)
            {
                ammoText.overflowMode = TextOverflowModes.Overflow;

                ammoText.text = $"{ammo.CurrentAmmo} / {data.magSize}";
                rect.SetAsLastSibling();
            }
            else
            {
                ammoText.text = "";
            }
        }
    }

    private void UpdateCommonReloadPrompt()
    {
        if (commonReloadPrompt == null) return;

        int currentIndex = controller.CurrentIndex;
        if (currentIndex < 0 || currentIndex >= controller.Slots.Length) return;

        GameObject activeWeapon = controller.Slots[currentIndex];

        if (activeWeapon == null)
        {
            commonReloadPrompt.SetActive(false);
            return;
        }

        WeaponAmmo ammo = activeWeapon.GetComponent<WeaponAmmo>();
        WeaponReloading reloading = activeWeapon.GetComponent<WeaponReloading>();

        bool shouldShow = ammo != null && reloading != null &&
                         ammo.CurrentAmmo <= promptThreshold &&
                         !ammo.IsFull &&
                         !reloading.IsReloading;

        commonReloadPrompt.SetActive(shouldShow);
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SelectUIController : MonoBehaviour
{
    [Header("Data List")]
    [SerializeField] private CharacterData[] availableCharacters;
    [SerializeField] private GameObject[] availableWeapons;

    [Header("UI Groups (Parents)")]
    [SerializeField] private Transform characterButtonParent;
    [SerializeField] private Transform weaponButtonParent;

    [Header("UI References")]
    [SerializeField] private Image bannerImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image primaryWeaponIconDisplay;
    [SerializeField] private Image secondaryWeaponIconDisplay;

    [Header("Scene Settings")]
    [SerializeField] private string playSceneName = "PlayScene";

    private int _nextWeaponSlotIndex = 0;

    private void Start()
    {
        if (GlobalSelectionManager.Instance == null)
        {
            GameObject go = new GameObject("GlobalSelectionManager");
            go.AddComponent<GlobalSelectionManager>();
        }

        SetupCharacterButtons();
        SetupWeaponButtons();
    }

    private void SetupCharacterButtons()
    {
        if (characterButtonParent == null) return;

        int childCount = characterButtonParent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Button btn = characterButtonParent.GetChild(i).GetComponent<Button>();
            if (btn != null)
            {
                int index = i;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnCharacterButtonClicked(index));
            }
        }
    }

    private void SetupWeaponButtons()
    {
        if (weaponButtonParent == null) return;

        int childCount = weaponButtonParent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Button btn = weaponButtonParent.GetChild(i).GetComponent<Button>();
            if (btn != null)
            {
                int index = i;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnWeaponButtonClicked(index));
            }
        }
    }

    public void OnCharacterButtonClicked(int index)
    {
        if (index < 0 || index >= availableCharacters.Length) return;

        CharacterData data = availableCharacters[index];
        GlobalSelectionManager.Instance.SelectCharacter(data);
        UpdateCharacterUI(data);
    }

    public void OnWeaponButtonClicked(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex >= availableWeapons.Length) return;

        GameObject weaponPrefab = availableWeapons[weaponIndex];

        GlobalSelectionManager.Instance.SelectWeapon(_nextWeaponSlotIndex, weaponPrefab);
        UpdateWeaponUI(_nextWeaponSlotIndex, weaponPrefab);

        _nextWeaponSlotIndex++;
        if (_nextWeaponSlotIndex > 1) _nextWeaponSlotIndex = 0;
    }

    private void UpdateCharacterUI(CharacterData data)
    {
        if (bannerImage != null) bannerImage.sprite = data.characterBanner;
        if (nameText != null) nameText.text = data.CharacterName;
        if (descriptionText != null) descriptionText.text = data.CharacterDescription;
    }

    private void UpdateWeaponUI(int slot, GameObject weaponPrefab)
    {
        WeaponShooting shooting = weaponPrefab.GetComponent<WeaponShooting>();
        if (shooting == null) return;

        WeaponData wData = shooting.GetWeaponData();
        if (wData == null) return;

        if (slot == 0 && primaryWeaponIconDisplay != null)
            primaryWeaponIconDisplay.sprite = wData.weaponIcon;
        else if (slot == 1 && secondaryWeaponIconDisplay != null)
            secondaryWeaponIconDisplay.sprite = wData.weaponIcon;
    }

    public void OnGameStartButtonClicked()
    {
        if (GlobalSelectionManager.Instance.selectedCharacterData == null) return;
        if (GlobalSelectionManager.Instance.selectedWeaponPrefabs[0] == null) return;

        SceneManager.LoadScene(playSceneName);
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectUIController : MonoBehaviour
{
    [Header("Character Data")]
    [SerializeField] private List<CharacterData> characterDataList;
    [SerializeField] private Transform characterListParent;
    [SerializeField] private GameObject characterButtonPrefab;

    [Header("Character UI")]
    [SerializeField] private Image characterBannerImage;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterDescriptionText;

    [Header("Weapon Prefabs")]
    [SerializeField] private List<GameObject> weaponPrefabs;
    [SerializeField] private Transform weaponListParent;
    [SerializeField] private GameObject weaponButtonPrefab;

    [Header("Weapon Hover UI (Tooltip)")]
    [SerializeField] private GameObject weaponInfoPanel;
    [SerializeField] private RectTransform weaponInfoRect;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI weaponDescriptionText;
    [SerializeField] private Vector2 tooltipOffset = new Vector2(20, -20);

    [Header("Selected Weapon Icons")]
    [SerializeField] private Image primaryWeaponImage;
    [SerializeField] private Image secondaryWeaponImage;

    [Header("General")]
    [SerializeField] private Button startButton;

    private int _currentWeaponSlot = 0;
    private Coroutine _tooltipCoroutine;

    private void Start()
    {
        if (weaponInfoPanel != null)
        {
            weaponInfoPanel.SetActive(false);

            CanvasGroup group = weaponInfoPanel.GetComponent<CanvasGroup>();
            if (group == null) group = weaponInfoPanel.AddComponent<CanvasGroup>();
            group.blocksRaycasts = false;
            group.interactable = false;
        }

        InitializeCharacterButtons();
        InitializeWeaponButtons();

        if (characterDataList.Count > 0) SelectCharacter(0);
        if (startButton != null) startButton.onClick.AddListener(OnStartButtonClick);
    }

    private void InitializeCharacterButtons()
    {
        foreach (Transform child in characterListParent) Destroy(child.gameObject);

        for (int i = 0; i < characterDataList.Count; i++)
        {
            int index = i;
            GameObject btnObj = Instantiate(characterButtonPrefab, characterListParent);

            Image img = btnObj.GetComponent<Image>();
            if (img != null) img.sprite = characterDataList[i].characterIcon;

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null) btn.onClick.AddListener(() => SelectCharacter(index));

            AddEventTrigger(btnObj, EventTriggerType.PointerEnter, () => UpdateCharacterInfo(index));
        }
    }

    private void InitializeWeaponButtons()
    {
        foreach (Transform child in weaponListParent) Destroy(child.gameObject);

        for (int i = 0; i < weaponPrefabs.Count; i++)
        {
            int index = i;
            if (weaponPrefabs[i] == null) continue;

            WeaponShooting weaponScript = weaponPrefabs[i].GetComponent<WeaponShooting>();
            if (weaponScript == null) continue;

            WeaponData data = weaponScript.GetWeaponData();
            if (data == null) continue;

            GameObject btnObj = Instantiate(weaponButtonPrefab, weaponListParent);

            Image img = btnObj.GetComponent<Image>();
            if (img != null) img.sprite = data.weaponIcon;

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null) btn.onClick.AddListener(() => SelectWeapon(index));

            AddEventTrigger(btnObj, EventTriggerType.PointerEnter, () => StartTooltipRoutine(index));
            AddEventTrigger(btnObj, EventTriggerType.PointerExit, () => HideTooltip());
        }
    }

    private void AddEventTrigger(GameObject obj, EventTriggerType type, UnityEngine.Events.UnityAction action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null) trigger = obj.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener((data) => action.Invoke());
        trigger.triggers.Add(entry);
    }

    private void StartTooltipRoutine(int index)
    {
        if (_tooltipCoroutine != null) StopCoroutine(_tooltipCoroutine);
        _tooltipCoroutine = StartCoroutine(TooltipDelay(index));
    }

    private IEnumerator TooltipDelay(int index)
    {
        yield return new WaitForSeconds(0.5f);
        ShowTooltip(index);
    }

    private void ShowTooltip(int index)
    {
        if (index < 0 || index >= weaponPrefabs.Count || weaponInfoPanel == null) return;

        WeaponShooting weaponScript = weaponPrefabs[index].GetComponent<WeaponShooting>();
        if (weaponScript != null)
        {
            WeaponData data = weaponScript.GetWeaponData();
            if (data != null)
            {
                if (weaponNameText != null) weaponNameText.text = data.WeaponName;
                if (weaponDescriptionText != null) weaponDescriptionText.text = data.WeaponDescription;

                if (weaponInfoRect != null)
                {
                    weaponInfoRect.position = Input.mousePosition + (Vector3)tooltipOffset;
                }

                weaponInfoPanel.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(weaponInfoRect);
            }
        }
    }

    private void HideTooltip()
    {
        if (_tooltipCoroutine != null) StopCoroutine(_tooltipCoroutine);
        if (weaponInfoPanel != null) weaponInfoPanel.SetActive(false);
    }

    private void SelectCharacter(int index)
    {
        if (GlobalSelectionManager.Instance != null)
        {
            GlobalSelectionManager.Instance.SelectCharacter(characterDataList[index]);
        }
        UpdateCharacterInfo(index);
    }

    private void SelectWeapon(int index)
    {
        if (index < 0 || index >= weaponPrefabs.Count) return;

        GameObject selectedPrefab = weaponPrefabs[index];
        WeaponShooting weaponScript = selectedPrefab.GetComponent<WeaponShooting>();
        WeaponData data = weaponScript != null ? weaponScript.GetWeaponData() : null;

        if (GlobalSelectionManager.Instance != null)
        {
            if (GlobalSelectionManager.Instance.selectedWeaponPrefabs == null)
                GlobalSelectionManager.Instance.selectedWeaponPrefabs = new GameObject[2];

            GlobalSelectionManager.Instance.SelectWeapon(_currentWeaponSlot, selectedPrefab);
        }

        if (data != null)
        {
            if (_currentWeaponSlot == 0 && primaryWeaponImage != null)
                primaryWeaponImage.sprite = data.weaponIcon;
            else if (_currentWeaponSlot == 1 && secondaryWeaponImage != null)
                secondaryWeaponImage.sprite = data.weaponIcon;
        }

        _currentWeaponSlot = (_currentWeaponSlot + 1) % 2;
    }

    private void UpdateCharacterInfo(int index)
    {
        if (index < 0 || index >= characterDataList.Count) return;

        CharacterData data = characterDataList[index];
        if (characterNameText != null) characterNameText.text = data.CharacterName;
        if (characterDescriptionText != null) characterDescriptionText.text = data.CharacterDescription;
        if (characterBannerImage != null) characterBannerImage.sprite = data.characterBanner;
    }

    private void OnStartButtonClick()
    {
        SceneManager.LoadScene("PlayScene");
    }
}
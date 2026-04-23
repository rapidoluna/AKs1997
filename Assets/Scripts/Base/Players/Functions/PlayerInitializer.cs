using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    private const int MaxSelectedWeaponSlots = 2;

    [SerializeField] private CharacterData debugCharacterData;
    [SerializeField] private Transform weaponHoldPoint;

    private CharacterData _activeCharacterData;

    public CharacterData ActiveCharacterData => _activeCharacterData;

    private void Start()
    {
        InitializeCharacter();
        InitializeWeapons();
    }

    private void InitializeCharacter()
    {
        if (GlobalSelectionManager.Instance != null && GlobalSelectionManager.Instance.selectedCharacterData != null)
        {
            _activeCharacterData = GlobalSelectionManager.Instance.selectedCharacterData;
        }
        else
        {
            _activeCharacterData = debugCharacterData;
        }

        if (_activeCharacterData == null) return;

        if (_activeCharacterData.characterPrefab != null)
        {
            GameObject model = Instantiate(_activeCharacterData.characterPrefab, transform);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
        }

        PlayerHealth health = GetComponent<PlayerHealth>();
        if (health != null) health.SetData(_activeCharacterData);

        var abilityProcessor = GetComponentInChildren<AbilityProcessor>();
        var ultimateProcessor = GetComponentInChildren<UltimateProcessor>();
        var playerWalking = GetComponent<PlayerWalking>();

        if (abilityProcessor != null)
            abilityProcessor.Initialize(_activeCharacterData.characterSpeciality, playerWalking, transform);

        if (ultimateProcessor != null)
            ultimateProcessor.Initialize(_activeCharacterData.characterUltimate, playerWalking, transform);
    }

    private void InitializeWeapons()
    {
        if (weaponHoldPoint == null) return;

        GlobalSelectionManager selectionManager = GlobalSelectionManager.Instance;
        if (selectionManager == null) return;

        WeaponController weaponController = weaponHoldPoint.GetComponent<WeaponController>();
        if (weaponController == null) return;

        GameObject[] selectedWeaponPrefabs = selectionManager.selectedWeaponPrefabs;
        if (selectedWeaponPrefabs == null || selectedWeaponPrefabs.Length == 0)
        {
            weaponController.InitializeWeapons();
            return;
        }

        int weaponCount = Mathf.Min(MaxSelectedWeaponSlots, selectedWeaponPrefabs.Length, weaponController.Slots.Length);
        for (int i = 0; i < weaponCount; i++)
        {
            GameObject weaponPrefab = selectedWeaponPrefabs[i];
            if (weaponPrefab != null)
            {
                GameObject weaponInstance = Instantiate(weaponPrefab, weaponHoldPoint);
                weaponInstance.transform.localPosition = Vector3.zero;
                weaponInstance.transform.localRotation = Quaternion.identity;
                weaponInstance.name = weaponPrefab.name;

                weaponController.RegisterWeapon(i, weaponInstance);
            }
        }

        weaponController.InitializeWeapons();
    }
}

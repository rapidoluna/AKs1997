using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    [SerializeField] private CharacterData debugCharacterData;
    [SerializeField] private Transform weaponHoldPoint;

    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerWalking playerWalking;
    [SerializeField] private AbilityProcessor abilityProcessor;
    [SerializeField] private UltimateProcessor ultimateProcessor;

    public CharacterData CharacterData { get; private set; }

    private void Awake()
    {
        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();
        if (playerWalking == null) playerWalking = GetComponent<PlayerWalking>();
        if (abilityProcessor == null) abilityProcessor = GetComponentInChildren<AbilityProcessor>();
        if (ultimateProcessor == null) ultimateProcessor = GetComponentInChildren<UltimateProcessor>();
    }

    private void Start()
    {
        InitCharacter();
        InitWeapons();
    }

    private void InitCharacter()
    {
        if (GlobalSelectionManager.Instance != null && GlobalSelectionManager.Instance.selectedCharacterData != null)
            CharacterData = GlobalSelectionManager.Instance.selectedCharacterData;
        else
            CharacterData = debugCharacterData;

        if (CharacterData == null) return;

        if (CharacterData.characterPrefab != null)
        {
            foreach (Transform child in transform)
                if (child.name.Contains("Model")) Destroy(child.gameObject);

            GameObject model = Instantiate(CharacterData.characterPrefab, transform);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.name = "PlayerModel";
        }

        if (playerHealth != null)
            playerHealth.SetData(CharacterData);

        Transform firePoint = weaponHoldPoint != null ? weaponHoldPoint : transform;

        if (abilityProcessor != null)
            abilityProcessor.Initialize(CharacterData.characterSpeciality, playerWalking, firePoint);

        if (ultimateProcessor != null)
            ultimateProcessor.Initialize(CharacterData.characterUltimate, playerWalking, firePoint);
    }

    private void InitWeapons()
    {
        if (GlobalSelectionManager.Instance == null || weaponHoldPoint == null) return;

        WeaponController controller = weaponHoldPoint.GetComponent<WeaponController>();
        if (controller == null) return;

        for (int i = 0; i < 2; i++)
        {
            GameObject prefab = GlobalSelectionManager.Instance.selectedWeaponPrefabs[i];
            if (prefab != null)
            {
                GameObject weaponObj = Instantiate(prefab, weaponHoldPoint);
                weaponObj.transform.localPosition = Vector3.zero;
                weaponObj.transform.localRotation = Quaternion.identity;
                controller.RegisterWeapon(i, weaponObj);
            }
        }

        controller.InitializeWeapons();
    }
}
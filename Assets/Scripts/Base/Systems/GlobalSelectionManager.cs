using UnityEngine;

public class GlobalSelectionManager : MonoBehaviour
{
    public static GlobalSelectionManager Instance;

    public CharacterData selectedCharacterData;
    public GameObject[] selectedWeaponPrefabs = new GameObject[2];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectCharacter(CharacterData data)
    {
        selectedCharacterData = data;
    }

    public void SelectWeapon(int slotIndex, GameObject weaponPrefab)
    {
        if (slotIndex >= 0 && slotIndex < selectedWeaponPrefabs.Length)
        {
            selectedWeaponPrefabs[slotIndex] = weaponPrefab;
        }
    }
}
using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    [SerializeField] private CharacterData characterData;
    [SerializeField] private Transform weaponHoldPoint;


    private void Awake()
    {
        if (characterData == null || characterData.characterPrefab == null) return;

        GameObject model = Instantiate(characterData.characterPrefab, transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;

        PlayerHealth health = GetComponent<PlayerHealth>();
        if (health != null) health.SetData(characterData);
    }
    public CharacterData CharacterData => characterData;
}
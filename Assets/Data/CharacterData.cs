using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "AKs97/CharacterData")]
public class CharacterData : ScriptableObject
{
    [SerializeField] private string characterName;
    [TextArea]
    [SerializeField] private string characterDescription;

    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float meleeDamage = 50;

    public AbilityData characterSpeciality;
    public AbilityData characterUltimate;

    public GameObject characterPrefab;
    public Sprite characterIcon;
    public Sprite characterBanner;

    public string CharacterName => characterName;
    public string CharacterDescription => characterDescription;
    public float MaxHealth => maxHealth;
    public float MeleeDamage => meleeDamage;
}
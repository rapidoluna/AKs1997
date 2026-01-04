using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "AKs97/CharacterData")]
public class CharacterData : ScriptableObject
{
    //캐릭터 정보
    [SerializeField] private string characterName;
    [SerializeField] private string characterDescription;

    //캐릭터 최대 체력, 근접 공격 대미지, 최대 부활 기회
    private float maxHealth = 100;
    private float meleeDamage = 50;

    //캐릭터 주특기 및 특수기
    public AbilityData characterSpeciality;
    public AbilityData characterUltimate;

    //캐릭터 관련 비주얼 요소
    public GameObject characterPrefab;
    public Sprite characterIcon;

    //프로퍼티
    public string CharacterName => characterName;
    public string CharacterDescription => characterDescription;
    public float MaxHealth => maxHealth;
    public float MeleeDamage => meleeDamage;
}

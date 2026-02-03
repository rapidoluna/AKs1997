using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct DropItem//드랍할 아이템 구조
{
    public GameObject itemPrefab;//아이템 프리팹
    [Range(0, 100)] public float dropRate;//드랍 확률
}

//적 공격 유형
public enum EnemyPattern
{ 
    Basic, 
    Agile, 
    Tactical, 
    Tanker
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "AKs97/EnemyData")]
public class EnemyData : ScriptableObject
{
    public EnemyPattern pattern;

    public string enemyName;
    public int maxHealth;
    public float walkSpeed;
    public float runSpeed;

    public float detectRange;
    public float attackRange;
    public int attackDamage;
    public float attackCooldown;

    public List<DropItem> dropTable = new List<DropItem>();

    public GameObject bulletPrefab;
    public float bulletSpeed = 1500f;

    public float dodgeChance = 0.3f;
    public float meleeRange = 2.5f;
    public int meleeDamage = 30;
    public float meleeCooldown = 1.5f;

    public GameObject enemyPrefab;
    public Sprite enemyIcon;
}
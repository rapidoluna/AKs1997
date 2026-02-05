using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct DropItem
{
    public GameObject itemPrefab;
    [Range(0, 100)] public float dropRate;
}

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
    public float shareRange = 20f;
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
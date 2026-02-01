using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct DropItem
{
    public GameObject itemPrefab;
    [Range(0, 100)] public float dropRate;
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "AKs97/EnemyData")]
public class EnemyData : ScriptableObject
{
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

    public GameObject enemyPrefab;
    public Sprite enemyIcon;
}
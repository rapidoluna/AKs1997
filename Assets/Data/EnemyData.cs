using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "AKs97/EnemyData")]
public class EnemyData : ScriptableObject
{
    //적 이름 및 체력, 이속
    public string enemyName;
    public int maxHealth;
    public float walkSpeed;
    public float runSpeed;

    //감지 범위 및 공격 범위
    //공격 대미지 및 쿨다운
    public float detectRange;
    public float attackRange;
    public int attackDamage;
    public float attackCooldown;

    //죽고 나서 드랍하는 아이템
    //아마도 총기나 플레이어가 찾아야 하는 물품으로 설정
    public GameObject dropItemPrefab;
    [Range(0, 100)] public float dropRate;

    //적이 총을 쏘는 경우 설정
    public GameObject bulletPrefab;
    public float bulletSpeed = 1500f;
}
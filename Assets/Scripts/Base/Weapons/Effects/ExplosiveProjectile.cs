using UnityEngine;
using System.Collections.Generic;

public class ExplosiveProjectile : Projectile
{
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private GameObject explosionVFX;

    protected override void DetectNearbyEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, soundRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                EnemyDetect detector = hitCollider.GetComponentInParent<EnemyDetect>();
                if (detector != null)
                {
                    detector.OnProjectileDetected(_owner.transform.position);
                }
            }
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (_owner == null) return;

        // [Debug] 충돌 감지 로그
        Debug.Log($"[충돌 감지] 대상: {other.name} | 태그: {other.tag} | Layer: {LayerMask.LayerToName(other.gameObject.layer)}");

        // 필터링 로그
        if (other.gameObject == _owner)
        {
            Debug.Log($" -> [무시] 발사자 본인({_owner.name})입니다.");
            return;
        }
        if (other.transform.IsChildOf(_owner.transform))
        {
            Debug.Log($" -> [무시] 발사자의 자식 오브젝트({other.name})입니다.");
            return;
        }
        if (other.CompareTag(_owner.tag))
        {
            Debug.Log($" -> [무시] 같은 태그({_owner.tag})를 가진 아군입니다.");
            return;
        }

        IDamageable directTarget = other.GetComponentInParent<IDamageable>() ?? other.GetComponent<IDamageable>();

        if (directTarget != null)
        {
            Debug.Log($" -> [직격] {other.name}에게 직격 대미지 {_damage} 적용");
            directTarget.TakeDamage(_damage);
        }

        if (!other.isTrigger || directTarget != null)
        {
            Debug.Log($" -> [폭발 실행] 직격 대상 {other.name}를 제외하고 폭발 처리 시작");
            Explode(directTarget);
        }
        else
        {
            Debug.Log(" -> [폭발 안함] Trigger이거나 유효한 충돌체가 아님");
        }
    }

    private void Explode(IDamageable excludeTarget)
    {
        if (explosionVFX != null)
            Instantiate(explosionVFX, transform.position, Quaternion.identity);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, targetLayer);
        HashSet<IDamageable> damagedEntities = new HashSet<IDamageable>();

        if (excludeTarget != null) damagedEntities.Add(excludeTarget);

        Debug.Log($"[폭발 범위] 반경 {explosionRadius}m 내 감지된 콜라이더 수: {hitColliders.Length}");

        foreach (var hit in hitColliders)
        {
            if (_owner != null)
            {
                if (hit.gameObject == _owner)
                {
                    Debug.Log($"   - [Skip] 발사자 본인: {hit.name}");
                    continue;
                }
                if (hit.transform.IsChildOf(_owner.transform))
                {
                    Debug.Log($"   - [Skip] 발사자의 자식: {hit.name}");
                    continue;
                }
                if (hit.CompareTag(_owner.tag))
                {
                    Debug.Log($"   - [Skip] 아군 태그: {hit.name}");
                    continue;
                }
            }

            IDamageable damageable = hit.GetComponentInParent<IDamageable>() ?? hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if (!damagedEntities.Contains(damageable))
                {
                    Debug.Log($"   - [광역 피해] {hit.name}에게 대미지 {_damage} 적용");
                    damageable.TakeDamage(_damage);
                    damagedEntities.Add(damageable);
                }
                else
                {
                    Debug.Log($"   - [중복 방지] {hit.name}은(는) 이미 직격 혹은 처리됨");
                }
            }
            else
            {
                Debug.Log($"   - [대상 아님] {hit.name} (IDamageable 없음)");
            }
        }
        DisableProjectile();
    }
}
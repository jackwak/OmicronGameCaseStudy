using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    private ProjectileData data;
    private IProjectileMovementStrategy movementStrategy;
    private float elapsedTime;
    private bool hasHit = false;
    
    public void Initialize(ProjectileData projectileData, Vector2 direction)
    {
        data = projectileData;
        hasHit = false;
        elapsedTime = 0f;
        
        movementStrategy = data.CreateMovementStrategy();
        movementStrategy.Initialize(transform.position, direction, data.speed, data);
        
        CancelInvoke();
        Invoke(nameof(ReturnToPool), data.lifetime);
    }
    
    void Update()
    {
        if (hasHit || movementStrategy == null) return;
        movementStrategy.UpdateMovement(transform, ref elapsedTime);
    }
    
    void ReturnToPool()
    {
        if (!hasHit && data != null)
        {
            ObjectPool.Instance.Return(data.projectilePoolKey, gameObject);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;
        
        if (((1 << other.gameObject.layer) & data.damageLayer) != 0)
        {
            OnHit(transform.position);
        }
    }
    
    void OnHit(Vector2 hitPosition)
    {
        if (hasHit) return;
        hasHit = true;
        
        if (data.movementType == ProjectileMovementType.CurvedTrajectory)
        {
            // Bomba - Alan hasarı
            Collider2D[] hits = Physics2D.OverlapCircleAll(hitPosition, data.explosionRadius, data.damageLayer);
            
            foreach (var hit in hits)
            {
                IDamagable damageable = hit.GetComponent<IDamagable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(data.damage);
                }
            }
        }
        else
        {
            // Bullet - Tek hedef hasarı
            Collider2D[] hits = Physics2D.OverlapCircleAll(hitPosition, 0.5f, data.damageLayer);
            if (hits.Length > 0)
            {
                IDamagable damageable = hits[0].GetComponent<IDamagable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(data.damage);
                }
            }
        }
        
        // Efekt spawn (hepsi için ortak)
        if (!string.IsNullOrEmpty(data.hitEffectPoolKey))
        {
            GameObject effect = ObjectPool.Instance.Get(data.hitEffectPoolKey, null);
            effect.transform.position = hitPosition;
            StartCoroutine(ReturnEffectToPool(effect, data.hitEffectPoolKey, 2f));
        }
        
        ObjectPool.Instance.Return(data.projectilePoolKey, gameObject);
    }
    
    IEnumerator ReturnEffectToPool(GameObject effect, string poolKey, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (effect != null)
        {
            ObjectPool.Instance.Return(poolKey, effect);
        }
    }
    
    void OnDrawGizmos()
    {
        if (data != null && data.explosionRadius > 0)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawWireSphere(transform.position, data.explosionRadius);
        }
    }
}

public enum ProjectileMovementType
{
    Straight,           // Düz gider (Top)
    CurvedTrajectory    // Eğrilen yörünge (Bomba)
}


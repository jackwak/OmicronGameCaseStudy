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

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        
        if (other.TryGetComponent(out IDamagable damagable))
        {
            OnHit(transform.position, damagable);
        }
    }
    
    void OnHit(Vector2 hitPosition, IDamagable damagable)
    {
        if (hasHit) return;
        hasHit = true;
        
        if (data.movementType == ProjectileMovementType.CurvedTrajectory)
        {
            // Bomba - Alan hasarı
            Collider[] hits = Physics.OverlapSphere(hitPosition, data.explosionRadius, data.damageLayer);
            
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
            Collider[] hits = Physics.OverlapSphere(hitPosition, 0.5f, data.damageLayer);
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
        /*if (!string.IsNullOrEmpty(data.hitEffectPoolKey))
        {
            GameObject effect = ObjectPool.Instance.Get(data.hitEffectPoolKey, null);
            effect.transform.position = hitPosition;
            StartCoroutine(ReturnEffectToPool(effect, data.hitEffectPoolKey, 2f));
        }*/
        
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
}

public enum ProjectileMovementType
{
    Straight,
    CurvedTrajectory
}


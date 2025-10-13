using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Projectile", menuName = "Combat/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    [Header("Object Pool")]
    public string projectilePoolKey = "Projectile_Cannon";
    
    [Header("Atış Ayarları")]
    [Tooltip("Bu projectile ne kadar sürede bir atılacak")]
    public float fireRate = 1f;
    
    [Tooltip("Bir pakette kaç mermi atılacak")]
    public int projectilesPerShot = 1;
    
    [Tooltip("Paket içindeki mermiler arası süre")]
    public float delayBetweenProjectiles = 0.1f;
    
    [Tooltip("Spread açısı (çoklu mermi için açılma)")]
    public float spreadAngle = 0f;
    
    [Header("Hareket")]
    public ProjectileMovementType movementType;
    public float speed = 10f;
    public float lifetime = 5f;
    
    [Header("Curved Trajectory Parametreleri")]
    [ShowIf("movementType", ProjectileMovementType.CurvedTrajectory)]
    [Tooltip("Minimum eğrilik açısı (örn: -30)")]
    public float minCurveAngle = -30f;
    
    [ShowIf("movementType", ProjectileMovementType.CurvedTrajectory)]
    [Tooltip("Maximum eğrilik açısı (örn: 30)")]
    public float maxCurveAngle = 30f;
    
    [ShowIf("movementType", ProjectileMovementType.CurvedTrajectory)]
    [Tooltip("Patlama yarıçapı (alan hasarı)")]
    public float explosionRadius = 3f;
    
    [Header("Hasar")]
    public float damage = 10f;
    public LayerMask damageLayer;
    
    [Header("Efektler")]
    [Tooltip("Çarpma/Patlama efekti")]
    public string hitEffectPoolKey = "Effect_Hit";
    
    public IProjectileMovementStrategy CreateMovementStrategy()
    {
        switch (movementType)
        {
            case ProjectileMovementType.Straight:
                return new StraightMovementStrategy();
            case ProjectileMovementType.CurvedTrajectory:
                return new CurvedTrajectoryMovementStrategy();
            default:
                return new StraightMovementStrategy();
        }
    }
}


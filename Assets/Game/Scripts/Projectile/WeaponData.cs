using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Combat/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Projectile'lar")]
    [Tooltip("Bu weapon'dan atılacak projectile'lar")]
    public List<ProjectileData> projectiles = new List<ProjectileData>();
}

[System.Serializable]
public class ProjectileConfig
{
    public ProjectileData projectileData;

    [Header("Paket Ayarları")]
    [Tooltip("Bir pakette kaç mermi atılacak")]
    public int projectilesPerShot = 1;

    [Tooltip("Paket içindeki mermiler arası süre")]
    public float delayBetweenProjectiles = 0.1f;

    [Header("Yön Ayarları")]
    [Tooltip("Spread açısı (çoklu mermi için açılma)\nÖrnek: 3 mermi, 60° spread = 60°, 90°, 120°")]
    public float spreadAngle = 0f;
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WeaponShooter : MonoBehaviour
{
    [Header("Ayarlar")]
    public WeaponData currentWeapon;
    public Transform firePoint;

    [Header("Debug")]
    public bool drawDebug = true;

    private const float BASE_ANGLE = 90f;
    private bool _canShoot;
    private Dictionary<ProjectileData, float> nextFireTimes = new Dictionary<ProjectileData, float>();

    void OnEnable()
    {
        EventManager.Instance.EnterGameState += SetCanShootTrue;
        EventManager.Instance.EnterFinishState += SetCanShootFalse;
        EventManager.Instance.InitializeWeaponData += SetWeaponData;
    }

    void OnDisable()
    {
        EventManager.Instance.EnterGameState -= SetCanShootTrue;
        EventManager.Instance.EnterFinishState -= SetCanShootFalse;
        EventManager.Instance.InitializeWeaponData -= SetWeaponData;
    }

    void Update()
    {
        if (!_canShoot) return;
        if (currentWeapon == null) return;

        // InputManager'dan dokunma kontrolü
        bool isTouching = false;
        if (InputManager.Instance != null)
        {
            isTouching = InputManager.Instance.IsTouching;
        }

        if (!isTouching) return;

        // Her projectile kendi fire rate'ine göre kontrol et
        foreach (var projectileData in currentWeapon.projectiles)
        {
            if (projectileData == null) continue;

            // Bu projectile'ın next fire time'ını al
            if (!nextFireTimes.ContainsKey(projectileData))
            {
                nextFireTimes[projectileData] = 0f;
            }

            // Ateşleme zamanı geldiyse ateşle
            if (Time.time >= nextFireTimes[projectileData])
            {
                FireProjectile(projectileData);
                nextFireTimes[projectileData] = Time.time + projectileData.fireRate;
            }
        }
    }

    private void SetWeaponData(WeaponData weaponData)
    {
        currentWeapon = weaponData;
    }

    void FireProjectile(ProjectileData data)
    {
        StartCoroutine(FirePacket(data));
    }

    IEnumerator FirePacket(ProjectileData data)
    {
        for (int i = 0; i < data.projectilesPerShot; i++)
        {
            float angleOffset = 0f;

            if (data.projectilesPerShot > 1 && data.spreadAngle > 0)
            {
                float totalSpread = data.spreadAngle;
                float angleStep = totalSpread / (data.projectilesPerShot - 1);
                angleOffset = -totalSpread / 2f + angleStep * i;
            }

            SpawnProjectile(data, BASE_ANGLE + angleOffset);

            if (i < data.projectilesPerShot - 1)
            {
                yield return new WaitForSeconds(data.delayBetweenProjectiles);
            }
        }
    }

    void SpawnProjectile(ProjectileData data, float angle)
    {
        GameObject proj = ObjectPool.Instance.Get(data.projectilePoolKey, null);
        proj.transform.position = firePoint.position;

        float rad = angle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        Projectile projectileScript = proj.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(data, direction);
        }
    }

    private void SetCanShootTrue()
    {
        _canShoot = true;
    }
    private void SetCanShootFalse()
    {
        _canShoot = false;
    }
}
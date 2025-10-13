using UnityEngine;

public interface IProjectileMovementStrategy
{
    void Initialize(Vector2 startPos, Vector2 direction, float speed, ProjectileData data);
    void UpdateMovement(Transform transform, ref float elapsedTime);
}

public class StraightMovementStrategy : IProjectileMovementStrategy
{
    private Vector2 velocity;
    
    public void Initialize(Vector2 startPos, Vector2 direction, float speed, ProjectileData data)
    {
        velocity = direction.normalized * speed;
    }
    
    public void UpdateMovement(Transform transform, ref float elapsedTime)
    {
        elapsedTime += Time.deltaTime;
        Vector2 currentPos = transform.position;
        currentPos += velocity * Time.deltaTime;
        transform.position = currentPos;
        
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}

public class CurvedTrajectoryMovementStrategy : IProjectileMovementStrategy
{
    private float speed;
    private float targetAngleOffset;
    private float initialRotation;
    private float lifetime;
    
    public void Initialize(Vector2 startPos, Vector2 direction, float speed, ProjectileData data)
    {
        this.speed = speed;
        this.lifetime = data.lifetime;
        
        targetAngleOffset = Random.Range(data.minCurveAngle, data.maxCurveAngle);
        initialRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
    
    public void UpdateMovement(Transform transform, ref float elapsedTime)
    {
        elapsedTime += Time.deltaTime;
        
        float t = Mathf.Clamp01(elapsedTime / lifetime);
        float currentAngle = Mathf.Lerp(initialRotation, initialRotation + targetAngleOffset, t);
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        
        Vector2 forward = transform.right;
        Vector2 currentPos = transform.position;
        currentPos += forward * speed * Time.deltaTime;
        transform.position = currentPos;
    }
}
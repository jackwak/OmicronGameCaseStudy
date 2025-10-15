using UnityEngine;

public interface IProjectileMovementStrategy
{
    void Initialize(Vector2 startPos, Vector2 direction, float speed, ProjectileData data);
    void UpdateMovement(Transform transform, ref float elapsedTime);
}

public class StraightMovementStrategy : IProjectileMovementStrategy
{
    private Vector2 _velocity;
    
    public void Initialize(Vector2 startPos, Vector2 direction, float speed, ProjectileData data)
    {
        _velocity = direction.normalized * speed;
    }
    
    public void UpdateMovement(Transform transform, ref float elapsedTime)
    {
        elapsedTime += Time.deltaTime;
        Vector2 currentPos = transform.position;
        currentPos += _velocity * Time.deltaTime;
        transform.position = currentPos;
        
        float angle = Mathf.Atan2(_velocity.y, _velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}

public class CurvedTrajectoryMovementStrategy : IProjectileMovementStrategy
{
    private float _speed;
    private float _targetAngleOffset;
    private float _initialRotation;
    private float _lifetime;
    
    public void Initialize(Vector2 startPos, Vector2 direction, float speed, ProjectileData data)
    {
        this._speed = speed;
        this._lifetime = data.lifetime;
        
        _targetAngleOffset = Random.Range(data.minCurveAngle, data.maxCurveAngle);
        _initialRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
    
    public void UpdateMovement(Transform transform, ref float elapsedTime)
    {
        elapsedTime += Time.deltaTime;
        
        float t = Mathf.Clamp01(elapsedTime / _lifetime);
        float currentAngle = Mathf.Lerp(_initialRotation, _initialRotation + _targetAngleOffset, t);
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        
        Vector2 forward = transform.right;
        Vector2 currentPos = transform.position;
        currentPos += forward * _speed * Time.deltaTime;
        transform.position = currentPos;
    }
}
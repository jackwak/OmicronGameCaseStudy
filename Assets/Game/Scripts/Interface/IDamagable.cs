public interface IDamagable
{
    float MaxHealth { get; set; }
    float Health { get; set; }
    void TakeDamage(float damage);
}
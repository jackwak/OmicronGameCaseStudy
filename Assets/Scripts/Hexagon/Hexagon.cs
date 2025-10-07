using UnityEngine;

public class Hexagon : MonoBehaviour, IDamagable
{
    [Header(" Datas ")]
    [SerializeField] private float _maxHealth;
    private float _health;

    public float MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = value;
    }

    public float Health
    {
        get => _health;
        set
        {
            _health = value;
            if (_health <= 0)
            {
                // Kill Hexagon
            }
        }
    }

    public void InitializeData(float health)
    {
        MaxHealth = health;
        ResetHealth();
    }
    
    private void ResetHealth(){
        Health = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
    }
}
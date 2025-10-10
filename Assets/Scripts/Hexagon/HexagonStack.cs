using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexagonStack : MonoBehaviour, IDamagable
{
    [Header(" References ")]
    [SerializeField] private Transform _hexagonsParent;
    [SerializeField] private TextMeshPro _healthText;

    [Header(" Settings ")]
    private float _hexagonsYOffset = 0.065f;
    private const string HEXAGON_POOL_KEY = "Hexagon";

    [Header(" Datas ")]
    private Stack<Hexagon> _activeHexagon = new Stack<Hexagon>();
    private float _maxHealth = 20f;
    private float _health;

    // Properties
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

    //initialization
    public void InitializeHexagonStack(float health, int count, Color color)
    {
        InitializeHealth(health);
        InitializeStackCount(count, color);
        InitializeText(health, count);
    }

    public void InitializeHealth(float health)
    {
        MaxHealth = health;
        ResetHealth();
    }

    private void InitializeText(float health, int count)
    {
        _healthText.text = health.ToString();
        _healthText.transform.localPosition = new Vector3(0, _hexagonsYOffset * count, -0.2f * count);
    }

    public void InitializeStackCount(int count, Color color)
    {
        //initialize stack count

        // take from pool
        for (int i = 0; i < count; i++)
        {
            ObjectPool.Instance.Get(HEXAGON_POOL_KEY, _hexagonsParent);
        }
        SetHexagonsPosition(color);
    }

    private void SetHexagonsPosition(Color color)
    {
        for (int i = 0; i < _hexagonsParent.childCount; i++)
        {
            Hexagon hexagon = _hexagonsParent.GetChild(i).GetComponent<Hexagon>();
            hexagon.SetLocalPosition(_hexagonsYOffset, i);
            hexagon.SetColor(color);

            _activeHexagon.Push(hexagon);
        }
    }

    private void ResetHealth()
    {
        Health = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
    }
}

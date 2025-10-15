using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using NUnit.Framework.Constraints;

public class OctagonStack : MonoBehaviour, IDamagable
{
    [Header(" References ")]
    [SerializeField] private Transform _octagonsParent;
    [SerializeField] private TextMeshPro _healthText;

    [Header(" Settings ")]
    private float _octagonsYOffset = 0.065f;
    private const string OCTAGON_POOL_KEY = "Octagon";

    [Header(" Datas ")]
    private Stack<Octagon> _activeHexagon = new Stack<Octagon>();
    private float _maxHealth = 20f;
    private float _health;
    private float _perOctagonHealth = 1;
    private int _currentOctagonCount = 0;

    private Tween _stackTakeDamageTween;

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

            UpdateHealthText();
            if (_health <= 0)
            {
                KillHexagonStack();
            }
        }
    }

    //initialization
    public void InitializeHexagonStack(float health, float perOctagonHealth, int count, Color color)
    {
        _perOctagonHealth = perOctagonHealth;
        _currentOctagonCount = count;
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
        _healthText.transform.localPosition = new Vector3(0, _octagonsYOffset * count, -0.2f * count);
    }

    public void InitializeStackCount(int count, Color color)
    {
        for (int i = 0; i < count; i++)
        {
            ObjectPool.Instance.Get(OCTAGON_POOL_KEY, _octagonsParent);
        }
        SetHexagonsPosition(color);
    }

    private void SetHexagonsPosition(Color color)
    {
        for (int i = 0; i < _octagonsParent.childCount; i++)
        {
            Octagon hexagon = _octagonsParent.GetChild(i).GetComponent<Octagon>();
            hexagon.SetLocalPosition(_octagonsYOffset, i);
            hexagon.SetColor(color);

            _activeHexagon.Push(hexagon);
        }
    }

    private void ResetHealth()
    {
        _health = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        
        TakeDamageAnimation();
        float previousHealth = Health;
        Health -= damage;
        CheckAndBreakOctagons(previousHealth);
    }

    private void UpdateHealthText()
    {
        if (_healthText != null)
        {
            _healthText.text = Mathf.Max(0, _health).ToString("F0");
        }
    }

    private void CheckAndBreakOctagons(float previousHealth)
    {
        // Her perOctagonHealth kadar hasar alındığında bir octagon kır
        int previousOctagonCount = Mathf.CeilToInt(previousHealth / _perOctagonHealth);
        int currentOctagonCount = Mathf.CeilToInt(_health / _perOctagonHealth);
        
        // Kırılması gereken octagon sayısı
        int octagonsToBreak = previousOctagonCount - currentOctagonCount;
        
        // Octagonları kır
        for (int i = 0; i < octagonsToBreak; i++)
        {
            if (_activeHexagon.Count > 0)
            {
                BreakTopOctagon();
            }
        }
        
        UpdateTextPosition();
    }

    private void BreakTopOctagon()
    {
        if (_activeHexagon.Count == 0) return;

        Octagon topHexagon = _activeHexagon.Pop();
        _currentOctagonCount--;

        SpriteRenderer sr = topHexagon.SpriteRenderer;
        if (sr != null)
        {
            Color originalColor = sr.color;
            sr.DOColor(Color.white, .05f)
              .SetLoops(2, LoopType.Yoyo)
              .SetEase(Ease.Linear)
              .OnComplete(() => ObjectPool.Instance.Return(OCTAGON_POOL_KEY, topHexagon.gameObject));
        }
    }

    private void UpdateTextPosition()
    {
        if (_currentOctagonCount > 0)
        {
            _healthText.transform.localPosition = new Vector3(0, _octagonsYOffset * _currentOctagonCount, -0.2f * _currentOctagonCount);
        }
    }

    private void TakeDamageAnimation()
    {
        _stackTakeDamageTween?.Complete();
        _stackTakeDamageTween = transform.DOPunchScale(Vector3.one * .2f, .2f, 1);
    }

    private void KillHexagonStack()
    {
        if (_octagonsParent != null)
        {
            while (_octagonsParent.childCount > 0)
            {
                Transform child = _octagonsParent.GetChild(0);
                ObjectPool.Instance.Return(OCTAGON_POOL_KEY, child.gameObject);
            }
        }

        _activeHexagon.Clear();
        _currentOctagonCount = 0;

        ObjectPool.Instance.Return("HexagonStack", gameObject);
    }
}
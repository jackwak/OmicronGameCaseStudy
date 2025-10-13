using UnityEngine;

public class Hexagon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public SpriteRenderer SpriteRenderer { get => _spriteRenderer; }
    
    public void SetColor(Color color)
    {
        if (_spriteRenderer != null)
            _spriteRenderer.color = color;
    }

    public void SetLocalPosition(float offSet, int index)
    {
        transform.localPosition = Vector3.up * offSet * index + new  Vector3(0, 0, -.1f * index);
    }
    
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
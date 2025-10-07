using UnityEngine;

public class Hexagon : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    
    public void SetColor(Color color)
    {
        if (meshRenderer != null)
            meshRenderer.material.color = color;
    }

    public void SetLocalPosition(float offSet, int index)
    {
        transform.localPosition = Vector3.up * offSet * index;
    }
    
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
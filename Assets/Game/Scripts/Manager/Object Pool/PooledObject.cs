using UnityEngine;

public class PooledObject : MonoBehaviour
{
    [HideInInspector] public string Key;
    [HideInInspector] public ObjectPool Manager;

    public void ReturnToPool()
    {
        Debug.LogError("Girdiii");
        if (Manager != null && !string.IsNullOrEmpty(Key))
        {
            Manager.Return(Key, gameObject);
        }
    }
}

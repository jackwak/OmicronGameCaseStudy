using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolEntry
{
    public string Key;
    public GameObject Prefab;
    public int InitialSize = 0;
    public Transform ParentOverride; // opsiyonel, bu pool’un altına iade edilsin
}

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    [Header("Pools")]
    public List<PoolEntry> Pools = new List<PoolEntry>();

    // key -> queue
    private readonly Dictionary<string, Queue<GameObject>> _pool = new();
    // key -> prefab
    private readonly Dictionary<string, GameObject> _prefabs = new();
    // key -> parent
    private readonly Dictionary<string, Transform> _parents = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        
        foreach (var entry in Pools)
        {
            if (string.IsNullOrEmpty(entry.Key) || entry.Prefab == null) continue;

            _prefabs[entry.Key] = entry.Prefab;
            if (!_pool.ContainsKey(entry.Key))
                _pool[entry.Key] = new Queue<GameObject>();

            // parent
            Transform parent = entry.ParentOverride != null
                ? entry.ParentOverride
                : CreateParent(entry.Key);
            _parents[entry.Key] = parent;

            // prewarm
            for (int i = 0; i < entry.InitialSize; i++)
            {
                var go = CreateNew(entry.Key);
                go.transform.SetParent(parent, worldPositionStays: false);
                go.SetActive(false);
                _pool[entry.Key].Enqueue(go);
            }
        }
    }

    private Transform CreateParent(string key)
    {
        var g = new GameObject($"[Pool] {key}");
        g.transform.SetParent(transform);
        return g.transform;
    }

    private GameObject CreateNew(string key)
    {
        if (!_prefabs.TryGetValue(key, out var prefab) || prefab == null)
        {
            Debug.LogError($"[ObjectPool] Prefab bulunamadı: {key}");
            return null;
        }

        var go = Instantiate(prefab);
        var po = go.GetComponent<PooledObject>();
        if (po == null) po = go.AddComponent<PooledObject>();
        po.Key = key;
        po.Manager = this;
        return go;
    }

    /// <summary>
    /// Havuzdan al (varsa kuyruqdan, yoksa instantiate eder)
    /// </summary>
    public GameObject Get(string key, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!_pool.ContainsKey(key))
        {
            // Dinamik olarak pool oluştur (prefab kayıtlı değilse hata verir)
            _pool[key] = new Queue<GameObject>();
            if (!_prefabs.ContainsKey(key))
            {
                Debug.LogError($"[ObjectPool] '{key}' için prefab kayıtlı değil!");
                return null;
            }
            if (!_parents.ContainsKey(key))
                _parents[key] = CreateParent(key);
        }

        GameObject go = null;
        if (_pool[key].Count > 0)
        {
            go = _pool[key].Dequeue();
        }
        else
        {
            go = CreateNew(key);
            if (go == null) return null;
        }

        if (parent != null) go.transform.SetParent(parent, worldPositionStays: false);

        go.transform.SetPositionAndRotation(position, rotation);
        go.SetActive(true);

        // IPoolable bildirimi (opsiyonel)
        if (go.TryGetComponent<IPoolable>(out var ip))
            ip.OnSpawned();

        return go;
    }

    public GameObject Get(string key, Transform parent = null)
        => Get(key, Vector3.zero, Quaternion.identity, parent);

    public GameObject Get(string key)
        => Get(key, Vector3.zero, Quaternion.identity, null);

    /// <summary>
    /// Nesneyi havuza iade et
    /// </summary>
    public void Return(string key, GameObject go)
    {
        if (go == null) return;

        if (!_pool.ContainsKey(key))
        {
            // pool yoksa oluştur
            _pool[key] = new Queue<GameObject>();
            if (!_parents.ContainsKey(key))
                _parents[key] = CreateParent(key);
        }

        // IPoolable bildirimi (opsiyonel)
        if (go.TryGetComponent<IPoolable>(out var ip))
            ip.OnDespawned();

        go.SetActive(false);
        go.transform.SetParent(_parents[key], worldPositionStays: false);
        _pool[key].Enqueue(go);
    }
}

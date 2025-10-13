using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ManualTrail2D : MonoBehaviour
{
    [Header("Trail Shape")]
    [Range(2, 256)] public int resolution = 12;     // Nokta sayısı
    [Min(0.001f)] public float segmentSpacing = 0.2f; // Her nokta arası mesafe (pozitif tut)
    [Min(0.001f)] public float lagTime = 0.15f;     // Segmentlerin birbirini takip “gecikmesi”

    [Header("Line Renderer (2D)")]
    public float startWidth = 0.12f;
    public float endWidth   = 0.00f;
    public string sortingLayerName = "Default";
    public int sortingOrder = 0;

    private LineRenderer _lr;
    private Vector3[] _pos;
    private Vector3[] _vel;

    void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = resolution;
        _lr.alignment = LineAlignment.View;  // 2D için ekrana hizala
        _lr.useWorldSpace = true;            // Parent hareketlerinden bağımsız hesaplayacağız
        _lr.startWidth = startWidth;
        _lr.endWidth = endWidth;
        _lr.sortingLayerName = sortingLayerName;
        _lr.sortingOrder = sortingOrder;

        _pos = new Vector3[resolution];
        _vel = new Vector3[resolution];

        // Başlangıç: hepsini transform.position’dan aşağıya doğru diz
        var basePos = transform.position;
        for (int i = 0; i < resolution; i++)
        {
            _pos[i] = basePos + Vector3.down * (segmentSpacing * i);
            _vel[i] = Vector3.zero;
        }
        _lr.SetPositions(_pos);
    }

    void Update()
    {
        // 1) İlk nokta her zaman objenin üstünde (anchor)
        _pos[0] = transform.position;

        // 2) Diğer noktalar bir önceki noktayı, aşağı offsetli hedefe doğru SmoothDamp ile takip eder
        for (int i = 1; i < resolution; i++)
        {
            Vector3 target = _pos[i - 1] + Vector3.down * segmentSpacing;
            _pos[i] = Vector3.SmoothDamp(_pos[i], target, ref _vel[i], lagTime);
        }

        _lr.SetPositions(_pos);
    }

    // Oyun içinde değerleri değiştirirsen anında uygula
    void OnValidate()
    {
        if (_lr == null) return;
        _lr.startWidth = startWidth;
        _lr.endWidth = endWidth;
        _lr.sortingLayerName = sortingLayerName;
        _lr.sortingOrder = sortingOrder;
        if (resolution < 2) resolution = 2;
        if (segmentSpacing < 0.001f) segmentSpacing = 0.001f;
        if (lagTime < 0.001f) lagTime = 0.001f;
    }
}

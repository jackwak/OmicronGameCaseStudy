using UnityEngine;

public class PatternSpawnTrigger : MonoBehaviour
{
    public float VerticalOffset = 0.5f;
    
    private Camera _mainCam;
    private bool _hasTriggered = false;

    void Awake()
    {
        _mainCam = Camera.main;
        UpdatePosition();
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying)
            UpdatePosition();
    }
#endif

    [ContextMenu("Update Pos")]
    private void UpdatePosition()
    {
        if (_mainCam == null)
            _mainCam = Camera.main;
        if (_mainCam == null)
            return;

        Vector3 topCenter = _mainCam.ViewportToWorldPoint(new Vector3(0.5f, 1f, _mainCam.nearClipPlane));
        topCenter.z = 0;
        transform.position = topCenter + Vector3.down * VerticalOffset;
    }

    void OnTriggerEnter(Collider other)
    {
        // Prevent multiple triggers
        if (_hasTriggered)
            return;
            
        if (other.TryGetComponent(out PatternSpawnPosition patternSpawnPosition))
        {
            _hasTriggered = true;
            
            // Trigger pattern spawn
            LevelManager.Instance.SpawnNextPattern(patternSpawnPosition.transform);
            
            // Reset trigger after a delay (optional)
            Invoke(nameof(ResetTrigger), 1f);
        }
    }
    
    private void ResetTrigger()
    {
        _hasTriggered = false;
    }
}
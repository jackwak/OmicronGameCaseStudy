using UnityEngine;

public class PatternSpawnTrigger : MonoBehaviour
{
    public float VerticalOffset = 0.5f;

    private Camera _mainCam;

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

    [ContextMenu(" Update  Pos ")]
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
        if (other.TryGetComponent(out FullscreenSquare square))
        {
            foreach (var position in square.transform.GetChild(1))
            {
                // pozisyonlarda spawn et
            }
        }
    }
}

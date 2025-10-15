using UnityEngine;

[ExecuteAlways]
public class FullscreenSquare : MonoBehaviour
{
    private Camera _mainCam;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private PatternSettings _patternSettings;

    void Awake()
    {
        _mainCam = Camera.main;
        FitToScreen();
        UpdateSpawnPosition();
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying)
        {
            FitToScreen();
            UpdateSpawnPosition();
        }
    }
#endif

    private void FitToScreen()
    {
        if (_mainCam == null)
            _mainCam = Camera.main;
        if (_renderer == null)
            _renderer = GetComponent<SpriteRenderer>();
        if (_mainCam == null || _renderer == null || _renderer.sprite == null)
            return;

        float spriteWidth = _renderer.sprite.bounds.size.x;
        float spriteHeight = _renderer.sprite.bounds.size.y;

        float worldHeight = _mainCam.orthographicSize * 2f;
        float worldWidth = worldHeight * _mainCam.aspect;

        Vector3 scale = _renderer.transform.localScale;
        scale.x = worldWidth / spriteWidth;
        scale.y = worldHeight / spriteHeight;
        _renderer.transform.localScale = scale;
    }

    private void UpdateSpawnPosition()
    {
        if (_spawnPosition == null || _renderer == null)
            return;

        Bounds bounds = _renderer.bounds;
        Vector3 bottomPosition = new Vector3(bounds.min.x, bounds.min.y, bounds.center.z);

        _spawnPosition.position = bottomPosition;
    }
}

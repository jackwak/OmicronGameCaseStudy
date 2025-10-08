using UnityEngine;

[ExecuteAlways]
public class FullscreenSquare : MonoBehaviour
{
    private Camera _mainCam;
    [SerializeField] private SpriteRenderer _renderer;

    void Awake()
    {
        _mainCam = Camera.main;
        FitToScreen();
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying)
            FitToScreen();
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

        Vector3 scale = transform.localScale;
        scale.x = worldWidth / spriteWidth;
        scale.y = worldHeight / spriteHeight;
        transform.localScale = scale;
    }
}

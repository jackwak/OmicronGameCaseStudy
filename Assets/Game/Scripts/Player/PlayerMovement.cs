using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float maxRotation = 15f;
    [SerializeField] private float rotationSpeed = 0.2f;

    private Tweener rotationTween;
    private float lastDeltaX = 0;
    private float minX;
    private float maxX;
    private bool _canPlayerMove;

    private void OnEnable()
    {
        EventManager.Instance.EnterGameState += SetCanPlayerMoveTrue;    
        EventManager.Instance.EnterFinishState += SetCanPlayerMoveFalse;    
    }

    void OnDisable()
    {
        EventManager.Instance.EnterGameState -= SetCanPlayerMoveTrue;    
        EventManager.Instance.EnterFinishState -= SetCanPlayerMoveFalse;  
    }

    void Start()
    {
        // Kameranın görüş alanını hesapla
        Camera mainCamera = Camera.main;
        float halfPlayerWidth = GetComponent<SpriteRenderer>() != null
            ? GetComponent<SpriteRenderer>().bounds.extents.x
            : 0.5f;

        minX = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + halfPlayerWidth;
        maxX = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - halfPlayerWidth;
    }

    void Update()
    {
        if (!_canPlayerMove) return;

        Vector2 delta = InputManager.Instance.SwipeDelta;
        Move(delta);
        HandleRotation(delta.x);
    }

    private void Move(Vector2 delta)
    {
        if (delta == Vector2.zero) return;

        Vector3 newPos = transform.position + new Vector3(delta.x, 0, 0) * moveSpeed * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        transform.position = newPos;
    }

    private void HandleRotation(float deltaX)
    {
        float targetRotation = 0;

        if (Mathf.Abs(deltaX) > 0.01f)
        {
            // Hıza göre rotasyon hesapla
            float normalizedSpeed = Mathf.Clamp(Mathf.Abs(deltaX), 0, 1);
            targetRotation = -Mathf.Sign(deltaX) * maxRotation * normalizedSpeed;
            lastDeltaX = deltaX;
        }
        else if (Mathf.Abs(lastDeltaX) > 0.01f)
        {
            // Hareket durdu, rotasyonu sıfırla
            targetRotation = 0;
            lastDeltaX = 0;
        }
        else
        {
            return;
        }

        // Mevcut tween'i öldür ve yenisini başlat
        rotationTween?.Kill();
        rotationTween = transform.DORotate(new Vector3(0, 0, targetRotation), rotationSpeed)
            .SetEase(Ease.OutQuad);
    }

    private void SetCanPlayerMoveTrue()
    {
        _canPlayerMove = true;
    }
    private void SetCanPlayerMoveFalse()
    {
        _canPlayerMove = false;
    }
}
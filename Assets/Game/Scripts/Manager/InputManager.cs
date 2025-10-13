using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private PlayerInput _playerInput;
    private Vector2 _swipeDelta;
    private bool _isTouching; // ← YENİ
    
    public Vector2 SwipeDelta => _swipeDelta;
    public bool IsTouching => _isTouching; // ← YENİ (dışarıya okunabilir)

    private void Awake()
    {
        Instance = this;
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Player.Enable();
        _playerInput.Player.Swipe.performed += OnSwipe;
        _playerInput.Player.Swipe.canceled += OnSwipeCanceled;
        
        // Touch started/ended events (Input System'de genelde Press action var)
        _playerInput.Player.Touch.started += OnTouchStarted;   // ← YENİ
        _playerInput.Player.Touch.canceled += OnTouchEnded;    // ← YENİ
    }

    private void OnDisable()
    {
        _playerInput.Player.Swipe.performed -= OnSwipe;
        _playerInput.Player.Swipe.canceled -= OnSwipeCanceled;
        _playerInput.Player.Touch.started -= OnTouchStarted;   // ← YENİ
        _playerInput.Player.Touch.canceled -= OnTouchEnded;    // ← YENİ
        _playerInput.Player.Disable();
    }

    private void OnSwipe(InputAction.CallbackContext context)
    {
        _swipeDelta = context.ReadValue<Vector2>();
    }

    private void OnSwipeCanceled(InputAction.CallbackContext context)
    {
        _swipeDelta = Vector2.zero;
    }
    
    // ← YENİ
    private void OnTouchStarted(InputAction.CallbackContext context)
    {
        _isTouching = true;
    }
    
    // ← YENİ
    private void OnTouchEnded(InputAction.CallbackContext context)
    {
        _isTouching = false;
    }
}

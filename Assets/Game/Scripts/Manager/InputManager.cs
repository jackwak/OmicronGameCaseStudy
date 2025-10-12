using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private PlayerInput _playerInput;
    private Vector2 _swipeDelta;
    public Vector2 SwipeDelta => _swipeDelta; // dışarıya okunabilir

    private void Awake()
    {
        Instance = this;
        _playerInput = new PlayerInput(); // 🔹 initialize et
    }

    private void OnEnable()
    {
        _playerInput.Player.Enable();
        _playerInput.Player.Swipe.performed += OnSwipe;
        _playerInput.Player.Swipe.canceled += OnSwipeCanceled;
    }

    private void OnDisable()
    {
        _playerInput.Player.Swipe.performed -= OnSwipe;
        _playerInput.Player.Swipe.canceled -= OnSwipeCanceled;
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
}

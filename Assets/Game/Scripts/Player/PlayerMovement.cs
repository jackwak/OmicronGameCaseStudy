using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;

    void Update()
    {
        if (InputManager.Instance == null) return;

        Vector2 delta = InputManager.Instance.SwipeDelta;
        Move(delta);
    }

    private void Move(Vector2 delta)
    {
        if (delta == Vector2.zero) return;

        transform.position += new Vector3(delta.x, 0, 0) * moveSpeed  * Time.deltaTime;
    }
}
